﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MediaLibrarian
{
    public partial class EditForm : Form
    {
        public EditForm(MainForm FormMain)
        {
            InitializeComponent();
            mainForm = FormMain;            
        }
        MainForm mainForm;
        LibManagerForm libManagerForm;
        List<Control> columnData = new List<Control>();
        string customDateTimeFormat = "d.MM.yyyy, HH:mm:ss";
        static string database = "baza.db";
        SQLiteConnection connection = new SQLiteConnection(string.Format("Data Source={0};", database));
        public bool EditMode = false;
        Bitmap loadedBitmap = null;
        public EventArgs e { get; set; }

        #region DatabaseAPI
        private void GetControlByType(string type, int i)
        {
            switch (type)
            {
                case "VARCHAR(128)": MakeATextBox(i); break;        //Строка
                case "TEXT": MakeATextArea(i); break;               //Текст
                case "VARCHAR(20)": MakeADateField(i); break;       //Поле дата
                case "CHAR(20)": MakeADateTimeField(i); break;      //Поле дата+время
                case "CHAR(5)": Make5Stars(i); break;               //Поле оценка(5)
                case "CHAR(10)": Make10Stars(i); break;             //Поле оценка(10)
                case "VARCHAR(10)": Make10Cubes(i); break;          //Поле приоритет
            }
        }
        private List<string> GetDataFromDatabase(string TableName, string ElementHeaderName, string ElementName)
        {
            SQLiteCommand GetData = new SQLiteCommand(String.Format("select * from `{0}` where `{1}`='{2}'", TableName, 
                ElementHeaderName, ElementName.Replace("'","''")), connection);
            DataTable EditString = new DataTable();
            connection.Open();
            EditString.Load(GetData.ExecuteReader());
            connection.Close();
            List<string> Items = new List<string>();
            foreach (var item in EditString.Rows[0].ItemArray)
            {
                Items.Add(item.ToString());
            }
            return Items;
        }
        private void PushDataIntoCreatedControls(List<string> Items)
        {
            for (int i = 0; i < columnData.Count; i++)
            {
                switch (columnData[i].GetType().ToString())
                {
                    case "System.Windows.Forms.TextBox": case "System.Windows.Forms.RichTextBox":
                        columnData[i].Text = Items[i]; break;
                    case "System.Windows.Forms.Panel":
                        if (!new List<string>(){null, "", "0"}.Contains(Items[i])) 
                            switch (columnData[i].Tag.ToString())
                        {
                            case "Star5" : Star5_Click(columnData[i].Controls[GetNumValue(Items[i]) - 1], e); break;
                            case "Star10": Star10_Click(columnData[i].Controls[GetNumValue(Items[i]) - 1], e); break;
                            case "Cube10": Cube10_Click(columnData[i].Controls[GetNumValue(Items[i]) - 1], e); break;
                        }
                        break;
                    case "System.Windows.Forms.DateTimePicker": 
                        try
                            {
                                (columnData[i] as DateTimePicker).Value = DateTime.Parse(Items[i]);
                            }
                        catch (Exception ex) {MessageBox.Show(ex.Message);}
                        break;
                }
            }
        }
        #endregion
        #region Items
        private void EditItem()
        {
            List<string> Names = new List<string>();
            Names.AddRange(from val in mainForm.columnsInfo select val.Name);
            List<string> Values = new List<string>();
            Values.AddRange(from data in columnData select data.Text);
            string updateQuery = String.Format("update `{0}` set ", mainForm.SelectedLibLabel.Text);
            for (int i = 0; i < Names.Count; i++)
            {
                updateQuery += "`" + Names[i] + "` = '" + Values[i].Replace("'", "''") + "'";
                if (Names.Count - i > 1) updateQuery += ", ";
            }
            updateQuery += " where `" + Names[0] + "` = '" + columnData[0].Tag.ToString().Replace("'", "''") + "'";            
            if (VerifyItem())
            {
                SQLiteCommand EditItem = new SQLiteCommand(updateQuery, connection);
                try { EditItem.ExecuteNonQuery(); }
                catch (SQLiteException) { 
                    MessageBox.Show("Доступ к базе временно заблокирован. Попробуйте еще раз",
                        "ОТшибка ",MessageBoxButtons.OK, MessageBoxIcon.Error); 
                    connection.Close();
                    this.DialogResult = DialogResult.Abort;
                    return; }
                connection.Close();
                SavePicture();
                mainForm.StatusLabel.Text = "Элемент \"" + columnData[0].Text + "\" изменен";
                Close();
            }

        }
        private void AddNewItem()
        {
            string Names = String.Join("` , `", from val in mainForm.columnsInfo select val.Name);
            string Values = String.Join("\" , \"", from data in columnData select data.Text);
            SQLiteCommand AddNewItem = new SQLiteCommand(String.Format("insert into `{0}` (`{1}`) values (\"{2}\")",
                mainForm.SelectedLibLabel.Text, Names, Values), connection);
            if (VerifyItem())
            {
                AddNewItem.ExecuteNonQuery();
                connection.Close();
                SavePicture();
                mainForm.StatusLabel.Text = "Добавлен элемент \"" + columnData[0].Text + "\"";
                Close();
            }
        }
        private bool VerifyItem()
        {
            SQLiteCommand Verify = new SQLiteCommand(string.Format("select `{0}` from `{1}` where `{0}` = '{2}'",
                mainForm.columnsInfo[0].Name, mainForm.SelectedLibLabel.Text, columnData[0].Text.Replace("'", "''")), connection);
            connection.Open();
            SQLiteDataReader reader = Verify.ExecuteReader();
            if (reader.HasRows && (columnData[0].Tag.ToString() != columnData[0].Text))
            {
                MessageBox.Show("Элемент с таким именем уже существует в библиотеке", "Обнаружен дубликат данных", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                connection.Close();
                this.DialogResult = DialogResult.Abort;
                return false;
            }
            reader.Close();
            reader.Dispose();
            return true;
        }
        public void DeleteItem(string ItemType, string ItemName)
        {
            SQLiteCommand DeleteItem = new SQLiteCommand(String.Format("delete from `{0}` where `{1}` = '{2}'", mainForm.SelectedLibLabel.Text,
                ItemType, ItemName), connection);
            connection.Open();
            DeleteItem.ExecuteNonQuery();
            connection.Close();
            string PathToPoster = String.Format(@"{0}\{1}\{2}.jpg", Environment.CurrentDirectory,
                        mainForm.ReplaceSymblos(mainForm.SelectedLibLabel.Text),
                        mainForm.ReplaceSymblos(ItemName));
            if (File.Exists(PathToPoster)) File.Delete(PathToPoster);
            mainForm.StatusLabel.Text = "Элемент \"" + ItemName + "\" успешно удален";
            UpdateCollection();
        }
        #endregion
        
        void DownloadPicture(string address)
        {
            LoadingLabel.Text = "Загрузка...";
            try
            {
                var request = System.Net.WebRequest.Create(address);
                var response = request.GetResponse();
                using (var responseStream = response.GetResponseStream())
                {
                    loadedBitmap = new Bitmap(responseStream);
                    LoadingLabel.Text = "Изображение загружено";
                    SaveButton.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                LoadingLabel.Text = ex.Message;
                loadedBitmap = null;
            }
        }
        void SavePicture()
        {
            string newStr = String.Format(@"{0}\{1}\{2}.jpg", Environment.CurrentDirectory,
                        mainForm.ReplaceSymblos(mainForm.SelectedLibLabel.Text),
                        mainForm.ReplaceSymblos(columnData[0].Text));
            string oldStr = String.Format(@"{0}\{1}\{2}.jpg", Environment.CurrentDirectory,
                        mainForm.ReplaceSymblos(mainForm.SelectedLibLabel.Text),
                        mainForm.ReplaceSymblos(columnData[0].Tag.ToString()));
            if (PosterImageTB.Text == ""&&EditMode)
                try
                {
                    if(File.Exists(newStr))File.Delete(newStr);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }            
            if(File.Exists(oldStr)&&EditMode) File.Move(oldStr, newStr);
            if (loadedBitmap != null)
                try
                {
                    using (loadedBitmap)
                    {
                        loadedBitmap.Save(newStr, ImageFormat.Jpeg);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DialogResult = DialogResult.Abort;
                }
            else return;
        }


        #region CreateControls
        void CreateHeaderLabel(int i)
        {
            EditPanel.Controls.Add(new Label() 
            {
                Size = new Size(220, 15),
                AutoEllipsis = true,
                Font = new Font("Tahoma", 9),
                FlatStyle = FlatStyle.System,
                Text = mainForm.columnsInfo[i].Name
            });
        }
        void MakeATextBox(int i)
        {
            TextBox tb = new TextBox()
            {
                Size = new Size(420, 25),
                MaxLength = 128
            };
            tb.TextChanged += tb_TextChanged;
            columnData.Add(tb);
        }
        void MakeATextArea(int i)
        {
            columnData.Add(new RichTextBox()
            {
                Size = new Size(420, 100),
                ScrollBars = RichTextBoxScrollBars.ForcedVertical,
                WordWrap = true,
            });
        }
        void MakeADateField(int i)
        {
            columnData.Add(new DateTimePicker()
            {
                Size = new Size(150, 20),
                Margin = new Padding() {Left = 47, Bottom = 5 },
                Font = new Font("Tahoma", 9),
                Format = DateTimePickerFormat.Long,
                Value = DateTime.Now,
            });
        }
        void MakeADateTimeField(int i)
        {
            columnData.Add(new DateTimePicker()
            {
                Size = new Size(160, 20),
                Margin = new Padding() {Left = 37, Bottom = 5 },
                Font = new Font("Tahoma", 9),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = customDateTimeFormat,
                Value = DateTime.Now,
            });
        }
        #endregion
        #region CreateStarsAndCubes
        void Make5Stars(int i) //i - порядок панели на форме
        {
            var StarsList = new List<Label>();
            Panel Stars5Panel = new Panel()
            {
                Size = new Size(190, 30),
                Tag = "Star5"
            };
            for (int ii = 0; ii < 5; ii++)
            {
                Label Star5 = new Label()
                {
                    Size = new Size(25, 30),
                    Location = new Point(ii * 25 + 65, 0),
                    Font = new Font("Tahoma", 16, FontStyle.Bold),
                    ForeColor = Color.Gray,
                    Text = "☆",
                    Tag = new int[] {i , StarsList.Count}
                };
                Star5.Click += new EventHandler(Star5_Click);
                StarsList.Add(Star5);
            }
            Stars5Panel.Controls.AddRange(StarsList.ToArray());
            columnData.Add(Stars5Panel);
        }
        void Make10Stars(int i) //i - порядок панели на форме
        {
            var StarsList = new List<Label>();
            Panel Stars10Panel = new Panel()
            {
                Size = new Size(195, 30),
                Tag = "Star10"
            };
            for (int ii = 0; ii < 10; ii++)
            {
                Label Star10 = new Label()
                {
                    Size = new Size(20, 30),
                    Location = new Point(ii * 19 + 2, 0),
                    FlatStyle = FlatStyle.System,
                    Font = new Font("Tahoma", 14, FontStyle.Bold),
                    ForeColor = Color.Gray,
                    Text = "☆",
                    Tag = new int[] {i, StarsList.Count}
                };
                Star10.Click += new EventHandler(Star10_Click);
                StarsList.Add(Star10);
            }
            Stars10Panel.Controls.AddRange(StarsList.ToArray());
            columnData.Add(Stars10Panel);
        }
        void Make10Cubes(int i) //i - порядок панели на форме
        {
            var CubeList = new List<Label>();
            Panel Cubes10Panel = new Panel()
            {
                Size = new Size(150, 30),
                Margin = new Padding() { Left = 42 },
                Tag = "Cube10"
            };
            for (int ii = 0; ii < 10; ii++)
            {
                Label Cube = new Label()
                {
                    Size = new Size(15, 30),
                    Location = new Point(ii * 15, 0),
                    Font = new Font("Tahoma", 16, FontStyle.Bold),
                    FlatStyle = FlatStyle.System,
                    ForeColor = Color.Gray,
                    Text = "▒",
                    Tag = new int[] {i, CubeList.Count}
                };
                Cube.Click += new EventHandler(Cube10_Click);
                CubeList.Add(Cube);
            }
            Cubes10Panel.Controls.AddRange(CubeList.ToArray());
            columnData.Add(Cubes10Panel);
        }
        #endregion
        #region ClickToBlocks
        private void Star5_Click(object sender, EventArgs e)
        {
            int ColCrd = ((int[])(sender as Label).Tag)[0];
            int ListCrd = ((int[])(sender as Label).Tag)[1];
            if ((sender as Label) == columnData[ColCrd].Controls[0] && columnData[ColCrd].Controls[0].Text == "★" && columnData[ColCrd].Controls[1].Text == "☆")
            {
                columnData[ColCrd].Controls[0].Text = "☆";
                columnData[ColCrd].Controls[0].ForeColor = Color.Gray;
                columnData[ColCrd].Text = "☆☆☆☆☆";
            }
            else
            {
                columnData[ColCrd].Text = "";
                for (int i = 0; i < ListCrd + 1; i++)
                {
                    if (ListCrd < 2) columnData[ColCrd].Controls[i].ForeColor = Color.Red;
                    if (ListCrd == 2) columnData[ColCrd].Controls[i].ForeColor = Color.Orange;
                    if (ListCrd > 2) columnData[ColCrd].Controls[i].ForeColor = Color.LimeGreen;
                    columnData[ColCrd].Controls[i].Text = "★";
                    columnData[ColCrd].Text += "★";
                }
                for (int j = columnData[ColCrd].Controls.Count - 1; j > ListCrd; j--)
                {
                    columnData[ColCrd].Controls[j].ForeColor = Color.Gray;
                    columnData[ColCrd].Controls[j].Text = "☆";
                    columnData[ColCrd].Text += "☆";
                }
            }
        }
        private void Star10_Click(object sender, EventArgs e)
        {
            int ColCrd = ((int[])(sender as Label).Tag)[0];
            int ListCrd = ((int[])(sender as Label).Tag)[1];
            if ((sender as Label) == columnData[ColCrd].Controls[0] && columnData[ColCrd].Controls[0].Text == "★" && columnData[ColCrd].Controls[1].Text == "☆")
            {
                columnData[ColCrd].Controls[0].Text = "☆";
                columnData[ColCrd].Controls[0].ForeColor = Color.Gray;
                columnData[ColCrd].Text = "☆☆☆☆☆☆☆☆☆☆";
            }
            else
            {
                columnData[ColCrd].Text = "";
                for (int i = 0; i < ListCrd + 1; i++)
                {
                    if (ListCrd < 4) columnData[ColCrd].Controls[i].ForeColor = Color.Red;
                    if (ListCrd > 3 && ListCrd < 8) columnData[ColCrd].Controls[i].ForeColor = Color.Orange;
                    if (ListCrd > 7) columnData[ColCrd].Controls[i].ForeColor = Color.LimeGreen;
                    columnData[ColCrd].Controls[i].Text = "★";
                    columnData[ColCrd].Text += "★";
                }
                for (int j = columnData[ColCrd].Controls.Count - 1; j > ListCrd; j--)
                {
                    columnData[ColCrd].Controls[j].ForeColor = Color.Gray;
                    columnData[ColCrd].Controls[j].Text = "☆";
                    columnData[ColCrd].Text += "☆";
                }
            }
        }
        private void Cube10_Click(object sender, EventArgs e)
        {
            int ColCrd = ((int[])(sender as Label).Tag)[0];
            int ListCrd = ((int[])(sender as Label).Tag)[1];
            if ((sender as Label) == columnData[ColCrd].Controls[0] && columnData[ColCrd].Controls[0].Text == "█" && columnData[ColCrd].Controls[1].Text == "▒")
            {
                columnData[ColCrd].Controls[0].Text = "▒";
                columnData[ColCrd].Controls[0].ForeColor = Color.Gray;
                columnData[ColCrd].Text = "▒▒▒▒▒▒▒▒▒▒";
            }
            else
            {
                columnData[ColCrd].Text = "";
                for (int i = 0; i < ListCrd + 1; i++)
                {
                    columnData[ColCrd].Controls[i].ForeColor = Color.SeaGreen;
                    columnData[ColCrd].Controls[i].Text = "█";
                    columnData[ColCrd].Text += "█";
                }
                for (int j = columnData[ColCrd].Controls.Count - 1; j > ListCrd; j--)
                {
                    columnData[ColCrd].Controls[j].ForeColor = Color.Gray;
                    columnData[ColCrd].Controls[j].Text = "▒";
                    columnData[ColCrd].Text += "▒";
                }
            }
        }
        #endregion
        #region ButtonClicks
        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (columnData[0].Text == "")
            {
                MessageBox.Show("Графа \"" + mainForm.columnsInfo[0].Name + "\" должна быть обязательно заполнена",
                    "Ошибка входных данных", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Abort;
                return;
            }
            if (EditMode) EditItem(); else AddNewItem();
            UpdateCollection();
        }
        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void SelectImageButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Abort;
            if (OpenFile.ShowDialog() == DialogResult.OK)
            {
                PosterImageTB.Text = OpenFile.FileName;
            }

        }

        #endregion

        private void FormReset()
        {
            EditMode = false;
            columnData.Clear();
            EditPanel.Controls.Clear();
            PosterImageTB.Text = "";
            LoadingLabel.Text = "";
        }
        private void UpdateCollection()
        {
            libManagerForm = new LibManagerForm(mainForm);
            libManagerForm.ReadTableFromDatabase(mainForm.SelectedLibLabel.Text);
            libManagerForm.Dispose();
        }
        int GetNumValue(string stars)
        {
            int digit = 0;
            switch (stars)
            {
                case "☆☆☆☆☆": digit = 0; break;
                case "☆☆☆☆☆☆☆☆☆☆": digit = 0; break;
                case "▒▒▒▒▒▒▒▒▒▒": digit = 0; break;
                case "★☆☆☆☆": digit = 1; break;
                case "★☆☆☆☆☆☆☆☆☆": digit = 1; break;
                case "█▒▒▒▒▒▒▒▒▒": digit = 1; break;
                case "★★☆☆☆": digit = 2; break;
                case "★★☆☆☆☆☆☆☆": digit = 2; break;
                case "██▒▒▒▒▒▒▒▒": digit = 2; break;
                case "★★★☆☆": digit = 3; break;
                case "★★★☆☆☆☆☆☆☆": digit = 3; break;
                case "███▒▒▒▒▒▒▒": digit = 3; break;
                case "★★★★☆": digit = 4; break;
                case "★★★★☆☆☆☆☆☆": digit = 4; break;
                case "████▒▒▒▒▒▒": digit = 4; break;
                case "★★★★★": digit = 5; break;
                case "★★★★★☆☆☆☆☆": digit = 5; break;
                case "█████▒▒▒▒▒": digit = 5; break;
                case "★★★★★★☆☆☆☆": digit = 6; break;
                case "██████▒▒▒▒": digit = 6; break;
                case "★★★★★★★☆☆☆": digit = 7; break;
                case "███████▒▒▒": digit = 7; break;
                case "★★★★★★★★☆☆": digit = 8; break;
                case "████████▒▒": digit = 8; break;
                case "★★★★★★★★★☆": digit = 9; break;
                case "█████████▒": digit = 9; break;
                case "★★★★★★★★★★": digit = 10; break;
                case "██████████": digit = 10; break;
                default: digit = 1; break;
            }
            return digit;
        }

        private void EditForm_Load(object sender, EventArgs e)
        {
            columnData.Clear();
            for (int i = 0; i < mainForm.columnsInfo.Count; i++)
            {
                CreateHeaderLabel(i);
                GetControlByType(mainForm.columnsInfo[i].Type, i);
                EditPanel.Controls.Add(columnData[i]);
            }
            if (EditMode)
            {
                PushDataIntoCreatedControls(GetDataFromDatabase(mainForm.SelectedLibLabel.Text, mainForm.columnsInfo[0].Name, 
                    mainForm.Collection.FocusedItem.Text));
                string PathToFile = String.Format(@"{0}\{1}\{2}.jpg", Environment.CurrentDirectory,
                        mainForm.ReplaceSymblos(mainForm.SelectedLibLabel.Text),
                        mainForm.ReplaceSymblos(columnData[0].Text));
                if (File.Exists(PathToFile))
                {
                    PosterImageTB.Text = PathToFile;
                    PosterImageTB_TextChanged(PosterImageTB, null);
                }
            }
            columnData[0].Tag = columnData[0].Text;
        }
        private void EditForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.Abort) e.Cancel = true;
            else FormReset();
        }
        private void EditForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F4: if (e.Alt) MessageBox.Show("Я не закрываюсь! Шутка))"); break;
            }
        }
        private void PosterImageTB_TextChanged(object sender, EventArgs e)
        {
            if (PosterImageTB.Text == "") { SaveButton.Enabled = true; return; }
            SaveButton.Enabled = false;
            if (new Regex("http://|https://|ftp://").IsMatch(PosterImageTB.Text))
                DownloadPicture(PosterImageTB.Text);           
            else 
            {
                if (new Regex(@"^(.+):(\\.*)*\.(.*)$").IsMatch(PosterImageTB.Text) && File.Exists(PosterImageTB.Text))
                {
                    try
                    {
                        using (FileStream localfileStream = File.Open(PosterImageTB.Text, 
                            FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            loadedBitmap = new Bitmap(localfileStream);                           
                            LoadingLabel.Text = "Выбран локальный файл";
                            SaveButton.Enabled = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        LoadingLabel.Text = ex.Message;
                        loadedBitmap = null;
                    }
                }
                else
                {
                    LoadingLabel.Text = "Файл не найден";
                    loadedBitmap = null;
                }
            }
        }
        void tb_TextChanged(object sender, EventArgs e)
        {
            (sender as TextBox).Text = (sender as TextBox).Text.
                Replace("<", "").Replace(">", "").Replace("|", "").Replace("/", "").Replace("\\", "");
        }
    }
}
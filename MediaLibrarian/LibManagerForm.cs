﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;

namespace MediaLibrarian
{
    public partial class LibManagerForm : Form
    {
        public LibManagerForm(MainForm FormMain)
        {
            InitializeComponent();
            mainForm = FormMain;
        }
        MainForm mainForm;
        bool Edited = false;
        public const int MaxFC = 20;
        public int FNo = -1;
        public string LibTableHeaders = "";
        List<TextBox> FieldName = new List<TextBox>();
        List<ComboBox> FieldType = new List<ComboBox>();
        List<Button> RemoveButton = new List<Button>();
        TextBox LibNameTB = new TextBox() { Size = new Size(265, 20), Location = new Point(130, 0), MaxLength = 50 };
        Point FieldPosition = new Point(5, 25);
        static string Database = "baza.db";
        SQLiteConnection connection = new SQLiteConnection(string.Format("Data Source={0};", Database));

        private int GetColumnLength(string type)
        {
            switch (type)
            {
                case "VARCHAR(128)": return 200;    //Строка
                case "TEXT": return 300;            //Текст
                case "VARCHAR(20)": return 120;     //Поле дата
                case "CHAR(20)": return 120;        //Поле дата+время
                case "CHAR(5)": return 90;          //Поле оценка (5)
                case "CHAR(10)": return 90;         //Поле оценка (10)
                case "VARCHAR(10)": return 90;      //Поле приоритет
            }
            return 120;
        }
        private string GetTypeString(int index)
        {
            switch (index)
            {
                case 0: return "VARCHAR(128)";      //Строка
                case 1: return "TEXT";              //Текст
                case 2: return "VARCHAR(20)";       //Поле дата
                case 3: return "CHAR(20)";          //Поле дата+время
                case 4: return "CHAR(5)";           //Поле оценка (5)
                case 5: return "CHAR(10)";          //Поле оценка (10)
                case 6: return "VARCHAR(10)";       //Поле приоритет
                default: return "VARCHAR(128)";
            }
        }
        private void ReadDatabase_ForLibsList()
        {
            LibsList.Items.Clear();
            SQLiteCommand GetTables = new SQLiteCommand("select name from sqlite_master where type='table' order by name;", connection);
            connection.Open();
            SQLiteDataReader ReadTables = GetTables.ExecuteReader();
            foreach (DbDataRecord table in ReadTables)
            {
                List<string> ColsList = new List<string>();
                ListViewItem Lib = new ListViewItem(table["name"].ToString());
                SQLiteCommand GetColumns = new SQLiteCommand(string.Format("pragma table_info(`{0}`);", table["name"]), connection);
                SQLiteDataReader ReadCols = GetColumns.ExecuteReader();
                foreach (DbDataRecord col in ReadCols)
                {
                    ColsList.Add(col["name"].ToString());
                }
                Lib.SubItems.Add(string.Join(", ", ColsList));
                LibsList.Items.Add(Lib);
            }
            connection.Close();
        }
        public void ReadTableFromDatabase(string TableName)
        {
            mainForm.Collection.Items.Clear();
            SQLiteCommand ReadTable = new SQLiteCommand(String.Format("select * from `{0}`", TableName), connection);
            DataTable data = new DataTable();
            connection.Open();
            SQLiteDataReader reader = ReadTable.ExecuteReader();
            data.Load(reader);
            reader.Close();
            connection.Close();
            foreach (DataRow row in data.Rows)
            {
                ListViewItem item = new ListViewItem(row[0].ToString());
                for (int i = 1; i < data.Columns.Count; i++)
                {
                    item.SubItems.Add(row[i].ToString());
                }
                mainForm.Collection.Items.Add(item);
            }
        }
        private void LibManagerForm_Load(object sender, EventArgs e)
        {
            this.Size = new Size(480, 220);
            ReadDatabase_ForLibsList();
            LibsList.TabIndex = 0;
            if (LibsList.Items.Count > 0) LibsList.Items[0].Focused = true;
        }
        private void CreateNewLibraryButton_Click(object sender, EventArgs e)
        {
            AddFieldsPanel.Controls.Clear();
            CollectionEditGB.Visible = true;
            AddFieldsPanel.Controls.Add(new Label()
            {
                Size = new Size(120, 20),
                Text = "Название библиотеки"
            });
            LibNameTB.KeyPress += new KeyPressEventHandler(this.TB_KeyPress);
            AddFieldsPanel.Controls.Add(LibNameTB);
            AddMoreFieldsButton.PerformClick();
            FieldType[0].Enabled = false;
            RemoveButton[0].Enabled = false;
            CreateNewLibraryButton.Enabled = false;
        }
        private void RemoveCollectionButton_Click(object sender, EventArgs e)
        {
            if (LibsList.SelectedItems.Count != 0)
                if (MessageBox.Show("Нажимая \"Удалить библиотеку\", \nВы осознанно принимаете решение удалить выбранную библиотеку\n(" + LibsList.FocusedItem.Text
                    + ")\nцеликом, включая все накопленные в ней элементы.\nПродолжить?", "Очень важное предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    if (mainForm.SelectedLibLabel.Text == LibsList.FocusedItem.Text)
                    {
                        mainForm.Collection.Clear();
                        mainForm.SelectedLibLabel.Text = "";
                        mainForm.ElementActionsGB.Enabled = false;
                        mainForm.StatusLabel.Text = " Была удалена библиотека \"" + LibsList.FocusedItem.Text + "\"";
                    }
                    SQLiteCommand DropTable = new SQLiteCommand(String.Format("drop table `{0}`;", LibsList.FocusedItem.Text), connection);
                    connection.Open();
                    DropTable.ExecuteNonQuery();
                    connection.Close();
                    ReadDatabase_ForLibsList();

                }
        }
        private void SaveLibraryButton_Click(object sender, EventArgs e)
        {
            if (LibNameTB.Text == "")
            {
                MessageBox.Show("Не заполнено название новой библиотеки", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            for (int i = 0; i < FieldName.Count; i++)
            {
                if (FieldName[i].Text == "" || FieldType[FieldName.IndexOf(FieldName[i])].SelectedIndex == -1)
                { MessageBox.Show("Пожалуйста, заполните все поля", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                for (int j = i + 1; j < FieldName.Count; j++)
                {
                    if (FieldName[i].Text == FieldName[j].Text)
                    { MessageBox.Show("Пожалуйста, воздержитесь от одинаковых имен для полей", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                }
            }
            string query = String.Format("create table `{0}` (", LibNameTB.Text);
            foreach (var item in FieldName)
            {
                query += "`" + item.Text + "` " + GetTypeString(FieldType[FieldName.IndexOf(item)].SelectedIndex);
                if (FieldName.IndexOf(item) == 0)
                {
                    query += " NOT NULL UNIQUE";
                }
                if (FieldName.IndexOf(item) != FieldName.Count - 1)
                {
                    query += ", ";
                }
            }
            query += ");";

            SQLiteCommand CreateTable = new SQLiteCommand(query, connection);
            connection.Open();
            SQLiteCommand VerifyName = new SQLiteCommand("select name from sqlite_master where type='table' order by name;", connection);
            SQLiteDataReader ReadTables = VerifyName.ExecuteReader();
            foreach (DbDataRecord table in ReadTables)
            {
                if (table["name"].ToString().ToLower().Equals(LibNameTB.Text.ToLower()))
                {
                    MessageBox.Show("Библиотека с таким именем уже существует", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    connection.Close(); return;
                }
            }
            CreateTable.ExecuteNonQuery();
            connection.Close();
            ReadDatabase_ForLibsList();
            CollectionEditGB.Visible = false;
            CreateNewLibraryButton.Enabled = true;
            RemoveCollectionButton.Enabled = true;
            FormReset();
            Edited = false;
        }
        private void AddMoreFieldsButton_Click(object sender, EventArgs e)
        {
            if (FieldName.Count > 20)
            {
                MessageBox.Show("Простите, но больше полей добавть нельзя!");
                return;
            }
            FNo = FieldName.Count;
            FieldName.Add(new TextBox()
            {
                Size = new Size(210, 20),
            });
            AddFieldsPanel.Controls.Add(FieldName[FNo]);
            FieldName[FNo].KeyPress += new KeyPressEventHandler(this.TB_KeyPress);

            FieldType.Add(new ComboBox()
            {
                Size = new Size(150, 21),
                DropDownStyle = ComboBoxStyle.DropDownList,
            });
            FieldType[FNo].Items.AddRange(new object[] {"Строка", "Текст", "Поле \"Дата\"",
            "Поле \"Дата + Время\"", "Поле \"Оценка (5)\"", "Поле \"Оценка (10)\"", "Поле \"Приоритет\""});
            AddFieldsPanel.Controls.Add(FieldType[FNo]);
            FieldType[FNo].SelectedIndex = 0;

            RemoveButton.Add(new Button()
            {
                Size = new Size(20, 21),
                Text = "-",
                Tag = FNo.ToString(),
                TabStop = true
            });
            RemoveButton[FNo].Click += new System.EventHandler(this.RemoveButton_Click);
            AddFieldsPanel.Controls.Add(RemoveButton[FNo]);
            Edited = true;
        }
        private void RemoveButton_Click(object sender, EventArgs e)
        {
            if (FieldName.Count <= 1)
            {
                MessageBox.Show("Это поле удалять нельзя!");
                return;
            }
            int In = RemoveButton.IndexOf(sender as Button);//int.Parse((sender as Button).Tag.ToString());
            AddFieldsPanel.Controls.Remove(FieldName[In]);
            FieldName.Remove(FieldName[In]);
            AddFieldsPanel.Controls.Remove(FieldType[In]);
            FieldType.Remove(FieldType[In]);
            AddFieldsPanel.Controls.Remove(RemoveButton[In]);
            RemoveButton.Remove(RemoveButton[In]);
        }
        private void LibsList_ItemActivate(object sender, EventArgs e)
        {
            mainForm.Collection.Clear();
            mainForm.SelectedLibLabel.Text = LibsList.FocusedItem.Text;
            SQLiteCommand GetColumns = new SQLiteCommand(string.Format("pragma table_info('{0}');", LibsList.FocusedItem.Text), connection);
            connection.Open();
            SQLiteDataReader ReadCols = GetColumns.ExecuteReader();
            foreach (DbDataRecord col in ReadCols)
            {
                mainForm.Collection.Columns.Add(col["name"].ToString(), GetColumnLength(col["type"].ToString()));
            }
            connection.Close();
            ReadTableFromDatabase(LibsList.FocusedItem.Text);
            this.Close();
            mainForm.ElementActionsGB.Enabled = true;
        }
        private void LibManagerForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete: RemoveCollectionButton.PerformClick(); break;
                case Keys.Escape: FormReset(); break;
            }
        }
        private void LibManagerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Edited)
            {
                if (MessageBox.Show("Выйти без сохранения?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    Edited = false;
                    FormReset();
                }
                else e.Cancel = true;
            }
            else FormReset();
        }
        private void TB_KeyPress(object sender, KeyPressEventArgs e)
        {
            /*if (e.KeyChar=)
                e.Handled = true;*/
        }
        private void FormReset()
        {
            AddFieldsPanel.Controls.Clear();
            CollectionEditGB.Visible = false;
            LibNameTB.Text = "";
            FieldName.Clear();
            FieldType.Clear();
            RemoveButton.Clear();
            FieldPosition = new Point(5, 25);
            CreateNewLibraryButton.Enabled = true;
            RemoveCollectionButton.Enabled = true;
        }
    }
}
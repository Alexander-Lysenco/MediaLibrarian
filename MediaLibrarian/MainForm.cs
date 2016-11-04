﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MediaLibrarian
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            libManagerForm = new LibManagerForm(this);
            editForm = new EditForm(this);
            settingsForm = new SettingsForm(this);
        }
        LibManagerForm libManagerForm;
        EditForm editForm;
        SettingsForm settingsForm;
        public List<Category> columnsInfo = new List<Category>();
        int LO;     //LocationOffset

        void LoadItemInfo()
        {
            LO = 5;

        }




        #region Buttons
        private void SelectCollectionButton_Click(object sender, EventArgs e)
        {
            libManagerForm.ShowDialog();
        }
        private void AddElementButton_Click(object sender, EventArgs e)
        {
            editForm.ShowDialog();
        }
        private void Edit_Click(object sender, EventArgs e)
        {
            if (Collection.SelectedItems.Count > 0)
            {
                editForm.EditMode = true;
                editForm.ShowDialog();
            }
            else
            {
                StatusLabel.Text = "Не выбран элемент для редактирования";
            }
        }
        private void DeleteElementButton_Click(object sender, EventArgs e)
        {
            if (Collection.SelectedItems.Count > 0)
            {
                editForm.DeleteItem(Collection.Columns[0].Text, Collection.SelectedItems[0].Text);
            }
            else
            {
                StatusLabel.Text = "Не выбран элемент для удаления";
            }
        }
        private void SearchButton_Click(object sender, EventArgs e)
        {
            SearchButton.PerformClick();
        }
        #endregion
        #region FileTSM
        private void OpenLibTSMI_Click(object sender, EventArgs e)
        {
            SelectCollectionButton.PerformClick();
        }

        private void CreateLibTSMI_Click(object sender, EventArgs e)
        {
            libManagerForm.Edited = true;
            libManagerForm.ShowDialog();
        }

        private void ClearLibTSMI_Click(object sender, EventArgs e)
        {

        }

        private void CloseAppTSMI_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        #endregion
        #region EditTSM
        private void AddElementTSMI_Click(object sender, EventArgs e)
        {
            AddElementButton.PerformClick();
        }
        private void EditElementTSMI_Click(object sender, EventArgs e)
        {
            EditElementButton.PerformClick();
        }
        private void DeleteElementTSMI_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Удалить элемент \"" + Collection.FocusedItem.Text + "\"?", "Подтверждение удаления элемента",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                DeleteElementButton.PerformClick();
        }
        private void FindElementTSMI_Click(object sender, EventArgs e)
        {
            SearchButton.PerformClick();
        }
        #endregion
        #region ViewTSM
        private void AutoSortingTSMI_Click(object sender, EventArgs e)
        {

        }
        private void FullScreenTSMI_Click(object sender, EventArgs e)
        {
            if (FullScreenTSMI.Checked) WindowState = FormWindowState.Maximized;
            else WindowState = FormWindowState.Normal;
        }
        private void PreferencesTSMI_Click(object sender, EventArgs e)
        {
            settingsForm.ShowDialog();
        }
        #endregion
        #region HelpTSM
        private void HelpTSMI_Click(object sender, EventArgs e)
        {

        }

        private void AboutTSMI_Click(object sender, EventArgs e)
        {

        }
        #endregion

        private void PosterBox_MouseClick(object sender, MouseEventArgs e)
        {
            PictureViewer PV = new PictureViewer();
            PV.ImageBox.Image = PosterBox.Image;
            PV.Show();
        }

        private void Collection_ItemActivate(object sender, EventArgs e)
        {
            EditElementButton.PerformClick();
        }
        private void Collection_SelectedIndexChanged(object sender, EventArgs e)
        {
            TitleLabel.Text = Collection.FocusedItem.Text;
            TitleHeaderLabel.Text = Collection.Columns[0].Text;
            try
            {
                using (FileStream fs = File.OpenRead(String.Format(@"{0}\{1}\{2}.jpg", Environment.CurrentDirectory,
                    ReplaceSymblos(SelectedLibLabel.Text),
                    ReplaceSymblos(Collection.FocusedItem.Text))))
                { 
                    PosterBox.Image = Image.FromStream(fs);
                }
            }
            catch (Exception)
            {
                PosterBox.Image = null;
            }
            LoadItemInfo();
        }
        public string ReplaceSymblos(string str)
        {
            str = str.Replace(":", "꞉").Replace("*", "˟").Replace("?", "‽").Replace("\"", "ʺ");
            return str;
        }


        void CreateHeader()
        {
            Label HeaderLabel = new Label() 
            {
                Location = new Point(3, ),
            };
        }
        void CreateStringLabel()
        { }
        void CreateTextLabel()
        { }
        void CreateDateTimeLabel()
        { }
        void CreateMarkLabel()
        { }
        void CreatePriorityLabel()
        { }
    }
}

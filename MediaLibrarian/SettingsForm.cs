using System;
using System.Windows.Forms;

namespace MediaLibrarian
{
    public partial class SettingsForm : Form
    {
        public SettingsForm(MainForm FormMain)
        {
            InitializeComponent();
            MainForm = FormMain;
        }
        MainForm MainForm;
        Settings Preferences;

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            Preferences = new Settings{
                RememberLastLibrary = rememberLastLibraryChk.Checked,
                FocusFirstItem = focusFirstItemChk.Checked,
                CropMaxViewSize = cropMaxViewSizeChk.Checked,
                PicMaxWidth = picMaxWidthNUD.Value,
                PicMaxHeight = picMaxHeightNUD.Value,
                StartFullScreen = fullScreenStartChk.Checked,
                AutoSortByName =  autoSortByNameChk.Checked,
                SelectedTheme = selectThemeCB.Text, 
                FormCaptionText = formCaptionTB.Text,
                MainColor = mainColorLabel.ForeColor,
                MainFont = mainFontLabel.Font};
            XmlManager.Serialize(Preferences);
            hintLabel.Text = "��������� ��������� �������.";
        }

        private void OK_Button_Click(object sender, EventArgs e)
        {
            applyButton.PerformClick();
            cancelButton.PerformClick();
        }

        private void mainColorLabel_Click(object sender, EventArgs e)
        {
            if (headerColorDialog.ShowDialog() == DialogResult.OK) 
            {
                mainColorLabel.Text = headerColorDialog.Color.Name;
                mainColorLabel.ForeColor = headerColorDialog.Color;
            }
        }

        private void mainFontLabel_Click(object sender, EventArgs e)
        {
            if(headerFontDialog.ShowDialog()==DialogResult.OK)
            {
                mainFontLabel.Text = headerFontDialog.Font.Name+"\n(�������� ���������)";
                mainFontLabel.Font = headerFontDialog.Font;
            }
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            toolTip.SetToolTip(rememberLastLibraryChk, "��� ������ � ��������� ����� ������������� \n����������� ��������� �������� ����� ����������");
            toolTip.SetToolTip(focusFirstItemChk, "����� � ��������� ��������� ����������, � � ��� ���� ��������, \n����� ������������� ������� ������ �������, � ����� ���������� ���������� � ���");
            toolTip.SetToolTip(cropMaxViewSizeChk, "�� ��������� ����������� �������� ���������� ���������� \n������ ������, � ����� ��������� ������ ��� ����. \n�� ������ ���������� ������ ����������� ��������.");
            toolTip.SetToolTip(fullScreenStartChk, "�����������, ���� ������, ����� ��� ������� \n��������� ��������������� �� ���� �����");
            toolTip.SetToolTip(autoSortByNameChk, "��� �����-���� ���������� � ������� ���������, \n��� ������ ����� ������������� ������������� �� ������� �������.");
            toolTip.SetToolTip(selectThemeLabel, "����� �������� ����� ����������� �������� �������� ��������� \n(�������� �����, ����� ����������, �������������� ��������).\n�� ������ ������� �������� ����� �� ������������ ���������.");
            toolTip.SetToolTip(fromCaptionLabel, "�� ������ ���������� ����������� ��������� ���������.\n���� ����� ������� ����������� ��������� - \"�����-������������\"");
            toolTip.SetToolTip(colorSelectLabel, "���������� �� ��������� ���� ����������, ����� ������� \n���� ����������� ��������� ������ �������� � �������� �����.\n����� ������� ����, ������� �� ������� ������ �� ���� �������.\n��� �������� ����������, ��� ��� ����� ��������� � ���� \n��������� ���� ����.");
            toolTip.SetToolTip(fontSelectLabel, "����� ������� ����� ����������� ����� �������� � �������� ����� \n(��� �������� - ��� ������ ������� �������, � �������� ����� \n������������ �������� ������� ����� � ��������).\n �������� ��������: �� ��� ������ ������������ ���������!");
            //toolTip.SetToolTip()
            //toolTip.SetToolTip();
            //toolTip.SetToolTip();
        }
    }
}
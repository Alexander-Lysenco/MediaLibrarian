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
            Preferences = new Settings(
                rememberLastLibraryChk.Checked,
                focusFirstItemChk.Checked,
                cropMaxViewSizeChk.Checked,
                picMaxWidthNUD.Value,
                picMaxHeightNUD.Value,
                fullScreenStartChk.Checked,
                autoSortByNameChk.Checked,
                selectThemeCB.Text, 
                formCaptionTB.Text,
                mainColorLabel.ForeColor,
                mainFontLabel.Font);
            Preferences.Serialize();
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
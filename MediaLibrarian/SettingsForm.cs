using System;
using System.Drawing;
using System.Windows.Forms;

namespace MediaLibrarian
{
    public partial class SettingsForm : Form
    {
        public SettingsForm(MainForm formMain)
        {
            InitializeComponent();
            _mainForm = formMain;
        }
        MainForm _mainForm;

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            _mainForm.Preferences = new Settings
            {
                RememberLastLibrary = rememberLastLibraryChk.Checked,
                LastLibraryName = _mainForm.SelectedLibLabel.Text,
                FocusFirstItem = focusFirstItemChk.Checked,
                CropMaxViewSize = cropMaxViewSizeChk.Checked,
                PicMaxWidth = picMaxWidthNUD.Value,
                PicMaxHeight = picMaxHeightNUD.Value,
                StartFullScreen = fullScreenStartChk.Checked,
                AutoSortByName = autoSortByNameChk.Checked,
                SelectedTheme = selectThemeCB.Text,
                FormCaptionText = formCaptionTB.Text,
                MainColor = mainColorLabel.ForeColor.ToArgb(),
                MainFont = new SFont(mainFontLabel.Font.FontFamily.Name, mainFontLabel.Font.Size, mainFontLabel.Font.Style)
            };
            XmlManager.Serialize(_mainForm.Preferences);
            hintLabel.Text = "��������� ��������� �������.";
            _mainForm.TitleLabel.ForeColor = _mainForm.SelectedLibLabel.ForeColor =
                _mainForm.ElementCount.ForeColor = Color.FromArgb(_mainForm.Preferences.MainColor);
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
            headerFontDialog.Font = mainFontLabel.Font;
            if(headerFontDialog.ShowDialog()==DialogResult.OK)
            {
                mainFontLabel.Text = headerFontDialog.Font.Name+"\n(�������� ���������)";
                mainFontLabel.Font = headerFontDialog.Font;
            }
        }

        public void RestoreSettings(Settings Preferences)
        {
            rememberLastLibraryChk.Checked = Preferences.RememberLastLibrary;
            _mainForm.SelectedLibLabel.Text = Preferences.LastLibraryName;
            focusFirstItemChk.Checked = Preferences.FocusFirstItem;
            cropMaxViewSizeChk.Checked = Preferences.CropMaxViewSize;
            picMaxWidthNUD.Value = Preferences.PicMaxWidth;
            picMaxHeightNUD.Value = Preferences.PicMaxHeight;
            fullScreenStartChk.Checked = Preferences.StartFullScreen;
            autoSortByNameChk.Checked = Preferences.AutoSortByName;
            selectThemeCB.Text = Preferences.SelectedTheme;
            formCaptionTB.Text = Preferences.FormCaptionText;
            mainColorLabel.ForeColor = Color.FromArgb(Preferences.MainColor);
            mainColorLabel.Text = mainColorLabel.ForeColor.Name;
            mainFontLabel.Font = new Font(Preferences.MainFont.FontFamilyName,
                Preferences.MainFont.FontSize, Preferences.MainFont.FontStyle);
            mainFontLabel.Text = Preferences.MainFont.FontFamilyName + "\n(�������� ���������)";
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

            try
            {
                RestoreSettings(_mainForm.Preferences);
            }
            catch (InvalidOperationException)
            {
                hintLabel.Text = "��������� ���� �������� �� ���������";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
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
        public void SaveSettings() 
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
                FormCaptionText = formCaptionTB.Text,
                ThemeColor1 = themeColor1CB.Text,
                ThemeColor2 = themeColor2CB.Text,
                MainColor = mainColorCB.Text,
                FontColor = fontColorCB.Text,
                MainFont = new SFont(mainFontLabel.Font.FontFamily.Name, mainFontLabel.Font.Size, mainFontLabel.Font.Style)
            };
            if (formCaptionTB.Text == "") _mainForm.Preferences.FormCaptionText = "�����-������������";
            XmlManager.Serialize(_mainForm.Preferences);
            hintLabel.Text = "��������� ��������� �������.";
            _mainForm.TitleLabel.ForeColor = _mainForm.SelectedLibLabel.ForeColor =
                _mainForm.ElementCount.ForeColor = Color.FromName(_mainForm.Preferences.MainColor);            
            _mainForm.Refresh();
            _mainForm.InitFont();
        }
        public void RestoreSettings(Settings Preferences)
        {
            rememberLastLibraryChk.Checked = Preferences.RememberLastLibrary;
            _mainForm.SelectedLibLabel.Text = Preferences.LastLibraryName;
            focusFirstItemChk.Checked = Preferences.FocusFirstItem;
            cropMaxViewSizeChk.Checked = Preferences.CropMaxViewSize;
            try
            {
                picMaxWidthNUD.Value = Preferences.PicMaxWidth;
                picMaxHeightNUD.Value = Preferences.PicMaxHeight;
            }
            catch (Exception)
            {
                picMaxWidthNUD.Value = 720;
                picMaxHeightNUD.Value = 720;
            }
            fullScreenStartChk.Checked = Preferences.StartFullScreen;
            autoSortByNameChk.Checked = Preferences.AutoSortByName;
            formCaptionTB.Text = Preferences.FormCaptionText;
            themeColor1CB.Text = Preferences.ThemeColor1;
            themeColor2CB.Text = Preferences.ThemeColor2;
            mainColorCB.Text = Preferences.MainColor;
            fontColorCB.Text = Preferences.FontColor;
            mainFontLabel.Font = new Font(Preferences.MainFont.FontFamilyName,
                Preferences.MainFont.FontSize, Preferences.MainFont.FontStyle);
            mainFontLabel.Text = Preferences.MainFont.FontFamilyName + "\n(�������� ���������)";
        }
        #region Buttons
        private void OK_Button_Click(object sender, EventArgs e)
        {
            SaveSettings();
            cancelButton.PerformClick();
        }
        private void CancelButton_Click(object sender, EventArgs e)
        {
            Hide();
        }
        private void ApplyButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }
        #endregion
        #region ColorsCB
        private void themeColor1CB_SelectedIndexChanged(object sender, EventArgs e)
        {
            themeColor1PB.BackColor = Color.FromName(themeColor1CB.Text);
        }
        private void themeColor2CB_SelectedIndexChanged(object sender, EventArgs e)
        {
            themeColor2PB.BackColor = Color.FromName(themeColor2CB.Text);
        }
        
        private void mainColorCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            mainColorPB.BackColor = Color.FromName(mainColorCB.Text);
        }
        private void fontColorCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            fontColorPB.BackColor = Color.FromName(fontColorCB.Text);
        }
        #endregion
        private void mainFontLabel_Click(object sender, EventArgs e)
        {
            headerFontDialog.Font = mainFontLabel.Font;
            if(headerFontDialog.ShowDialog()==DialogResult.OK)
            {
                mainFontLabel.Text = headerFontDialog.Font.Name+"\n(�������� ���������)";
                mainFontLabel.Font = headerFontDialog.Font;
            }
        }
        private void SettingsForm_Load(object sender, EventArgs e)
        {
            string[] colors = Enum.GetNames(typeof(KnownColor));            
            foreach (string color in colors)
            {
                if (((Color.FromName(color)).IsSystemColor == false) && ((Color.FromName(color)).Name != Color.Transparent.Name))
                {
                    themeColor1CB.Items.Add(color);
                    themeColor2CB.Items.Add(color);
                    mainColorCB.Items.Add(color);
                    fontColorCB.Items.Add(color);
                }
            }
            toolTip.SetToolTip(rememberLastLibraryChk, "��� ������ � ��������� ����� ������������� \n����������� ��������� �������� ����� ����������");
            toolTip.SetToolTip(focusFirstItemChk, "����� � ��������� ��������� ����������, � � ��� ���� ��������, \n����� ������������� ������� ������ �������, � ����� ���������� ���������� � ���");
            toolTip.SetToolTip(cropMaxViewSizeChk, "�� ��������� ����������� �������� ���������� ���������� \n������ ������, � ����� ��������� ������ ��� ����. \n�� ������ ���������� ������ ����������� ��������");
            toolTip.SetToolTip(fullScreenStartChk, "�����������, ���� ������, ����� ��� ������� \n��������� ��������������� �� ���� �����");
            toolTip.SetToolTip(autoSortByNameChk, "��� �����-���� ���������� � ������� ���������, \n��� ������ ����� ������������� ������������� �� ������� �������");
            toolTip.SetToolTip(fromCaptionLabel, "�� ������ ���������� ����������� ��������� ���������.\n���� ����� ������� ����������� ��������� - \"�����-������������\"\n���� �� ������ ������, �������� ������");
            toolTip.SetToolTip(backgroundGB, "����� �������� ����� ����������� �������� ���� ���������.\n������������ ����� ����������� ��������� ���� ����������� ������,\n������� ����� ������� �� ������ �����");
            toolTip.SetToolTip(mainColorLabel, "���������� �� ��������� ���� ����������, ����� ������� \n���� ����������� ��������� ������ �������� � �������� �����.\n������ � ����, ��� ��������� ���� ���� ����� �� �������������\n� ���������� ������� ����");
            toolTip.SetToolTip(fontColorLabel, "���� �� ������� ������ ����� ��� ����, � ��������� ��������\n��������� �� �����, ����� ������ �� ���� ���������");
            toolTip.SetToolTip(fontSelectLabel, "����� ������� ����� ����������� ����� �������� � �������� ����� \n(��� �������� - ��� ������ ������� �������, � �������� ����� \n������������ �������� ������� ����� � ��������).\n �������� ��������: �� ��� ������ ������������ ���������!");

            screenResolutionLabel.Text = String.Format("���������� ������: {0}�{1}",
                SystemInformation.PrimaryMonitorSize.Width, SystemInformation.PrimaryMonitorSize.Height);
            picMaxWidthNUD.Maximum = SystemInformation.PrimaryMonitorSize.Width;
            picMaxHeightNUD.Maximum = SystemInformation.PrimaryMonitorSize.Height;
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
                MessageBox.Show(ex.Message, "������ �������� ����� ��������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
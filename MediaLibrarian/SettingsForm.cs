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
                FormCaptionText = formCaptionTB.Text,
                ThemeColor = themeColorCB.Text,
                MainColor = mainColorCB.Text,
                FontColor = fontColorCB.Text,
                MainFont = new SFont(mainFontLabel.Font.FontFamily.Name, mainFontLabel.Font.Size, mainFontLabel.Font.Style)
            };
            XmlManager.Serialize(_mainForm.Preferences);
            hintLabel.Text = "��������� ��������� �������.";
            _mainForm.TitleLabel.ForeColor = _mainForm.SelectedLibLabel.ForeColor =
                _mainForm.ElementCount.ForeColor = Color.FromName(_mainForm.Preferences.MainColor);
            _mainForm.BackColor = Color.FromName(_mainForm.Preferences.ThemeColor);
            _mainForm.InitFont();
        }

        private void OK_Button_Click(object sender, EventArgs e)
        {
            applyButton.PerformClick();
            cancelButton.PerformClick();
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
            formCaptionTB.Text = Preferences.FormCaptionText;
            themeColorCB.Text = Preferences.ThemeColor;
            mainColorCB.Text = Preferences.MainColor;
            fontColorCB.Text = Preferences.FontColor;
            mainFontLabel.Font = new Font(Preferences.MainFont.FontFamilyName,
                Preferences.MainFont.FontSize, Preferences.MainFont.FontStyle);
            mainFontLabel.Text = Preferences.MainFont.FontFamilyName + "\n(�������� ���������)";
        }
        private void SettingsForm_Load(object sender, EventArgs e)
        {
            string[] colors = Enum.GetNames(typeof(KnownColor));            
            foreach (string color in colors)
            {
                if (((Color.FromName(color)).IsSystemColor == false) && ((Color.FromName(color)).Name != Color.Transparent.Name))
                {
                    themeColorCB.Items.Add(color);
                    mainColorCB.Items.Add(color);
                    fontColorCB.Items.Add(color);
                }
            }
            toolTip.SetToolTip(rememberLastLibraryChk, "��� ������ � ��������� ����� ������������� \n����������� ��������� �������� ����� ����������");
            toolTip.SetToolTip(focusFirstItemChk, "����� � ��������� ��������� ����������, � � ��� ���� ��������, \n����� ������������� ������� ������ �������, � ����� ���������� ���������� � ���");
            toolTip.SetToolTip(cropMaxViewSizeChk, "�� ��������� ����������� �������� ���������� ���������� \n������ ������, � ����� ��������� ������ ��� ����. \n�� ������ ���������� ������ ����������� ��������.");
            toolTip.SetToolTip(fullScreenStartChk, "�����������, ���� ������, ����� ��� ������� \n��������� ��������������� �� ���� �����");
            toolTip.SetToolTip(autoSortByNameChk, "��� �����-���� ���������� � ������� ���������, \n��� ������ ����� ������������� ������������� �� ������� �������.");
            toolTip.SetToolTip(themeColorLabel, "����� �������� ����� ����������� �������� �������� ��������� \n(�������� �����, ����� ����������, �������������� ��������).\n�� ������ ������� �������� ����� �� ������������ ���������.");
            toolTip.SetToolTip(fromCaptionLabel, "�� ������ ���������� ����������� ��������� ���������.\n���� ����� ������� ����������� ��������� - \"�����-������������\"");
            toolTip.SetToolTip(mainColorLabel, "���������� �� ��������� ���� ����������, ����� ������� \n���� ����������� ��������� ������ �������� � �������� �����.\n����� ������� ����, ������� �� ������� ������ �� ���� �������.\n��� �������� ����������, ��� ��� ����� ��������� � ���� \n��������� ���� ����.");
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

        private void themeColorCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            themeColorPB.BackColor = Color.FromName(themeColorCB.Text);
        }

        private void mainColorCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            mainColorPB.BackColor = Color.FromName(mainColorCB.Text);
        }

        private void fontColorCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            fontColorPB.BackColor = Color.FromName(fontColorCB.Text);
        }
    }
}
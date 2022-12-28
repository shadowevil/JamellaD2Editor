using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;
using Microsoft.Web.WebView2.Core;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using TextBox = System.Windows.Forms.TextBox;
using System.Xml.Linq;
using ToolTip = System.Windows.Forms.ToolTip;
using TrackBar = System.Windows.Forms.TrackBar;

namespace Jamella_Diablo_2_Editor
{
    public partial class Form1 : Form
    {
        public CharacterHandling charHandle;

        public Form1()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            webView21.Source = new Uri(Directory.GetCurrentDirectory() + "\\Data\\video.html");
            webView21.NavigationCompleted += WebView21_NavigationCompleted;
            webView21.EnsureCoreWebView2Async();

            btnAddStr.MouseEnter += BtnAdd_MouseEnter;
            btnAddDex.MouseEnter += BtnAdd_MouseEnter;
            btnAddVit.MouseEnter += BtnAdd_MouseEnter;
            btnAddEne.MouseEnter += BtnAdd_MouseEnter;

            btnAddStr.MouseLeave += BtnAdd_MouseLeave;
            btnAddDex.MouseLeave += BtnAdd_MouseLeave;
            btnAddVit.MouseLeave += BtnAdd_MouseLeave;
            btnAddEne.MouseLeave += BtnAdd_MouseLeave;

            btnAddStr.MouseDown += BtnAdd_MouseDown;
            btnAddDex.MouseDown += BtnAdd_MouseDown;
            btnAddVit.MouseDown += BtnAdd_MouseDown;
            btnAddEne.MouseDown += BtnAdd_MouseDown;

            btnAddStr.MouseUp += BtnAdd_MouseUp;
            btnAddDex.MouseUp += BtnAdd_MouseUp;
            btnAddVit.MouseUp += BtnAdd_MouseUp;
            btnAddEne.MouseUp += BtnAdd_MouseUp;

            charHandle = new CharacterHandling(this);
            lblStatus.Text = "Jamella Initalized...";
        }

        private void BtnAdd_MouseUp(object? sender, EventArgs e)
        {
            if (sender == null) return;
            PictureBox? box = sender as PictureBox;
            if (box == null) return;
            box.Image = Jamella_Diablo_2_Editor.Properties.Resources.DefaultButton;
        }

        private void BtnAdd_MouseDown(object? sender, EventArgs e)
        {
            if (sender == null) return;
            PictureBox? box = sender as PictureBox;
            if (box == null) return;
            box.Image = Jamella_Diablo_2_Editor.Properties.Resources.ButtonClick;
        }

        private void BtnAdd_MouseEnter(object? sender, EventArgs e)
        {
            if (sender == null) return;
            PictureBox? box = sender as PictureBox;
            if (box == null) return;
            box.Image = Jamella_Diablo_2_Editor.Properties.Resources.HoverButton;
        }
        private void BtnAdd_MouseLeave(object? sender, EventArgs e)
        {
            if (sender == null) return;
            PictureBox? box = sender as PictureBox;
            if (box == null) return;
            box.Image = Jamella_Diablo_2_Editor.Properties.Resources.DefaultButton;
        }

        private async void WebView21_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            // Prevent right clicking of anything
            string script2 = "document.addEventListener('contextmenu', event => event.preventDefault());";
            // Bypass chromium no auto-play unless muted feature
            string script = "document.getElementById('vid').muted = false;";
            // Prevent any kind of interaction with the web viewer
            string script3 = "document.addEventListener('keydown', (e) => { e = e || window.event;" + 
                                                                                    "e.preventDefault();" +
                                                                         "});";
            await webView21.CoreWebView2.ExecuteScriptAsync(script3);
            await webView21.CoreWebView2.ExecuteScriptAsync(script);
            await webView21.CoreWebView2.ExecuteScriptAsync(script2);
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "D2S Files|*.d2s";
            dialog.Title = "Select a Diablo II Resurrected Save File";
            dialog.InitialDirectory = GetDefaultDirectory();
            dialog.RestoreDirectory = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                charHandle.openFilePath = dialog.FileName;
                try
                {
                    charHandle.LoadCharacter();

                }
                catch (System.IO.IOException)
                {
                    MessageBox.Show("Error file already in use.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    charHandle.Character = null;
                    return;
                }
                charHandle.LoadCharacterData();
            }
        }

        public static string GetDefaultDirectory()
        {
            // Check if the default directory exists.
            string defaultDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Saved Games\\Diablo II Resurrected");
            if (Directory.Exists(defaultDirectory))
            {
                return defaultDirectory;
            }
            else
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
        }

        private void NumberLimit_Check(object sender, int minLimit, int maxLimit)
        {
            TextBox? textBox = sender as TextBox;
            if (textBox == null) return;
            int value;
            if (int.TryParse(textBox.Text, out value))
            {
                if (value > maxLimit)
                {
                    textBox.Text = maxLimit.ToString();
                }
                if (value < minLimit)
                {
                    textBox.Text = minLimit.ToString();
                }
            }
        }

        public void NumbersOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow only numbers, backspace, and delete
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtExperience_TextChanged(object sender, EventArgs e)
        {
            long value;
            if (long.TryParse(txtExperience.Text, out value))
            {
                if (value > 3600000000)
                {
                    txtExperience.Text = "3837739017";
                }
                if (value < 0)
                {
                    txtExperience.Text = "0";
                }
            }
        }

        private void txtLevel_TextChanged(object sender, EventArgs e)
        {
            if (txtLevel.Text.Length <= 0) return;
            ulong experience = 0;
            D2SLib.Model.Save.Attributes.ExperienceChart.TryGetValue(Convert.ToInt32(txtLevel.Text), out experience);
            txtExperience.Text = experience.ToString();
        }

        private void txtLevel_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow only numbers, backspace, and delete
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

            // Check the length of the textbox
            int length = txtLevel.Text.Length;
            if (length >= 2 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }

            // Check the value of the textbox
            int value;
            if (int.TryParse(txtLevel.Text, out value))
            {
                if (value > 99)
                {
                    txtLevel.Text = "99";
                    e.Handled = true;
                }
                if (value < 1)
                {
                    txtLevel.Text = "1";
                    e.Handled = true;
                }
            }
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            // Get the current text in the textbox
            string text = txtName.Text;

            // Check the length of the text
            if (text.Length < 2 || text.Length > 15)
            {
                // If the length is not within the allowed range, clear the textbox
                txtName.BackColor = Color.Red;
                return;
            }

            // Check the first character of the text
            if (!char.IsLetter(text[0]))
            {
                // If the first character is not a letter, clear the textbox
                txtName.BackColor = Color.Red;
                return;
            }

            // Check for more than one hyphen or underscore
            int hyphenCount = 0;
            int underscoreCount = 0;
            foreach (char c in text)
            {
                if (c == '-')
                {
                    hyphenCount++;
                }
                else if (c == '_')
                {
                    underscoreCount++;
                }
            }
            if (hyphenCount > 1 || underscoreCount > 1)
            {
                // If there are more than one hyphen or underscore, clear the textbox
                txtName.BackColor = Color.Red;
                return;
            }

            txtName.BackColor = TextBox.DefaultBackColor;
        }

        private void txtStat_TextChanged(object sender, EventArgs e)
        {
            NumberLimit_Check(sender, 0, 1023);
        }

        private void txtGold_TextChanged(object sender, EventArgs e)
        {
            NumberLimit_Check(sender, 0, (Convert.ToInt32(txtLevel.Text) * 10_000));
        }

        private void txtGoldStash_TextChanged(object sender, EventArgs e)
        {
            NumberLimit_Check(sender, 0, 2_500_000);
        }

        public void SkillTextBoxChanged(object sender, EventArgs e)
        {
            NumberLimit_Check(sender, 0, 99);
        }

        private void btnAddStr_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(lblStatPoints.Text) > 0 && Convert.ToInt32(txtStr.Text) < 1024)
            {
                lblStatPoints.Text = Convert.ToString(Convert.ToInt32(lblStatPoints.Text) - 1);
                txtStr.Text = Convert.ToString(Convert.ToInt32(txtStr.Text) + 1);
            }
        }

        private void btnAddDex_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(lblStatPoints.Text) > 0 && Convert.ToInt32(txtDex.Text) < 1024)
            {
                lblStatPoints.Text = Convert.ToString(Convert.ToInt32(lblStatPoints.Text) - 1);
                txtDex.Text = Convert.ToString(Convert.ToInt32(txtDex.Text) + 1);
            }
        }

        private void btnAddVit_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(lblStatPoints.Text) > 0 && Convert.ToInt32(txtVit.Text) < 1024)
            {
                lblStatPoints.Text = Convert.ToString(Convert.ToInt32(lblStatPoints.Text) - 1);
                txtVit.Text = Convert.ToString(Convert.ToInt32(txtVit.Text) + 1);
            }
        }

        private void btnAddEne_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(lblStatPoints.Text) > 0 && Convert.ToInt32(txtEne.Text) < 1024)
            {
                lblStatPoints.Text = Convert.ToString(Convert.ToInt32(lblStatPoints.Text) - 1);
                txtEne.Text = Convert.ToString(Convert.ToInt32(txtEne.Text) + 1);
            }
        }

        private void btnInventoryMAX_Click(object sender, EventArgs e)
        {
            txtGold.Text = Convert.ToString((Convert.ToInt32(txtLevel.Text) * 10_000));
        }

        private void btnStashMAX_Click(object sender, EventArgs e)
        {
            txtGoldStash.Text = Convert.ToString(2500000);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            charHandle.SaveCharacter();
        }

        private void activeSkillAmount_ValueChanged(object sender, EventArgs e)
        {
            PictureBox? _pb = Controls["tabController"].Controls["tabSkills"].Controls.Find("activeSkillPicture", true)[0] as PictureBox;
            TextBox? _txt = Controls["tabController"].Controls["tabSkills"].Controls.Find("activeSkillText", true)[0] as TextBox;
            TextBox? selectedSkill = Controls["tabController"].Controls["tabSkills"].Controls.Find(_pb.AccessibleDescription.Replace("pb", "txt"), true)[0] as TextBox;
            selectedSkill.Text = _txt.Text = (sender as TrackBar).Value.ToString();
        }
    }
}
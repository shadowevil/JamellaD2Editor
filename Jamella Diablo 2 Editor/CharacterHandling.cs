using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using D2SLib;
using DataRow = D2SLib.Model.Data.DataRow;
using D2SLib.Model.Save;
using static System.Windows.Forms.DataFormats;
using System.Globalization;

namespace Jamella_Diablo_2_Editor
{
    public class CharacterHandling
    {
        enum Classes
        {
            Amazon,
            Sorcoress,
            Necromancer,
            Paladin,
            Barbarian,
            Druid,
            Assassin
        };

        public D2S? Character;
        public string openFilePath = String.Empty;
        public Form1 mainForm;
        private ToolTip tooltips;

        public CharacterHandling(Form1 form1)
        {
            mainForm = form1;
            Character = null;
            tooltips = new ToolTip();
        }

        public Control? FindControlByName(Control parent, string name)
        {
            // Check if the current control has the specified name
            if (parent.Name == name)
            {
                // Return the control if it has the specified name
                return parent;
            }

            // Loop through all child controls of the current control
            foreach (Control control in parent.Controls)
            {
                // Recursively search for the control in the child control
                Control? foundControl = FindControlByName(control, name);

                // Return the control if it was found in the child control
                if (foundControl != null)
                {
                    return foundControl;
                }
            }

            // Return null if the control was not found
            return null;
        }

        public T? GetControl<T>(string t, string c) where T : Control
        {
            T? found = FindControlByName((mainForm.Controls["tabController"] as TabControl).TabPages[t], c) as T;
            if (found == null)
            {
                throw new Exception("Unable to find " + c);
            }
            return found;
        }
        
        public void setStatusLabel(string s)
        {
            if (mainForm.Controls["statusStrip1"] == null)
            {
                throw new Exception("statusStrip1 does not exist...");
            }
            if ((mainForm.Controls["statusStrip1"] as StatusStrip).Items["lblStatus"] == null)
            {
                throw new Exception("lblStatus does not exist...");
            }

            (mainForm.Controls["statusStrip1"] as StatusStrip).Items["lblStatus"].Text = s;
        }

        public void LoadCharacter()
        {
            ResetSkillPage();
            Character = null;
            Character = Core.ReadD2S(File.ReadAllBytes(openFilePath));
            setStatusLabel(Character.Name + ".d2s loaded...");
        }

        public void ResetSkillPage()
        {
            foreach (Control c in mainForm.Controls["tabController"].Controls["tabSkills"].Controls["cgbTab3"].Controls)
            {
                c.Text = "0";
                c.BackgroundImage = null;
            }
            foreach (Control c in mainForm.Controls["tabController"].Controls["tabSkills"].Controls["cgbTab2"].Controls)
            {
                c.Text = "0";
                c.BackgroundImage = null;
            }
            foreach (Control c in mainForm.Controls["tabController"].Controls["tabSkills"].Controls["cgbTab1"].Controls)
            {
                c.Text = "0";
                c.BackgroundImage = null;
            }
        }

        public void LoadCharacterData()
        {
            if (Character == null) return;

            GetControl<TextBox>("tabStats", "txtName").Text = Character.Name;
            GetControl<ComboBox>("tabStats", "cboClass").SelectedIndex = Character.ClassId;
            GetControl<TextBox>("tabStats", "txtLevel").Text = Convert.ToString(Character.Level);
            int experience = 0;
            Character.Attributes.Stats.TryGetValue("experience", out experience);
            GetControl<TextBox>("tabStats", "txtExperience").Text = Convert.ToString(experience);

            int strength = 0;
            Character.Attributes.Stats.TryGetValue("strength", out strength);
            int dexterity = 0;
            Character.Attributes.Stats.TryGetValue("dexterity", out dexterity);
            int vitality = 0;
            Character.Attributes.Stats.TryGetValue("vitality", out vitality);
            int energy = 0;
            Character.Attributes.Stats.TryGetValue("energy", out energy);
            int remainingStatPoints = 0;
            Character.Attributes.Stats.TryGetValue("statpts", out remainingStatPoints);
            int gold = 0;
            int stashGold = 0;
            Character.Attributes.Stats.TryGetValue("gold", out gold);
            Character.Attributes.Stats.TryGetValue("goldbank", out stashGold);
            int currenthp = 0;
            int maxhp = 0;
            int currentmp = 0;
            int maxmp = 0;
            int currentsp = 0;
            int maxsp = 0;
            Character.Attributes.Stats.TryGetValue("hitpoints", out currenthp);
            Character.Attributes.Stats.TryGetValue("maxhp", out maxhp);
            Character.Attributes.Stats.TryGetValue("mana", out currentmp);
            Character.Attributes.Stats.TryGetValue("maxmana", out maxmp);
            Character.Attributes.Stats.TryGetValue("stamina", out currentsp);
            Character.Attributes.Stats.TryGetValue("maxstamina", out maxsp);

            GetControl<TextBox>("tabStats", "txtStr").Text = strength.ToString();
            GetControl<TextBox>("tabStats", "txtDex").Text = dexterity.ToString();
            GetControl<TextBox>("tabStats", "txtVit").Text = vitality.ToString();
            GetControl<TextBox>("tabStats", "txtEne").Text = energy.ToString();
            GetControl<Label>("tabStats", "lblStatPoints").Text = remainingStatPoints.ToString();

            GetControl<TextBox>("tabStats", "txtGold").Text = gold.ToString();
            GetControl<TextBox>("tabStats", "txtGoldStash").Text = stashGold.ToString();

            GetControl<TextBox>("tabStats", "txtCurrentHP").Text = Convert.ToString(currenthp);
            GetControl<TextBox>("tabStats", "txtMaxHP").Text = Convert.ToString(maxhp);
            GetControl<TextBox>("tabStats", "txtCurrentMP").Text = Convert.ToString(currentmp);
            GetControl<TextBox>("tabStats", "txtMaxMP").Text = Convert.ToString(maxmp);
            GetControl<TextBox>("tabStats", "txtCurrentSP").Text = Convert.ToString(currentsp);
            GetControl<TextBox>("tabStats", "txtMaxSP").Text = Convert.ToString(maxsp);

            GetControl<CheckBox>("tabStats", "chkHardcore").Checked = Character.Status.IsHardcore;
            GetControl<CheckBox>("tabStats", "chkDied").Checked = Character.Status.IsDead;
            GetControl<CheckBox>("tabStats", "chkExpansion").Checked = Character.Status.IsExpansion;
            GetControl<CheckBox>("tabStats", "chkLadder").Checked = Character.Status.IsLadder;
            if (Character.Status.IsHardcore)
            {
                GetControl<ComboBox>("tabStats", "cboProgression").Items.Clear();
                GetControl<ComboBox>("tabStats", "cboProgression").Items.AddRange(new string[] {
                    "None",
                    "Destroyer (Normal completed)",
                    "Conqueror (Nightmare completed)",
                    "Guardian (Hell completed)",
                });
            }
            else
            {
                GetControl<ComboBox>("tabStats", "cboProgression").Items.Clear();
                GetControl<ComboBox>("tabStats", "cboProgression").Items.AddRange(new string[] {
                    "None",
                    "Slayer (Normal completed)",
                    "Champion (Nightmare completed)",
                    "Patriarch/Matriarch (Hell completed)",
                });
            }
            GetControl<ComboBox>("tabStats", "cboProgression").SelectedIndex = (Character.Progression / 5);

            ResetSkillPage();
            tooltips.RemoveAll();
            tooltips.Active = true;
            tooltips.AutoPopDelay = 0;
            tooltips.InitialDelay = 0;
            tooltips.IsBalloon = true;
            foreach (ClassSkill cs in Character.ClassSkills.Skills)
            {
                DataRow? dr = ResourceFilesData.Instance.MetaData.SkillTreeData.GetById((int)cs.Id);
                string skillname = dr["skill"].Value;
                string skilldesc = dr["Description"].Value;
                if (skilldesc.Contains("\\n"))
                {
                    string[] strs = skilldesc.Split("\\n");
                    for (int i = 0; i < strs.Length; i++) strs[i] = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(strs[i]);
                    skilldesc = string.Join("\n", strs);
                }
                int skillPage = Convert.ToInt32(dr["SkillPage"].Value);
                PictureBox? pb = null;
                TextBox? txt = null;

                pb = GetAvailablePictureBox("cgbTab" + skillPage);
                txt = GetControl<TextBox>("tabSkills", pb.Name.Replace("pb", "txt"));
                txt.MaxLength = 2;
                txt.KeyPress += mainForm.NumbersOnly_KeyPress;
                txt.TextChanged += mainForm.SkillTextBoxChanged;
                tooltips.SetToolTip(pb, skillname + "\n\n" + skilldesc);
                tooltips.SetToolTip(txt, skillname + "\n\n" + skilldesc);

                if (!File.Exists(Directory.GetCurrentDirectory() + "\\Data\\" + ((Classes)Character.ClassId).ToString() + "\\" + cs.Id + ".png"))
                    throw new Exception("Some shit is missing!");

                pb.BackgroundImage = Image.FromFile(Directory.GetCurrentDirectory() + "\\Data\\" + ((Classes)Character.ClassId).ToString() + "\\" + cs.Id + ".png");
                pb.MouseEnter += (sender, e) => {
                    if(File.Exists(Directory.GetCurrentDirectory() + "\\Data\\" + ((Classes)Character.ClassId).ToString() + "\\" + cs.Id + "_h.png"))
                        pb.BackgroundImage = Image.FromFile(Directory.GetCurrentDirectory() + "\\Data\\" + ((Classes)Character.ClassId).ToString() + "\\" + cs.Id + "_h.png");
                };

                pb.MouseMove += (sender, e) =>
                {
                    Cursor.Current = Cursors.Hand;
                };

                pb.Click += (sender, e) => {
                    PictureBox? s_pb = (sender as PictureBox);
                    TextBox? s_txt = GetControl<TextBox>("tabSkills", s_pb.Name.Replace("pb", "txt"));

                    PictureBox? _pb = GetControl<PictureBox>("tabSkills", "activeSkillPicture");
                    TextBox? _txt = GetControl<TextBox>("tabSkills", "activeSkillText");

                    TrackBar? tb = GetControl<TrackBar>("tabSkills", "activeSkillAmount");

                    _pb.BackgroundImage = s_pb.BackgroundImage;
                    _pb.AccessibleDescription = s_pb.Name;
                    _txt.Text = s_txt.Text;
                    tb.Maximum = 99;
                    tb.Minimum = 0;
                    tb.Value = Convert.ToInt32(_txt.Text);
                };

                pb.MouseLeave += (sender, e) => {
                    if (File.Exists(Directory.GetCurrentDirectory() + "\\Data\\" + ((Classes)Character.ClassId).ToString() + "\\" + cs.Id + ".png"))
                        (sender as PictureBox).BackgroundImage = Image.FromFile(Directory.GetCurrentDirectory() + "\\Data\\" + ((Classes)Character.ClassId).ToString() + "\\" + cs.Id + ".png");
                };
                txt.Text = cs.Points.ToString();
            }
            PictureBox? s_pb = GetControl<PictureBox>("tabSkills", "st1_pb1");
            TextBox ? s_txt = GetControl<TextBox>("tabSkills", s_pb.Name.Replace("pb", "txt"));

            PictureBox? _pb = GetControl<PictureBox>("tabSkills", "activeSkillPicture");
            TextBox? _txt = GetControl<TextBox>("tabSkills", "activeSkillText");

            TrackBar? tb = GetControl<TrackBar>("tabSkills", "activeSkillAmount");

            _pb.BackgroundImage = s_pb.BackgroundImage;
            _pb.AccessibleDescription = s_pb.Name;
            _txt.Text = s_txt.Text;
            tb.Maximum = 99;
            tb.Minimum = 0;
            tb.Value = Convert.ToInt32(_txt.Text);

            if (Character.Attributes.Stats.ContainsKey("newskills"))
                GetControl<TextBox>("tabSkills", "txtRemainingSkills").Text = Character.Attributes.Stats["newskills"].ToString();
            else GetControl<TextBox>("tabSkills", "txtRemainingSkills").Text = "0";

            mainForm.Text = Character.Name + " | " + mainForm.Text;
        }

        public PictureBox? GetAvailablePictureBox(string groupboxname)
        {
            CustomGrpBox? cgb = GetControl<CustomGrpBox>("tabSkills", groupboxname);
            if (cgb == null) throw new Exception("tabSkills does not contain " + groupboxname);

            for(int i=1;i<=10;i++)
            {
                Control? c = cgb.Controls["st" + cgb.Name.Substring(cgb.Name.Length - 1) + "_pb" + i];
                if (c == null)
                {
                    throw new Exception("some shit went wrong again...");
                }
                if (c is PictureBox)
                {
                    PictureBox? pb = c as PictureBox;
                    if (pb == null) break;
                    if (pb.BackgroundImage == null)
                    {
                        return pb;
                    }
                }
            }
            return null;
        }

        public void SaveCharacter()
        {
            if (Character == null) return;

            Character.Name = GetControl<TextBox>("tabStats", "txtName").Text;
            Character.ClassId = (byte)GetControl<ComboBox>("tabStats", "cboClass").SelectedIndex;
            Character.Level = Convert.ToByte(GetControl<TextBox>("tabStats", "txtLevel").Text);
            Character.Attributes.Stats["level"] = Convert.ToInt32(GetControl<TextBox>("tabStats", "txtLevel").Text);
            Character.Attributes.Stats["experience"] = Convert.ToInt32(GetControl<TextBox>("tabStats", "txtExperience").Text);
            Character.Attributes.Stats["strength"] = Convert.ToInt32(GetControl<TextBox>("tabStats", "txtStr").Text);
            Character.Attributes.Stats["dexterity"] = Convert.ToInt32(GetControl<TextBox>("tabStats", "txtDex").Text);
            Character.Attributes.Stats["vitality"] = Convert.ToInt32(GetControl<TextBox>("tabStats", "txtVit").Text);
            Character.Attributes.Stats["energy"] = Convert.ToInt32(GetControl<TextBox>("tabStats", "txtEne").Text);
            Character.Attributes.Stats["statpts"] = Convert.ToInt32(GetControl<Label>("tabStats", "lblStatPoints").Text);
            Character.Attributes.Stats["gold"] = Convert.ToInt32(GetControl<TextBox>("tabStats", "txtGold").Text);
            Character.Attributes.Stats["goldbank"] = Convert.ToInt32(GetControl<TextBox>("tabStats", "txtGoldStash").Text);
            Character.Attributes.Stats["hitpoints"] = Convert.ToInt32(GetControl<TextBox>("tabStats", "txtCurrentHP").Text);
            Character.Attributes.Stats["maxhp"] = Convert.ToInt32(GetControl<TextBox>("tabStats", "txtMaxHP").Text);
            Character.Attributes.Stats["mana"] = Convert.ToInt32(GetControl<TextBox>("tabStats", "txtCurrentMP").Text);
            Character.Attributes.Stats["maxmana"] = Convert.ToInt32(GetControl<TextBox>("tabStats", "txtMaxMP").Text);
            Character.Attributes.Stats["stamina"] = Convert.ToInt32(GetControl<TextBox>("tabStats", "txtCurrentSP").Text);
            Character.Attributes.Stats["maxstamina"] = Convert.ToInt32(GetControl<TextBox>("tabStats", "txtMaxSP").Text);

            setStatusLabel("Attempting to save " + Character.Name + ".d2s...");
            byte[] saveBytes = Core.WriteD2S(Character);
            File.WriteAllBytes(openFilePath, saveBytes);
            setStatusLabel(Character.Name + ".d2s successfully saved (" + saveBytes.Length + " bytes)...");
        }
    }
}

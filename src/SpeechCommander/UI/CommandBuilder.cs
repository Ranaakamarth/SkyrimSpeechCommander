using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using sp = System.Speech.Recognition;

namespace SpeechCommander.UI
{
    public partial class CommandBuilder : Form
    {
        private RecognitionEngine engine;
        private Profile currentProfile;
        private System.IO.FileSystemWatcher dialogueWatcher;
        //private sp.Grammar currentDialog;
        //private List<Action> currentDialogActions;
        private Profile dialogProfile;

        public Action CurrentAction
        {
            get
            {
                if (this.lb_Actions.SelectedIndex != -1)
                    return this.currentProfile.Actions[this.lb_Actions.SelectedIndex];
                else
                    return null;
            }
            set
            {
                //if (this.lb_Actions.SelectedIndex != -1)
                this.currentProfile.Actions[this.lb_Actions.SelectedIndex] = value;
            }
        }
        public string CurrentPhrase
        {
            get
            {
                if (this.CurrentAction != null && this.lb_Phrases.SelectedIndex != -1 && this.lb_Phrases.SelectedIndex < this.CurrentAction.Phrases.Count)
                    return this.CurrentAction.Phrases[this.lb_Phrases.SelectedIndex];
                else
                    return null;
            }
            set
            {
                this.CurrentAction.Phrases[this.lb_Phrases.SelectedIndex] = value;
            }
        }
        public Command CurrentKeystroke
        {
            get
            {
                if (this.CurrentAction != null && this.lb_KeystrokeList.SelectedIndex != -1 && this.lb_KeystrokeList.SelectedIndex < this.CurrentAction.Commands.Count)
                    return this.CurrentAction.Commands[this.lb_KeystrokeList.SelectedIndex];
                else
                    return null;
            }
            set
            {
                this.CurrentAction.Commands[this.lb_KeystrokeList.SelectedIndex] = value;
            }
        }
        public int DialoguePosition
        {
            get;
            private set;
        }

        const string FILE_DIALOGUETEXT = "CurrentDialogue.diag";
        const string FILE_DIALOGUESTATE = "DialogueState.diag";

        public CommandBuilder()
        {
            InitializeComponent();

            this.currentProfile = new Profile();

            dialogueWatcher = new System.IO.FileSystemWatcher();
            dialogueWatcher.NotifyFilter = System.IO.NotifyFilters.LastWrite;
            dialogueWatcher.Changed += new System.IO.FileSystemEventHandler(dialogueWatcher_Changed);

            LoadProfile();

        }

        public void LoadProfile()
        {
            this.tb_ProfileName.Text = this.currentProfile.ProfileName;

            if (this.currentProfile.RequiredConfidence < (double)this.nud_Confidence.Minimum || this.currentProfile.RequiredConfidence > (double)this.nud_Confidence.Maximum)
                this.currentProfile.RequiredConfidence = .9;
            this.nud_Confidence.Value = (decimal)this.currentProfile.RequiredConfidence;

            if (this.currentProfile.EndTimeout < (double)this.nud_Timeout.Minimum || this.currentProfile.EndTimeout > (double)this.nud_Timeout.Maximum)
                this.currentProfile.EndTimeout = 15;
            this.nud_Timeout.Value = (decimal)this.currentProfile.EndTimeout;

            this.tb_DialogueFilePath.Text = this.currentProfile.CurrentDialogPath;
            this.cb_Dialogue.Checked = this.currentProfile.EngageNpcDialogue;
            this.tb_DialogueFilePath.Text = this.currentProfile.CurrentDialogPath;
            try
            {
                this.dialogueWatcher.Path = System.IO.Path.GetDirectoryName(this.currentProfile.CurrentDialogPath);
            }
            catch (ArgumentException)
            {
               // this.dialogueWatcher.Path = null;
                //this.tb_DialogueFilePath.Text = string.Empty;
            }
            this.dialogueWatcher.Filter = "*.diag";

            LoadActionList();
        }

        public void LoadActionList()
        {

            bool same = false;
            if (lb_Actions.Items.Count == this.currentProfile.Actions.Count)
            {
                same = true;
                for (int i = 0; i < lb_Actions.Items.Count; i++)
                {
                    if (lb_Actions.Items[i] as string != this.currentProfile.Actions[i].ActionName)
                        same = false;
                }
            }

            if (!same)
            {
                this.lb_Actions.Items.Clear();

                foreach (var action in this.currentProfile.Actions)
                {
                    this.lb_Actions.Items.Add(action.ActionName);
                }
            }

            if (this.CurrentAction != null)
            {
                bttn_RemoveAction.Enabled = true;
                tb_RenameAction.Enabled = true;
                bttn_RenameAction.Enabled = true;
            }
            else
            {
                bttn_RemoveAction.Enabled = false;
                tb_RenameAction.Enabled = false;
                bttn_RenameAction.Enabled = false;
            }

            tb_AddAction.Text = string.Empty;
            tb_RenameAction.Text = string.Empty;
            if (this.CurrentAction != null)
                tb_RenameAction.Text = this.CurrentAction.ActionName;

            LoadPhraseList();
            LoadKeystrokeList();
        }

        public void LoadPhraseList()
        {
            if (this.CurrentAction != null)
            {
                this.lb_Phrases.Enabled = true;
                tb_AddPhrase.Enabled = true;
                bttn_AddPhrase.Enabled = true;
                if (this.CurrentPhrase != null)
                {
                    bttn_RemovePhrase.Enabled = true;
                    tb_RenamePhrase.Enabled = true;
                    bttn_RenamePhrase.Enabled = true;
                }
                else
                {
                    bttn_RemovePhrase.Enabled = false;
                    tb_RenamePhrase.Enabled = false;
                    bttn_RenamePhrase.Enabled = false;
                }

                bool same = false;
                if (lb_Phrases.Items.Count == this.CurrentAction.Phrases.Count)
                {
                    same = true;
                    for (int i = 0; i < lb_Phrases.Items.Count; i++)
                    {
                        if (lb_Phrases.Items[i] as string != this.CurrentAction.Phrases[i])
                            same = false;
                    }
                }

                if (!same)
                {
                    this.lb_Phrases.Items.Clear();

                    foreach (var phrase in this.CurrentAction.Phrases)
                    {
                        this.lb_Phrases.Items.Add(phrase);
                    }
                }

                tb_AddPhrase.Text = string.Empty;
                tb_RenamePhrase.Text = this.CurrentPhrase;
            }
            else
            {
                this.lb_Phrases.Enabled = false;
                tb_AddPhrase.Enabled = false;
                bttn_AddPhrase.Enabled = false;
                bttn_RemovePhrase.Enabled = false;
                tb_RenamePhrase.Enabled = false;
                bttn_RenamePhrase.Enabled = false;

                this.lb_Phrases.Items.Clear();

                tb_AddPhrase.Text = string.Empty;
                tb_RenamePhrase.Text = string.Empty;
            }
        }

        public void LoadKeystrokeList()
        {
            if (this.CurrentAction != null)
            {
                this.lb_KeystrokeList.Enabled = true;
                tb_AddKeystrokeList.Enabled = true;
                bttn_AddKeystrokeList.Enabled = true;
                nud_ActionRepeat.Enabled = true;
                nud_ActionPausedDuration.Enabled = true;

                if (this.CurrentKeystroke != null)
                {
                    bttn_RemoveKeystrokeList.Enabled = true;
                    tb_RenameKeystrokeList.Enabled = true;
                    bttn_RenameKeystrokeList.Enabled = true;
                }
                else
                {
                    bttn_RemoveKeystrokeList.Enabled = false;
                    tb_RenameKeystrokeList.Enabled = false;
                    bttn_RenameKeystrokeList.Enabled = false;
                }

                bool same = false;
                if (lb_KeystrokeList.Items.Count == this.CurrentAction.Commands.Count)
                {
                    same = true;
                    for (int i = 0; i < lb_KeystrokeList.Items.Count; i++)
                    {
                        if (lb_KeystrokeList.Items[i] as string != this.CurrentAction.Commands[i].CommandName)
                            same = false;
                    }
                }

                if (!same)
                {
                    this.lb_KeystrokeList.Items.Clear();

                    foreach (var command in this.CurrentAction.Commands)
                    {
                        this.lb_KeystrokeList.Items.Add(command.CommandName);
                    }
                }

                tb_AddKeystrokeList.Text = string.Empty;
                tb_RenameKeystrokeList.Text = string.Empty;
                if (this.CurrentKeystroke != null)
                    tb_RenameKeystrokeList.Text = this.CurrentKeystroke.CommandName;

                if (this.CurrentAction.Repeat < nud_ActionRepeat.Minimum || this.CurrentAction.Repeat > nud_ActionRepeat.Maximum)
                    this.CurrentAction.Repeat = 1;
                nud_ActionRepeat.Value = (decimal)this.CurrentAction.Repeat;

                if (this.CurrentAction.PausedDuration < nud_ActionPausedDuration.Minimum || this.CurrentAction.PausedDuration > nud_ActionPausedDuration.Maximum)
                    this.CurrentAction.PausedDuration = 25;
                nud_ActionPausedDuration.Value = (decimal)this.CurrentAction.PausedDuration;
            }
            else
            {
                this.lb_KeystrokeList.Enabled = false;
                tb_AddKeystrokeList.Enabled = false;
                bttn_AddKeystrokeList.Enabled = false;
                bttn_RemoveKeystrokeList.Enabled = false;
                tb_RenameKeystrokeList.Enabled = false;
                bttn_RenameKeystrokeList.Enabled = false;
                nud_ActionRepeat.Enabled = false;
                nud_ActionPausedDuration.Enabled = false;

                this.lb_KeystrokeList.Items.Clear();

                tb_AddKeystrokeList.Text = string.Empty;
                tb_RenameKeystrokeList.Text = string.Empty;
                nud_ActionRepeat.Value = 1;
                nud_ActionPausedDuration.Value = 25;
            }

            LoadKeystroke();
        }

        public void LoadKeystroke()
        {
            if (this.CurrentKeystroke != null)
            {
                cb_KeystrokeKey.Enabled = true;
                cb_ModifierKey.Enabled = true;
                if (this.CurrentKeystroke.ToggleKeypress)
                {
                    nud_KeystrokeHeld.Enabled = false;
                    nud_KeystrokeRepeat.Enabled = false;
                    nud_KeystrokePaused.Enabled = false;
                    tb_TextInput.Enabled = false;
                }
                else
                {
                    nud_KeystrokeHeld.Enabled = true;
                    nud_KeystrokeRepeat.Enabled = true;
                    nud_KeystrokePaused.Enabled = true;
                    tb_TextInput.Enabled = true;
                }

                cb_KeystrokeToggle.Enabled = true;

                if (this.CurrentKeystroke.Key != null)
                    cb_KeystrokeKey.SelectedItem = this.CurrentKeystroke.Key.ToString();
                else
                    cb_KeystrokeKey.SelectedIndex = 0;

                if (this.CurrentKeystroke.ModifierKey != null)
                    cb_ModifierKey.SelectedItem = this.CurrentKeystroke.ModifierKey.ToString();
                else
                    cb_ModifierKey.SelectedIndex = 0;

                if (this.CurrentKeystroke.Repeat < nud_KeystrokeRepeat.Minimum || this.CurrentKeystroke.Repeat > nud_KeystrokeRepeat.Maximum)
                    this.CurrentKeystroke.Repeat = 1;
                nud_KeystrokeRepeat.Value = (decimal)this.CurrentKeystroke.Repeat;

                if (this.CurrentKeystroke.HeldDuration < nud_KeystrokeHeld.Minimum || this.CurrentKeystroke.HeldDuration > nud_KeystrokeHeld.Maximum)
                    this.CurrentKeystroke.HeldDuration = 50;
                nud_KeystrokeHeld.Value = (decimal)this.CurrentKeystroke.HeldDuration;

                if (this.CurrentKeystroke.PausedDuration < nud_KeystrokePaused.Minimum || this.CurrentKeystroke.PausedDuration > nud_KeystrokePaused.Maximum)
                    this.CurrentKeystroke.PausedDuration = 25;
                nud_KeystrokePaused.Value = (decimal)this.CurrentKeystroke.PausedDuration;

                cb_KeystrokeToggle.Checked = this.CurrentKeystroke.ToggleKeypress;
                tb_TextInput.Text = this.CurrentKeystroke.TextInput;
            }
            else
            {
                cb_KeystrokeKey.Enabled = false;
                cb_ModifierKey.Enabled = false;
                nud_KeystrokeHeld.Enabled = false;
                nud_KeystrokeRepeat.Enabled = false;
                nud_KeystrokePaused.Enabled = false;
                cb_KeystrokeToggle.Enabled = false;
                tb_TextInput.Enabled = false;


                cb_KeystrokeKey.SelectedIndex = 0;
                cb_ModifierKey.SelectedIndex = 0;
                nud_KeystrokeRepeat.Value = 1;
                nud_KeystrokeHeld.Value = 50;
                nud_KeystrokePaused.Value = 25;
                cb_KeystrokeToggle.Checked = false;
            }
        }

        #region Profile
        private void tb_ProfileName_TextChanged(object sender, EventArgs e)
        {
            this.currentProfile.ProfileName = tb_ProfileName.Text;
        }

        private void nud_Confidence_ValueChanged(object sender, EventArgs e)
        {
            this.currentProfile.RequiredConfidence = (double)this.nud_Confidence.Value;
        }

        private void nud_Timeout_ValueChanged(object sender, EventArgs e)
        {
            this.currentProfile.EndTimeout = (int)nud_Timeout.Value;
        }

        private void cb_Dialogue_CheckedChanged(object sender, EventArgs e)
        {
            this.currentProfile.EngageNpcDialogue = this.cb_Dialogue.Checked;
        }

        private void bttn_DialogueFilePath_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog diag = new OpenFileDialog())
            {
                diag.DefaultExt = "diag";
                diag.Filter = "CurrentDialogue|*.diag";
                diag.Title = "Find CurrentDialog.diag";

                DialogResult result = diag.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    this.tb_DialogueFilePath.Text = diag.FileName;
                }
            }
        }

        private void tb_DialogueFilePath_TextChanged(object sender, EventArgs e)
        {
            this.currentProfile.CurrentDialogPath = this.tb_DialogueFilePath.Text;
        }
        #endregion

        #region Action
        private void lb_Actions_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadActionList();
        }

        private void tb_AddAction_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                bttn_AddAction_Click(sender, e);
            }
        }

        private void bttn_AddAction_Click(object sender, EventArgs e)
        {
            if (tb_AddAction.Text != string.Empty)
            {
                if (!currentProfile.Actions.Any(act => act.ActionName == tb_AddAction.Text))
                {
                    Action action = new Action();
                    action.ActionName = tb_AddAction.Text;
                    currentProfile.Actions.Add(action);
                    lb_Actions.Items.Add(action);
                    lb_Actions.SelectedItem = action;
                }
            }
        }

        private void tb_RenameAction_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                bttn_RenameAction_Click(sender, e);
            }
        }

        private void bttn_RenameAction_Click(object sender, EventArgs e)
        {
            if (this.CurrentAction != null && tb_RenameAction.Text != string.Empty && !currentProfile.Actions.Any(act => act.ActionName == tb_RenameAction.Text))
            {
                this.CurrentAction.ActionName = tb_RenameAction.Text;
                LoadActionList();
            }
        }

        private void bttn_RemoveAction_Click(object sender, EventArgs e)
        {
            if (this.CurrentAction != null)
            {
                this.currentProfile.Actions.Remove(this.CurrentAction);
                LoadActionList();
            }
        }
        #endregion

        #region Phrase
        private void lb_Phrases_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadPhraseList();
        }

        private void tb_AddPhrase_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                bttn_AddPhrase_Click(sender, e);
            }
        }

        private void bttn_AddPhrase_Click(object sender, EventArgs e)
        {
            if (tb_AddPhrase.Text != string.Empty)
            {
                if (!this.CurrentAction.Phrases.Contains(tb_AddPhrase.Text))
                {
                    this.CurrentAction.Phrases.Add(tb_AddPhrase.Text);
                    LoadPhraseList();
                }
            }
        }

        private void tb_RenamePhrase_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                bttn_RenamePhrase_Click(sender, e);
            }
        }

        private void bttn_RenamePhrase_Click(object sender, EventArgs e)
        {
            if (this.CurrentPhrase != null && tb_RenamePhrase.Text != string.Empty && !this.CurrentAction.Phrases.Contains(tb_RenamePhrase.Text))
            {
                this.CurrentPhrase = tb_RenamePhrase.Text;
                LoadPhraseList();
            }
        }

        private void bttn_RemovePhrase_Click(object sender, EventArgs e)
        {
            if (this.CurrentPhrase != null)
            {
                this.CurrentAction.Phrases.Remove(this.CurrentPhrase);
                LoadPhraseList();
            }
        }
        #endregion

        #region KeystrokeList
        private void lb_KeyStrokeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadKeystrokeList();
        }

        private void tb_AddKeystrokeList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                bttn_AddKeystrokeList_Click(sender, e);
            }
        }

        private void bttn_AddKeystrokeList_Click(object sender, EventArgs e)
        {
            if (tb_AddKeystrokeList.Text != string.Empty)
            {
                if (!this.CurrentAction.Commands.Any(cmd => cmd.CommandName == tb_AddKeystrokeList.Text))
                {
                    this.CurrentAction.Commands.Add(new Command() { CommandName = tb_AddKeystrokeList.Text });
                    LoadKeystrokeList();
                }
            }
        }

        private void tb_RenameKeystrokeList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                bttn_RenameKeystrokeList_Click(sender, e);
            }
        }

        private void bttn_RenameKeystrokeList_Click(object sender, EventArgs e)
        {
            if (this.CurrentKeystroke != null && tb_RenameKeystrokeList.Text != string.Empty && !this.CurrentAction.Commands.Any(cmd => cmd.CommandName == tb_RenameKeystrokeList.Text))
            {
                this.CurrentKeystroke.CommandName = tb_RenameKeystrokeList.Text;
                LoadKeystrokeList();
            }
        }

        private void bttn_RemoveKeystrokeList_Click(object sender, EventArgs e)
        {
            if (this.CurrentKeystroke != null)
            {
                this.CurrentAction.Commands.Remove(this.CurrentKeystroke);
                LoadKeystrokeList();
            }
        }


        private void nud_ActionPausedDuration_ValueChanged(object sender, EventArgs e)
        {
            if (this.CurrentAction != null)
            {
                this.CurrentAction.PausedDuration = (int)nud_ActionPausedDuration.Value;
            }
        }

        private void nud_ActionRepeat_ValueChanged(object sender, EventArgs e)
        {
            if (this.CurrentAction != null)
            {
                this.CurrentAction.Repeat = (int)nud_ActionRepeat.Value;
            }
        }
        #endregion

        #region Keystroke
        private void cb_CommandKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.CurrentKeystroke != null)
            {
                WindowsInput.VirtualKeyCode code;
                if (Enum.TryParse<WindowsInput.VirtualKeyCode>(cb_KeystrokeKey.Items[cb_KeystrokeKey.SelectedIndex].ToString(), out code))
                {
                    this.CurrentKeystroke.Key = code;
                }
                else
                {
                    this.CurrentKeystroke.Key = null;
                }
            }
        }

        private void cb_ModifierKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.CurrentKeystroke != null)
            {
                WindowsInput.VirtualKeyCode code;
                if (Enum.TryParse<WindowsInput.VirtualKeyCode>(cb_ModifierKey.Items[cb_ModifierKey.SelectedIndex].ToString(), out code))
                {
                    this.CurrentKeystroke.ModifierKey = code;
                }
                else
                {
                    this.CurrentKeystroke.ModifierKey = null;
                }
            }
        }

        private void tb_TextInput_TextChanged(object sender, EventArgs e)
        {
            if (this.CurrentKeystroke != null)
            {
                this.CurrentKeystroke.TextInput = tb_TextInput.Text;
            }
        }

        private void cb_KeystrokeToggle_CheckedChanged(object sender, EventArgs e)
        {
            if (this.CurrentKeystroke != null)
            {
                this.CurrentKeystroke.ToggleKeypress = cb_KeystrokeToggle.Checked;
                LoadKeystroke();
            }
        }

        private void nud_KeystrokeHeld_ValueChanged(object sender, EventArgs e)
        {
            if (this.CurrentKeystroke != null)
            {
                this.CurrentKeystroke.HeldDuration = (int)nud_KeystrokeHeld.Value;
            }
        }

        private void nud_KeystrokePaused_ValueChanged(object sender, EventArgs e)
        {
            if (this.CurrentKeystroke != null)
            {
                this.CurrentKeystroke.PausedDuration = (int)nud_KeystrokePaused.Value;
            }
        }

        private void nud_KeystrokeRepeat_ValueChanged(object sender, EventArgs e)
        {
            if (this.CurrentKeystroke != null)
            {
                this.CurrentKeystroke.Repeat = (int)nud_KeystrokeRepeat.Value;
            }
        }
        #endregion

        #region Toolstrip
        private void tsmi_New_Click(object sender, EventArgs e)
        {
            this.currentProfile = new Profile();

            LoadProfile();
        }

        private void tsmi_Open_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog diag = new OpenFileDialog())
            {
                diag.DefaultExt = "xml";
                diag.Filter = "Xml Profile|*.xml";
                diag.Title = "Open Xml Profile";

                DialogResult result = diag.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    this.currentProfile.Open(diag.FileName);

                    LoadProfile();
                }
            }
        }

        private void tsmi_Save_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog diag = new SaveFileDialog())
            {
                diag.DefaultExt = "xml";
                diag.Filter = "Xml Profile|*.xml";
                diag.Title = "Save Xml Profile";

                DialogResult result = diag.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    this.currentProfile.Save(diag.FileName);
                }
            }
        }

        private void tsmi_Throw_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsmi_Start_Click(object sender, EventArgs e)
        {
            if (this.currentProfile.EngageNpcDialogue)
                this.dialogueWatcher.EnableRaisingEvents = true;

            this.tb_RecognizedWord.BackColor = Color.LightGreen;
            if (engine != null)
                engine.Dispose();

            if (this.currentProfile.Actions.Count == 0)
                engine = new RecognitionEngine();
            else
                engine = new RecognitionEngine(this.currentProfile);
            engine.onWordRecognized += this.AddRecognizedText;

            if (!this.engine.Running)
                engine.StartAsync(sp.RecognizeMode.Multiple);
        }

        private void tsmi_Stop_Click(object sender, EventArgs e)
        {
            this.dialogueWatcher.EnableRaisingEvents = false;
            this.tb_RecognizedWord.BackColor = SystemColors.Control;
            this.tb_RecognizedWord.Text = string.Empty;

            if (this.engine != null)
                engine.Stop();
        }

        private void tsmi_emulateActivePhrase_Click(object sender, EventArgs e)
        {
            if (this.CurrentPhrase != null)
            {
                this.tb_RecognizedWord.BackColor = Color.Yellow;
                Application.DoEvents();
                System.Threading.Thread.Sleep(2000);

                if (engine != null)
                    engine.Dispose();


                engine = new RecognitionEngine(this.currentProfile);
                engine.onWordRecognized += this.AddRecognizedText;

                if (!this.engine.Running)
                    engine.EmulateRecognize(this.CurrentPhrase);
                this.tb_RecognizedWord.BackColor = SystemColors.Control;
                this.engine.Stop();
                this.tb_RecognizedWord.Text = string.Empty;
            }
        }
        #endregion

        private void AddRecognizedText(string text)
        {
            this.tb_RecognizedWord.Text = text;
        }

        void dialogueWatcher_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            Console.WriteLine(String.Format("Changetype: {0}, Fullpath: {1}, Name: {2}", e.ChangeType, e.FullPath, e.Name));
            if (e.Name == FILE_DIALOGUETEXT)
            {
                UpdateDialogueOptions();
            }
            if (e.Name == FILE_DIALOGUESTATE)
            {
                UpdateDialoguePosition();
            }
        }

        private void UpdateDialoguePosition()
        {
            string filename = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(this.currentProfile.CurrentDialogPath), FILE_DIALOGUESTATE);

            bool readSuccess = true;
            do
            {

                try
                {
                    using (System.IO.StreamReader rdr = new System.IO.StreamReader(filename))
                    {
                        readSuccess = true;
                        int position = 0;
                        string line = rdr.ReadLine();
                        if (int.TryParse(line, out position))
                            this.DialoguePosition = position;
                    }
                }
                catch (System.IO.IOException)
                {
                    readSuccess = false;
                }

            }
            while (!readSuccess);

            Console.WriteLine("Modified Position! " + this.DialoguePosition);
            for (int index = 0; index < this.dialogProfile.Actions.Count - 1; index++) // -1 to ignore goodbye
            {
                lock (this.dialogProfile.Actions[index].Commands)
                {
                    Command cmd = this.dialogProfile.Actions[index].Commands[0];

                    if (this.DialoguePosition > index)
                        cmd.Key = WindowsInput.VirtualKeyCode.VK_W;
                    else
                        cmd.Key = WindowsInput.VirtualKeyCode.VK_S;
                    cmd.Repeat = Math.Abs(this.DialoguePosition - index);
                }
            }

        }

        void UpdateDialogueOptions()
        {
            if (this.engine.Running)
            {
                List<Action> actionlist = new List<Action>();
                bool readSuccess = true;
                do
                {

                    try
                    {
                        string filename = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(this.currentProfile.CurrentDialogPath), FILE_DIALOGUETEXT);
                        using (System.IO.StreamReader rdr = new System.IO.StreamReader(filename))
                        {
                            readSuccess = true;
                            int index = 0;

                            while (!rdr.EndOfStream)
                            {
                                string text = rdr.ReadLine().Split('(')[0].Replace('?','.');

                                Command select = new Command();
                                select.CommandName = "Select";
                                select.Key = WindowsInput.VirtualKeyCode.VK_E;
                                select.HeldDuration = 50;
                                select.PausedDuration = 25;

                                Action action = new Action();
                                action.ActionName = text;
                                action.Phrases.Add(text);


                                Command move = new Command();
                                move.CommandName = String.Format("Go Down {0}", index);
                                if (this.DialoguePosition > index)
                                    move.Key = WindowsInput.VirtualKeyCode.VK_W;
                                else
                                    move.Key = WindowsInput.VirtualKeyCode.VK_S;
                                move.Repeat = Math.Abs(this.DialoguePosition - index);

                                move.HeldDuration = 50;
                                move.PausedDuration = 25;


                                action.Commands.Add(move);
                                action.Commands.Add(select);

                                actionlist.Add(action);
                                ++index;

                            }
                        }
                    }
                    catch (System.IO.IOException)
                    {
                        readSuccess = false;
                    }

                }
                while (!readSuccess);

                if (actionlist.Count > 0)
                {
                    Profile prof = new Profile();
                    prof.Actions = actionlist;
                    prof.Actions.Add(GenerateGoodbyeAction());
                    prof.ProfileName = "Dialogue";
                    bool same = false;
                    if (this.dialogProfile != null && this.dialogProfile.Actions.Count == prof.Actions.Count)
                    {
                        same = true;
                        for (int i = 0; i < this.dialogProfile.Actions.Count; i++)
                        {
                            if (this.dialogProfile.Actions[i].ActionName != prof.Actions[i].ActionName)
                                same = false;
                        }
                    }
                    if (!same)
                    {
                        if (this.dialogProfile != null)
                        {
                            this.engine.RemoveGrammar(this.dialogProfile.Grammar, this.dialogProfile.Actions);
                        }

                        this.dialogProfile = prof;
                        this.dialogProfile.UpdateGrammar();
                        this.currentProfile.Grammar.Enabled = false;
                        this.engine.AddGrammar(this.dialogProfile.Grammar, this.dialogProfile.Actions);
                        Console.WriteLine("Dialogue Mode Initialized");
                    }
                }
                else
                {
                    Console.WriteLine("End Dialogue");
                    if (this.dialogProfile != null)
                    {
                        this.engine.RemoveGrammar(this.dialogProfile.Grammar, this.dialogProfile.Actions);
                    }
                    this.dialogProfile = null;
                    this.currentProfile.Grammar.Enabled = true;
                }
            }
        }

        private Action GenerateGoodbyeAction()
        {
            Action action = new Action();
            action.ActionName = "Goodbye";
            Command cmd = new Command();
            cmd.CommandName = "Exit";
            cmd.Key = WindowsInput.VirtualKeyCode.TAB;
            cmd.HeldDuration = 50;
            cmd.PausedDuration = 25;
            action.Commands.Add(cmd);

            action.Phrases.AddRange(new string[] 
            {
                "Goodbye",
                "See yuh",
                "audios",
                "bye",
                "talk to you later",
                "Never mind"
            });

            return action;
        }
    }
}

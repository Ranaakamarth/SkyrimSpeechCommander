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
        private DialogueRecognitionEngine engine;
        private Profile currentProfile;
        //private System.IO.FileSystemWatcher dialogueWatcher;
        //private sp.Grammar currentDialog;
        //private List<Action> currentDialogActions;
        //private Profile dialogProfile;


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
        //public int DialoguePosition
        //{
        //    get;
        //    private set;
        //}
        public string CurrentDialogueCancelPhrase
        {
            get
            {
                if (this.currentProfile.Dialogue.Enabled && this.lb_DialogueGoodbyeList.SelectedIndex != -1 && this.lb_DialogueGoodbyeList.SelectedIndex < this.currentProfile.Dialogue.CancelPhrases.Count)
                    return this.currentProfile.Dialogue.CancelPhrases[this.lb_DialogueGoodbyeList.SelectedIndex];
                else
                    return null;
            }
            set
            {
                this.currentProfile.Dialogue.CancelPhrases[this.lb_DialogueGoodbyeList.SelectedIndex] = value;
            }
        }
        public Command CurrentDialogueCommand
        {
            get
            {
                Command cmd;// = null;
                switch (cb_DialogueCommand.Items[cb_DialogueCommand.SelectedIndex].ToString())
                {
                    case "Previous":
                        cmd = this.currentProfile.Dialogue.CommandPrevious;
                        if (cmd == null)
                        {
                            this.currentProfile.Dialogue.CommandPrevious = new Command();
                            cmd = this.currentProfile.Dialogue.CommandPrevious;
                            cmd.CommandName = "Previous";
                        }
                        break;
                    case "Next":
                        cmd = this.currentProfile.Dialogue.CommandNext;
                        if (cmd == null)
                        {
                            this.currentProfile.Dialogue.CommandNext = new Command();
                            cmd = this.currentProfile.Dialogue.CommandNext;
                            cmd.CommandName = "Next";
                        }
                        break;
                    case "Accept":
                        cmd = this.currentProfile.Dialogue.CommandAccept;
                        if (cmd == null)
                        {
                            this.currentProfile.Dialogue.CommandAccept = new Command();
                            cmd = this.currentProfile.Dialogue.CommandAccept;
                            cmd.CommandName = "Accept";
                        }
                        break;
                    case "Cancel":
                        cmd = this.currentProfile.Dialogue.CommandCancel;
                        if (cmd == null)
                        {
                            this.currentProfile.Dialogue.CommandCancel = new Command();
                            cmd = this.currentProfile.Dialogue.CommandCancel;
                            cmd.CommandName = "Cancel";
                        }
                        break;
                    default:
                        cmd = null;
                        break;
                }
                return cmd;
            }
            set
            {
                Command cmd;// = null;
                switch (cb_DialogueCommand.Items[cb_DialogueCommand.SelectedIndex].ToString())
                {
                    case "Previous":
                        cmd = this.currentProfile.Dialogue.CommandPrevious;
                        break;
                    case "Next":
                        cmd = this.currentProfile.Dialogue.CommandNext;
                        break;
                    case "Accept":
                        cmd = this.currentProfile.Dialogue.CommandAccept;
                        break;
                    case "Cancel":
                        cmd = this.currentProfile.Dialogue.CommandCancel;
                        break;
                    default:
                        cmd = null;
                        break;
                }
                cmd = value;
            }
        }
        public string CurrentPausePhrase
        {
            get
            {
                if (this.currentProfile.EnableVoicePausing && this.lb_PausePhrases.SelectedIndex != -1 && this.lb_PausePhrases.SelectedIndex < this.currentProfile.PauseRecognitionPhrases.Count)
                    return this.currentProfile.PauseRecognitionPhrases[this.lb_PausePhrases.SelectedIndex];
                else
                    return null;
            }
            set
            {
                this.currentProfile.PauseRecognitionPhrases[this.lb_PausePhrases.SelectedIndex] = value;
            }
        }
        public string CurrentUnpausePhrase
        {
            get
            {
                if (this.currentProfile.EnableVoicePausing && this.lb_UnpausePhrases.SelectedIndex != -1 && this.lb_UnpausePhrases.SelectedIndex < this.currentProfile.UnpauseRecognitionPhrases.Count)
                    return this.currentProfile.UnpauseRecognitionPhrases[this.lb_UnpausePhrases.SelectedIndex];
                else
                    return null;
            }
            set
            {
                this.currentProfile.UnpauseRecognitionPhrases[this.lb_UnpausePhrases.SelectedIndex] = value;
            }
        }

        //const string FILE_DIALOGUETEXT = "CurrentDialogue.diag";
        //const string FILE_DIALOGUESTATE = "DialogueState.diag";

        public CommandBuilder()
        {
            InitializeComponent();

            this.currentProfile = new Profile();

            //dialogueWatcher = new System.IO.FileSystemWatcher();
            //dialogueWatcher.NotifyFilter = System.IO.NotifyFilters.LastWrite;
            //dialogueWatcher.Changed += dialogueWatcher_Changed;
            //this.dialogueWatcher.Filter = "*.diag";

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

            LoadDialogue();
            LoadActionList();
            LoadVoiceToggle();
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

                    tb_RenameKeystrokeList.Text = this.CurrentKeystroke.CommandName;
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

        public void LoadDialogue()
        {
            this.cb_DialogueEnabled.Checked = this.currentProfile.Dialogue.Enabled;

            if (this.currentProfile.Dialogue.Enabled)
            {
                this.tb_DialogueFolderPath.Enabled = true;
                this.bttn_DialogueFolderPath.Enabled = true;
                this.cb_DialogueCommand.Enabled = true;
                this.cb_DialogueCommandKey.Enabled = true;
                this.cb_DialogueCommandModifierKey.Enabled = true;
                this.nud_DialogueCommandHeld.Enabled = true;
                this.nud_DialogueCommandPaused.Enabled = true;

                this.lb_DialogueGoodbyeList.Enabled = true;
                this.tb_DialogueGoodbyeAdd.Enabled = true;
                this.bttn_DialogueGoodbyeAdd.Enabled = true;

                bool same = false;

                if (this.currentProfile.Dialogue.CancelPhrases == null)
                    this.currentProfile.Dialogue.CancelPhrases = new List<string>();

                if (lb_DialogueGoodbyeList.Items.Count == this.currentProfile.Dialogue.CancelPhrases.Count)
                {
                    same = true;
                    for (int i = 0; i < lb_DialogueGoodbyeList.Items.Count; i++)
                    {
                        if (lb_DialogueGoodbyeList.Items[i] as string != this.currentProfile.Dialogue.CancelPhrases[i])
                            same = false;
                    }
                }

                if (!same)
                {
                    this.lb_DialogueGoodbyeList.Items.Clear();

                    foreach (var phrase in this.currentProfile.Dialogue.CancelPhrases)
                    {
                        this.lb_DialogueGoodbyeList.Items.Add(phrase);
                    }
                }

                if (this.CurrentDialogueCancelPhrase != null)
                {
                    this.tb_DialogueGoodbyeRename.Enabled = true;
                    this.bttn_DialogueGoodbyeRemove.Enabled = true;
                    this.bttn_DialogueGoodbyeRename.Enabled = true;

                    this.tb_DialogueGoodbyeRename.Text = this.CurrentDialogueCancelPhrase;
                }
                else
                {
                    this.tb_DialogueGoodbyeRename.Enabled = false;
                    this.bttn_DialogueGoodbyeRemove.Enabled = false;
                    this.bttn_DialogueGoodbyeRename.Enabled = false;

                    this.tb_DialogueGoodbyeRename.Text = string.Empty;
                }

                this.tb_DialogueGoodbyeAdd.Text = string.Empty;
                this.tb_DialogueFolderPath.Text = this.currentProfile.Dialogue.FilePath;
                this.cb_DialogueEnabled.Checked = this.currentProfile.Dialogue.Enabled;
            }
            else
            {
                this.tb_DialogueFolderPath.Enabled = false;
                this.bttn_DialogueFolderPath.Enabled = false;
                this.cb_DialogueCommand.Enabled = false;
                this.cb_DialogueCommandKey.Enabled = false;
                this.cb_DialogueCommandModifierKey.Enabled = false;
                this.nud_DialogueCommandHeld.Enabled = false;
                this.nud_DialogueCommandPaused.Enabled = false;

                this.lb_DialogueGoodbyeList.Enabled = false;
                this.tb_DialogueGoodbyeAdd.Enabled = false;
                this.bttn_DialogueGoodbyeAdd.Enabled = false;

                this.tb_DialogueGoodbyeRename.Enabled = false;
                this.bttn_DialogueGoodbyeRemove.Enabled = false;
                this.bttn_DialogueGoodbyeRename.Enabled = false;

                this.tb_DialogueGoodbyeAdd.Text = string.Empty;
                this.tb_DialogueGoodbyeRename.Text = string.Empty;
                this.tb_DialogueFolderPath.Text = this.currentProfile.Dialogue.FilePath;

                this.lb_DialogueGoodbyeList.Items.Clear();

            }

            LoadDialogueCommand();
        }

        public void LoadDialogueCommand()
        {
            if (this.CurrentDialogueCommand != null)
            {
                //cb_DialogueCommandKey.Enabled = true;
                //cb_DialogueCommandModifierKey.Enabled = true;
                //nud_DialogueCommandHeld.Enabled = true;
                //nud_DialogueCommandPaused.Enabled = true;

                if (this.CurrentDialogueCommand.Key != null)
                    cb_DialogueCommandKey.SelectedItem = this.CurrentDialogueCommand.Key.ToString();
                else
                    cb_DialogueCommandKey.SelectedIndex = 0;

                if (this.CurrentDialogueCommand.ModifierKey != null)
                    cb_DialogueCommandModifierKey.SelectedItem = this.CurrentDialogueCommand.ModifierKey.ToString();
                else
                    cb_DialogueCommandModifierKey.SelectedIndex = 0;

                if (this.CurrentDialogueCommand.HeldDuration < nud_DialogueCommandHeld.Minimum || this.CurrentDialogueCommand.HeldDuration > nud_DialogueCommandHeld.Maximum)
                    this.CurrentDialogueCommand.HeldDuration = 50;
                nud_DialogueCommandHeld.Value = (decimal)this.CurrentDialogueCommand.HeldDuration;

                if (this.CurrentDialogueCommand.PausedDuration < nud_DialogueCommandPaused.Minimum || this.CurrentDialogueCommand.PausedDuration > nud_DialogueCommandPaused.Maximum)
                    this.CurrentDialogueCommand.PausedDuration = 25;
                nud_DialogueCommandPaused.Value = (decimal)this.CurrentDialogueCommand.PausedDuration;
            }
            else
            {
                //cb_DialogueCommandKey.Enabled = false;
                //cb_DialogueCommandModifierKey.Enabled = false;
                //nud_DialogueCommandHeld.Enabled = false;
                //nud_DialogueCommandPaused.Enabled = false;

                cb_DialogueCommandKey.SelectedIndex = 0;
                cb_DialogueCommandModifierKey.SelectedIndex = 0;
                nud_DialogueCommandHeld.Value = 50;
                nud_DialogueCommandPaused.Value = 25;
            }
        }

        public void LoadVoiceToggle()
        {
            this.cb_VoiceToggle.Checked = this.currentProfile.EnableVoicePausing;

            #region Populate listboxes
            bool same = false;
            if (lb_PausePhrases.Items.Count == this.currentProfile.PauseRecognitionPhrases.Count)
            {
                same = true;
                for (int i = 0; i < lb_PausePhrases.Items.Count; i++)
                {
                    if (lb_PausePhrases.Items[i] as string != this.currentProfile.PauseRecognitionPhrases[i])
                        same = false;
                }
            }

            if (!same)
            {
                this.lb_PausePhrases.Items.Clear();

                foreach (var phrase in this.currentProfile.PauseRecognitionPhrases)
                {
                    this.lb_PausePhrases.Items.Add(phrase);
                }
            }

            same = false;
            if (lb_UnpausePhrases.Items.Count == this.currentProfile.UnpauseRecognitionPhrases.Count)
            {
                same = true;
                for (int i = 0; i < lb_UnpausePhrases.Items.Count; i++)
                {
                    if (lb_UnpausePhrases.Items[i] as string != this.currentProfile.UnpauseRecognitionPhrases[i])
                        same = false;
                }
            }

            if (!same)
            {
                this.lb_UnpausePhrases.Items.Clear();

                foreach (var phrase in this.currentProfile.UnpauseRecognitionPhrases)
                {
                    this.lb_UnpausePhrases.Items.Add(phrase);
                }
            }
            #endregion

            if (this.currentProfile.EnableVoicePausing)
            {
                tb_AddPausePhrase.Enabled = true;
                bttn_AddPausePhrase.Enabled = true;
                tb_AddPausePhrase.Text = string.Empty;

                if (this.CurrentPausePhrase != null)
                {
                    tb_RenamePausePhrase.Enabled = true;
                    bttn_RenamePausePhrase.Enabled = true;
                    bttn_RemovePausePhrase.Enabled = true;

                    tb_RenamePausePhrase.Text = this.CurrentPausePhrase;
                }
                else
                {
                    tb_RenamePausePhrase.Enabled = false;
                    bttn_RenamePausePhrase.Enabled = false;
                    bttn_RemovePausePhrase.Enabled = false;

                    tb_RenamePausePhrase.Text = string.Empty;
                }

                tb_AddUnpausePhrase.Enabled = true;
                bttn_AddUnpausePhrase.Enabled = true;
                tb_AddUnpausePhrase.Text = string.Empty;

                if (this.CurrentUnpausePhrase != null)
                {
                    bttn_RemoveUnpausePhrase.Enabled = true;
                    tb_RenameUnpausePhrase.Enabled = true;
                    bttn_RenameUnpausePhrase.Enabled = true;

                    tb_RenameUnpausePhrase.Text = this.CurrentUnpausePhrase;
                }
                else
                {
                    bttn_RemoveUnpausePhrase.Enabled = false;
                    tb_RenameUnpausePhrase.Enabled = false;
                    bttn_RenameUnpausePhrase.Enabled = false;

                    tb_RenameUnpausePhrase.Text = string.Empty;

                }





            }
            else
            {
                tb_AddPausePhrase.Enabled = false;
                bttn_AddPausePhrase.Enabled = false;
                tb_RenamePausePhrase.Enabled = false;
                bttn_RenamePausePhrase.Enabled = false;
                bttn_RemovePausePhrase.Enabled = false;


                tb_AddUnpausePhrase.Enabled = false;
                bttn_AddUnpausePhrase.Enabled = false;
                bttn_RemoveUnpausePhrase.Enabled = false;
                tb_RenameUnpausePhrase.Enabled = false;
                bttn_RenameUnpausePhrase.Enabled = false;
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
            if (this.CurrentAction != null && tb_RenameAction.Text != string.Empty && !this.currentProfile.Actions.Any(act => act.ActionName == tb_RenameAction.Text))
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
                    SafeSave(diag.FileName);
                }
            }
        }

        private bool SafeSave(string path)
        {
            System.Windows.Forms.DialogResult result;
            do
            {
                result = System.Windows.Forms.DialogResult.Ignore;
                //try
                //{
                this.currentProfile.Save(path);
                //}
                //catch (System.IO.IOException e)
                //{
                //    result = System.Windows.Forms.MessageBox.Show(e.Message, "File Save Error", System.Windows.Forms.MessageBoxButtons.AbortRetryIgnore);
                //    if (result == System.Windows.Forms.DialogResult.Abort)
                //    {
                //        return false;
                //    }
                //}
            }
            while (result == System.Windows.Forms.DialogResult.Retry);
            return true;
        }

        private void tsmi_Throw_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsmi_Start_Click(object sender, EventArgs e)
        {
            //if (this.currentProfile.Dialogue.Enabled)
            //{
            //    try
            //    {
            //        this.dialogueWatcher.EnableRaisingEvents = true;
            //    }
            //    catch (ArgumentException ex)
            //    {
            //        Console.WriteLine(String.Format("The dialogue file path,'{0}', is invalid!", this.dialogueWatcher.Path));
            //    }
            //}

            this.tb_RecognizedWord.BackColor = Color.LightGreen;
            if (engine != null)
                engine.Dispose();

            if (this.currentProfile.Actions.Count == 0)
                engine = new DialogueRecognitionEngine();
            else
                engine = new DialogueRecognitionEngine(this.currentProfile);
            engine.onWordRecognized += this.AddRecognizedText;

            if (!this.engine.Running)
                engine.StartAsync(sp.RecognizeMode.Multiple);
        }

        private void tsmi_Stop_Click(object sender, EventArgs e)
        {
            //this.dialogueWatcher.EnableRaisingEvents = false;
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


                engine = new DialogueRecognitionEngine(this.currentProfile);
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

        //void dialogueWatcher_Changed(object sender, System.IO.FileSystemEventArgs e)
        //{
        //    Console.WriteLine(String.Format("Changetype: {0}, Fullpath: {1}, Name: {2}", e.ChangeType, e.FullPath, e.Name));
        //    if (e.Name == FILE_DIALOGUETEXT)
        //    {
        //        UpdateDialogueOptions();
        //    }
        //    if (e.Name == FILE_DIALOGUESTATE)
        //    {
        //        UpdateDialoguePosition();
        //    }
        //}

        //private void UpdateDialoguePosition()
        //{
        //    string filename = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(this.currentProfile.Dialogue.FilePath), FILE_DIALOGUESTATE);

        //    int position = 0;
        //    bool readSuccess = true;
        //    do
        //    {

        //        try
        //        {
        //            using (System.IO.StreamReader rdr = new System.IO.StreamReader(filename))
        //            {
        //                readSuccess = true;

        //                string line = rdr.ReadLine();
        //                int.TryParse(line, out position);
        //            }
        //        }
        //        catch (System.IO.IOException)
        //        {
        //            readSuccess = false;
        //        }

        //    }
        //    while (!readSuccess);

        //    if (position != this.DialoguePosition)
        //    {
        //        this.DialoguePosition = position;
        //        Console.WriteLine("Modified Position! " + this.DialoguePosition);
        //        for (int index = 0; index < this.dialogProfile.Actions.Count - 1; index++) // -1 to ignore goodbye
        //        {
        //            lock (this.dialogProfile.Actions[index].Commands)
        //            {
        //                Command move = this.dialogProfile.Actions[index].Commands[0];
        //                Command tmp;

        //                if (this.DialoguePosition > index)
        //                    tmp = this.currentProfile.Dialogue.CommandPrevious;
        //                else
        //                    tmp = this.currentProfile.Dialogue.CommandNext;

        //                move.Repeat = Math.Abs(this.DialoguePosition - index);

        //                move.CommandName = String.Format("Go Down {0}", index);
        //                move.Key = tmp.Key;
        //                move.ModifierKey = tmp.ModifierKey;
        //                move.HeldDuration = tmp.HeldDuration;
        //                move.PausedDuration = tmp.PausedDuration;
        //            }
        //        }
        //    }
        //}

        //void UpdateDialogueOptions()
        //{
        //    if (this.engine.Running)
        //    {
        //        List<Action> actionlist = new List<Action>();
        //        bool readSuccess = true;
        //        do
        //        {

        //            try
        //            {
        //                string filename = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(this.currentProfile.Dialogue.FilePath), FILE_DIALOGUETEXT);
        //                using (System.IO.StreamReader rdr = new System.IO.StreamReader(filename))
        //                {
        //                    readSuccess = true;
        //                    int index = 0;

        //                    while (!rdr.EndOfStream)
        //                    {
        //                        string text = rdr.ReadLine().Split('(')[0].Replace('?', '.');

        //                        Action action = new Action();
        //                        action.ActionName = text;
        //                        action.Phrases.Add(text);


        //                        Command move = new Command();
        //                        Command tmp;

        //                        if (this.DialoguePosition > index)
        //                            tmp = this.currentProfile.Dialogue.CommandPrevious;
        //                        else
        //                            tmp = this.currentProfile.Dialogue.CommandNext;

        //                        move.Repeat = Math.Abs(this.DialoguePosition - index);

        //                        move.CommandName = String.Format("Go Down {0}", index);
        //                        move.Key = tmp.Key;
        //                        move.ModifierKey = tmp.ModifierKey;
        //                        move.HeldDuration = tmp.HeldDuration;
        //                        move.PausedDuration = tmp.PausedDuration;

        //                        action.Commands.Add(move);
        //                        action.Commands.Add(this.currentProfile.Dialogue.CommandAccept);
        //                        actionlist.Add(action);
        //                        ++index;

        //                    }
        //                }
        //            }
        //            catch (System.IO.IOException)
        //            {
        //                readSuccess = false;
        //            }

        //        }
        //        while (!readSuccess);

        //        if (actionlist.Count > 0)
        //        {
        //            Profile prof = new Profile();
        //            prof.Actions = actionlist;
        //            prof.Actions.Add(GenerateGoodbyeAction());
        //            prof.ProfileName = "Dialogue";
        //            bool same = false;
        //            if (this.dialogProfile != null && this.dialogProfile.Actions.Count == prof.Actions.Count)
        //            {
        //                same = true;
        //                for (int i = 0; i < this.dialogProfile.Actions.Count; i++)
        //                {
        //                    if (this.dialogProfile.Actions[i].ActionName != prof.Actions[i].ActionName)
        //                        same = false;
        //                }
        //            }
        //            if (!same)
        //            {


        //                prof.UpdateGrammar();

        //                var changes = new List<UpdateOperation>();
        //                changes.Add(new UpdateOperation()
        //                {
        //                    UpdateType = UpdateOperationType.DisableGrammar,
        //                    Grammar = this.currentProfile.Grammar
        //                });
        //                changes.Add(new UpdateOperation()
        //                {
        //                    UpdateType = UpdateOperationType.AddGrammar,
        //                    Grammar = prof.Grammar,
        //                    AssociatedActions = prof.Actions
        //                });

        //                if (this.dialogProfile != null)
        //                {
        //                    changes.Add(new UpdateOperation()
        //                    {
        //                        UpdateType = UpdateOperationType.RemoveGrammar,
        //                        Grammar = this.dialogProfile.Grammar,
        //                        AssociatedActions = this.dialogProfile.Actions
        //                    });
        //                    //this.engine.RemoveGrammar(this.dialogProfile.Grammar, this.dialogProfile.Actions);
        //                }

        //                this.engine.ExecuteGrammarChanges(changes);

        //                this.dialogProfile = prof;
        //                Console.WriteLine("Dialogue Mode Initializing");
        //            }
        //        }
        //        else if (this.dialogProfile != null)
        //        {
        //            Console.WriteLine("End Dialogue");

        //            var changes = new List<UpdateOperation>();
        //            changes.Add(new UpdateOperation()
        //            {
        //                UpdateType = UpdateOperationType.EnableGrammar,
        //                Grammar = this.currentProfile.Grammar
        //            });
        //            changes.Add(new UpdateOperation()
        //            {
        //                UpdateType = UpdateOperationType.RemoveGrammar,
        //                Grammar = this.dialogProfile.Grammar,
        //                AssociatedActions = this.dialogProfile.Actions
        //            });

        //            this.engine.ExecuteGrammarChanges(changes);

        //            this.dialogProfile = null;
        //        }
        //    }
        //}

        //private Action GenerateGoodbyeAction()
        //{
        //    Action action = new Action();
        //    action.ActionName = "Goodbye";
        //    action.Commands.Add(this.currentProfile.Dialogue.CommandCancel);
        //    action.Phrases.AddRange(this.currentProfile.Dialogue.CancelPhrases);

        //    return action;
        //}

        #region Dialogue
        private void cb_Dialogue_CheckedChanged(object sender, EventArgs e)
        {
            this.currentProfile.Dialogue.Enabled = this.cb_DialogueEnabled.Checked;
            LoadDialogue();
        }

        private void bttn_DialogueFilePath_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog diag = new FolderBrowserDialog())
            {
                diag.Description = "Dialogue Information Folder";

                DialogResult result = diag.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    this.tb_DialogueFolderPath.Text = diag.SelectedPath;
                }
            }
        }

        private void tb_DialogueFilePath_TextChanged(object sender, EventArgs e)
        {
            this.currentProfile.Dialogue.FilePath = this.tb_DialogueFolderPath.Text;

            //try
            //{
            //    this.dialogueWatcher.Path = System.IO.Path.GetDirectoryName(this.currentProfile.Dialogue.FilePath);
            //}
            //catch (ArgumentException)
            //{
            //    // this.dialogueWatcher.Path = null;
            //    //this.tb_DialogueFilePath.Text = string.Empty;
            //}
        }

        private void cb_DialogueCommand_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDialogue();
        }

        private void cb_DialogueCommandKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.CurrentDialogueCommand != null)
            {
                WindowsInput.VirtualKeyCode code;
                if (Enum.TryParse<WindowsInput.VirtualKeyCode>(cb_DialogueCommandKey.Items[cb_DialogueCommandKey.SelectedIndex].ToString(), out code))
                {
                    this.CurrentDialogueCommand.Key = code;
                }
                else
                {
                    this.CurrentDialogueCommand.Key = null;
                }
            }
        }

        private void cb_DialogueCommandModifierKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.CurrentDialogueCommand != null)
            {
                WindowsInput.VirtualKeyCode code;
                if (Enum.TryParse<WindowsInput.VirtualKeyCode>(cb_DialogueCommandModifierKey.Items[cb_DialogueCommandModifierKey.SelectedIndex].ToString(), out code))
                {
                    this.CurrentDialogueCommand.ModifierKey = code;
                }
                else
                {
                    this.CurrentDialogueCommand.ModifierKey = null;
                }
            }
        }

        private void nud_DialogueCommandHeld_ValueChanged(object sender, EventArgs e)
        {

        }

        private void nud_DialogueCommandPaused_ValueChanged(object sender, EventArgs e)
        {

        }

        private void lb_DialogueGoodbyeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDialogue();
        }

        private void tb_DialogueGoodbyeAdd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                bttn_DialogueGoodbyeAdd_Click(sender, e);
            }
        }

        private void tb_DialogueGoodbyeRename_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                bttn_DialogueGoodbyeRename_Click(sender, e);
            }
        }

        private void bttn_DialogueGoodbyeAdd_Click(object sender, EventArgs e)
        {
            if (tb_DialogueGoodbyeAdd.Text != string.Empty)
            {
                if (!this.currentProfile.Dialogue.CancelPhrases.Contains(tb_DialogueGoodbyeAdd.Text))
                {
                    this.currentProfile.Dialogue.CancelPhrases.Add(tb_DialogueGoodbyeAdd.Text);
                    LoadDialogue();
                }
            }
        }

        private void bttn_DialogueGoodbyeRemove_Click(object sender, EventArgs e)
        {
            if (this.CurrentDialogueCancelPhrase != null)
            {
                this.currentProfile.Dialogue.CancelPhrases.Remove(this.CurrentDialogueCancelPhrase);
                LoadDialogue();
            }
        }

        private void bttn_DialogueGoodbyeRename_Click(object sender, EventArgs e)
        {
            if (this.CurrentDialogueCancelPhrase != null && tb_DialogueGoodbyeRename.Text != string.Empty && !this.currentProfile.Dialogue.CancelPhrases.Contains(tb_DialogueGoodbyeRename.Text))
            {
                this.CurrentDialogueCancelPhrase = tb_DialogueGoodbyeRename.Text;
                LoadDialogue();
            }
        }
        #endregion

        private void cb_VoiceToggle_CheckedChanged(object sender, EventArgs e)
        {
            this.currentProfile.EnableVoicePausing = this.cb_VoiceToggle.Checked;

            LoadVoiceToggle();
        }

        private void bttn_AddPausePhrase_Click(object sender, EventArgs e)
        {
            if ( tb_AddPausePhrase.Text != string.Empty)
            {
                if (!this.currentProfile.PauseRecognitionPhrases.Contains(tb_AddPausePhrase.Text))
                {
                    this.currentProfile.PauseRecognitionPhrases.Add(tb_AddPausePhrase.Text);
                    LoadVoiceToggle();
                }
            }
        }

        private void bttn_RemovePausePhrase_Click(object sender, EventArgs e)
        {
            if (this.CurrentPausePhrase != null)
            {
                this.currentProfile.PauseRecognitionPhrases.Remove(this.CurrentPausePhrase);
                LoadVoiceToggle();
            }
        }

        private void tb_AddPausePhrase_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                bttn_AddPausePhrase_Click(sender, e);
            }
        }

        private void tb_RenamePausePhrase_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                bttn_RenamePausePhrase_Click(sender, e);
            }
        }

        private void bttn_RenamePausePhrase_Click(object sender, EventArgs e)
        {
            if (this.CurrentPausePhrase != null && tb_RenamePausePhrase.Text != string.Empty && !this.currentProfile.PauseRecognitionPhrases.Contains(tb_RenamePausePhrase.Text))
            {
                this.CurrentPausePhrase = tb_RenamePausePhrase.Text;
                LoadVoiceToggle();
            }
        }

        private void bttn_AddUnpausePhrase_Click(object sender, EventArgs e)
        {
            if (tb_AddUnpausePhrase.Text != string.Empty)
            {
                if (!this.currentProfile.UnpauseRecognitionPhrases.Contains(tb_AddUnpausePhrase.Text))
                {
                    this.currentProfile.UnpauseRecognitionPhrases.Add(tb_AddUnpausePhrase.Text);
                    LoadVoiceToggle();
                }
            }
        }

        private void bttn_RemoveUnpausePhrase_Click(object sender, EventArgs e)
        {
            if (this.CurrentUnpausePhrase != null)
            {
                this.currentProfile.UnpauseRecognitionPhrases.Remove(this.CurrentUnpausePhrase);
                LoadVoiceToggle();
            }
        }

        private void tb_AddUnpausePhrase_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                bttn_AddUnpausePhrase_Click(sender, e);
            }
        }

        private void tb_RenameUnpausePhrase_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                bttn_RenameUnpausePhrase_Click(sender, e);
            }
        }

        private void bttn_RenameUnpausePhrase_Click(object sender, EventArgs e)
        {
            if (this.CurrentUnpausePhrase != null && tb_RenameUnpausePhrase.Text != string.Empty && !this.currentProfile.UnpauseRecognitionPhrases.Contains(tb_RenameUnpausePhrase.Text))
            {
                this.CurrentUnpausePhrase = tb_RenameUnpausePhrase.Text;
                LoadVoiceToggle();
            }
        }

        private void lb_UnpausePhrases_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadVoiceToggle();
        }

        private void lb_PausePhrases_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadVoiceToggle();
        }
    }
}

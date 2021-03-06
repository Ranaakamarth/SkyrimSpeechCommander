﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpeechCommander
{
    public class DialogueRecognitionEngine : RecognitionEngine
    {

        const string FILE_DIALOGUETEXT = "CurrentDialogue.diag";
        const string FILE_DIALOGUESTATE = "DialogueState.diag";
        private System.IO.FileSystemWatcher dialogueWatcher;
        private Profile dialogueProfile;
        private int dialoguePosition;

        public DialogueRecognitionEngine()
            : base()
        {
            dialogueWatcher = new System.IO.FileSystemWatcher();
            dialogueWatcher.NotifyFilter = System.IO.NotifyFilters.LastWrite;
            dialogueWatcher.Changed += dialogueWatcher_Changed;
            this.dialogueWatcher.Filter = "*.diag";
        }

        public DialogueRecognitionEngine(Profile profile)
            : base(profile)
        {
            dialogueWatcher = new System.IO.FileSystemWatcher();
            dialogueWatcher.NotifyFilter = System.IO.NotifyFilters.LastWrite;
            dialogueWatcher.Changed += dialogueWatcher_Changed;
            this.dialogueWatcher.Filter = "*.diag";
        }

        public override void StartAsync(System.Speech.Recognition.RecognizeMode mode)
        {
            try
            {
                this.dialogueWatcher.Path = this.currentProfile.Dialogue.FilePath;
                this.dialogueWatcher.EnableRaisingEvents = true;
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Failed to watch for dialogue changes!");
                // this.dialogueWatcher.Path = null;
                //this.tb_DialogueFilePath.Text = string.Empty;
            }

            base.StartAsync(mode);
        }

        public override void Stop()
        {
            this.dialogueWatcher.EnableRaisingEvents = false;

            base.Stop();
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
            string filename = System.IO.Path.Combine(this.currentProfile.Dialogue.FilePath, FILE_DIALOGUESTATE);

            int position = 0;
            bool readSuccess = true;
            do
            {

                try
                {
                    using (System.IO.StreamReader rdr = new System.IO.StreamReader(filename))
                    {
                        readSuccess = true;

                        string line = rdr.ReadLine();
                        int.TryParse(line, out position);
                    }
                }
                catch (System.IO.IOException)
                {
                    readSuccess = false;
                }

            }
            while (!readSuccess);

            if (position != this.dialoguePosition)
            {
                this.dialoguePosition = position;
                Console.WriteLine("Modified Position! " + this.dialoguePosition);
                if (this.dialogueProfile != null)
                {
                    for (int index = 0; index < this.dialogueProfile.Actions.Count - 1; index++) // -1 to ignore goodbye
                    {
                        lock (this.dialogueProfile.Actions[index].Commands)
                        {
                            Command move = this.dialogueProfile.Actions[index].Commands[0];
                            Command tmp;

                            if (this.dialoguePosition > index)
                                tmp = this.currentProfile.Dialogue.CommandPrevious;
                            else
                                tmp = this.currentProfile.Dialogue.CommandNext;

                            move.Repeat = Math.Abs(this.dialoguePosition - index);

                            move.CommandName = String.Format("Go Down {0}", index);
                            move.Key = tmp.Key;
                            move.ModifierKey = tmp.ModifierKey;
                            move.HeldDuration = tmp.HeldDuration;
                            move.PausedDuration = tmp.PausedDuration;
                        }
                    }
                }
            }
        }

        void UpdateDialogueOptions()
        {
            if (this.Running)
            {
                List<Action> actionlist = new List<Action>();
                bool readSuccess = true;
                do
                {

                    try
                    {
                        string filename = System.IO.Path.Combine(this.currentProfile.Dialogue.FilePath, FILE_DIALOGUETEXT);
                        using (System.IO.StreamReader rdr = new System.IO.StreamReader(filename))
                        {
                            readSuccess = true;
                            int index = 0;

                            while (!rdr.EndOfStream)
                            {
                                string text = rdr.ReadLine();
                                if (text.Split('(')[0].Length > 0)
                                    text = text.Split('(')[0];
                                text = System.Text.RegularExpressions.Regex.Replace(text, "[?\"!\\.]", "");
                                //Replace('?', '.').Replace('\"',' ').
                                Action action = new Action();
                                action.ActionName = text;
                                action.Phrases.Add(text);


                                Command move = new Command();
                                Command tmp;

                                if (this.dialoguePosition > index)
                                    tmp = this.currentProfile.Dialogue.CommandPrevious;
                                else
                                    tmp = this.currentProfile.Dialogue.CommandNext;

                                move.Repeat = Math.Abs(this.dialoguePosition - index);

                                move.CommandName = String.Format("Go Down {0}", index);
                                move.Key = tmp.Key;
                                move.ModifierKey = tmp.ModifierKey;
                                move.HeldDuration = tmp.HeldDuration;
                                move.PausedDuration = tmp.PausedDuration;

                                action.Commands.Add(move);
                                action.Commands.Add(this.currentProfile.Dialogue.CommandAccept);
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
                    if (this.dialogueProfile != null && this.dialogueProfile.Actions.Count == prof.Actions.Count)
                    {
                        same = true;
                        for (int i = 0; i < this.dialogueProfile.Actions.Count; i++)
                        {
                            if (this.dialogueProfile.Actions[i].ActionName != prof.Actions[i].ActionName)
                                same = false;
                        }
                    }
                    if (!same)
                    {


                        prof.UpdateGrammar();

                        var changes = new List<UpdateOperation>();
                        changes.Add(new UpdateOperation()
                        {
                            UpdateType = UpdateOperationType.DisableGrammar,
                            Grammar = this.currentProfile.Grammar
                        });
                        changes.Add(new UpdateOperation()
                        {
                            UpdateType = UpdateOperationType.AddGrammar,
                            Grammar = prof.Grammar,
                            AssociatedActions = prof.Actions
                        });

                        if (this.dialogueProfile != null)
                        {
                            changes.Add(new UpdateOperation()
                            {
                                UpdateType = UpdateOperationType.RemoveGrammar,
                                Grammar = this.dialogueProfile.Grammar,
                                AssociatedActions = this.dialogueProfile.Actions
                            });
                            //this.engine.RemoveGrammar(this.dialogProfile.Grammar, this.dialogProfile.Actions);
                        }

                        this.ExecuteGrammarChanges(changes);

                        this.dialogueProfile = prof;
                        Console.WriteLine("Dialogue Mode Initializing");
                    }
                }
                else if (this.dialogueProfile != null)
                {
                    Console.WriteLine("End Dialogue");

                    var changes = new List<UpdateOperation>();
                    changes.Add(new UpdateOperation()
                    {
                        UpdateType = UpdateOperationType.EnableGrammar,
                        Grammar = this.currentProfile.Grammar
                    });
                    changes.Add(new UpdateOperation()
                    {
                        UpdateType = UpdateOperationType.RemoveGrammar,
                        Grammar = this.dialogueProfile.Grammar,
                        AssociatedActions = this.dialogueProfile.Actions
                    });

                    this.ExecuteGrammarChanges(changes);

                    this.dialogueProfile = null;
                }
            }
        }

        private Action GenerateGoodbyeAction()
        {
            Action action = new Action();
            action.ActionName = "Goodbye";
            action.Commands.Add(this.currentProfile.Dialogue.CommandCancel);
            action.Phrases.AddRange(this.currentProfile.Dialogue.CancelPhrases);

            return action;
        }

    }
}

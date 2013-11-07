﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace SpeechCommander.ViewModel
{
    public class ActionTabViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        public ObservableCollection<Model.Action> Actions
        {
            get
            {
                return actions;
            }
            set
            {
                if (actions != value)
                {
                    actions = value;
                    //RaisePropertyChanged("Actions");
                }
            }
        }
        private ObservableCollection<Model.Action> actions;

        public Model.Action CurrentAction
        {
            get
            {
                return currentAction;
            }
            set
            {
                if (currentAction != value)
                {
                    currentAction = value;
                    if (this.currentAction != null && this.currentAction.ActionName != null)
                        this.ActionName = currentAction.ActionName;
                    else
                        this.ActionName = string.Empty;
                    RaisePropertyChanged("CurrentAction");
                }
            }
        }
        private Model.Action currentAction;

        public string ActionName
        {
            get
            {
                return actionName;
            }
            set
            {
                if (actionName != value)
                {
                    actionName = value;
                    RaisePropertyChanged("ActionName");
                }
            }
        }
        private string actionName;

        public string PhraseName
        {
            get
            {
                return phraseName;
            }
            set
            {
                if (phraseName != value)
                {
                    phraseName = value;
                    RaisePropertyChanged("PhraseName");
                }
            }
        }
        private string phraseName;

        public string CurrentPhrase
        {
            get
            {
                return currentPhrase;
            }
            set
            {
                if (currentPhrase != value)
                {
                    currentPhrase = value;
                    if (this.currentPhrase != null)
                        this.PhraseName = currentPhrase;
                    else
                        this.PhraseName = string.Empty;
                    RaisePropertyChanged("CurrentPhrase");
                }
            }
        }
        private string currentPhrase;

        public Model.Command CurrentCommand
        {
            get
            {
                return currentCommand;
            }
            set
            {
                if (currentCommand != value)
                {
                    currentCommand = value;
                    if (this.currentCommand != null)
                    {
                        this.CommandName = this.CurrentCommand.CommandName;

                        if (this.CommandsVM == null)
                            this.CommandsVM = new ViewModel.CommandsViewModel() { Command = this.CurrentCommand };
                        else
                            this.CommandsVM.Command = this.CurrentCommand;
                    }
                    else
                    {
                        this.CommandName = string.Empty;
                        this.CommandsVM = null;
                    }
                    RaisePropertyChanged("CurrentCommand");
                }
            }
        }
        private Model.Command currentCommand;

        public ViewModel.CommandsViewModel CommandsVM
        {
            get
            {
                return this.commandVM;
            }
            set
            {
                if (commandVM != value)
                {
                    commandVM = value;
                    RaisePropertyChanged("CommandsVM");
                }
            }
        }
        private ViewModel.CommandsViewModel commandVM;

        public string CommandName
        {
            get
            {
                return commandName;
            }
            set
            {
                if (commandName != value)
                {
                    commandName = value;
                    RaisePropertyChanged("CommandName");
                }
            }
        }
        private string commandName;

        public ActionTabViewModel(System.Collections.ObjectModel.ObservableCollection<Model.Action> actions)
        {
            this.Actions = actions;

            this.AddActionCommand = new ActionCommand(this.AddAction,
              () => this.ActionName != null &&
                    !this.Actions.Any(act => act.ActionName == this.ActionName.Trim()) &&
                    this.ActionName.Trim().Length > 0);
            this.RemoveActionCommand = new ActionCommand(this.RemoveAction,
              () => this.CurrentAction != null);
            this.RenameActionCommand = new ActionCommand(this.RenameAction,
              () => this.ActionName != null &&
                    this.CurrentAction != null && !this.Actions.Any(act => act.ActionName == this.ActionName.Trim()) &&
                    this.ActionName.Trim().Length > 0);

            this.AddCommandCommand = new ActionCommand(this.AddCommand,
              () => this.CurrentAction != null &&
                    this.CommandName != null &&
                    !this.CurrentAction.Commands.Any(cmd => cmd.CommandName == this.CommandName.Trim()) &&
                    this.CommandName.Trim().Length > 0);
            this.RemoveCommandCommand = new ActionCommand(this.RemoveCommand,
              () => this.CurrentAction != null &&
                    this.CurrentCommand != null);
            this.RenameCommandCommand = new ActionCommand(this.RenameCommand,
              () => this.CurrentAction != null &&
                    this.CommandName != null &&
                    this.CurrentCommand != null &&
                    !this.CurrentAction.Commands.Any(cmd => cmd.CommandName == this.CommandName.Trim()) &&
                    this.CommandName.Trim().Length > 0);

            this.AddPhraseCommand = new ActionCommand(this.AddPhrase, 
              () => this.CurrentAction != null &&
                    this.PhraseName != null &&
                    !this.CurrentAction.Phrases.Any(phr => phr == this.PhraseName.Trim()) &&
                    this.PhraseName.Trim().Length > 0);
            this.RemovePhraseCommand = new ActionCommand(this.RemovePhrase, 
              () => this.CurrentAction != null &&
            this.CurrentPhrase != null);
            this.RenamePhraseCommand = new ActionCommand(this.RenamePhrase,               
              () => this.CurrentAction != null &&
                    this.PhraseName != null &&
                    this.CurrentPhrase != null &&
                    !this.CurrentAction.Phrases.Any(phr => phr == this.PhraseName.Trim()) &&
                    this.PhraseName.Trim().Length > 0);
        }

        private void AddAction()
        {
            Model.Action action = new Model.Action() { ActionName = this.ActionName.Trim() };
            this.Actions.Add(action);
            this.CurrentAction = action;
            //this.ActionName = string.Empty;
        }

        private void RenameAction()
        {
            this.CurrentAction.ActionName = this.ActionName.Trim();
        }

        private void RemoveAction()
        {
            this.Actions.Remove(this.CurrentAction);
            //this.CurrentAction = null;
        }

        private void AddCommand()
        {
            Model.Command cmd = new Model.Command() { CommandName = this.CommandName };
            this.CurrentAction.Commands.Add(cmd);
            this.CurrentCommand = cmd;
        }

        private void RenameCommand()
        {
            this.CurrentCommand.CommandName = this.CommandName.Trim();
        }

        private void RemoveCommand()
        {
            this.CurrentAction.Commands.Remove(this.CurrentCommand);
        }

        private void AddPhrase()
        {
            this.CurrentAction.Phrases.Add(this.PhraseName.Trim());
            this.CurrentPhrase = this.PhraseName.Trim();
        }

        private void RenamePhrase()
        {
            int index = this.CurrentAction.Phrases.IndexOf(this.CurrentPhrase);
            this.CurrentAction.Phrases[index] = this.PhraseName.Trim();
            this.CurrentPhrase = this.CurrentAction.Phrases[index];
        }

        private void RemovePhrase()
        {
            this.CurrentAction.Phrases.Remove(this.CurrentPhrase);
        }

        #region ICommands
        public ICommand AddActionCommand
        {
            get;
            private set;
        }

        public ICommand RemoveActionCommand
        {
            get;
            private set;
        }

        public ICommand RenameActionCommand
        {
            get;
            private set;
        }

        public ICommand AddCommandCommand
        {
            get;
            private set;
        }

        public ICommand RemoveCommandCommand
        {
            get;
            private set;
        }

        public ICommand RenameCommandCommand
        {
            get;
            private set;
        }

        public ICommand AddPhraseCommand
        {
            get;
            private set;
        }

        public ICommand RemovePhraseCommand
        {
            get;
            private set;
        }

        public ICommand RenamePhraseCommand
        {
            get;
            private set;
        }
        #endregion

        public void RaisePropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(name));
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }
}

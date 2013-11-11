using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace SpeechCommander.ViewModel
{
    public class ActionViewModel : System.ComponentModel.INotifyPropertyChanged
    {

        public Model.Action Action
        {
            get
            {
                return action;
            }
            set
            {
                if (action != value)
                {
                    action = value;
                    RaisePropertyChanged("Action");
                }
            }
        }
        private Model.Action action;

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
                            this.CommandsVM = new ViewModel.CommandsViewModel(this.CurrentCommand);
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

        public ActionViewModel(Model.Action action)
        {
            this.Action = action;

            if (this.Action.Commands != null && this.Action.Commands.Count == 1)
            {
                this.CurrentCommand = this.Action.Commands[0];
            }

            this.AddCommandCommand = new ActionCommand(this.AddCommand,
              () => this.Action != null &&
                    this.CommandName != null &&
                    !this.Action.Commands.Any(cmd => cmd.CommandName == this.CommandName.Trim()) &&
                    this.CommandName.Trim().Length > 0);
            this.RemoveCommandCommand = new ActionCommand(this.RemoveCommand,
              () => this.Action != null &&
                    this.CurrentCommand != null);
            this.RenameCommandCommand = new ActionCommand(this.RenameCommand,
              () => this.Action != null &&
                    this.CommandName != null &&
                    this.CurrentCommand != null &&
                    !this.Action.Commands.Any(cmd => cmd.CommandName == this.CommandName.Trim()) &&
                    this.CommandName.Trim().Length > 0);

            this.AddPhraseCommand = new ActionCommand(this.AddPhrase,
              () => this.Action != null &&
                    this.PhraseName != null &&
                    !this.Action.Phrases.Any(phr => phr == this.PhraseName.Trim()) &&
                    this.PhraseName.Trim().Length > 0);
            this.RemovePhraseCommand = new ActionCommand(this.RemovePhrase,
              () => this.Action != null &&
            this.CurrentPhrase != null);
            this.RenamePhraseCommand = new ActionCommand(this.RenamePhrase,
              () => this.Action != null &&
                    this.PhraseName != null &&
                    this.CurrentPhrase != null &&
                    !this.Action.Phrases.Any(phr => phr == this.PhraseName.Trim()) &&
                    this.PhraseName.Trim().Length > 0);
        }

        private void AddCommand()
        {
            Model.Command cmd = new Model.Command() { CommandName = this.CommandName };
            this.Action.Commands.Add(cmd);
            this.CurrentCommand = cmd;
        }

        private void RenameCommand()
        {
            this.CurrentCommand.CommandName = this.CommandName.Trim();
        }

        private void RemoveCommand()
        {
            this.Action.Commands.Remove(this.CurrentCommand);
        }

        private void AddPhrase()
        {
            this.Action.Phrases.Add(this.PhraseName.Trim());
            this.CurrentPhrase = this.PhraseName.Trim();
        }

        private void RenamePhrase()
        {
            int index = this.Action.Phrases.IndexOf(this.CurrentPhrase);
            this.Action.Phrases[index] = this.PhraseName.Trim();
            this.CurrentPhrase = this.Action.Phrases[index];
        }

        private void RemovePhrase()
        {
            this.Action.Phrases.Remove(this.CurrentPhrase);
        }

        #region ICommands
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

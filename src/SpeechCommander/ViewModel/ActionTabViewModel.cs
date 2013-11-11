using System;
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
                    if (this.currentAction != null)
                    {
                        if (this.ActionVM == null)
                            this.ActionVM = new ActionViewModel(this.CurrentAction);
                        else
                            this.ActionVM.Action = this.CurrentAction;

                        if (this.CurrentAction.Commands.Count == 1)
                        {
                            this.ActionVM.CurrentCommand = this.CurrentAction.Commands[0];
                        }

                        if (this.currentAction.ActionName != null)
                            this.ActionName = currentAction.ActionName;
                        else
                            this.ActionName = string.Empty;
                    }
                    else
                        this.ActionVM = null;

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

        public ActionViewModel ActionVM
        {
            get
            {
                return actionVM;
            }
            set
            {
                if (actionVM != value)
                {
                    actionVM = value;
                    RaisePropertyChanged("ActionVM");
                }
            }
        }
        private ActionViewModel actionVM;

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

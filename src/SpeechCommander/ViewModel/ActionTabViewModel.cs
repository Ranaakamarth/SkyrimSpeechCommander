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
        public List<Model.Action> Actions
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
                    RaisePropertyChanged("Actions");
                }
            }
        }
        private List<Model.Action> actions;
        //public ObservableCollection<Model.Action> Actions
        //{
        //    get
        //    {
        //        return actions;
        //    }
        //    set
        //    {
        //        if (actions != value)
        //        {
        //            actions = value;
        //            RaisePropertyChanged("Actions");
        //        }
        //    }
        //}

        //private ObservableCollection<Model.Action> actions;

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
                    RaisePropertyChanged("CurrentAction");
                }
            }
        }
        private Model.Action currentAction;

        public ActionTabViewModel()
        {
            this.AddActionCommand = new ActionCommand(this.AddAction, () => true);
            this.RemoveActionCommand = new ActionCommand(this.RemoveAction, () => this.CurrentAction != null);
            this.RenameActionCommand = new ActionCommand(this.RenameAction, () => this.CurrentAction != null);
            //Console.WriteLine("Created Action tab vm");
        }

        private void AddAction()
        {
            this.Actions.Add(new Model.Action() { ActionName = "Brand new action!" });
            Console.WriteLine();
        }

        private void RenameAction()
        {
            this.CurrentAction.ActionName = "New Name";
        }

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

        private void RemoveAction()
        {
            if (this.CurrentAction != null)
            {
                this.Actions.Remove(this.CurrentAction);
                Console.WriteLine();
            }
        }

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

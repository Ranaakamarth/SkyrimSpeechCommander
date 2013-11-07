using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;

namespace SpeechCommander.Model
{
    [DataContract(Name="Action", Namespace="")]
    public class Action : System.ComponentModel.INotifyPropertyChanged
    {
        [DataMember()]
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
        [DataMember()]
        public ObservableCollection<string> Phrases
        {
            get
            {
                return phrases;
            }
            set
            {
                if (phrases != value)
                {
                    phrases = value;
                    RaisePropertyChanged("Phrases");
                }
            }
        }
        private ObservableCollection<string> phrases;
        [DataMember()]
        public int Repeat
        {
            get
            {
                return repeat;
            }
            set
            {
                if (repeat != value)
                {
                    repeat = value;
                    RaisePropertyChanged("Repeat");
                }
            }
        }
        private int repeat;
        [DataMember()]
        public int PausedDuration
        {
            get
            {
                return pausedDuration;
            }
            set
            {
                if (pausedDuration != value)
                {
                    pausedDuration = value;
                    RaisePropertyChanged("PausedDuration");
                }
            }
        }
        private int pausedDuration;
        [DataMember()]
        public ObservableCollection<Command> Commands
        {
            get
            {
                return commands;
            }
            set
            {
                if (commands != value)
                {
                    commands = value;
                    RaisePropertyChanged("Commands");
                }
            }
        }
        private ObservableCollection<Command> commands;

        public Action()
        {
            this.Commands = new ObservableCollection<Command>();
            this.Phrases = new ObservableCollection<string>();
            this.Repeat = 1;
            this.PausedDuration = 25;
        }

        public void Execute()
        {
            lock (this.Commands)
            {
                for (int i = 0; i < this.Repeat; ++i)
                {
                    foreach (Command cmd in this.Commands)
                    {
                        cmd.Execute();
                    }
                    System.Threading.Thread.Sleep(this.PausedDuration);
                }
            }
        }

        public override string ToString()
        {
            return this.ActionName;
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

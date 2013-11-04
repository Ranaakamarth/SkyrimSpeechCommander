using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using sp = System.Speech.Recognition;
using System.Collections.ObjectModel;

namespace SpeechCommander.Model
{
    [DataContract(Name="DialogueProfile", Namespace="")]
    public class DialogueProfile : System.ComponentModel.INotifyPropertyChanged
    {
        [DataMember()]
        public bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                if (enabled != value)
                {
                    enabled = value;
                    RaisePropertyChanged("Enabled");
                }
            }
        }
        private bool enabled;
        [DataMember()]
        public string FilePath
        {
            get
            {
                return filePath;
            }
            set
            {
                if (filePath != value)
                {
                    filePath = value;
                    RaisePropertyChanged("FilePath");
                }
            }
        }
        private string filePath;
        [DataMember()]
        public Command CommandPrevious
        {
            get
            {
                return commandPrevious;
            }
            set
            {
                if (commandPrevious != value)
                {
                    commandPrevious = value;
                    RaisePropertyChanged("CommandPrevious");
                }
            }
        }
        private Command commandPrevious;
        [DataMember()]
        public Command CommandNext
        {
            get
            {
                return commandNext;
            }
            set
            {
                if (commandNext != value)
                {
                    commandNext = value;
                    RaisePropertyChanged("CommandNext");
                }
            }
        }
        private Command commandNext;
        [DataMember()]
        public Command CommandAccept
        {
            get
            {
                return commandAccept;
            }
            set
            {
                if (commandAccept != value)
                {
                    commandAccept = value;
                    RaisePropertyChanged("CommandAccept");
                }
            }
        }
        private Command commandAccept;
        [DataMember()]
        public Command CommandCancel
        {
            get
            {
                return commandCancel;
            }
            set
            {
                if (commandCancel != value)
                {
                    commandCancel = value;
                    RaisePropertyChanged("CommandCancel");
                }
            }
        }
        private Command commandCancel;
        [DataMember()]
        public ObservableCollection<string> CancelPhrases
        {
            get
            {
                return cancelPhrases;
            }
            set
            {
                if (cancelPhrases != value)
                {
                    cancelPhrases = value;
                    RaisePropertyChanged("CancelPhrases");
                }
            }
        }
        private ObservableCollection<string> cancelPhrases;

        public DialogueProfile()
        {
            this.CancelPhrases = new ObservableCollection<string>();
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

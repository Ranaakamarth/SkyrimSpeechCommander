using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechCommander.ViewModel
{
    public class CommandsViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        public Model.Command Command
        {
            get
            {
                return command;
            }
            set
            {
                if (command != value)
                {
                    command = value;
                    RaisePropertyChanged("Command");
                }
            }
        }
        private Model.Command command;

        public CommandsViewModel()
        {
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechCommander.ViewModel
{
    public class MainViewModel //: System.ComponentModel.INotifyPropertyChanged
    {
        private Model.Profile profile;
        public Model.Profile Profile
        {
            get
            {
                return this.profile;
            }
            set
            {
                if (this.profile != value)
                {
                    this.profile = value;
                    //RaisePropertyChanged("Profile");
                    //this.ActionViewModel = new ActionTabViewModel() { Actions = this.Profile.Actions };
                }
            }
        }

        private ViewModel.ActionTabViewModel actionViewModel;
        public ViewModel.ActionTabViewModel ActionViewModel
        {
            get
            {
                return actionViewModel;
            }
            set
            {
                if (actionViewModel != value)
                {
                    actionViewModel = value;
                    //RaisePropertyChanged("ActionViewModel");
                }
            }
        }

        public MainViewModel()
        {
            this.Profile = new Model.Profile();
            this.ActionViewModel = new ActionTabViewModel() { Actions = Profile.Actions };
            //this.ActionTabViewModel = new ViewModel.ActionTabViewModel() { Actions = this.Profile.Actions };
            Console.WriteLine("Created View Model");
        }

        //public void RaisePropertyChanged(string name)
        //{
        //    if (this.PropertyChanged != null)
        //    {
        //        this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(name));
        //    }
        //}

        //public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }
}

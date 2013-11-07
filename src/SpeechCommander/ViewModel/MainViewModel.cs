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
                    this.ActionVM = new ActionTabViewModel(this.Profile.Actions);
                    this.ProfileVM = new ProfileViewModel(this.Profile);
                }
            }
        }

        private ViewModel.ActionTabViewModel actionVM;
        public ViewModel.ActionTabViewModel ActionVM
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
                    //RaisePropertyChanged("ActionViewModel");
                }
            }
        }

        private ViewModel.ProfileViewModel profileVM;
        public ViewModel.ProfileViewModel ProfileVM
        {
            get
            {
                return profileVM;
            }
            set
            {
                if (profileVM != value)
                {
                    profileVM = value;

                }
            }
        }

        public MainViewModel()
        {
            this.Profile = new Model.Profile();
            this.ActionVM = new ActionTabViewModel(this.Profile.Actions);
            this.ProfileVM = new ProfileViewModel(this.Profile);
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

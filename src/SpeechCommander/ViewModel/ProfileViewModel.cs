using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace SpeechCommander.ViewModel
{
    public class ProfileViewModel : INotifyPropertyChanged
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

        private ViewModel.VoicePausingViewModel voicePausingVM;
        public ViewModel.VoicePausingViewModel VoicePausingVM
        {
            get
            {
                return voicePausingVM;
            }
            set
            {
                if (voicePausingVM != value)
                {
                    voicePausingVM = value;
                }
            }
        }

        public ProfileViewModel(Model.Profile profile)
        {
            this.Profile = profile;
            this.VoicePausingVM = new VoicePausingViewModel(this.Profile);
        }

        public void RaisePropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

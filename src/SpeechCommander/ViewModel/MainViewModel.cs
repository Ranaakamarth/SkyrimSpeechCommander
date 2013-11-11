using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SpeechCommander.ViewModel
{
    public class MainViewModel : System.ComponentModel.INotifyPropertyChanged
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

                    this.ActionVM = new ActionTabViewModel(this.Profile.Actions);
                    this.ProfileVM = new ProfileViewModel(this.Profile);
                    RaisePropertyChanged("Profile");
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
                    RaisePropertyChanged("ActionVM");
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
                    RaisePropertyChanged("ProfileVM");
                }
            }
        }

        private Model.DialogueRecognitionEngine engine;
        public Model.DialogueRecognitionEngine Engine
        {
            get
            {
                return engine;
            }
            set
            {
                if (engine != value)
                {
                    engine = value;
                }
            }
        }

        public MainViewModel()
        {
            this.Profile = new Model.Profile();
            this.ActionVM = new ActionTabViewModel(this.Profile.Actions);
            this.ProfileVM = new ProfileViewModel(this.Profile);
            this.Engine = new Model.DialogueRecognitionEngine();

            this.NewProfileCommand  = new ActionCommand( NewProfile, () => true);
            this.OpenProfileCommand = new ActionCommand( OpenProfile, () => true);
            this.SaveProfileCommand = new ActionCommand( SaveProfile, () => true);

            this.StartRecognitionCommand = new ActionCommand(StartRecognition, () => this.Profile.Actions.Count > 0 && !Engine.Running);
            this.StopRecognitionCommand = new ActionCommand(StopRecognition, () => Engine.Running);
        }

        private void StartRecognition()
        {
            Engine.LoadProfile(this.Profile);
            //Engine = new Model.DialogueRecognitionEngine(this.Profile);
            Engine.StartAsync(System.Speech.Recognition.RecognizeMode.Multiple);
        }

        private void StopRecognition()
        {
            Engine.Stop();
        }

        private void NewProfile()
        {
            this.Profile = new Model.Profile();
        }

        private void OpenProfile()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.DefaultExt = ".xml";
            openFileDialog.Title = "Open a profile";
            openFileDialog.Filter = "Profiles (.xml)|*.xml";

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                this.Profile = Model.Profile.Open(openFileDialog.FileName);
            }
        }

        private void SaveProfile()
        {

            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.DefaultExt = ".xml";
            saveFileDialog.Title = "Save a profile";
            saveFileDialog.Filter = "Profiles (.xml)|*.xml";

            bool? result = saveFileDialog.ShowDialog();

            if (result == true)
            {
                this.Profile.Save(saveFileDialog.FileName);
            }
        }

        public ICommand StartRecognitionCommand
        {
            get;
            private set;
        }

        public ICommand StopRecognitionCommand
        {
            get;
            private set;
        }

        public ICommand NewProfileCommand
        {
            get;
            private set;
        }

        public ICommand OpenProfileCommand
        {
            get;
            private set;
        }

        public ICommand SaveProfileCommand
        {
            get;
            private set;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.ComponentModel;

namespace SpeechCommander.ViewModel
{
    public class VoicePausingViewModel : INotifyPropertyChanged
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

        private string pausePhraseName;
        public string PausePhraseName
        {
            get
            {
                return pausePhraseName;
            }
            set
            {
                if (pausePhraseName != value)
                {
                    pausePhraseName = value;
                    RaisePropertyChanged("PausePhraseName");
                }
            }
        }

        private string currentPausePhrase;
        public string CurrentPausePhrase
        {
            get
            {
                return currentPausePhrase;
            }
            set
            {
                if (currentPausePhrase != value)
                {
                    currentPausePhrase = value;
                    RaisePropertyChanged("CurrentPausePhrase");
                }
            }
        }

        private string unpausePhraseName;
        public string UnpausePhraseName
        {
            get
            {
                return unpausePhraseName;
            }
            set
            {
                if (unpausePhraseName != value)
                {
                    unpausePhraseName = value;
                    RaisePropertyChanged("UnpausePhraseName");
                }
            }
        }

        private string currentUnpausePhrase;
        public string CurrentUnpausePhrase
        {
            get
            {
                return currentUnpausePhrase;
            }
            set
            {
                if (currentUnpausePhrase != value)
                {
                    currentUnpausePhrase = value;
                    RaisePropertyChanged("CurrentUnpausePhrase");
                }
            }
        }

        public VoicePausingViewModel(Model.Profile profile)
        {
            this.Profile = profile;

            this.AddPausePhraseCommand = new ActionCommand(this.AddPausePhrase,
                () => this.PausePhraseName != null &&
                      this.PausePhraseName.Trim().Length > 0 &&
                      !this.Profile.PauseRecognitionPhrases.Any(phr => phr == this.PausePhraseName.Trim()));
            this.RemovePausePhraseCommand = new ActionCommand(this.RemovePausePhrase,
                () => this.CurrentPausePhrase != null);
            this.RenamePausePhraseCommand = new ActionCommand(this.RenamePausePhrase,
                () => this.CurrentPausePhrase != null &&
                      this.PausePhraseName != null &&
                      this.PausePhraseName.Trim().Length > 0 &&
                      !this.Profile.PauseRecognitionPhrases.Any(phr => phr == this.PausePhraseName.Trim()));

            this.AddUnpausePhraseCommand = new ActionCommand(this.AddUnpausePhrase,
                () => this.UnpausePhraseName != null &&
                      this.UnpausePhraseName.Trim().Length > 0 &&
                      !this.Profile.UnpauseRecognitionPhrases.Any(phr => phr == this.UnpausePhraseName.Trim()));
            this.RemoveUnpausePhraseCommand = new ActionCommand(this.RemoveUnpausePhrase,
                () => this.CurrentUnpausePhrase != null);
            this.RenameUnpausePhraseCommand = new ActionCommand(this.RenameUnpausePhrase,
                () => this.CurrentUnpausePhrase != null &&
                      this.UnpausePhraseName != null &&
                      this.UnpausePhraseName.Trim().Length > 0 &&
                      !this.Profile.UnpauseRecognitionPhrases.Any(phr => phr == this.UnpausePhraseName.Trim()));
        }

        private void AddPausePhrase()
        {
            this.Profile.PauseRecognitionPhrases.Add(this.PausePhraseName.Trim());
            this.CurrentPausePhrase = this.PausePhraseName.Trim();
        }

        private void RemovePausePhrase()
        {
            this.Profile.PauseRecognitionPhrases.Remove(this.CurrentPausePhrase);
        }

        private void RenamePausePhrase()
        {
            int index = this.Profile.PauseRecognitionPhrases.IndexOf(this.CurrentPausePhrase);
            this.Profile.PauseRecognitionPhrases[index] = this.PausePhraseName.Trim();
            this.CurrentPausePhrase = this.Profile.PauseRecognitionPhrases[index];
        }

        private void AddUnpausePhrase()
        {
            this.Profile.UnpauseRecognitionPhrases.Add(this.UnpausePhraseName.Trim());
            this.CurrentUnpausePhrase = this.UnpausePhraseName.Trim();
        }

        private void RemoveUnpausePhrase()
        {
            this.Profile.UnpauseRecognitionPhrases.Remove(this.CurrentUnpausePhrase);
        }

        private void RenameUnpausePhrase()
        {
            int index = this.Profile.UnpauseRecognitionPhrases.IndexOf(this.CurrentUnpausePhrase);
            this.Profile.UnpauseRecognitionPhrases[index] = this.UnpausePhraseName.Trim();
            this.CurrentUnpausePhrase = this.Profile.UnpauseRecognitionPhrases[index];
        }

        #region ICommands
        public ICommand AddPausePhraseCommand
        {
            get;
            private set;
        }

        public ICommand RemovePausePhraseCommand
        {
            get;
            private set;
        }

        public ICommand RenamePausePhraseCommand
        {
            get;
            private set;
        }

        public ICommand AddUnpausePhraseCommand
        {
            get;
            private set;
        }

        public ICommand RemoveUnpausePhraseCommand
        {
            get;
            private set;
        }

        public ICommand RenameUnpausePhraseCommand
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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using sp = System.Speech.Recognition;
using System.Collections.ObjectModel;


namespace SpeechCommander.Model
{
    [DataContract(Name="Profile", Namespace="")]
    public class Profile : System.ComponentModel.INotifyPropertyChanged
    {
        [DataMember()]
        public ObservableCollection<Action> Actions
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
        private ObservableCollection<Action> actions;
        [DataMember()]
        public string ProfileName
        {
            get
            {
                return profileName;
            }
            set
            {
                if (profileName != value)
                {
                    profileName = value;
                    RaisePropertyChanged("ProfileName");
                }
            }
        }
        private string profileName;
        [DataMember()]
        public double RequiredConfidence
        {
            get
            {
                return requiredConfidence;
            }
            set
            {
                if (requiredConfidence != value)
                {
                    requiredConfidence = value;
                    RaisePropertyChanged("RequiredConfidence");
                }
            }
        }
        private double requiredConfidence;
        [DataMember()]
        public bool EnableVoicePausing
        {
            get
            {
                return enableVoicePausing;
            }
            set
            {
                if (enableVoicePausing != value)
                {
                    enableVoicePausing = value;
                    RaisePropertyChanged("EnableVoicePausing");
                }
            }
        }
        private bool enableVoicePausing;
        [DataMember()]
        public ObservableCollection<string> PauseRecognitionPhrases
        {
            get
            {
                return pauseRecognitionPhrases;
            }
            set
            {
                if (pauseRecognitionPhrases != value)
                {
                    pauseRecognitionPhrases = value;
                    RaisePropertyChanged("PauseRecognitionPhrases");
                }
            }
        }
        private ObservableCollection<string> pauseRecognitionPhrases;
        [DataMember()]
        public ObservableCollection<string> UnpauseRecognitionPhrases
        {
            get
            {
                return unpauseRecognitionPhrases;
            }
            set
            {
                if (unpauseRecognitionPhrases != value)
                {
                    unpauseRecognitionPhrases = value;
                    RaisePropertyChanged("UnpauseRecognitionPhrases");
                }
            }
        }
        private ObservableCollection<string> unpauseRecognitionPhrases;
        [DataMember()]
        public DialogueProfile Dialogue
        {
            get
            {
                return dialogue;
            }
            set
            {
                if (dialogue != value)
                {
                    dialogue = value;
                    RaisePropertyChanged("Dialogue");
                }
            }
        }
        private DialogueProfile dialogue;

        public sp.Grammar Grammar { get; set; }

        public Profile()
        {
            this.ProfileName = string.Empty;
            this.Actions = new ObservableCollection<Action>();
            this.PauseRecognitionPhrases = new ObservableCollection<string>();
            this.UnpauseRecognitionPhrases = new ObservableCollection<string>();
            this.Dialogue = new DialogueProfile();
        }

        public void Save(string filename)
        {
            using (System.IO.FileStream stream = System.IO.File.Create(filename))
            {
                this.Save(stream);
            }
        }

        public void Save(System.IO.Stream stream)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(Profile));
            serializer.WriteObject(stream, this);
        }

        public static Profile Open(string filename)
        {
            using (System.IO.FileStream stream = System.IO.File.OpenRead(filename))
            {
                return Profile.Open(stream);
            }
        }

        public static Profile Open(System.IO.Stream stream)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(Profile));
            Profile openedProfile = serializer.ReadObject(stream) as Profile;

            if (openedProfile.Actions == null)
                openedProfile.Actions = new ObservableCollection<Action>();
            if (openedProfile.PauseRecognitionPhrases == null)
                openedProfile.PauseRecognitionPhrases = new ObservableCollection<string>();
            if (openedProfile.UnpauseRecognitionPhrases == null)
                openedProfile.UnpauseRecognitionPhrases = new ObservableCollection<string>();
            if (openedProfile.Dialogue == null)
                openedProfile.Dialogue = new DialogueProfile();

            return openedProfile;
        }

        public sp.Grammar UpdateGrammar()
        {
            sp.Choices choices = new sp.Choices();
            foreach (Action action in this.Actions)
            {
                foreach (string phrase in action.Phrases)
                {
                    sp.SemanticResultValue temp = new sp.SemanticResultValue(phrase, action.ActionName);

                    choices.Add(temp);
                }
            }

            sp.GrammarBuilder builder = new sp.GrammarBuilder();
            builder.Append(new sp.SemanticResultKey("command", choices));
            this.Grammar = new sp.Grammar(builder);
            this.Grammar.Name = this.ProfileName;
            return this.Grammar;
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

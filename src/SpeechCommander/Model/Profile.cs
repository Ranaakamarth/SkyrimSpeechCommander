using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using sp = System.Speech.Recognition;

namespace SpeechCommander.Model
{
    [DataContract(Name="Profile", Namespace="")]
    public class Profile
    {
        [DataMember()]
        public List<Action> Actions { get; set; }
        [DataMember()]
        public string ProfileName { get; set; }
        [DataMember()]
        public double RequiredConfidence { get; set; }
        [DataMember()]
        public bool EnableVoicePausing { get; set; }
        [DataMember()]
        public List<string> PauseRecognitionPhrases { get; set; }
        [DataMember()]
        public List<string> UnpauseRecognitionPhrases { get; set; }
        [DataMember()]
        public DialogueProfile Dialogue { get; set; }

        public sp.Grammar Grammar { get; set; }

        public Profile()
        {
            this.Actions = new List<Action>();
            this.PauseRecognitionPhrases = new List<string>();
            this.UnpauseRecognitionPhrases = new List<string>();
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

        public void Open(string filename)
        {
            using (System.IO.FileStream stream = System.IO.File.OpenRead(filename))
            {
                this.Open(stream);
            }
        }

        public void Open(System.IO.Stream stream)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(Profile));
            Profile openedProfile = serializer.ReadObject(stream) as Profile;

            this.ProfileName = openedProfile.ProfileName;
            this.Actions = openedProfile.Actions;
            this.RequiredConfidence = openedProfile.RequiredConfidence;
            this.PauseRecognitionPhrases = openedProfile.PauseRecognitionPhrases;
            this.UnpauseRecognitionPhrases = openedProfile.UnpauseRecognitionPhrases;
            this.EnableVoicePausing = openedProfile.EnableVoicePausing;
            this.Dialogue = openedProfile.Dialogue;


            if (this.Actions == null)
                this.Actions = new List<Action>();
            if (this.PauseRecognitionPhrases == null)
                this.PauseRecognitionPhrases = new List<string>();
            if (this.UnpauseRecognitionPhrases == null)
                this.UnpauseRecognitionPhrases = new List<string>();
            if (this.Dialogue == null)
                this.Dialogue = new DialogueProfile();
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using sp = System.Speech.Recognition;

namespace SpeechCommander
{
    [DataContractAttribute(Name = "Profile", Namespace = "")]
    public class Profile
    {
        [DataMember()]
        public List<Action> Actions { get; set; }

        [DataMember()]
        public string ProfileName { get; set; }

        [DataMember()]
        public double RequiredConfidence { get; set; }

        [DataMember()]
        public int EndTimeout { get; set; }

        [DataMember()]
        public DialogueProfile Dialogue { get; set; }

        [DataMember()]
        public bool EnableVoicePausing { get; set; }

        [DataMember()]
        public List<string> PauseRecognitionPhrases { get; set; }

        [DataMember()]
        public List<string> UnpauseRecognitionPhrases { get; set; }

        public sp.Grammar Grammar { get; set; }

        public Profile()
        {
            this.Actions = new List<Action>();
            this.Dialogue = new DialogueProfile();
            this.PauseRecognitionPhrases = new List<string>();
            this.UnpauseRecognitionPhrases = new List<string>();
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
            if (this.Actions == null)
                this.Actions = new List<Action>();
            this.Dialogue = openedProfile.Dialogue;
            if (this.Dialogue == null)
                this.Dialogue = new DialogueProfile();
            this.EndTimeout = openedProfile.EndTimeout;
            this.RequiredConfidence = openedProfile.RequiredConfidence;
            this.PauseRecognitionPhrases = openedProfile.PauseRecognitionPhrases;
            if (this.PauseRecognitionPhrases == null)
                this.PauseRecognitionPhrases = new List<string>();
            this.UnpauseRecognitionPhrases = openedProfile.UnpauseRecognitionPhrases;
            if (this.UnpauseRecognitionPhrases == null)
                this.UnpauseRecognitionPhrases = new List<string>();
            this.EnableVoicePausing = openedProfile.EnableVoicePausing;
        }

        public sp.Grammar UpdateGrammar()
        {
            sp.GrammarBuilder commands = new sp.GrammarBuilder();
            sp.Choices choices = new sp.Choices();
            foreach (Action action in this.Actions)
            {
                foreach (string phrase in action.Phrases)
                {
                    sp.SemanticResultValue temp = new sp.SemanticResultValue(phrase, action.ActionName);
                    choices.Add(temp);
                    commands.Append(temp);
                }
            }
            commands.Append(choices);

            sp.GrammarBuilder builder = new sp.GrammarBuilder();
            builder.Append(new sp.SemanticResultKey("command", choices));
            this.Grammar = new sp.Grammar(builder);
            this.Grammar.Name = this.ProfileName;
            return this.Grammar;
        }
    }
}

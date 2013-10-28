using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using sp = System.Speech.Recognition;

namespace SpeechCommander
{
    [DataContractAttribute(Name="Profile", Namespace="")]
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
        public bool EngageNpcDialogue { get; set; }

        [DataMember()]
        public string CurrentDialogPath { get; set; }

        public sp.Grammar Grammar { get; set; }

        public Profile()
        {
            this.Actions = new List<Action>();
        }

        public void Save(string filename)
        {
            this.Save(System.IO.File.Create(filename));
        }

        public void Save(System.IO.Stream stream)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(Profile));
            serializer.WriteObject(stream, this);
        }

        public void Open(string filename)
        {
            this.Open(System.IO.File.OpenRead(filename));
        }

        public void Open(System.IO.Stream stream)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(Profile));
            Profile openedProfile = serializer.ReadObject(stream) as Profile;
            this.ProfileName = openedProfile.ProfileName;
            this.Actions = openedProfile.Actions;
            this.CurrentDialogPath = openedProfile.CurrentDialogPath;
            this.EndTimeout = openedProfile.EndTimeout;
            this.EngageNpcDialogue = openedProfile.EngageNpcDialogue;
            this.RequiredConfidence = openedProfile.RequiredConfidence;
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
            return this.Grammar ;
        }
    }
}

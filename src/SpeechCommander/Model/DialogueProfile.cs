using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using sp = System.Speech.Recognition;

namespace SpeechCommander.Model
{
    [DataContract(Name="DialogueProfile", Namespace="")]
    public class DialogueProfile
    {
        [DataMember()]
        public bool Enabled { get; set; }
        [DataMember()]
        public string FilePath { get; set; }
        [DataMember()]
        public Command CommandPrevious { get; set; }
        [DataMember()]
        public Command CommandNext { get; set; }
        [DataMember()]
        public Command CommandAccept { get; set; }
        [DataMember()]
        public Command CommandCancel { get; set; }
        [DataMember()]
        public List<string> CancelPhrases { get; set; }

        public DialogueProfile()
        {
            this.CancelPhrases = new List<string>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace SpeechCommander
{
    [DataContractAttribute(Name = "Action", Namespace = "")]
    public class Action
    {
        [DataMember()]
        public string ActionName { get; set; }

        [DataMember()]
        public List<string> Phrases { get; set; }

        [DataMember()]
        public int Repeat { get; set; }

        [DataMember()]
        public int PausedDuration { get; set; }

        [DataMember()]
        public List<Command> Commands { get; set; }

        public Action()
        {
            this.Commands = new List<Command>();
            this.Phrases = new List<string>();
            this.Repeat = 1;
            this.PausedDuration = 25;
        }

        public void Execute()
        {
            lock (this.Commands)
            {
                for (int i = 0; i < this.Repeat; ++i)
                {
                    foreach (Command cmd in this.Commands)
                    {
                        cmd.Execute();
                    }

                    System.Threading.Thread.Sleep(this.PausedDuration);
                }
            }
        }

        public override string ToString()
        {
            return this.ActionName;
        }
    }
}

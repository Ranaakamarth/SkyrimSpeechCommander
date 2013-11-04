using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using WindowsInput;

namespace SpeechCommander.Model
{
    [DataContract(Name = "Command", Namespace = "")]
    public class Command
    {
        [DataMember()]
        public string CommandName { get; set; }
        [DataMember()]
        public int Repeat { get; set; }
        [DataMember()]
        public int HeldDuration { get; set; }
        [DataMember()]
        public int PausedDuration { get; set; }
        [DataMember()]
        public bool ToggleKeypress { get; set; }
        [DataMember()]
        public string TextInput { get; set; }

        private bool keyDown;

        public Nullable<VirtualKeyCode> Key
        {
            get
            {
                return this.key;
            }
            set
            {
                this.key = value;
            }
        }
        [DataMember(), EnumMember()]
        private Nullable<VirtualKeyCode> key;

        public Nullable<VirtualKeyCode> ModifierKey
        {
            get
            {
                return this.modifierKey;
            }
            set
            {
                this.modifierKey = value;
            }
        }
        [DataMember(), EnumMember()]
        private Nullable<VirtualKeyCode> modifierKey;

        public Command()
        {
            this.CommandName = string.Empty;
            this.Repeat = 1;
            this.HeldDuration = 50;
            this.PausedDuration = 25;
            this.ToggleKeypress = false;
            this.keyDown = false;
            this.key = null;
            this.modifierKey = null;
            this.TextInput = string.Empty;
        }

        public void Execute()
        {
            for (int i = 0; i < this.Repeat; ++i)
            {
                if (ToggleKeypress && this.key != null)
                {
                    keyDown = !keyDown;
                    if (keyDown)
                    {
                        if (this.modifierKey != null)
                            InputSimulator.AsyncHoldKey((VirtualKeyCode)this.modifierKey);
                        InputSimulator.AsyncHoldKey((VirtualKeyCode)this.key);
                    }
                    else
                    {
                        InputSimulator.AsyncReleaseKey((VirtualKeyCode)this.key);
                        if (this.modifierKey != null)
                            InputSimulator.AsyncReleaseKey((VirtualKeyCode)this.modifierKey);
                    }
                }
                else if (TextInput != null && TextInput.Length > 0)
                {
                    // Doesn't work yet!
                    InputSimulator.SimulateTextEntry(this.TextInput);
                }
                else if (this.key != null)
                {
                    if (this.modifierKey != null)
                        InputSimulator.SimulateModifiedKeyStroke((VirtualKeyCode)this.modifierKey, (VirtualKeyCode)this.key, this.HeldDuration);
                    else
                        InputSimulator.SimulateKeyPress((VirtualKeyCode)this.key, this.HeldDuration);
                }
                System.Threading.Thread.Sleep(this.PausedDuration);
            }
        }

        public override string ToString()
        {
            return this.CommandName;
        }
    }
}

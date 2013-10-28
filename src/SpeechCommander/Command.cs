using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using WindowsInput;

namespace SpeechCommander
{
    [DataContractAttribute(Name = "Command", Namespace = "")]
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

        private volatile bool KeyDown;

        //private System.Threading.Thread holdThread;

        public Nullable<WindowsInput.VirtualKeyCode> Key
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

        public Nullable<WindowsInput.VirtualKeyCode> ModifierKey
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
        private Nullable<WindowsInput.VirtualKeyCode> key;

        [DataMember(), EnumMember()]
        private Nullable<WindowsInput.VirtualKeyCode> modifierKey;

        private const int KEYEVENTF_EXTENDEDKEY = 0x0001;
        private const int KEYEVENTF_KEYUP = 0x0002;

        public Command()
        {
            this.CommandName = string.Empty;
            this.Repeat = 1;
            this.HeldDuration = 50;
            this.PausedDuration = 25;
            this.ToggleKeypress = false;
            this.KeyDown = false;
            this.key = null;
            this.TextInput = string.Empty;
        }

        public void Execute()
        {
            for (int i = 0; i < this.Repeat; ++i)
            {
                if (ToggleKeypress && this.key != null)
                {
                    KeyDown = !KeyDown;
                    if (KeyDown)
                    {
                        InputSimulator.AsyncHoldKey((WindowsInput.VirtualKeyCode)this.key);
                        if (this.modifierKey != null)
                            InputSimulator.AsyncHoldKey((WindowsInput.VirtualKeyCode)this.modifierKey);
                    }
                    else
                    {
                        InputSimulator.AsyncReleaseKey((WindowsInput.VirtualKeyCode)this.key);
                        if (this.modifierKey != null)
                            InputSimulator.AsyncReleaseKey((WindowsInput.VirtualKeyCode)this.modifierKey);
                    }
                }
                else if (TextInput != null && TextInput.Length > 0)
                {
                    WindowsInput.InputSimulator.SimulateTextEntry(this.TextInput);
                }
                else if (this.key != null)
                {
                    if (this.modifierKey != null)
                        InputSimulator.SimulateModifiedKeyStroke((VirtualKeyCode)this.modifierKey, (VirtualKeyCode)this.key, this.HeldDuration);
                    else
                        InputSimulator.SimulateKeyPress((VirtualKeyCode)this.key, this.HeldDuration);
                    //System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
                    //watch.Start();
                    //while (watch.ElapsedMilliseconds < this.HeldDuration)
                    //{
                    //    WindowsInput.InputSimulator.SimulateKeyDown((WindowsInput.VirtualKeyCode)this.Key);
                    //    System.Threading.Thread.Sleep(20);
                    //}
                    //watch.Stop();
                    //WindowsInput.InputSimulator.SimulateKeyUp((WindowsInput.VirtualKeyCode)this.Key);
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


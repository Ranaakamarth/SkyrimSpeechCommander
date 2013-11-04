using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using WindowsInput;

namespace SpeechCommander.Model
{
    [DataContract(Name = "Command", Namespace = "")]
    public class Command : System.ComponentModel.INotifyPropertyChanged
    {
        [DataMember()]
        public string CommandName
        {
            get
            {
                return commandName;
            }
            set
            {
                if (commandName != value)
                {
                    commandName = value;
                    RaisePropertyChanged("CommandName");
                }
            }
        }
        private string commandName;
        [DataMember()]
        public int Repeat
        {
            get
            {
                return repeat;
            }
            set
            {
                if (repeat != value)
                {
                    repeat = value;
                    RaisePropertyChanged("Repeat");
                }
            }
        }
        private int repeat;
        [DataMember()]
        public int HeldDuration
        {
            get
            {
                return heldDuration;
            }
            set
            {
                if (heldDuration != value)
                {
                    heldDuration = value;
                    RaisePropertyChanged("HeldDuration");
                }
            }
        }
        private int heldDuration;
        [DataMember()]
        public int PausedDuration
        {
            get
            {
                return pausedDuration;
            }
            set
            {
                if (pausedDuration != value)
                {
                    pausedDuration = value;
                    RaisePropertyChanged("PausedDuration");
                }
            }
        }
        private int pausedDuration;
        [DataMember()]
        public bool ToggleKeypress
        {
            get
            {
                return toggleKeypress;
            }
            set
            {
                if (toggleKeypress != value)
                {
                    toggleKeypress = value;
                    RaisePropertyChanged("ToggleKeypress");
                }
            }
        }
        private bool toggleKeypress;
        [DataMember()]
        public string TextInput
        {
            get
            {
                return textInput;
            }
            set
            {
                if (textInput != value)
                {
                    textInput = value;
                    RaisePropertyChanged("TextInput");
                }
            }
        }
        private string textInput;

        private bool keyDown;

        [DataMember()]//, EnumMember()]
        public Nullable<VirtualKeyCode> Key
        {
            get
            {
                return key;
            }
            set
            {
                if (key != value)
                {
                    key = value;
                    RaisePropertyChanged("Key");
                }
            }
        }
        private Nullable<VirtualKeyCode> key;

        [DataMember()]//, EnumMember()]
        public Nullable<VirtualKeyCode> ModifierKey
        {
            get
            {
                return modifierKey;
            }
            set
            {
                if (modifierKey != value)
                {
                    modifierKey = value;
                    RaisePropertyChanged("ModifierKey");
                }
            }
        }
        private Nullable<VirtualKeyCode> modifierKey;

        public Command()
        {
            this.CommandName = string.Empty;
            this.Repeat = 1;
            this.HeldDuration = 50;
            this.PausedDuration = 25;
            this.ToggleKeypress = false;
            this.keyDown = false;
            this.Key = null;
            this.ModifierKey = null;
            this.TextInput = string.Empty;
        }

        public void Execute()
        {
            for (int i = 0; i < this.Repeat; ++i)
            {
                if (ToggleKeypress && this.Key != null)
                {
                    keyDown = !keyDown;
                    if (keyDown)
                    {
                        if (this.ModifierKey != null)
                            InputSimulator.AsyncHoldKey((VirtualKeyCode)this.ModifierKey);
                        InputSimulator.AsyncHoldKey((VirtualKeyCode)this.Key);
                    }
                    else
                    {
                        InputSimulator.AsyncReleaseKey((VirtualKeyCode)this.Key);
                        if (this.ModifierKey != null)
                            InputSimulator.AsyncReleaseKey((VirtualKeyCode)this.ModifierKey);
                    }
                }
                else if (this.TextInput != null && this.TextInput.Length > 0)
                {
                    // Doesn't work yet!
                    InputSimulator.SimulateTextEntry(this.TextInput);
                }
                else if (this.Key != null)
                {
                    if (this.ModifierKey != null)
                        InputSimulator.SimulateModifiedKeyStroke((VirtualKeyCode)this.ModifierKey, (VirtualKeyCode)this.Key, this.HeldDuration);
                    else
                        InputSimulator.SimulateKeyPress((VirtualKeyCode)this.Key, this.HeldDuration);
                }
                System.Threading.Thread.Sleep(this.PausedDuration);
            }
        }

        public override string ToString()
        {
            return this.CommandName;
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

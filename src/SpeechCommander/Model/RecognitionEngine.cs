using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using sp = System.Speech.Recognition;

namespace SpeechCommander.Model
{
    public delegate void RecognizedWordEvent(sp.SpeechRecognizedEventArgs e);

    public class RecognitionEngine : IDisposable, System.ComponentModel.INotifyPropertyChanged
    {
        protected sp.SpeechRecognitionEngine engine;

        protected Profile currentProfile;
        protected List<Action> actions;
        private sp.Grammar pauseGramamr;
        private const string PAUSEDCOMMAND = "PauseVoiceRecognitionCommand";
        private const string UNPAUSEDCOMMAND = "UnpauseVoiceRecognitionCommand";

        public Profile CurrentProfile
        {
            get
            {
                return currentProfile;
            }
        }
        public bool Running
        {
            //get
            //{
            //    //if (engine.AudioState != sp.AudioState.Stopped)
            //    //    return true;
            //    //else
            //    //    return false;
            //}
            get
            {
                return running;
            }
            private set
            {
                if (running != value)
                {
                    running = value;
                    RaisePropertyChanged("Running");
                }
            }
        }
        private bool running;

        public RecognizedWordEvent RecognizedWord;

        public RecognitionEngine()
        {
            this.actions = new List<Action>();
            engine = new sp.SpeechRecognitionEngine();
            engine.SetInputToDefaultAudioDevice();
            engine.SpeechRecognized += engine_SpeechRecognized;
            engine.RecognizerUpdateReached += engine_RecognizerUpdateReached;

            engine.LoadGrammar(new sp.DictationGrammar());
        }

        public RecognitionEngine(Profile profile)
        {
            this.actions = new List<Action>();
            engine = new sp.SpeechRecognitionEngine();
            engine.SetInputToDefaultAudioDevice();
            engine.SpeechRecognized += engine_SpeechRecognized;
            engine.RecognizerUpdateReached += engine_RecognizerUpdateReached;

            LoadProfile(profile);
        }

        public void LoadProfile(Profile profile)
        {
            this.currentProfile = profile;
            if (this.actions != null)
                this.actions.Clear();

            this.currentProfile.UpdateGrammar();

            List<UpdateOperation> ops = new List<UpdateOperation>();
            foreach (sp.Grammar gram in this.engine.Grammars)
            {
                ops.Add(new UpdateOperation()
                {
                    UpdateType = UpdateOperationType.RemoveGrammar,
                    Grammar = gram
                });
            }
            ops.Add(new UpdateOperation()
            {
                UpdateType = UpdateOperationType.AddGrammar,
                Grammar = this.currentProfile.Grammar,
                AssociatedActions = this.currentProfile.Actions
            });

            this.pauseGramamr = null;
                if (this.currentProfile.EnableVoicePausing)
                {
                    this.pauseGramamr = GeneratePauseGrammar();
                    ops.Add(new UpdateOperation()
                    {
                        UpdateType = UpdateOperationType.AddGrammar,
                        Grammar = pauseGramamr
                    });
                }

                this.ExecuteGrammarChanges(ops);
        }

        public void ExecuteGrammarChanges(List<UpdateOperation> changes)
        {
            if (this.Running)
                this.engine.RequestRecognizerUpdate(changes);
            else
            {
                foreach (var change in changes)
                {
                    EngineUpdate(change);
                }
            }

            foreach (var change in changes)
            {
                switch (change.UpdateType)
                {
                    case UpdateOperationType.AddGrammar:
                        if (change.AssociatedActions != null)
                            this.actions.AddRange(change.AssociatedActions);
                        break;
                    case UpdateOperationType.RemoveGrammar:
                        if (change.AssociatedActions != null)
                        {
                            foreach (var action in change.AssociatedActions)
                            {
                                this.actions.Remove(action);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private sp.Grammar GeneratePauseGrammar()
        {
            sp.Choices choices = new sp.Choices();
            foreach (string pausePhrase in this.currentProfile.PauseRecognitionPhrases)
            {
                sp.SemanticResultValue temp = new sp.SemanticResultValue(pausePhrase, PAUSEDCOMMAND);
                choices.Add(temp);
            }
            foreach (string unpausePhrase in this.currentProfile.UnpauseRecognitionPhrases)
            {
                sp.SemanticResultValue temp = new sp.SemanticResultValue(unpausePhrase, UNPAUSEDCOMMAND);
                choices.Add(temp);
            }
            sp.GrammarBuilder builder = new sp.GrammarBuilder();
            builder.Append(new sp.SemanticResultKey("command", choices));
            sp.Grammar grammar = new sp.Grammar(builder);
            grammar.Name = "PauseCommands";
            return grammar;
        }
        

        void engine_RecognizerUpdateReached(object sender, sp.RecognizerUpdateReachedEventArgs e)
        {
            var changesList = e.UserToken as List<UpdateOperation>;
            if (changesList != null)
            {
                foreach (var changeOp in changesList)
                {
                    EngineUpdate(changeOp);
                }
            }
        }

        private void EngineUpdate(UpdateOperation change)
        {
            switch (change.UpdateType)
            {
                case UpdateOperationType.AddGrammar:
                    if (change.Grammar != null)
                        this.engine.LoadGrammar(change.Grammar);
                    break;
                case UpdateOperationType.RemoveGrammar:
                    if (this.engine.Grammars.Contains(change.Grammar))
                        this.engine.UnloadGrammar(change.Grammar);
                    break;
                case UpdateOperationType.EnableGrammar:
                    if (change.Grammar != null)
                        change.Grammar.Enabled = true;
                    break;
                case UpdateOperationType.DisableGrammar:
                    if (change.Grammar != null)
                        change.Grammar.Enabled = false;
                    break;
                default:
                    break;
            }
        }

        void engine_SpeechRecognized(object sender, sp.SpeechRecognizedEventArgs e)
        {
            Console.WriteLine(String.Format("{0}\t\"{1}\"", e.Result.Confidence, e.Result.Text));

            if (e.Result.Confidence >= this.currentProfile.RequiredConfidence)
            {
                onWordRecognized(e);

                Action action = this.actions.Find(act => act.ActionName == e.Result.Semantics["command"].Value.ToString());

                if (e.Result.Semantics["command"].Value.ToString() == PAUSEDCOMMAND)
                    VoicePause();
                else if (e.Result.Semantics["command"].Value.ToString() == UNPAUSEDCOMMAND)
                    VoiceUnpause();
                else if (action != null)
                    action.Execute();
            }
        }

        private void VoiceUnpause()
        {
            List<UpdateOperation> ops = new List<UpdateOperation>();

            foreach (var grammar in this.engine.Grammars)
            {
                ops.Add(new UpdateOperation()
                {
                    UpdateType = UpdateOperationType.EnableGrammar,
                    Grammar = grammar
                });
            }

            this.ExecuteGrammarChanges(ops);
        }

        private void VoicePause()
        {
            if (this.pauseGramamr != null)
            {
                List<UpdateOperation> ops = new List<UpdateOperation>();

                var grammars = this.engine.Grammars.Where(gr => gr != this.pauseGramamr);
                foreach (var grammar in grammars)
                {
                    ops.Add(new UpdateOperation()
                    {
                        UpdateType = UpdateOperationType.DisableGrammar,
                        Grammar = grammar
                    });
                }

                this.ExecuteGrammarChanges(ops);
            }
        }

        private void onWordRecognized(sp.SpeechRecognizedEventArgs e)
        {
            if (this.RecognizedWord != null)
                this.RecognizedWord(e);
        }

        public virtual void StartAsync(sp.RecognizeMode mode)
        {
            Running = true;
            this.engine.RecognizeAsync(mode);
        }

        public virtual void Stop()
        {
            Running = false;
            this.engine.RecognizeAsyncCancel();
            WindowsInput.InputSimulator.AsyncReleaseAllKeys();
        }

        public void EmulateRecognize(string text)
        {
            this.engine.EmulateRecognizeAsync(text);
        }

        public void Dispose()
        {
            this.engine.Dispose();
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

    public enum UpdateOperationType
    {
        AddGrammar,
        RemoveGrammar,
        EnableGrammar,
        DisableGrammar
    }

    public class UpdateOperation
    {
        public UpdateOperationType UpdateType;
        public sp.Grammar Grammar;
        public IEnumerable<Action> AssociatedActions;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sp = System.Speech.Recognition;

namespace SpeechCommander
{
    public delegate void RecognizedWord(string word);

    public class RecognitionEngine : IDisposable
    {
        protected sp.SpeechRecognitionEngine engine;
        protected Profile currentProfile;
        protected List<Action> actions;
        private sp.Grammar pauseGrammar;
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
            get
            {
                if (engine.AudioState != sp.AudioState.Stopped)
                    return true;
                else
                    return false;
            }
        }
        public RecognizedWord onWordRecognized;

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

        void engine_SpeechRecognized(object sender, sp.SpeechRecognizedEventArgs e)
        {
            Console.WriteLine(e.Result.Confidence.ToString() + "\t\"" + e.Result.Text + "\"");
            if (e.Result.Confidence >= this.currentProfile.RequiredConfidence)
            {
                if (onWordRecognized != null)
                    onWordRecognized(String.Format("Command: \"{0}\"\tPhrase: \"{1}\"", e.Result.Semantics["command"].Value.ToString(), e.Result.Text));

                Action action = this.actions.Find(act => act.ActionName == e.Result.Semantics["command"].Value.ToString());


                if (e.Result.Semantics["command"].Value.ToString() == PAUSEDCOMMAND)
                    VoicePause();
                else if (e.Result.Semantics["command"].Value.ToString() == UNPAUSEDCOMMAND)
                    VoiceUnpause();
                else if (action != null)
                    action.Execute();
            }
        }

        private void VoicePause()
        {
            if (this.pauseGrammar != null)
            {
                List<UpdateOperation> ops = new List<UpdateOperation>();

                var grammars = this.engine.Grammars.Where(gr => gr != this.pauseGrammar);
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

        public virtual void StartAsync(sp.RecognizeMode mode)
        {
            this.engine.RecognizeAsync(mode);
        }

        public void EmulateRecognize(string text)
        {
            this.engine.EmulateRecognize(text);
        }

        public virtual void Stop()
        {
            this.engine.RecognizeAsyncCancel();

            WindowsInput.InputSimulator.AsyncReleaseAllKeys();
        }

        public void Dispose()
        {
            this.engine.Dispose();
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

            this.pauseGrammar = null;
            if (this.currentProfile.EnableVoicePausing)
            {
                this.pauseGrammar = GeneratePauseGrammar();
                ops.Add(new UpdateOperation()
                {
                    UpdateType = UpdateOperationType.AddGrammar,
                    Grammar = pauseGrammar
                });
            }

            this.ExecuteGrammarChanges(ops);
        }

        private sp.Grammar GeneratePauseGrammar()
        {
            sp.Choices choices = new sp.Choices();
            foreach (string pausePhrase in this.currentProfile.PauseRecognitionPhrases)
            {
                sp.SemanticResultValue temp = new sp.SemanticResultValue(pausePhrase, "PauseVoiceRecognitionCommand");
                choices.Add(temp);
            }
            foreach (string unpausePhrase in this.currentProfile.UnpauseRecognitionPhrases)
            {
                sp.SemanticResultValue temp = new sp.SemanticResultValue(unpausePhrase, "UnpauseVoiceRecognitionCommand");
                choices.Add(temp);
            }
            sp.GrammarBuilder builder = new sp.GrammarBuilder();
            builder.Append(new sp.SemanticResultKey("command", choices));
            sp.Grammar grammar = new sp.Grammar(builder);
            grammar.Name = "PauseCommands";
            return grammar;
        }

        public void AddGrammar(sp.Grammar grammar, IEnumerable<Action> associatedActions)
        {
            //this.engine.LoadGrammar(grammar);
            if (this.Running)
                this.engine.RequestRecognizerUpdate(new UpdateOperation() { UpdateType = UpdateOperationType.AddGrammar, Grammar = grammar });
            else
                this.engine.LoadGrammar(grammar);
            this.actions.AddRange(associatedActions);
        }

        public void RemoveGrammar(sp.Grammar grammar, IEnumerable<Action> associatedActions)
        {
            this.engine.RequestRecognizerUpdate(new UpdateOperation() { UpdateType = UpdateOperationType.RemoveGrammar, Grammar = grammar });

            foreach (Action action in associatedActions)
            {
                this.actions.Remove(action);
            }
        }

        public void EnableGrammar(sp.Grammar grammar)
        {
            this.engine.RequestRecognizerUpdate(new UpdateOperation() { UpdateType = UpdateOperationType.EnableGrammar, Grammar = grammar });
        }

        public void DisableGrammar(sp.Grammar grammar)
        {
            this.engine.RequestRecognizerUpdate(new UpdateOperation() { UpdateType = UpdateOperationType.DisableGrammar, Grammar = grammar });
        }

        public void ExecuteGrammarChanges(List<UpdateOperation> changes)
        {
            if (this.Running)
                this.engine.RequestRecognizerUpdate(changes);
            else
            {
                foreach (var change in changes)
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
                        foreach (Action action in change.AssociatedActions)
                        {
                            this.actions.Remove(action);
                        }
                        break;
                    default:
                        break;
                }
            }
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

            var change = e.UserToken as UpdateOperation;
            if (change != null)
            {
                EngineUpdate(change);
            }
            Console.WriteLine("Recognizer Updated");
        }

        private void EngineUpdate(UpdateOperation changeOp)
        {
            switch (changeOp.UpdateType)
            {
                case UpdateOperationType.AddGrammar:
                    if (changeOp.Grammar != null)
                        this.engine.LoadGrammar(changeOp.Grammar);
                    break;
                case UpdateOperationType.RemoveGrammar:
                    if (this.engine.Grammars.Contains(changeOp.Grammar))
                        this.engine.UnloadGrammar(changeOp.Grammar);
                    break;
                case UpdateOperationType.EnableGrammar:
                    if (changeOp.Grammar != null)
                        changeOp.Grammar.Enabled = true;
                    break;
                case UpdateOperationType.DisableGrammar:
                    if (changeOp.Grammar != null)
                        changeOp.Grammar.Enabled = false;
                    break;
                default:
                    break;
            }
        }
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

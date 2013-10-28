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
        private sp.SpeechRecognitionEngine engine;
        private Profile currentProfile;
        private List<Action> actions;

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
                if (action != null)
                    action.Execute();
            }
        }

        public void StartAsync(sp.RecognizeMode mode)
        {
            this.engine.RecognizeAsync(mode);
        }

        public void EmulateRecognize(string text)
        {
            this.engine.EmulateRecognize(text);
        }

        public void Stop()
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

            //engine.EndSilenceTimeout = new TimeSpan(0, 0, 0, 0, profile.EndTimeout);
            this.currentProfile.UpdateGrammar();
            this.AddGrammar(this.currentProfile.Grammar, this.currentProfile.Actions);
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
            this.engine.RequestRecognizerUpdate(changes);

            foreach (var change in changes)
            {
                switch (change.UpdateType)
                {
                    case UpdateOperationType.AddGrammar:
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
                    this.engine.LoadGrammar(changeOp.Grammar);
                    break;
                case UpdateOperationType.RemoveGrammar:
                    if (this.engine.Grammars.Contains(changeOp.Grammar))
                        this.engine.UnloadGrammar(changeOp.Grammar);
                    break;
                case UpdateOperationType.EnableGrammar:
                    changeOp.Grammar.Enabled = true;
                    break;
                case UpdateOperationType.DisableGrammar:
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

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
                this.engine.RequestRecognizerUpdate(new Tuple<bool, sp.Grammar>(true, grammar));
            else
                this.engine.LoadGrammar(grammar);
            this.actions.AddRange(associatedActions);
        }

        public void RemoveGrammar(sp.Grammar grammar, IEnumerable<Action> associatedActions)
        {
            this.engine.RequestRecognizerUpdate(new Tuple<bool, sp.Grammar>(false, grammar));
            foreach (Action action in associatedActions)
            {
                this.actions.Remove(action);
            }
        }

        void engine_RecognizerUpdateReached(object sender, sp.RecognizerUpdateReachedEventArgs e)
        {
            
            var changes = e.UserToken as Tuple<bool, sp.Grammar>;
            if (changes.Item1)
            {
                this.engine.LoadGrammar(changes.Item2);
            }
            else
            {
                if (this.engine.Grammars.Contains(changes.Item2))
                    this.engine.UnloadGrammar(changes.Item2);
            }

            Console.WriteLine("Recognizer Updated");
        }
    }
}

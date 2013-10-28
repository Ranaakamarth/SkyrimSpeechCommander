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

            engine.LoadGrammar(new sp.DictationGrammar());
        }

        public RecognitionEngine(Profile profile)
        {
            engine = new sp.SpeechRecognitionEngine();
            engine.SetInputToDefaultAudioDevice();
            engine.SpeechRecognized += engine_SpeechRecognized;

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
            if (this.actions == null)
                this.actions = new List<Action>();
            this.engine.LoadGrammar(grammar);
            this.engine.RequestRecognizerUpdate();
            this.actions.AddRange(associatedActions);
            Console.WriteLine();
        }

        public void RemoveGrammar(sp.Grammar grammar, IEnumerable<Action> associatedActions)
        {
          if (this.engine.Grammars.Contains(grammar))
            this.engine.UnloadGrammar(grammar);
            foreach (Action action in associatedActions)
            {
                this.actions.Remove(action);
            }
        }
    }
}

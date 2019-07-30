using System;
using System.Collections.Generic;
using Glue.Native;
using Newtonsoft.Json;
using WindowsInput;
using WindowsInput.Native;

namespace Glue.Actions
{
    public class ActionTyping : Action
    {
        public string TypedString => this.typedString;
        public long CharDelayMS => this.charDelayMS;
        public long DwellTimeMS => this.dwellTimeMS;

        [JsonProperty]
        private readonly string typedString;
        [JsonProperty]
        private readonly long charDelayMS;
        [JsonProperty]
        private readonly long dwellTimeMS;

        public ActionTyping(string typedString, long keyDelayMS, long dwellTimeMS)
        {
            this.typedString=typedString;
            this.charDelayMS=keyDelayMS;
            this.dwellTimeMS=dwellTimeMS;
            this.Type = "TYPING";
        }

        public override Action[] Schedule()
        {
            List<Action> actions = new List<Action>();

            // TODO INPUT generated with AddCharacters(char) or AddCharacters(string) mask information
            // The keyboard hook (KeyInterceptor) can't make sense of input generated this way
            INPUT[] inputs = new InputBuilder().AddCharacters(this.typedString).ToArray();

            int actionCount = 0;
            long actionTimeMS = TimeProvider.GetTickCount();

            foreach (INPUT input in inputs)
            {
                Action action = new ActionKey(
                    input, 
                    (actionCount % 2) == 0 
                        ? ActionKey.Movement.PRESS 
                        : ActionKey.Movement.RELEASE,
                    actionTimeMS);

                actions.Add(action);

                actionTimeMS += 
                    (actionCount % 2) == 0 
                        ? this.charDelayMS 
                        : this.dwellTimeMS;

                actionCount++;
            }

            return actions.ToArray();
        }

        public override void Play()
        {
            // ActionTyping is only used to schedule ActionKey 
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return base.ToString() + " " + this.typedString;
        }
    }
}

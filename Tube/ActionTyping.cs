using System;
using System.Collections.Generic;
using WindowsInput;
using WindowsInput.Native;

namespace Glue
{
    public class ActionTyping : Action, IAction
    {
        public string TypedString => this.typedString;
        public long CharDelayMS => this.charDelayMS;
        public long DwellTimeMS => this.dwellTimeMS;

        private readonly String typedString;
        private readonly long charDelayMS;
        private readonly long dwellTimeMS;

        public ActionTyping(string typedString, long keyDelayMS, long dwellTimeMS)
        {
            this.typedString=typedString;
            this.charDelayMS=keyDelayMS;
            this.dwellTimeMS=dwellTimeMS;
        }

        public new IAction[] Schedule()
        {
            List<IAction> actions = new List<IAction>();

            // TODO INPUT generated with AddCharacters(char) or AddCharacters(string) mask information
            // The keyboard hook (KeyInterceptor) can't make sense of input generated this way
            INPUT[] inputs = new InputBuilder().AddCharacters(this.typedString).ToArray();

            int actionCount = 0;
            long actionTimeMS = ActionQueue.Now();

            foreach (INPUT input in inputs)
            {
                IAction action = new ActionKey(
                    input, 
                    (actionCount % 2) == 0 
                        ? ActionKey.Type.PRESS 
                        : ActionKey.Type.RELEASE,
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
    }
}

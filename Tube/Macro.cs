using System.Collections.Generic;
using Glue.Actions;
using Newtonsoft.Json;

namespace Glue
{
    public class Macro
    {
        [JsonProperty]
        private readonly string name;

        [JsonProperty]
        private readonly long delayTimeMS;         // Time delay before playing first action

        [JsonProperty]
        private readonly List<Action> actions = new List<Action>();

        [JsonConstructor]
        public Macro(string name, long delayTimeMS)        
        {
            this.name = name;
            this.delayTimeMS = delayTimeMS;
        }

        public Macro AddAction(Action action)
        {
            actions.Add(action);
            return this;
        }

        internal void Play()
        {
            // Schedule each action in the macro and add it to the output queue
            foreach (Action action in actions)
            {
                // TODO use object pooled actions for better GC
                // BUGBUG Pass in delay time! action.Schedule(this.delayTimeMS);
                Action[] scheduledActions = action.Schedule();

                foreach (Action scheduledAction in scheduledActions)
                {
                    ActionQueueThread.Schedule(scheduledAction);
                }
            }
        }
    }
}

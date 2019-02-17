﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace Glue
{
    class Macro
    {
        [JsonProperty]
        private readonly long delayTimeMS;         // Time delay before playing first action

        [JsonProperty]
        private readonly List<Action> actions = new List<Action>();

        public Macro(long delayTimeMS)
        {
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
                    ActionQueue.Enqueue(scheduledAction, scheduledAction.TimeScheduledMS);
                }
            }
        }
    }
}

using System.Collections.Generic;
using Glue.Actions;
using Glue.Native;
using Newtonsoft.Json;

namespace Glue
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Macro
    {
        public string Name => name;
        public long DelayTimeMS => delayTimeMS;
        public List<Action> Actions => actions;

        // private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
            Actions.Add(action);
            return this;
        }

        internal void Play()
        {
            // Remember time macro was triggered 
            long timeMacro = TimeProvider.GetTickCount();

            // Actions schedule themselves relative to this
            // Updated with each action scheduled
            long timeScheduleFrom = TimeProvider.GetTickCount();

            // Schedule each action in the macro and add it to the output queue
            foreach (Action action in Actions)
            {
                // TODO use object pooled actions for better GC
                // BUGBUG Pass in delay time! action.Schedule(this.delayTimeMS);
                Action[] scheduledActions = action.Schedule(timeScheduleFrom);

                foreach (Action scheduledAction in scheduledActions)
                {
                    ActionQueueThread.Schedule(scheduledAction);
                    timeScheduleFrom += scheduledAction.TimeScheduledMS - timeMacro;

                    // LOGGER.Debug(string.Format("Play() schedule delta: {0:n0}ms", scheduledAction.TimeScheduledMS - timeMacro));
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using Glue.Native;
using Newtonsoft.Json;
using Action = Glue.Actions.Action;

namespace Glue
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Macro
    {
        public string Name => name;
        public long DelayTimeMS => delayTimeMS;
        public List<Action> Actions => actions;

        public static TimeProvider Time { get => s_timeProvider; set => s_timeProvider = value; }
        public static IActionScheduler Scheduler { get => s_actionScheduler; set => s_actionScheduler = value; }

        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static TimeProvider s_timeProvider = new TimeProvider();
        private static IActionScheduler s_actionScheduler = Tube.Scheduler;

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

        public void Play()
        {
            long timeNow = Time.Now();

            // Start scheduling actions after macro delay time
            long timeScheduleFrom = timeNow + this.DelayTimeMS;

            // Schedule each action and add results to the output queue
            foreach (Action action in Actions)
            {
                Action[] scheduledActions = action.Schedule(timeScheduleFrom);

                foreach (Action scheduledAction in scheduledActions)
                {
                    Scheduler.Schedule(scheduledAction);

                    if (LOGGER.IsDebugEnabled)
                    {
                        string message = String.Format("Scheduled at tick {0:n0} in {1:n0}ms: {2}",
                            scheduledAction.ScheduledTick,
                            scheduledAction.ScheduledTick - timeNow,
                            scheduledAction.ToString()
                            );
                            LOGGER.Debug(message);
                    }

                    // Set time to schedule next action
                    timeScheduleFrom += scheduledAction.DelayMS;
                }
            }
        }
    }
}

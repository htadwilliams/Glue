using System.Collections.Generic;

namespace Glue
{
    class Macro
    {
        private readonly long delayTimeMS;         // Time delay before playing first action
        private readonly List<IAction> actions = new List<IAction>();

        public Macro(long delayTimeMS)
        {
            this.delayTimeMS = delayTimeMS;
        }

        public Macro AddAction(IAction action)
        {
            actions.Add(action);
            return this;
        }

        internal void Play()
        {
            // Schedule each action in the macro and add it to the output queue
            foreach (IAction action in actions)
            {
                // TODO use object pooled actions for better GC
                // BUGBUG Pass in delay time! action.Schedule(this.delayTimeMS);
                IAction[] scheduledActions = action.Schedule();

                foreach (IAction scheduledAction in scheduledActions)
                {
                    ActionQueue.Enqueue(scheduledAction, scheduledAction.TimeScheduledMS);
                }
            }
        }
    }
}

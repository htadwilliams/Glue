using System.Collections.Generic;

namespace Glue
{
    class Macro
    {
        private readonly long delayTimeMS;         // Time delay before playing first action
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
                ActionQueue.Enqueue(action, action.Schedule());
            }
        }
    }
}

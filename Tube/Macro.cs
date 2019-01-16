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
            foreach (Action action in actions)
            {
                action.Schedule();
                ActionQueue.Enqueue(action);
            }
        }
    }
}

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
                // Make copy of action for the queue 
                // TODO create new scheduled action type to save memory 
                // TODO use object pooled actions for better GC
                Action actionCopy = new Action(action);
                ActionQueue.Enqueue(actionCopy, actionCopy.Schedule());
            }
        }
    }
}

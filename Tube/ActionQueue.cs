using Glue.native;
using Priority_Queue;
using System;

namespace Glue
{
    class ActionQueue
    {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly SimplePriorityQueue<Action, long> s_actions = new SimplePriorityQueue<Action, long>();

        public int Count { get => s_actions.Count; }
        public Action First { get => s_actions.First; }

        internal void Enqueue(Action action)
        {
            s_actions.Enqueue(action, action.TimeScheduledMS);
        }

        internal void Cancel(string name)
        {
            short cancelCount = 0;
            foreach (Action action in s_actions)
            {
                if (null != action.Name && action.Name.Equals(name))
                {
                    cancelCount++;
                    s_actions.Remove(action);
                }
            }

            LOGGER.Info(String.Format("Canceled {0} instances of Action with name = [" + name + "]", cancelCount));
        }

        internal int GetMSUntilNextAction()
        {
            if (s_actions.Count != 0)
            {
                // BUGBUG assumes 1 tick == 1 MS which is not true on all systems
                return (int) (s_actions.First.TimeScheduledMS - TimeProvider.GetTickCount());
            }

            // to wait indefinitely
            return -1;
        }

        internal void Dequeue()
        {
            s_actions.Dequeue();
        }
    }
}

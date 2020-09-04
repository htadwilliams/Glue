using Glue.Native;
using Priority_Queue;
using System.Collections.Generic;

namespace Glue.Actions
{
    class Queue
    {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly SimplePriorityQueue<Action, long> actions = new SimplePriorityQueue<Action, long>();

        public int Count { get => actions.Count; }
        public Action First { get => actions.First; }

        internal void Enqueue(Action action)
        {
            actions.Enqueue(action, action.ScheduledTick);
        }

        internal void Cancel(string name)
        {
            int cancelCount = 0;

            if (name.Equals("*"))
            {
                cancelCount = actions.Count;
                actions.Clear();
            }
            else
            {
                foreach (Action action in actions)
                {
                    if (null != action.Name && action.Name.Contains(name))
                    {
                        cancelCount++;
                        actions.Remove(action);
                    }
                }
            }

            LOGGER.Info(System.String.Format("Canceled {0} Action(s) with name = [" + name + "]", cancelCount));
        }

        internal int GetMSUntilNextAction()
        {
            if (actions.Count != 0)
            {
                return (int) (actions.First.ScheduledTick - TimeProvider.GetTickCount());
            }

            // to wait indefinitely
            return -1;
        }

        internal IReadOnlyCollection<Action> GetActions()
        {
            return new List<Action>(actions);
        }

        internal void Dequeue()
        {
            actions.Dequeue();
        }
    }
}

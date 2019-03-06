using System;
using System.Runtime.InteropServices;
using System.Threading;
using Priority_Queue;

namespace Glue
{
    class ActionQueue
    {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // Using https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp copied directly into the project for now 
        // Thanks to BlueRaja.admin@gmail.com
        private static SimplePriorityQueue<Action, long> actions = new SimplePriorityQueue<Action, long>();
        private static Thread thread = null;
        private static EventWaitHandle eventWait = new AutoResetEvent (false);

        public static void Start()
        {
            if (null == thread)
            {
                thread = new Thread(new ThreadStart(Threadproc))
                {
                    Name = "ActionQueue"
                };
                thread.Start();
            }
        }

        public static void Enqueue(Action action, long timeScheduledMS)
        {
            actions.Enqueue(action, timeScheduledMS);
            eventWait.Set();
        }

        public static long Now()
        {
            // LOGGER.Debug("Now=" + GetTickCount64());

            // Commented out first attempt - may wish to play with it again
            // (long)(new TimeSpan(DateTime.Now.Ticks)).TotalMilliseconds;

            return (long) GetTickCount64();
        }

        private static int GetWaitUntilNextAction()
        {
            if (actions.Count != 0)
            {
                // BUGBUG assumes 1 tick == 1 MS which is not true on all systems
                return (int) (actions.First.TimeScheduledMS - Now());
            }
                
            // to wait indefinitely
            return -1;
        }

        private static void Threadproc()
        {
            LOGGER.Debug("Thread starting...");

            while (true)
            {
                Action action;
                while 
                    (
                        (actions.Count > 0) && 
                        ((action = actions.First).TimeScheduledMS <= Now())
                    )
                {
                    // Actions should play quickly ~1MS for now (see) 
                    // following comment).  If worker thread pool is used 
                    // asynchronous action processing will be supported.
                    // TODO Asynchronous action support: Submit actions.Play to worker thread pool
                    action.Play();

                    actions.Dequeue();
                }

                // Wait until next event is ready to fire, 
                // or events are added to the queue
                eventWait.WaitOne(GetWaitUntilNextAction());
            }
        }

        #region Win API Functions and Constants

        [DllImport("kernel32")]
        extern static UInt64 GetTickCount64();

        #endregion
    }
}

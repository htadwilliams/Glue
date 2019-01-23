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
        private static SimplePriorityQueue<IAction, long> actions = new SimplePriorityQueue<IAction, long>();

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

        public static void Enqueue(IAction action, long timeScheduledMS)
        {
            actions.Enqueue(action, timeScheduledMS);
            eventWait.Set();
        }

        public static long Now()
        {
            // LOGGER.Debug("Now=" + GetTickCount64());

            // The following looked like too much overhead in temporary object creation 
            // TODO Not sure if native call is worse though will have to perf test a bit
            // (long)(new TimeSpan(DateTime.Now.Ticks)).TotalMilliseconds;

            return (long) GetTickCount64();
        }

        private static int GetWaitUntilNextAction()
        {
            int waitUntilNextActionMS = 0;

            if (actions.Count > 0)
            {
                waitUntilNextActionMS = (int) (actions.First.TimeScheduledMS - Now());
            }
            else if (actions.Count == 0)
            {
                // -to wait indefinitely
                waitUntilNextActionMS = -1;
            }

            return waitUntilNextActionMS;
        }

        private static void Threadproc()
        {
            LOGGER.Debug("Starting...");

            while (true)
            {
                IAction action;
                while 
                    (
                        (actions.Count > 0) && 
                        ((action = actions.First).TimeScheduledMS <= Now())
                    )
                {
                    action.Play();
                    actions.Dequeue();
                }

                eventWait.WaitOne(GetWaitUntilNextAction());
            }
        }

        [DllImport("kernel32")]
        extern static UInt64 GetTickCount64();
    }
}

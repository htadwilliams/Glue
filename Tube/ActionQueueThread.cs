using System.Threading;
using Glue.native;

namespace Glue
{
    class ActionQueueThread
    {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // Using https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp copied directly into the project 
        // Thanks to BlueRaja.admin@gmail.com
        private static readonly ActionQueue s_actions = new ActionQueue();
        private static Thread s_thread = null;
        private static readonly EventWaitHandle s_eventWaitNextAction = new AutoResetEvent (false);

        public static void Start()
        {
            if (null == s_thread)
            {
                s_thread = new Thread(new ThreadStart(Threadproc))
                {
                    Name = "ActionQueue",

                    // set or app won't exit when main thread closes (e.g. form is closed)
                    IsBackground = true
                };

                s_thread.Start();
            }
        }

        public static void Schedule(Action action)
        {
            s_actions.Enqueue(action);

            // wake up the thread
            s_eventWaitNextAction.Set();
        }

        internal static void Cancel(string name)
        {
            s_actions.Cancel(name);
        }

       private static void Threadproc()
        {
            LOGGER.Debug("Thread starting...");

            while (true)
            {
                Action action;
                while 
                    (
                        (s_actions.Count > 0) && 
                        ((action = s_actions.First).TimeScheduledMS <= TimeProvider.GetTickCount())
                    )
                {
                    // Actions should play quickly ~1MS for now (see) 
                    // following comment).  If worker thread pool is used 
                    // asynchronous action processing will be supported.
                    // TODO Asynchronous action support: Submit action.Play to worker thread pool
                    action.Play();
                    s_actions.Dequeue();
                }

                // Wait until next event is ready to fire 
                // or events are added to the queue via Schedule()
                s_eventWaitNextAction.WaitOne(s_actions.GetMSUntilNextAction());
            }
        }
    }
}

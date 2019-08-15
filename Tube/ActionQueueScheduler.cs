using System.Threading;
using Glue.Actions;
using Glue.Native;

namespace Glue
{
    public interface IActionScheduler
    {
        void Schedule(Action action);

        void Cancel(string name);
    }

    class ActionQueueScheduler : IActionScheduler
    {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // Using https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp copied directly into the project 
        // Thanks to BlueRaja.admin@gmail.com
        private readonly ActionQueue actions = new ActionQueue();
        private Thread thread = null;
        private readonly EventWaitHandle eventWaitNextAction = new AutoResetEvent (false);

        public void Start()
        {
            if (null == thread)
            {
                thread = new Thread(new ThreadStart(Threadproc))
                {
                    Name = "ActionQueue",

                    // set or app won't exit when main app thread closes
                    IsBackground = true
                };

                thread.Start();
            }
        }

        public void Schedule(Action action)
        {
            actions.Enqueue(action);

            // wake up the thread
            eventWaitNextAction.Set();
        }

        public void Cancel(string name)
        {
            actions.Cancel(name);
        }

       private void Threadproc()
        {
            LOGGER.Debug("Thread starting...");

            while (true)
            {
                Action action;
                while 
                    (
                        (actions.Count > 0) && 
                        ((action = actions.First).ScheduledTick <= TimeProvider.GetTickCount())
                    )
                {
                    // Actions should play quickly ~1MS for now (see following TODO)
                    // TODO Asynchronous action support: Submit action.Play to worker thread pool
                    action.Play();
                    actions.Dequeue();
                }

                // Wait until next event is ready to fire 
                // or events are added to the queue via Schedule()
                eventWaitNextAction.WaitOne(actions.GetMSUntilNextAction());
            }
        }
    }
}

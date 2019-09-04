using System.Linq;
using System.Threading;
using Glue.Native;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Glue.Actions
{
    public delegate void OnQueueChange(ReadOnlyCollection<Action> queue);

    public interface IActionScheduler
    {
        void Schedule(Action action);

        void Cancel(string name);

        // event OnQueueChange;
    }

    class ActionQueueScheduler : IActionScheduler
    {
        public event OnQueueChange QueueChangeEvent;

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

            NotifySubscribers();

            // wake up the thread
            eventWaitNextAction.Set();
        }

        private void NotifySubscribers()
        {
            if (null != this.QueueChangeEvent)
            {
                List<Action> actions = this.actions.GetActions().ToList<Action>();
                actions.Sort();
                QueueChangeEvent(actions.AsReadOnly());
            }
        }

        public void Cancel(string name)
        {
            actions.Cancel(name);
            NotifySubscribers();
        }

       private void Threadproc()
        {
            LOGGER.Debug("Thread: [" + this.thread.Name + "] starting...");

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
                    NotifySubscribers();
                }

                // Wait until next event is ready to fire 
                // or events are added to the queue via Schedule()
                eventWaitNextAction.WaitOne(actions.GetMSUntilNextAction());
            }
        }
    }
}

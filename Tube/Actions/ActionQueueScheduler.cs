using Glue.Native;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

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
        private const string THREAD_NAME = "ActionQueue";

        // Using https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp copied directly into the project 
        // Thanks to BlueRaja.admin@gmail.com
        private readonly ActionQueue actions = new ActionQueue();
        private Thread thread = null;
        private readonly EventWaitHandle eventWaitNextAction = new AutoResetEvent (false);

        public void Start()
        {
            if (null == thread)
            {
                thread = new Thread(new ThreadStart(ScheduleThreadProc))
                {
                    Name = THREAD_NAME,

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

        public ReadOnlyCollection<Action> GetActions()
        {
            List<Action> actions = this.actions.GetActions().ToList<Action>();
            actions.Sort();
            return actions.AsReadOnly();
        }

        private void NotifySubscribers()
        {
            if (null != this.QueueChangeEvent)
            {
                QueueChangeEvent(GetActions());
            }
        }

        public void Cancel(string name)
        {
            actions.Cancel(name);
            NotifySubscribers();
        }

       private void ScheduleThreadProc()
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
                    actions.Dequeue();

                    // Actions should play quickly ~1MS for now (see following TODO)
                    // TODO Asynchronous action support: Submit action.Play to worker thread pool
                    action.Play();
                    NotifySubscribers();
                }

                // Wait until next event is ready to fire 
                // or events are added to the queue via Schedule()
                eventWaitNextAction.WaitOne(actions.GetMSUntilNextAction());
            }
        }
    }
}

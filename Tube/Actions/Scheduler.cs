using Glue.Native;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace Glue.Actions
{
    public class Scheduler : IActionScheduler
    {
        public event OnQueueChange QueueChangeEvent;

        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string THREAD_NAME = "ActionQueue";

        // Using https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp copied directly into the project 
        // Thanks to BlueRaja.admin@gmail.com
        private readonly Queue actions = new Queue();
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

        public void Cancel(string name)
        {
            actions.Cancel(name);
            NotifySubscribers();
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
                    NotifySubscribers();

                    try
                    {
                        ThreadPool.QueueUserWorkItem(action.PlayWaitCallback, this);
                    }
                    catch (NotSupportedException e)
                    {
                        LOGGER.Debug("Exception submitting Action.Play() to ThreadPool", e);
                        action.Play();
                    }
                }

                // Wait until next event is ready to fire 
                // or events are added to the queue via Schedule()
                eventWaitNextAction.WaitOne(actions.GetMSUntilNextAction());
            }
        }
    }
}

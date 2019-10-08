using System;

namespace Glue.Events
{
    public class BusEventArgs<T> : EventArgs
    {
        private readonly T busEvent;

        public BusEventArgs(T busEvent)
        {
            this.busEvent = busEvent;
        }

        public T BusEvent
        {
            get
            {
                return busEvent;
            }
        }
    }

    public class EventBus<T>
    {
        private static EventBus<T> s_instance = null;
        private static readonly object s_lock = new object();

        protected EventBus()
        {
        }

        public event EventHandler<BusEventArgs<T>> EventRecieved;

        public static EventBus<T> Instance
        {
            get
            {
                lock (s_lock)
                {
                    if (s_instance == null)
                    {
                        s_instance = new EventBus<T>();
                    }
                    return s_instance;
                }
            }
        }

        public int SendEvent(object sender, T newEvent)
        {
            lock (s_lock)
            {
                if (null != EventRecieved)
                {
                    EventRecieved(sender, new BusEventArgs<T>(newEvent));

                    return EventRecieved.GetInvocationList().Length;
                }
                else 
                {
                    return 0;
                }
            }
        }

        public int Empty()
        {
            int itemsDumped = 0;

            lock (s_lock)
            {
                if (null != EventRecieved)
                {
                    itemsDumped = EventRecieved.GetInvocationList().Length;
                    EventRecieved = null;
                }

                return itemsDumped;
            }
        }
    }
}

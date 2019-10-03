using System;

namespace Glue
{
    public class BusEventArgs<T> : EventArgs
    {
        private T busEvent;

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

        public event EventHandler<BusEventArgs<T>> EventRecieved;

        public void CreateEvent(object sender, T newEvent)
        {
            EventRecieved?.Invoke(sender, new BusEventArgs<T>(newEvent));
        }
    }
}

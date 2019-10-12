using System;
using System.Collections.Generic;

namespace Glue.Events
{
    public class ReturningEventBus<TEventArgs, TReturnArgs>
    {
        private static ReturningEventBus<TEventArgs, TReturnArgs> s_instance = null;
        private static readonly object s_lock = new object();

        public delegate TReturnArgs ReturningEventHandler(object sender, TEventArgs e);
        public event ReturningEventHandler ReturningEventRecieved;

        protected ReturningEventBus()
        {
        }

        public static ReturningEventBus<TEventArgs, TReturnArgs> Instance
        {
            get
            {
                lock (s_lock)
                {
                    if (s_instance == null)
                    {
                        s_instance = new ReturningEventBus<TEventArgs, TReturnArgs>();
                    }
                    return s_instance;
                }
            }
        }

        public List<TReturnArgs> SendEvent(object sender, TEventArgs newEvent)
        {
            List<TReturnArgs> returnList = null;

            lock (s_lock)
            {
                if (null != ReturningEventRecieved)
                {
                    returnList = new List<TReturnArgs>(ReturningEventRecieved.GetInvocationList().Length);

                    foreach(Delegate returningEventRecieved in ReturningEventRecieved.GetInvocationList())
                    {
                        object returnObject = returningEventRecieved.DynamicInvoke(new object[] {sender, newEvent});
                        returnList.Add((TReturnArgs) returnObject);
                    }
                }

                return returnList;
            }
        }

        public int Empty()
        {
            int itemsDumped = 0;

            lock (s_lock)
            {
                if (null != ReturningEventRecieved)
                {
                    itemsDumped = ReturningEventRecieved.GetInvocationList().Length;
                    ReturningEventRecieved = null;
                }

                return itemsDumped;
            }
        }
    }
}

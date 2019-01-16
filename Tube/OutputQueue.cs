using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Glue
{
    class ActionQueue
    {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static ConcurrentQueue<Action> actions = new ConcurrentQueue<Action>();
        private static Thread thread = null;

        public static void Start()
        {
            if (null == thread)
            {
                thread = new Thread(new ThreadStart(Threadproc));
                thread.Start();
            }
        }

        public static void Enqueue(Action action)
        {
            actions.Enqueue(action);
        }

        static void Threadproc()
        {
            LOGGER.Debug("Starting...");

            while (true)
            {
                while (actions.TryDequeue(out Action action))
                {
                    LOGGER.Debug("Action key code: " + action.Key.ToString());
                }
                Thread.Sleep(1);
            }
        }
    }
}

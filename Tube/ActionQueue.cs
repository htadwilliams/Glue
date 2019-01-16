using Priority_Queue;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Glue
{
    class ActionQueue
    {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // Using https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp copied directly into the project for now 
        // Thanks to BlueRaja.admin@gmail.com 
        private static SimplePriorityQueue<Action, long> actions = new SimplePriorityQueue<Action, long>();
        private static Thread thread = null;

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

        public static void Enqueue(Action action, long timeScheduledMS)
        {
            actions.Enqueue(action, timeScheduledMS);
        }

        public static long Now()
        {
            // LOGGER.Debug("Now=" + GetTickCount64());
            // The following looked like too much overhead in temporary object creation 
            // TODO Not sure if native call is worse though will have to perf test a bit
            // (long)(new TimeSpan(DateTime.Now.Ticks)).TotalMilliseconds;
            return (long) GetTickCount64();
        }

        private static void Threadproc()
        {
            LOGGER.Debug("Starting...");

            // TODO Add thread termination condition or make sure one isn't needed
            while (true)
            {
                if (actions.Count > 0)
                {
                    Action action = actions.First;

                    if (action.TimeScheduledMS <= Now())
                    {
                        // TODO trigger actual key presses!
                        actions.Dequeue();
                        LOGGER.Debug("Action: Type=" + action.ActionType.ToString() + " Key= " + action.Key.ToString());
                    }
                }

                // TODO How long should thread between queue checks? Configurable?
                Thread.Sleep(1);
            }
        }

        [DllImport("kernel32")]
        extern static UInt64 GetTickCount64();
    }
}

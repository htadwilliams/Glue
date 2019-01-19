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
                Action action;
                while 
                    (
                        (actions.Count > 0) && 
                        ((action = actions.First).TimeScheduledMS <= Now())
                    )
                {
                    action.Play();
                    actions.Dequeue();
                }

                // TODO How long should thread between queue checks? Configurable?
                // Sleep(0) should yield a timeslice but shows a considerable bump 
                // in CPU use in the debugger.  Need to perf test with release build.

                // TODO Look into making thread wakeup signaled 
                // Use something like a wait handle rather than polling the queue
                Thread.Sleep(1);
            }
        }

        [DllImport("kernel32")]
        extern static UInt64 GetTickCount64();
    }
}

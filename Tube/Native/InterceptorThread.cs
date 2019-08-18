using Glue.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Glue
{
    /// <summary>
    /// 
    /// Prototype
    /// 
    /// This thread allows hooking keyboard and mouse from their own thread.
    /// 
    /// </summary>
    class InterceptorThread
    {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Thread thread = null;

        public void Start()
        {
            if (null == thread)
            {
                thread = new Thread(new ThreadStart(Threadproc))
                {
                    Name = "HookThread",

                    // main app thread closure will force this thread to close
                    IsBackground = true
                };

                thread.Start();
            }
        }

        private void Threadproc()
        {
            LOGGER.Debug("Thread: [" + this.thread.Name + "] starting...");

            // Native keyboard and mouse hook initialization
            KeyInterceptor.Initialize(KeyboardHandler.HookCallback);
            MouseInterceptor.Initialize(MouseHandler.HookCallback);

            while (true)
            {
                System.Windows.Forms.Application.DoEvents();

                // Without sleep thread utilization goes from 0% to 3% or so
                // Different numbers for sleep don't have a noticeable affect
                Thread.Sleep(1);
            }
        }
    }
}

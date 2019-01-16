using log4net.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace Glue
{
    static class GlueTube
    {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static Main mainForm = null;
        public static Main MainForm { get => mainForm; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

#pragma warning disable CS0618 // Type or member is obsolete
            XmlConfigurator.Configure(new FileInfo(fileName: ConfigurationSettings.AppSettings["log4net-config-file"]));
#pragma warning restore CS0618 // Type or member is obsolete

            // Console is intended for convenience during debugging
            // but has some performance impact, and is a bit
            // of a hack.
            if (LOGGER.IsDebugEnabled)
            {
                WinConsole.Initialize(false);
            }

            LOGGER.Info("Startup!");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Native class invocation 
            KeyHandler.Initialize();
            KeyInterceptor.Initialize(KeyHandler.HookCallback);
            MouseInterceptor.Initialize();
            ActionQueue.Start();

            mainForm = new Main();

            LOGGER.Debug("Entering Application.Run()...");
            Application.Run(mainForm);
            LOGGER.Debug("...returned from Application.Run().");

            // Native class de-initialization
            MouseInterceptor.Cleanup();
            KeyInterceptor.Cleanup();

            LOGGER.Info("Exiting");
        }
    }
}

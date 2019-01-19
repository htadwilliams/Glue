using log4net.Config;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
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
            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = "Main";
            }


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


            // Using https://github.com/sharpdx/sharpdx
            // See quick example code 
            // https://stackoverflow.com/questions/3929764/taking-input-from-a-joystick-with-c-sharp-net
            LOGGER.Info("Enumerating DirectInput devices...");
            DirectInput directInput = new DirectInput();
            for (int i = 17; i < 28; i++)
            {
                foreach (
                    var deviceInstance
                    in directInput.GetDevices((DeviceType) i, DeviceEnumerationFlags.AllDevices))
                {
                    String message = String.Format(
                        "Input device: {0} type={1} GUID=[{2}]",
                        deviceInstance.InstanceName,
                        deviceInstance.Type.ToString(),
                        deviceInstance.ProductGuid.ToString());
                    LOGGER.Info(message);
                }
            }

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

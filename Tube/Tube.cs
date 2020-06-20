using Glue.Actions;
using Glue.Events;
using Glue.Forms;
using Glue.Native;
using Glue.Triggers;
using log4net.Config;
using NerfDX.DirectInput;
using NerfDX.Events;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using WindowsInput.Native;

[assembly: XmlConfigurator(Watch = true)]

namespace Glue
{
    internal static class Tube
    {
        #region Automatic properties
        internal static Dictionary<VirtualKeyCode, KeyboardRemapEntry> KeyMap { get; set; }
        public static Dictionary<string, Macro> Macros { get; set; }
        public static ViewMain MainForm { get; set; }
        public static string FileName { get; set; } = FILENAME_DEFAULT;
        public static Scheduler Scheduler { get; } = new Scheduler();
        public static bool MouseLocked { get; set; } = false;
        public static DirectInputManager DirectInputManager { get; private set; }
        public static List<Trigger> Triggers { get; set; }
        #endregion

        #region Private static fields
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static bool s_writeOnExit = false;
        #endregion

        #region Constants
        private const string FILENAME_DEFAULT = "macros.glue";

        // Override file used for form persistence
        private const string FORM_SETTINGS_FILENAME = "Glue.form-settings.json";
        #endregion
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = "Main";
            }

            try
            {
                InitLogging();
                InitData();

                // Starts thread for timed queue of actions such as pressing keys,
                // activating game controller buttons, playing sounds, etc.
                Scheduler.Start();

                // Initialization of forms and context should be done before anything
                // that can generate input Events.
                FormSettings.FileName = FORM_SETTINGS_FILENAME;
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                TrayApplicationContext<ViewMain> trayApplicationContext = new TrayApplicationContext<ViewMain>();
                MainForm = (ViewMain) trayApplicationContext.MainForm;

                // Native keyboard and mouse hook initialization
                KeyInterceptor.Initialize(KeyboardHandler.HookCallback);

                // TODO Make mouse hook a toggle-able option in GUI
                // TODO Mouse hook should release when windows are being sized / dragged
                // MouseInterceptor.Initialize(MouseHandler.HookCallback);

                DirectInputManager = new DirectInputManager(new Logger4net(typeof(DirectInputManager).Name));
                DirectInputManager.Initialize();

                string fileName = args.Length > 0
                    ? args[0]
                    : FILENAME_DEFAULT;

                ProcessFileArg(fileName);

                LOGGER.Debug("Entering Application.Run()...");
                Application.Run(trayApplicationContext);
                LOGGER.Debug("...returned from Application.Run().");
            }
            finally
            {
                // Native class de-initialization
                MouseInterceptor.Cleanup();
                KeyInterceptor.Cleanup();
            }

            if (s_writeOnExit)
            {
                SaveFile(FileName);
            }

            LOGGER.Info("Exiting");
        }

        internal static void ProcessFileArg(String fileName)
        {
            // Developer mode 
            if (fileName.Contains("DEFAULT"))
            {
                LOGGER.Info("Started with DEFAULT flag. Generating default content.");
                FileName = FILENAME_DEFAULT;
                DefaultContent.Generate();

                if (!File.Exists(FileName)) 
                {
                    s_writeOnExit = true;
                }
            }
            // Developer mode
            else if (fileName.Contains("EMPTY"))
            {
                FileName = FILENAME_DEFAULT;
                LOGGER.Info("Starting EMPTY!");

                if (!File.Exists(FileName)) 
                {
                    s_writeOnExit = true;
                }
            }
            // Normal operation
            else 
            {
                if (!File.Exists(fileName))
                {
                    LOGGER.Info("File [" + FileName + "] does not exist - creating example content");
                    DefaultContent.Generate();
                    s_writeOnExit = true;
                }
                else
                {
                    LoadFile(fileName);
                    s_writeOnExit = false;
                }
                FileName = fileName;;
                MainForm.SetCaption(FileName);
            }
        }

        internal static void PlayMacro(string macroName)
        {
            if (Macros.TryGetValue(macroName, out Macro macro))
            {
                LOGGER.Debug("Playing macro [" + macroName + "]");

                macro.ScheduleActions();
                EventBus<EventMacro>.Instance.SendEvent(null, new EventMacro(macroName));
            }
            else
            {
                LOGGER.Warn("Macro not found: " + macroName);
            }
        }

        private static void InitLogging()
        {
            // This is how the log4net examples do it, and nobody has bothered
            // to update them to get rid of the warning
#pragma warning disable CS0618 // Type or member is obsolete
            XmlConfigurator.Configure(new FileInfo(fileName: ConfigurationSettings.AppSettings["log4net-config-file"]));
#pragma warning restore CS0618 // Type or member is obsolete

            // Console is intended for convenience during debugging
            // but has some performance impact, and is a bit of a hack.
            if (LOGGER.IsDebugEnabled)
            {
                WinConsole.Initialize(false);
            }

            LOGGER.Info("Startup!");
        }

        private static void SaveFile(string fileName)
        {
            LOGGER.Info("Saving to [" + fileName + "]");
            JsonSerializer serializer = new JsonSerializer
            {
                TypeNameHandling = TypeNameHandling.None,
                NullValueHandling = NullValueHandling.Ignore,
            };

            using (StreamWriter sw = new StreamWriter(fileName))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;

                writer.WriteComment(
                    "\r\n\r\nGlue macro tool file\r\n\r\n" + 
                    "Json containing the following elements\r\n\r\n" + 
                    "    macros     Actions to be performed such as pressing a key or playing a sound.\r\n" + 
                    "    triggers   Bind keys or key combinations to macros.\r\n" + 
                    "    keyMap     Each entry remaps a key on the keyboard.\r\n\r\n");
                sw.Write("\r\n");

                JsonWrapper jsonWrapper = new JsonWrapper(Triggers, KeyMap, Macros);
                serializer.Serialize(writer, jsonWrapper);
            }
        }

        private static void InitData()
        {
            KeyMap = new Dictionary<VirtualKeyCode, KeyboardRemapEntry>();
            Macros = new Dictionary<string, Macro>();

            if (null != Triggers)
            {
                foreach (Trigger trigger in Triggers)
                {
                    trigger.Dispose();
                }
            }
            Triggers = new List<Trigger>();
        }

        public static bool LoadFile(string fileName)
        {
            InitData();
            LOGGER.Info("Loading file [" + fileName + "]");

            if (!File.Exists(fileName))
            {
                return false;
            }

            JsonSerializer serializer = new JsonSerializer
            {
                DefaultValueHandling = DefaultValueHandling.Populate
            };

            try
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    using (JsonReader reader = new JsonTextReader(sr))
                    {
                        reader.SupportMultipleContent = true;
                        reader.Read();

                        JsonWrapper jsonWrapper = serializer.Deserialize<JsonWrapper>(reader);

                        Macros = jsonWrapper.GetMacroMap();
                        KeyMap = jsonWrapper.GetKeyboardMap();
                        Triggers = jsonWrapper.GetTriggers();
                    }
                }
            }
            catch (JsonSerializationException e)
            {
                HandleJsonException(fileName, e);
                return false;
            }
            catch (JsonReaderException e)
            {
                HandleJsonException(fileName, e);
                return false;
            }

            LOGGER.Info(String.Format("    Loaded {0} macros", Macros.Count));
            LOGGER.Info(String.Format("    Loaded {0} triggers", Triggers.Count));
            LOGGER.Info(String.Format("    Loaded {0} remapped keys", KeyMap.Count));

            return true;
        }

        internal static void HandleJsonException(string fileName, Exception e)
        {
            string output = 
                "Failed to load " + fileName + ".\r\n\r\n" +
                e.Message;

            MessageBox.Show(output, "Glue - File Load Error");
            LOGGER.Error(output, e);
                         
            return;
        }

        internal static void ActivateMouseLock(LockAction lockAction)
        {
            switch (lockAction)
            {
                case LockAction.Lock:
                    MouseLocked = true;
                    break;

                case LockAction.Unlock:
                    MouseLocked = false;
                    break;

                case LockAction.Toggle:
                    MouseLocked = !MouseLocked;
                    break;
            }

            LOGGER.Info("Activated mouse lock: action = " + lockAction + " lock = " + MouseLocked);
        }
    }
}

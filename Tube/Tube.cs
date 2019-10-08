using Glue.Actions;
using Glue.Events;
using Glue.Forms;
using Glue.Native;
using Glue.Triggers;
using log4net.Config;
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
        internal static Dictionary<VirtualKeyCode, KeyboardRemapEntry> KeyMap { get => s_keyMap; set => s_keyMap = value; }
        public static Dictionary<string, Macro> Macros { get => s_macros; set => s_macros = value; }
        public static ViewMain MainForm { get => s_mainForm; set => s_mainForm = value; }
        public static string FileName { get => s_fileName; set => s_fileName = value; }
        public static Scheduler Scheduler { get => s_actionScheduler; }
        public static bool MouseLocked { get => s_lockMouse; set => s_lockMouse = value; }
        public static DirectInputManager DirectInputManager => s_directInputManager;
        public static TriggerManager TriggerManager { get => s_triggerManager; set => s_triggerManager = value; }

        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // Core data structures for macros, triggers, and keyboard remapping
        private static Dictionary<string, Macro> s_macros;
        private static TriggerManager s_triggerManager = new TriggerManager();
        private static Dictionary<VirtualKeyCode, KeyboardRemapEntry> s_keyMap;

        // Core sub-systems 
        private static readonly Scheduler s_actionScheduler = new Scheduler();
        private static bool s_lockMouse = false;
        private static readonly DirectInputManager s_directInputManager = new DirectInputManager();

        // GUI objects
        private static ViewMain s_mainForm;
        private static TrayApplicationContext s_context;

        // GUI state
        private const string FILENAME_DEFAULT = "macros.glue";
        private static bool s_writeOnExit = false;
        private static string s_fileName = FILENAME_DEFAULT;

        // Override file used for form persistence
        private const string FORM_SETTINGS_FILENAME = "Glue.form-settings.json";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            FileName = args.Length > 0
                ? args[0]
                : FILENAME_DEFAULT;

            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = "Main";
            }

            try
            {
                InitLogging();
                s_directInputManager.Initialize();

                // Native keyboard and mouse hook initialization
                KeyInterceptor.Initialize(KeyboardHandler.HookCallback);

                // TODO Make mouse hook a toggle-able option in GUI
                // TODO Mouse hook should release when windows are being sized / dragged
                MouseInterceptor.Initialize(MouseHandler.HookCallback);

                // Starts thread for timed queue of actions such as pressing keys,
                // activating game controller buttons, playing sounds, etc.
                s_actionScheduler.Start();

                if (!FileName.Contains("EMPTY"))
                {
                    LoadFile(FileName);
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                FormSettings.FileName = FORM_SETTINGS_FILENAME;

                s_context = new TrayApplicationContext();
                MainForm = (ViewMain) s_context.MainForm;

                LOGGER.Debug("Entering Application.Run()...");
                Application.Run(s_context);
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

        public static void AddRemap(VirtualKeyCode keyOld, VirtualKeyCode keyNew, string procName)
        {
            Tube.KeyMap.Add(keyOld, new KeyboardRemapEntry(keyOld, keyNew, procName));
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

                JsonWrapper jsonWrapper = new JsonWrapper(TriggerManager, KeyMap, Macros);
                serializer.Serialize(writer, jsonWrapper);
            }
        }

        private static void InitData()
        {
            KeyMap = new Dictionary<VirtualKeyCode, KeyboardRemapEntry>();
            Macros = new Dictionary<string, Macro>();

            TriggerManager.Clear();
        }

        public static void LoadFile(string fileName)
        {
            InitData();

            LOGGER.Info("Loading file [" + fileName + "]");

            if (File.Exists(fileName))
            {
                List<TriggerKeyboard> keyboardTriggers;
                List<TriggerController> controllerTriggers;

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
                            keyboardTriggers = jsonWrapper.KeyboardTriggers;
                            controllerTriggers = jsonWrapper.ControllerTriggers;
                        }
                    }
                }
                catch (JsonReaderException e)
                {
                    string message = 
                        "Failed to load " + fileName + ".\r\n\r\n" +
                        e.Message;

                    MessageBox.Show(message, "Glue - File Load Error");
                    LOGGER.Error(message, e);
                         
                    return;
                }

                if (null != Macros && Macros.Count != 0)
                {
                    LOGGER.Info(String.Format("    Loaded {0} macros", Macros.Count));
                }
                if (null != keyboardTriggers && keyboardTriggers.Count != 0)
                {
                    TriggerManager.KeyboardTriggers.Clear();
                    TriggerManager.AddTriggers(keyboardTriggers);

                    LOGGER.Info(String.Format("    Loaded {0} keyboard triggers", keyboardTriggers.Count));
                }
                if (null != controllerTriggers && controllerTriggers.Count != 0)
                {
                    TriggerManager.ControllerTriggers.Clear();
                    TriggerManager.AddTriggers(controllerTriggers);

                    LOGGER.Info(String.Format("    Loaded {0} controller triggers", controllerTriggers.Count));
                }
                if (null != KeyMap && KeyMap.Count != 0)
                {
                    LOGGER.Info(String.Format("    Loaded {0} remapped keys", KeyMap.Count));
                }

                FileName = fileName;
            }
            else
            {
                LOGGER.Info("File not found - creating example content");
                DefaultContent.Generate();
                SaveFile(fileName);
            }
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

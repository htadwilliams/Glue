using Glue.Actions;
using Glue.Events;
using Glue.Forms;
using Glue.Native;
using Glue.Triggers;
using Interceptor;
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
        public static bool WriteOnExit { get => s_writeOnExit; set => s_writeOnExit = value; }
        public static CmdReader CmdFileReader { get; set; }
        public static Input IntercepterDriverWrapper { get; private set; }

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

                // Careful. Does bad things if the event handlers block the driver.
                // TODO: Make Interceptor driver use toggle-able in the GUI
                InitIntercepterDriver();

                // Native keyboard and mouse hook initialization
                KeyInterceptor.Initialize(KeyboardHandler.HookCallback);

                // TODO Make mouse hook a toggle-able option in GUI
                // TODO Mouse hook should release when windows are being sized / dragged
                MouseInterceptor.Initialize(MouseHandler.HookCallback);

                DirectInputManager = new DirectInputManager(new Logger4net(typeof(DirectInputManager).Name));
                DirectInputManager.Initialize();

                CmdFileReader = new CmdReader();

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
                CmdFileReader.Dispose();
            }

            if (WriteOnExit)
            {
                SaveFile(FileName);
            }

            LOGGER.Info("Exiting");
        }

        private static void InitIntercepterDriver()
        {
            IntercepterDriverWrapper = new Input
            {
                KeyboardFilterMode = KeyboardFilterMode.All,

                // For mouse lock
                MouseFilterMode = MouseFilterMode.MouseMove
            };

            // Probably not needed unless we need the filter driver for both input and output
            // IntercepterDriverWrapper.OnKeyPressed += IntercepterDriverWrapper_OnKeyPressed;

            // Clicking in console with this enabled is very bad. 
            IntercepterDriverWrapper.OnMousePressed += IntercepterDriverWrapper_OnMousePressed;

            try
            {
                IntercepterDriverWrapper.Load();
            }
            catch (DllNotFoundException notFoundException)
            {
                LOGGER.Info("Unable to load keyboard filter driver DLL. Make sure DLL is in application directory: " + notFoundException);
                return;
            }
            catch (Exception e)
            {
                LOGGER.Warn("Failed to load keyboard filter driver. SendInput will be used: " + e);
                return;
            }

            LOGGER.Info("Keyboard filter driver loaded and will be used instead of SendInput()!");
        }

        private static void IntercepterDriverWrapper_OnMousePressed(object sender, MousePressedEventArgs e)
        {
            // Lock the mouse
            e.Handled = Tube.MouseLocked;
        }

        //private static void IntercepterDriverWrapper_OnKeyPressed(object sender, KeyPressedEventArgs e)
        //{
        //    string state;
        //    switch (e.State)
        //    {
        //        case KeyState.Down:
        //            state = "+";
        //            break;

        //        case KeyState.Up:
        //            state = "-";
        //            break;

        //        default:
        //            state = e.State + " ";
        //            break;
        //    }

        //    LOGGER.Debug("Filter driver key pressed: " + state + e.Key );
        //}

        internal static void ProcessFileArg(String fileName)
        {
            // Developer mode 
            if (fileName.Contains("DEFAULT"))
            {
                LOGGER.Info("Started with DEFAULT flag. Generating default content.");
                FileName = FILENAME_DEFAULT;
                DefaultContent.Generate();

                WriteOnExit = true;
            }
            // Developer mode
            else if (fileName.Contains("EMPTY"))
            {
                FileName = FILENAME_DEFAULT;
                LOGGER.Info("Starting EMPTY!");

                if (!File.Exists(FileName)) 
                {
                    WriteOnExit = true;
                }
            }
            // Normal operation
            else 
            {
                if (!File.Exists(fileName))
                {
                    LOGGER.Info("File [" + FileName + "] does not exist - creating example content");
                    DefaultContent.Generate();
                    WriteOnExit = true;
                }
                else
                {
                    LoadFile(fileName);

                    // don't write over a file that's only being read 
                    // 
                    // write flag should be set if user takes an action that 
                    // modifies content via GUI
                    WriteOnExit = false;
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
                EventBus<EventUserInfo>.Instance.SendEvent(
                    null, 
                    new EventUserInfo("[" + macroName + "]"));
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
                string newline = Environment.NewLine;

                writer.WriteComment(
                    newline + 
                    newline + 
                    "Glue macro tool file" + newline + 
                    newline +
                    "Json containing the following elements" + newline +
                    newline +
                    "    macros     Actions to be performed such as pressing a key or playing a sound." + newline + 
                    "    triggers   Bind keys or key combinations to macros." + newline + 
                    "    keyMap     Each entry remaps a key on the keyboard." + newline +
                         newline);
                sw.Write(newline);

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
                "Failed to load " + fileName + Environment.NewLine +
                Environment.NewLine +
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

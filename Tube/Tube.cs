using Glue.Actions;
using Glue.Event;
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
using WindowsInput;
using WindowsInput.Native;
using static Glue.Actions.ActionKey;

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
        private static bool s_writeOnExit = false;                            // Set if example content created, or if GUI changes content
        private static string s_fileName = FILENAME_DEFAULT;

        // Override file used for form persistence
        private const string FORM_SETTINGS_FILENAME         = "Glue.form-settings.json";

        //
        // TODO Move example content generation to its own class
        //

        // used to generate example content 
        private const string FILENAME_DEFAULT               = "macros.glue";
        private const string PROCESS_NAME_NOTEPAD           = "notepad";      // also matches Notepad++
        private const string PROCESS_NAME_WASD              = "fallout4.exe";

        // example content tweakables
        private const int TIME_REPEAT_SOUND_MS              = 5 * 1000;       // For several sound delay loops 
        private const int TIME_DELAY_GLOBAL_MS              = 100;            // Delay fudge everywhere - can crank up for debugging
        private const int TIME_DWELL_GLOBAL_MS              = 250;            // Pressed keys are held this long
        private const int TIME_DELAY_ACTION                 = 8 * 1000;       // Bound to trigger for single delayed action

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

        private static void AddRemap(VirtualKeyCode keyOld, VirtualKeyCode keyNew, string procName)
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

                JsonWrapper jsonWrapper = new JsonWrapper(TriggerManager.KeyboardTriggers, KeyMap, Macros);
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
                List<TriggerKeyboard> triggers;

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
                            triggers = jsonWrapper.Triggers;
                        }
                    }
                }
                catch (Exception e)
                {
                    // TODO User friendly error message
                    MessageBox.Show(e.Message);
                    LOGGER.Error("Failed to load file with exception: " + e);
                    return;
                }

                if (null != Macros && Macros.Count != 0)
                {
                    LOGGER.Info(String.Format("    Loaded {0} macros", Macros.Count));
                }
                if (null != triggers && triggers.Count != 0)
                {
                    LOGGER.Info(String.Format("    Loaded {0} triggers", triggers.Count));
                    TriggerManager.Clear();
                    TriggerManager.AddTriggers(triggers);
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
                CreateDefaultContent();
                SaveFile(fileName);
            }
        }

        internal static void ToggleMouseLock()
        {
            MouseLocked = !MouseLocked;
            LOGGER.Info("Toggled Mouse lock: " + MouseLocked);
        }
        
        private static void CreateDefaultContent()
        {
            string macroName;
            //
            // Create macro with several actions bound to CTRL-Z
            //
            Macro macro = new Macro(macroName = "delayed-action", TIME_DELAY_ACTION) 
                .AddAction(new ActionSound(TIME_DELAY_GLOBAL_MS,  "ahha.wav"))
                .AddAction(new ActionKey(TIME_DELAY_GLOBAL_MS,    VirtualKeyCode.RETURN,  ButtonStates.Both))
                ;
            Macros.Add(macroName, macro);
            // Setup trigger
            TriggerKeyboard trigger = new TriggerKeyboard(Keys.Z, macroName);
            trigger.AddModifier(Keys.LControlKey);
            TriggerManager.Add(trigger);

            //
            // Create and bind a typing macro (string of text) 
            // 
            macro = new Macro(macroName = "typing-stuff", 2000);
            
            macro.AddAction(
                new ActionTyping(
                    // Thinking of Maud you forget everything else
                    "Lorem ipsum dollar sit amet, Cogito Maud obliviscaris aliorum.",
                    10,     // delay MS
                    10));   // dwell time MS
            Macros.Add(macroName, macro);
            trigger = new TriggerKeyboard(Keys.C, macroName);
            trigger.AddModifier(Keys.LControlKey);
            trigger.AddModifier(Keys.LMenu);
            TriggerManager.Add(trigger);

            //
            // Create a trigger that alternates between macros - ripple fire example
            //
            macro = new Macro(macroName = "sound-servomotor", 0);
            macro.AddAction(new ActionSound(TIME_DELAY_GLOBAL_MS, "sound_servomotor.wav"));
            Macros.Add(macroName, macro);

            macro = new Macro(macroName = "sound-ahha", 0);
            macro.AddAction(new ActionSound(TIME_DELAY_GLOBAL_MS, "ahha.wav"));
            Macros.Add(macroName, macro);

            trigger = new TriggerKeyboard(Keys.S, new List<string> { "sound-servomotor", "sound-ahha" });
            trigger.AddModifier(Keys.LControlKey);
            TriggerManager.Add(trigger);

            //
            // Drum kit!
            // Schedule repeating sound every N MS 
            //
            (string soundMacroName, string soundFile, string repeatMacroName, string stopMacroName, Keys triggerKey)[] macroTable =
            {
                ("sound-dice",  "dice_roll.wav",        "repeat-sound-dice",    "stop-sound-dice",  Keys.Oemcomma),
                ("sound-tock",  "sound_click_tock.wav", "repeat-sound-tock",    "stop-sound-tock",  Keys.OemPeriod),
                ("sound-estop", "elev_stop.wav",        "repeat-sound-estop",   "stop-sound-estop", Keys.OemQuestion),
            };

            foreach((string soundMacroName, string soundFile, string repeatMacroName, string stopMacroName, Keys triggerKey) in macroTable)
            {
                // Sound macro
                macro = new Macro(macroName = soundMacroName, TIME_DELAY_GLOBAL_MS);
                macro.AddAction(new ActionSound(0, soundFile));
                Macros.Add(macroName, macro);

                // Repeater macro and trigger
                macro = new Macro(macroName = repeatMacroName, TIME_DELAY_GLOBAL_MS);
                macro.AddAction(new ActionRepeat(TIME_REPEAT_SOUND_MS, macroName, soundMacroName ));
                Macros.Add(macroName, macro);
                trigger = new TriggerKeyboard(triggerKey, repeatMacroName);
                trigger.AddModifier(Keys.LMenu);
                TriggerManager.Add(trigger);

                // Stopper macro and trigger
                macro = new Macro(macroName = stopMacroName, TIME_DELAY_GLOBAL_MS);
                macro.AddAction(new ActionCancel(repeatMacroName));
                Macros.Add(macroName, macro);
                trigger = new TriggerKeyboard(triggerKey, macroName);
                trigger.AddModifier(Keys.LControlKey);
                TriggerManager.Add(trigger);
            }

            //
            // Macro bound to mouse button X1 / X2
            //
            macro = new Macro(macroName = "F10", 0);
            macro.AddAction(new ActionKey(50, VirtualKeyCode.F10, ButtonStates.Both));
            Macros.Add(macroName, macro);

            // Same macro bound to two triggers            
            trigger = new TriggerKeyboard(Keys.XButton1, macroName);
            TriggerManager.Add(trigger);
            trigger = new TriggerKeyboard(Keys.XButton2, macroName);
            TriggerManager.Add(trigger);

            // 
            // Toggle - hold SPACE key every other time it is pressed 
            //
            macro = new Macro(macroName = "toggle-down", TIME_DELAY_GLOBAL_MS);
            macro.AddAction(new ActionKey(0, VirtualKeyCode.SPACE, ButtonStates.Press));
            Macros.Add(macroName, macro);

            macro = new Macro(macroName = "toggle-up", TIME_DELAY_GLOBAL_MS);
            macro.AddAction(new ActionKey(0, VirtualKeyCode.SPACE, ButtonStates.Release));
            Macros.Add(macroName, macro);

            trigger = new TriggerKeyboard(
                Keys.Space, ButtonStates.Both, new List<string> {"toggle-down", null, "toggle-up", null}, true);
            trigger.AddModifier(Keys.LControlKey);
            TriggerManager.Add(trigger);

            // 
            // Create mouse movement
            //
            macro = new Macro(macroName = "mouse-nudge-left", TIME_DELAY_GLOBAL_MS);
            macro.AddAction(
                new ActionMouse(
                    0, 
                    ActionMouse.CoordinateMode.RELATIVE,
                    -1, 0));
            Macros.Add(macroName, macro);
            trigger = new TriggerKeyboard(Keys.Left, macroName);
            trigger.AddModifier(Keys.LMenu);
            TriggerManager.Add(trigger);

            macro = new Macro(macroName = "mouse-center", TIME_DELAY_GLOBAL_MS);
            macro.AddAction(
                new ActionMouse(
                    0, 
                    ActionMouse.CoordinateMode.ABSOLUTE, 
                    65535 / 2, 65535 / 2));
            Macros.Add(macroName, macro);
            trigger = new TriggerKeyboard(Keys.Home, macroName);
            trigger.AddModifier(Keys.LMenu);
            TriggerManager.Add(trigger);

            macro = new Macro(macroName = "mouse-origin", TIME_DELAY_GLOBAL_MS);
            macro.AddAction(
                new ActionMouse(
                    0, 
                    ActionMouse.CoordinateMode.PIXEL, 
                    1, 1));
            Macros.Add(macroName, macro);
            trigger = new TriggerKeyboard(Keys.End, macroName);
            trigger.AddModifier(Keys.LMenu);
            TriggerManager.Add(trigger);

            macro = new Macro(macroName = "mouse-click-nomove", TIME_DELAY_GLOBAL_MS);
            macro.AddAction(
                new ActionMouse(
                    0,
                    ActionMouse.ClickType.CLICK,
                    MouseButton.LeftButton));
            Macros.Add(macroName, macro);
            trigger = new TriggerKeyboard(Keys.Delete, macroName);
            trigger.AddModifier(Keys.LMenu);
            TriggerManager.Add(trigger);

            macro = new Macro(macroName = "cancel-all", TIME_DELAY_GLOBAL_MS);
            macro.AddAction(new ActionCancel("*"));
            Macros.Add(macroName, macro);

            // TODO Trigger mod keys should allow logical combinations e.g. (LControlKey | RControlKey) 
            trigger = new TriggerKeyboard(Keys.C, macroName);
            trigger.AddModifier(Keys.LControlKey);
            TriggerManager.Add(trigger);
            trigger = new TriggerKeyboard(Keys.C, macroName);
            trigger.AddModifier(Keys.RControlKey);
            TriggerManager.Add(trigger);

            // Toggle mouse-lock or "Mouse SAFETY"
            macro = new Macro(macroName = "lock-mouse", 0);
            macro.AddAction(new ActionMouseLock());
            macro.AddAction(new ActionSound(TIME_DELAY_GLOBAL_MS, "sound_click_latch.wav"));

            Macros.Add(macroName, macro);
            trigger = new TriggerKeyboard(Keys.L, macroName);
            trigger.AddModifier(Keys.LControlKey);
            TriggerManager.Add(trigger);
            trigger = new TriggerKeyboard(Keys.L, macroName);
            trigger.AddModifier(Keys.RControlKey);
            TriggerManager.Add(trigger);

            // Sunless skies (and other games) won't allow binding to shift key
            // Mapping A to Shift allows binding game functions to that instead.
            AddRemap(VirtualKeyCode.LSHIFT, VirtualKeyCode.VK_A, "skies.exe");

            // Evil evil swap for people typing into notepad!  Easy for quick functional test.
            AddRemap(VirtualKeyCode.VK_B, VirtualKeyCode.VK_V, PROCESS_NAME_NOTEPAD);
            AddRemap(VirtualKeyCode.VK_V, VirtualKeyCode.VK_B, PROCESS_NAME_NOTEPAD);

            // KILL WASD
            // 
            // Remap movement block from this:
            // 
            // QWErtyuiop
            // ASDfghjkl;
            // 
            // to this:
            //
            // rQWEtyuiop
            // fASDghjkl;

            // WASD block will slide to right so this displaces R and F to make room
            AddRemap(VirtualKeyCode.VK_Q, VirtualKeyCode.VK_R, PROCESS_NAME_WASD);
            AddRemap(VirtualKeyCode.VK_A, VirtualKeyCode.VK_F, PROCESS_NAME_WASD);

            // Remap WASD movement block to ESDF (plus rotation keys)
            AddRemap(VirtualKeyCode.VK_W, VirtualKeyCode.VK_Q, PROCESS_NAME_WASD);
            AddRemap(VirtualKeyCode.VK_E, VirtualKeyCode.VK_W, PROCESS_NAME_WASD);
            AddRemap(VirtualKeyCode.VK_R, VirtualKeyCode.VK_E, PROCESS_NAME_WASD);
            AddRemap(VirtualKeyCode.VK_S, VirtualKeyCode.VK_A, PROCESS_NAME_WASD);
            AddRemap(VirtualKeyCode.VK_D, VirtualKeyCode.VK_S, PROCESS_NAME_WASD);
            AddRemap(VirtualKeyCode.VK_F, VirtualKeyCode.VK_D, PROCESS_NAME_WASD);
        }
    }
}

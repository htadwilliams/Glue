using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using log4net.Config;
using Newtonsoft.Json;
using SharpDX.DirectInput;
using WindowsInput.Native;
using static Glue.ActionKey;
using static Glue.Trigger;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace Glue
{
    static class GlueTube
    {
        public static Main MainForm => s_mainForm;
        public static Dictionary<Keys, Trigger> Triggers => s_triggers;
        public static Dictionary<VirtualKeyCode, KeyRemap> KeyMap => s_keyMap;
        public static Dictionary<String, Macro> Macros => s_macros;

        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const string FILENAME_DEFAULT               = "macros.json";
        private const string PROCESS_NAME_NOTEPAD           = "notepad";      // also matches Notepad++
        private const string PROCESS_NAME_WASD              = "fallout4.exe"; 

        private static Main s_mainForm = null;
        private static bool s_writeOnExit = false;      // Set if loading fails, or if GUI changes contents 

        private static Dictionary<Keys, Trigger> s_triggers = new Dictionary<Keys, Trigger>();
        private static Dictionary<VirtualKeyCode, KeyRemap> s_keyMap = new Dictionary<VirtualKeyCode, KeyRemap>();
        private static Dictionary<String, Macro> s_macros = new Dictionary<String, Macro>();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            String fileName = args.Length > 0
                ? args[0]
                : FILENAME_DEFAULT;

            if (Thread.CurrentThread.Name == null)
            {
                Thread.CurrentThread.Name = "Main";
            }

            KeyHandler.InitKeyTable();

            try
            {
                InitLogging();
                InitDirectX();
                LoadFile(fileName);

                // Native keyboard and mouse hook initialization
                KeyInterceptor.Initialize(KeyHandler.HookCallback);
                MouseInterceptor.Initialize(MouseHandler.HookCallback);

                // Starts thread for timed queue of actions such as pressing keys,
                // activating game controller buttons, playing sounds, etc.
                ActionQueue.Start();

                LOGGER.Debug("Entering Application.Run()...");
                InitForm();
                Application.Run(s_mainForm);
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
                SaveFile(fileName);
            }

            LOGGER.Info("Exiting");
        }

        internal static void PlayMacro(string macroName)
        {
            if (s_macros.TryGetValue(macroName, out Macro macro))
            {
                LOGGER.Debug("Playing macro [" + macroName + "]");
                macro.Play();
            }
            else
            {
                LOGGER.Warn("Macro not found: " + macroName);
            }
        }

        private static void InitForm()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            s_mainForm = new Main();
        }

        // TODO All the DirectX stuff should be moved to another wrapper class
        private static void InitDirectX()
        {
            // Using https://github.com/sharpdx/sharpdx
            // See quick example code 
            // https://stackoverflow.com/questions/3929764/taking-input-from-a-joystick-with-c-sharp-net
            LOGGER.Info("Enumerating DirectX Input devices...");

            try
            {
                DirectInput directInput = new DirectInput();

                foreach (DeviceType deviceType in Enum.GetValues(typeof(DeviceType)))
                {
                    IList<DeviceInstance> deviceInstances = directInput.GetDevices(deviceType, DeviceEnumerationFlags.AllDevices);
                    if (deviceInstances.Count > 0)
                    {
                        LOGGER.Info("    Devices of type [" + deviceType.ToString() + "]:");

                        foreach (DeviceInstance deviceInstance in deviceInstances)
                        {
                            String message = String.Format(
                                "        {0} : GUID=[{1}]",
                                deviceInstance.InstanceName,
                                deviceInstance.ProductGuid.ToString());
                            LOGGER.Info(message);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LOGGER.Error("Error enumerating DirectX devices: ", e);
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
            GlueTube.KeyMap.Add(keyOld, new KeyRemap(keyOld, keyNew, procName));
        }

        private static void SaveFile(String fileName)
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

                // Order of writes and reads to this file must be kept in sync
                // with LoadFile()
                writer.WriteComment(
                    "\r\nOrder of this file's contents must be maintained\r\n" + 
                    "    1) Macros\r\n" + 
                    "    2) Triggers\r\n" + 
                    "    3) Remapping\r\n");

                // TODO Serialize lists instead of maps where possible (build maps when deserializing)
                sw.Write("\r\n\r\n");
                writer.WriteComment("\r\nMacros\r\n");
                sw.Write("\r\n");
                serializer.Serialize(writer, s_macros);

                sw.Write("\r\n\r\n");
                writer.WriteComment("\r\nTriggers\r\n");
                sw.Write("\r\n");
                serializer.Serialize(writer, s_triggers);

                sw.Write("\r\n\r\n");
                writer.WriteComment(
                    "\r\nInput remapping\r\n\r\n" + 
                    "Partial process name matches are accepted - TODO added for regex support.\r\n\r\n" + 
                    "Process names must not include drive letters and backslash must be escaped e.g.\r\n" + 
                    "    \"C:\\Windows\\System32\\Notepad.exe\" -> \"\\\\Windows\\\\System32\\\\Notepad\"\r\n"
                );
                sw.Write("\r\n");
                serializer.Serialize(writer, s_keyMap);
            }
        }

        private static void LoadFile(String fileName)
        {
            LOGGER.Info("Loading file [" + fileName + "]");

            JsonSerializer serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            try
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    using (JsonReader reader = new JsonTextReader(sr))
                    {
                        reader.SupportMultipleContent = true;

                        reader.Read();
                        s_macros = serializer.Deserialize<Dictionary<String, Macro>>(reader);

                        reader.Read();
                        s_triggers = serializer.Deserialize<Dictionary<Keys, Trigger>>(reader);

                        reader.Read();
                        s_keyMap = serializer.Deserialize<Dictionary<VirtualKeyCode, KeyRemap>>(reader);
                    }
                }
            }
            catch (Exception e)
            {
                LOGGER.Error("Failed to load file with exception: " + e);

                CreateDefaultContent();
                s_writeOnExit = true;
                return;
            }

            s_writeOnExit = false;

            if (null != s_macros && s_macros.Count != 0)
            {
                LOGGER.Info(String.Format("    Loaded {0} macros", s_macros.Count));
            }
            if (null != s_triggers && s_triggers.Count != 0)
            {
                LOGGER.Info(String.Format("    Loaded {0} triggers", s_triggers.Count));
            }
            if (null != s_keyMap && s_keyMap.Count != 0)
            {
                LOGGER.Info(String.Format("    Loaded {0} remapped keys", s_keyMap.Count));
            }
        }

        private static void CreateDefaultContent()
        {
            LOGGER.Info("File not found or load failed - creating example content");

            s_triggers = new Dictionary<Keys, Trigger>();
            s_keyMap = new Dictionary<VirtualKeyCode, KeyRemap>();

            //
            // Create macro with several actions bound to CTRL-Z
            //
            String macroName = "delayed-action";
            Macro macro = new Macro(macroName, 10) // Fire 10ms after triggered
                .AddAction(new ActionKey(VirtualKeyCode.VK_R, Movement.PRESS, 10))
                .AddAction(new ActionKey(VirtualKeyCode.VK_R, Movement.RELEASE, 10))

                .AddAction(new ActionKey(VirtualKeyCode.RETURN, Movement.PRESS, 4000))
                .AddAction(new ActionKey(VirtualKeyCode.RETURN, Movement.RELEASE, 4010))

                .AddAction(new ActionKey(VirtualKeyCode.VK_Q, Movement.PRESS, 4020))
                .AddAction(new ActionKey(VirtualKeyCode.VK_Q, Movement.RELEASE, 4030))
                ;
            s_macros.Add(macroName, macro);
            // Setup trigger
            Trigger trigger = new Trigger(Keys.Z, macroName);
            trigger.AddModifier(Keys.LControlKey);
            Triggers.Add(trigger.TriggerKey, trigger);

            //
            // Create and bind a typing macro (string of text) bound to CTRL-C
            // 
            macroName = "typing-stuff";
            macro = new Macro(macroName, 2000);
            
            macro.AddAction(
                new ActionTyping(
                    // Thinking of Maud you forget everything else
                    // Latin for Maud is actually Matillis but nobody would ever figure it out
                    "Lorem ipsum dollar sit amet, Cogito Maud obliviscaris aliorum.",
                    10,     // delay MS
                    10));   // dwell time MS
            s_macros.Add(macroName, macro);
            trigger = new Trigger(Keys.C, macroName);
            trigger.AddModifier(Keys.LControlKey);
            Triggers.Add(trigger.TriggerKey, trigger);

            //
            // Create and bind a sound macro that alternates sounds
            //
            macroName = "sound-servomotor";
            macro = new Macro(macroName, 0);
            macro.AddAction(new ActionSound("sound_servomotor.wav"));
            s_macros.Add(macroName, macro);

            macroName = "sound-ahha";
            macro = new Macro(macroName, 0);
            macro.AddAction(new ActionSound("ahha.wav"));
            s_macros.Add(macroName, macro);

            trigger = new Trigger(Keys.S, new List<String> { "sound-servomotor", "sound-ahha" });
            trigger.AddModifier(Keys.LControlKey);
            Triggers.Add(trigger.TriggerKey, trigger);

            //
            // Macro bound to mouse button X1 / X2
            //
            macroName = "XF10";
            macro = new Macro(macroName, 0);
            macro.AddAction(new ActionKey(VirtualKeyCode.F10, Movement.PRESS, 0));
            macro.AddAction(new ActionKey(VirtualKeyCode.F10, Movement.RELEASE, 100));
            s_macros.Add(macroName, macro);

            // Same macro bound to two triggers            
            trigger = new Trigger(Keys.XButton1, macroName);
            Triggers.Add(trigger.TriggerKey, trigger);
            trigger = new Trigger(Keys.XButton2, macroName);
            Triggers.Add(trigger.TriggerKey, trigger);

            // 
            // Toggle - hold SPACE key every other time it is pressed 
            //
            macroName = "toggle-down";
            macro = new Macro(macroName, 0);
            macro.AddAction(new ActionKey(VirtualKeyCode.SPACE, Movement.PRESS, 0));
            s_macros.Add(macroName, macro);

            macroName = "toggle-up";
            macro = new Macro(macroName, 0);
            macro.AddAction(new ActionKey(VirtualKeyCode.SPACE, Movement.RELEASE, 0));
            s_macros.Add(macroName, macro);

            trigger = new Trigger(Keys.Space, new List<string> {"toggle-down", null, "toggle-up", null}, TriggerType.Both, true);
            // Triggers.Add(trigger.TriggerKey, trigger);

            // 
            // Create mouse movement
            //
            macroName = "mouse-nudge";
            macro = new Macro(macroName, 20);
            // macro.AddAction(new ActionMouse(ActionMouse.Movement.ABSOLUTE, 65535 / 2, 65535 / 2, 500));
            macro.AddAction(new ActionMouse(ActionMouse.Movement.RELATIVE, 1, 1, 500));
            s_macros.Add(macroName, macro);
            trigger = new Trigger(Keys.Left, macroName);
            trigger.AddModifier(Keys.LMenu);
            Triggers.Add(trigger.TriggerKey, trigger);

            // Sunless skies (and other Unity games) won't allow binding to shift key
            // Mapping A to Shift allows binding game functions to that instead.
            // AddRemap(new KeyRemap(VirtualKeyCode.LSHIFT, VirtualKeyCode.VK_A, "skies.exe"));

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

        public static bool CheckAndFireTriggers(int vkCode, Movement movement)
        {
            bool eatInput = false;

            // Triggers fire macros (and other things)
            if (GlueTube.Triggers != null && GlueTube.Triggers.TryGetValue((Keys) vkCode, out Trigger trigger))
            {
                switch (trigger.Type)
                {
                    case TriggerType.Both:
                        eatInput = trigger.Fire();
                    break;

                    case TriggerType.Down:
                    if (Movement.PRESS == movement)
                    {
                        eatInput = trigger.Fire();
                    }
                    break;

                    case TriggerType.Up:
                    if (Movement.RELEASE == movement)
                    {
                        eatInput = trigger.Fire();
                    }
                    break;
                }
            }

            return eatInput;
        }
    }
}

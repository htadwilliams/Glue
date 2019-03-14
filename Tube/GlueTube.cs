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

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace Glue
{
    static class GlueTube
    {
        public static Main MainForm { get => s_mainForm; }
        public static Dictionary<Keys, Trigger> Triggers => s_triggers;
        public static Dictionary<VirtualKeyCode, KeyRemap> KeyMap => s_keyMap;

        private const string FILENAME_DEFAULT = "macros.json";

        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static Main s_mainForm = null;

        // TODO App needs separate collection of macros (probably Dictionary)
        private static Dictionary<Keys, Trigger> s_triggers = new Dictionary<Keys, Trigger>();
        private static Dictionary<VirtualKeyCode, KeyRemap> s_keyMap = new Dictionary<VirtualKeyCode, KeyRemap>();

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
                LoadMacros(fileName);

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

            SaveMacros(fileName);
            LOGGER.Info("Exiting");
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
            LOGGER.Info("Enumerating DirectInput devices...");
            DirectInput directInput = new DirectInput();

            // TODO Huh? Get rid of hard-coded enumeration range with magic numbers, and this horrible for loop
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

        private static void AddRemap(KeyRemap keyRemap)
        {
            GlueTube.KeyMap.Add(keyRemap.KeyOld, keyRemap);
        }

        private static void SaveMacros(String macroFileName)
        {
            LOGGER.Info("Saving macros to [" + macroFileName + "]");
            JsonSerializer serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
            };

            using (StreamWriter sw = new StreamWriter(macroFileName))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;
                serializer.Serialize(writer, Triggers);
            }
        }

        private static void LoadMacros(String macroFileName)
        {
            // TODO remove hard-coded key remaps and read from JSON
            // Bind things to shift (really A) for sunless skies!
            AddRemap(new KeyRemap(VirtualKeyCode.LSHIFT, VirtualKeyCode.VK_A, "skies.exe"));

            // Evil evil swap for people typing into notepad!  Easy for quick functional test.
            AddRemap(new KeyRemap(VirtualKeyCode.VK_B, VirtualKeyCode.VK_V, "notepad.exe"));
            AddRemap(new KeyRemap(VirtualKeyCode.VK_V, VirtualKeyCode.VK_B, "notepad.exe"));

            // KILL WASD!!!
            AddRemap(new KeyRemap(VirtualKeyCode.VK_E, VirtualKeyCode.VK_W, "notepad.exe"));
            AddRemap(new KeyRemap(VirtualKeyCode.VK_S, VirtualKeyCode.VK_A, "notepad.exe"));
            AddRemap(new KeyRemap(VirtualKeyCode.VK_D, VirtualKeyCode.VK_S, "notepad.exe"));
            AddRemap(new KeyRemap(VirtualKeyCode.VK_F, VirtualKeyCode.VK_D, "notepad.exe"));

            // Slide keys over to make room for killing WASD
            AddRemap(new KeyRemap(VirtualKeyCode.VK_W, VirtualKeyCode.VK_E, "notepad.exe"));
            AddRemap(new KeyRemap(VirtualKeyCode.VK_A, VirtualKeyCode.VK_F, "notepad.exe"));

            LOGGER.Info("Loading macros from [" + macroFileName + "]");

            JsonSerializer serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            try
            {
                using (StreamReader sr = new StreamReader(macroFileName))
                {
                    using (JsonReader reader = new JsonTextReader(sr))
                    {
                        s_triggers = serializer.Deserialize<Dictionary<Keys, Trigger>>(reader);
                    }
                }
            }
            catch (Exception e)
            {
                LOGGER.Error("Failed to load macros with exception: " + e);
            }

            //
            // Create and bind a macro with delayed key presses
            //
            if (null != s_triggers && s_triggers.Count != 0)
            {
                LOGGER.Info(String.Format("Loaded file with {0} triggers!", s_triggers.Count));
            }

            else
            {
                LOGGER.Info("Macro file not found or load failed - creating example macros");

                s_triggers = new Dictionary<Keys, Trigger>();

                //
                // Create macro with several actions bound to CTRL-Z
                //
                Macro macro = new Macro(10) // Fire 10ms after triggered
                    .AddAction(new ActionKey(VirtualKeyCode.VK_R, ActionKey.Movement.PRESS, 10))
                    .AddAction(new ActionKey(VirtualKeyCode.VK_R, ActionKey.Movement.RELEASE, 10))

                    .AddAction(new ActionKey(VirtualKeyCode.RETURN, ActionKey.Movement.PRESS, 4000))
                    .AddAction(new ActionKey(VirtualKeyCode.RETURN, ActionKey.Movement.RELEASE, 4010))

                    .AddAction(new ActionKey(VirtualKeyCode.VK_Q, ActionKey.Movement.PRESS, 4020))
                    .AddAction(new ActionKey(VirtualKeyCode.VK_Q, ActionKey.Movement.RELEASE, 4030))
                    ;
                // Setup trigger
                Trigger trigger = new Trigger(Keys.Z, macro);
                trigger.AddModifier(Keys.LControlKey);
                Triggers.Add(trigger.TriggerKey, trigger);

                //
                // Create and bind a typing macro (string of text) bound to CTRL-C
                // 
                macro = new Macro(2000);
                macro.AddAction(
                    new ActionTyping(
                        "Lorem ipsum dolor sit amet, tincidunt eget elit vivamus consequat, mi ac urna.", 
                        10,     // delay MS
                        10));   // dwell time MS
                trigger = new Trigger(Keys.C, macro);
                trigger.AddModifier(Keys.LControlKey);
                Triggers.Add(trigger.TriggerKey, trigger);

                //
                // Create and bind a sound macro
                //
                macro = new Macro(0);
                macro.AddAction(new ActionSound("sound_servomotor.wav"));
                trigger = new Trigger(Keys.S, macro);
                trigger.AddModifier(Keys.LControlKey);
                Triggers.Add(trigger.TriggerKey, trigger);

                // For sunless skies
                //macro = new Macro(0)
                //    .AddAction(new ActionKey(VirtualKeyCode.VK_A, ActionKey.Movement.PRESS, 50))
                //    .AddAction(new ActionKey(VirtualKeyCode.VK_A, ActionKey.Movement.RELEASE, 50));
                //trigger = new Trigger(Keys.LShiftKey, macro);
                //Triggers.Add(trigger.TriggerKey, trigger);
            }
        }
    }
}

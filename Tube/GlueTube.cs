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
        public static Main MainForm { get => mainForm; }
        public static Dictionary<Keys, Trigger> Triggers => triggers;
        public static Dictionary<VirtualKeyCode, KeyRemap> KeyMap => keyMap;

        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static Main mainForm = null;
        private static Dictionary<Keys, Trigger> triggers = new Dictionary<Keys, Trigger>();
        private static Dictionary<VirtualKeyCode, KeyRemap> keyMap = new Dictionary<VirtualKeyCode, KeyRemap>();

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

            // This is how the log4net examples do it, and nobody has bothered
            // to update them to get rid of the warning
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

            if (LOGGER.IsDebugEnabled)
            {
                LOGGER.Debug("Arguments ...");
                foreach(string arg in args)
                {
                    LOGGER.Debug(arg);
                }
            }

            // Using https://github.com/sharpdx/sharpdx
            // See quick example code 
            // https://stackoverflow.com/questions/3929764/taking-input-from-a-joystick-with-c-sharp-net
            LOGGER.Info("Enumerating DirectInput devices...");
            DirectInput directInput = new DirectInput();

            // TODO Huh? Get rid of hard-coded enumeration range and this horrible for loop
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

            LoadMacros(args[0]);

            // TODO remove hard-coded key remap
            KeyRemap remap = new KeyRemap(VirtualKeyCode.LSHIFT, VirtualKeyCode.VK_A, "skies.exe");
            GlueTube.KeyMap.Add(remap.KeyOld, remap);

            // Native keyboard and mouse hook initialization
            KeyHandler.Initialize();
            KeyInterceptor.Initialize(KeyHandler.HookCallback);
            MouseInterceptor.Initialize();

            // Starts thread for queue of actions such as pressing keys,
            // pushing buttons, playing sounds, etc.
            ActionQueue.Start();

            mainForm = new Main();

            LOGGER.Debug("Entering Application.Run()...");
            Application.Run(mainForm);
            LOGGER.Debug("...returned from Application.Run().");

            // Native class de-initialization
            MouseInterceptor.Cleanup();
            KeyInterceptor.Cleanup();

            SaveMacros(args[0]);

            LOGGER.Info("Exiting");
        }

        private static void SaveMacros(String macroFileName)
        {
            LOGGER.Info("Saving macros to [" + macroFileName + "]");
            JsonSerializer serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                // TypeNameHandling = TypeNameHandling.All
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
                        triggers = serializer.Deserialize<Dictionary<Keys, Trigger>>(reader);
                    }
                }
            }
            catch (Exception e)
            {
                LOGGER.Error("Exception: " + e);
            }


            //
            // Create and bind a macro with delayed key presses
            //
            if (null != triggers && triggers.Count != 0)
            {
                LOGGER.Info(String.Format("Loaded file with {0} triggers!", triggers.Count));
            }

            else
            {
                LOGGER.Info("Macro file not found or load failed - creating example macros");

                triggers = new Dictionary<Keys, Trigger>();

                // Create macro with several actions
                Macro macro = new Macro(10) // Fire 10ms after triggered
                    .AddAction(new ActionKey(VirtualKeyCode.VK_R, ActionKey.Movement.PRESS, 10))
                    .AddAction(new ActionKey(VirtualKeyCode.VK_R, ActionKey.Movement.RELEASE, 10))

                    .AddAction(new ActionKey(VirtualKeyCode.RETURN, ActionKey.Movement.PRESS, 4000))
                    .AddAction(new ActionKey(VirtualKeyCode.RETURN, ActionKey.Movement.RELEASE, 4010))

                    .AddAction(new ActionKey(VirtualKeyCode.VK_Q, ActionKey.Movement.PRESS, 4020))
                    .AddAction(new ActionKey(VirtualKeyCode.VK_Q, ActionKey.Movement.RELEASE, 4030))
                    ;

                // Bind macro to trigger (Ctrl-Z and possibly other modifiers)
                Trigger trigger = new Trigger(Keys.Z, macro);
                trigger.AddModifier(Keys.LControlKey);
                trigger.AddModifier(Keys.S);
                trigger.AddModifier(Keys.LMenu);
                Triggers.Add(trigger.TriggerKey, trigger);

                //
                // Create and bind a typing macro (string of text)
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
            }
        }
    }
}

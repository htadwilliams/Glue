﻿using System;
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

        private const string FILENAME_DEFAULT           = "macros.json";
        private const string PROCESS_NAME_VBSWAP        = "notepad.exe";
        private const string PROCESS_NAME_KILLWASD      = "somegame.exe";

        // Magic numbers for DirectX device enumeration
        // TODO kill magic numbers for DirectX device enumeration
        private const int ID_DX_DEVICETYPE_BEGIN = 1;
        // private const int ID_DX_DEVICETYPE_BEGIN = 17;
        private const int ID_DX_DEVICETYPE_END = 28;

        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static Main s_mainForm = null;
        private static bool s_writeOnExit = false;      // Set if loading fails, or if GUI changes contents 

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

                foreach(DeviceType deviceType in Enum.GetValues(typeof(DeviceType)))
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
            catch(Exception e)
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

        private static void AddRemap(KeyRemap keyRemap)
        {
            GlueTube.KeyMap.Add(keyRemap.KeyOld, keyRemap);
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
                writer.WriteComment("Order of this file's contents must be maintained\r\n    1) Triggers\r\n    2) Input remapping");
                sw.Write("\r\n\r\n");

                writer.WriteComment("Triggers");
                sw.Write("\r\n");
                serializer.Serialize(writer, s_triggers);

                sw.Write("\r\n\r\n");
                writer.WriteComment("Input remapping");
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

            // 
            // Create mouse movement
            //

            macro = new Macro(20);
            // macro.AddAction(new ActionMouse(ActionMouse.Movement.ABSOLUTE, 65535 / 2, 65535 / 2, 500));
            macro.AddAction(new ActionMouse(ActionMouse.Movement.RELATIVE, 1, 1, 500));
            trigger = new Trigger(Keys.Left, macro);
            trigger.AddModifier(Keys.LMenu);
            Triggers.Add(trigger.TriggerKey, trigger);

            // Sunless skies (and other Unity games) won't allow binding to shift key
            // Mapping A to Shift allows binding game functions to that instead.
            AddRemap(new KeyRemap(VirtualKeyCode.LSHIFT, VirtualKeyCode.VK_A, "skies.exe"));

            // Evil evil swap for people typing into notepad!  Easy for quick functional test.
            AddRemap(new KeyRemap(VirtualKeyCode.VK_B, VirtualKeyCode.VK_V, PROCESS_NAME_VBSWAP));
            AddRemap(new KeyRemap(VirtualKeyCode.VK_V, VirtualKeyCode.VK_B, PROCESS_NAME_VBSWAP));

            // KILL WASD!!!
            AddRemap(new KeyRemap(VirtualKeyCode.VK_E, VirtualKeyCode.VK_W, PROCESS_NAME_KILLWASD));
            AddRemap(new KeyRemap(VirtualKeyCode.VK_S, VirtualKeyCode.VK_A, PROCESS_NAME_KILLWASD));
            AddRemap(new KeyRemap(VirtualKeyCode.VK_D, VirtualKeyCode.VK_S, PROCESS_NAME_KILLWASD));
            AddRemap(new KeyRemap(VirtualKeyCode.VK_F, VirtualKeyCode.VK_D, PROCESS_NAME_KILLWASD));

            // Slide keys over to make room for killing WASD
            AddRemap(new KeyRemap(VirtualKeyCode.VK_W, VirtualKeyCode.VK_E, PROCESS_NAME_KILLWASD));
            AddRemap(new KeyRemap(VirtualKeyCode.VK_A, VirtualKeyCode.VK_F, PROCESS_NAME_KILLWASD));
        }
    }
}

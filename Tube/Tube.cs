﻿using Glue.Actions;
using Glue.Forms;
using Glue.Native;
using log4net.Config;
using Newtonsoft.Json;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;
using static Glue.Actions.ActionKey;
using static Glue.Trigger;
using Keyboard = Glue.Native.Keyboard;

[assembly: XmlConfigurator(Watch = true)]

namespace Glue
{
    internal static class Tube
    {
        internal static Dictionary<Keys, Trigger> Triggers { get => s_triggers; set => s_triggers = value; }
        internal static Dictionary<VirtualKeyCode, KeyboardRemapEntry> KeyMap { get => s_keyMap; set => s_keyMap = value; }
        public static Dictionary<string, Macro> Macros { get => s_macros; set => s_macros = value; }
        public static ViewMain MainForm { get => s_mainForm; set => s_mainForm = value; }
        public static string FileName { get => s_fileName; set => s_fileName = value; }
        public static ActionQueueScheduler Scheduler { get => s_actionScheduler; }

        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // Core data structures
        private static Dictionary<Keys, Trigger> s_triggers;
        private static Dictionary<VirtualKeyCode, KeyboardRemapEntry> s_keyMap;

        private static Dictionary<string, Macro> s_macros;

        // For friendly display of keys in GUI
        private static readonly Dictionary<Keys, string> keyMap = new Dictionary<Keys, string>();

        // used to generate example content 
        private const string FILENAME_DEFAULT               = "macros.glue";
        private const string PROCESS_NAME_NOTEPAD           = "notepad";      // also matches Notepad++
        private const string PROCESS_NAME_WASD              = "fallout4.exe"; 

        private static ViewMain s_mainForm;
        private static bool s_writeOnExit = false;      // Set if loading fails, or if GUI changes contents 
        private static string s_fileName = FILENAME_DEFAULT;

        private static List<VirtualKeyCode> s_keysDown = new List<VirtualKeyCode>();
        private static bool s_lastLoggedOutputWasSpace = false;
        private static TrayApplicationContext s_context;
        private static ActionQueueScheduler s_actionScheduler = new ActionQueueScheduler();

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

            InitKeyTable();

            try
            {
                InitLogging();
                InitDirectX();
                LoadFile(FileName);

                // Native keyboard and mouse hook initialization
                KeyInterceptor.Initialize(KeyboardHandler.HookCallback);
                MouseInterceptor.Initialize(MouseHandler.HookCallback);

                // Starts thread for timed queue of actions such as pressing keys,
                // activating game controller buttons, playing sounds, etc.
                s_actionScheduler.Start();

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

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

        public static void InitKeyTable()
        {
            // Maps virtual key codes to friendly text for user display
            (Keys key, string text)[] keyTable =
            {
                // Key code             Friendly text
                (Keys.D0,               "0"),
                (Keys.D1,               "1"),
                (Keys.D2,               "2"),
                (Keys.D3,               "3"),
                (Keys.D4,               "4"),
                (Keys.D5,               "5"),
                (Keys.D6,               "6"),
                (Keys.D7,               "7"),
                (Keys.D8,               "8"),
                (Keys.D9,               "9"),
                (Keys.Oemcomma,         ","),
                (Keys.OemPeriod,        "."),
                (Keys.OemQuestion,      "?"),
                (Keys.Add,              "+"),
                (Keys.Space,            " "),
                (Keys.Return,           "\r\n"),
                (Keys.Oem1,             ";"),
                (Keys.OemOpenBrackets,  "["),
                (Keys.Oem5,             "\\"),
                (Keys.Oem6,             "]"),
                (Keys.Oem7,             "'"),
                (Keys.Back,             "←"),
            };
            foreach ((Keys key, string text) in keyTable)
            {
                keyMap.Add(key, text);
            }
        }

        internal static void PlayMacro(string macroName)
        {
            if (Macros.TryGetValue(macroName, out Macro macro))
            {
                LOGGER.Debug("Playing macro [" + macroName + "]");
                macro.Play();
            }
            else
            {
                LOGGER.Warn("Macro not found: " + macroName);
            }
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
                            string message = String.Format(
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

                JsonWrapper jsonWrapper = new JsonWrapper(Triggers, KeyMap, Macros);
                serializer.Serialize(writer, jsonWrapper);
            }
        }

        private static void InitData()
        {
            Triggers = new Dictionary<Keys, Trigger>();
            KeyMap = new Dictionary<VirtualKeyCode, KeyboardRemapEntry>();
            Macros = new Dictionary<string, Macro>();
        }

        public static void LoadFile(string fileName)
        {
            InitData();

            LOGGER.Info("Loading file [" + fileName + "]");

            if (File.Exists(fileName))
            {
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
                            Triggers = jsonWrapper.GetTriggerMap();
                            KeyMap = jsonWrapper.GetKeyboardMap();
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
                if (null != Triggers && Triggers.Count != 0)
                {
                    LOGGER.Info(String.Format("    Loaded {0} triggers", Triggers.Count));
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

        public static bool CheckAndFireTriggers(int vkCode, MoveType movement)
        {
            bool eatInput = false;

            // Triggers fire macros 
            if (Tube.Triggers != null && 
                Tube.Triggers.TryGetValue((Keys) vkCode, out Trigger trigger))
            {
                switch (trigger.Type)
                {
                    case TriggerType.Both:
                        eatInput = trigger.Fire();
                    break;

                    case TriggerType.Down:
                    if (MoveType.PRESS == movement)
                    {
                        eatInput = trigger.Fire();
                    }
                    break;

                    case TriggerType.Up:
                    if (MoveType.RELEASE == movement)
                    {
                        eatInput = trigger.Fire();
                    }
                    break;
                }
            }

            return eatInput;
        }

        public static void LogKeyDown(int vkCode)
        {
            if (!s_keysDown.Contains((VirtualKeyCode) vkCode))
            {
                s_keysDown.Add((VirtualKeyCode)vkCode);
                MainForm.UpdateKeys(s_keysDown);
            }

            if (MainForm.LogInput)
            {
                LOGGER.Debug("+" + (VirtualKeyCode)vkCode);

                string output;
                if (MainForm.RawKeyNames)
                {
                    output = "+" + (VirtualKeyCode)vkCode + " ";
                }
                else
                {
                    output = FormatKeyString(vkCode, s_lastLoggedOutputWasSpace);

                    // Set flag for next time this method is called
                    s_lastLoggedOutputWasSpace
                        = (output.EndsWith(" ") ||
                           output.EndsWith("\r\n"));
                }

                MainForm.AppendText(output);
            }
        }

        public static void LogKeyUp(int vkCode)
        {
            while (s_keysDown.Contains((VirtualKeyCode) vkCode))
            {
                s_keysDown.Remove((VirtualKeyCode) vkCode);
            }
            MainForm.UpdateKeys(s_keysDown);

            if (MainForm.LogInput)
            {
                LOGGER.Debug("-" + (VirtualKeyCode)vkCode);

                if (MainForm.RawKeyNames)
                {
                    MainForm.AppendText("-" + (VirtualKeyCode)vkCode + " ");
                }
            }
        }

        internal static void LogMouseMove(int xPos, int yPos)
        {
            MainForm.LogMouseMove(xPos, yPos);
        }

        internal static void LogMouseClick(MouseButton button, int xPos, int yPos)
        {
            MainForm.LogMouseClick(xPos, yPos);
        }

        private static string FormatKeyString(int vkCode, bool padLeft)
        {
            string output;
            if (keyMap.TryGetValue((Keys)vkCode, out string text))
            {
                output = text;
            }
            else
            {
                output = ((Keys)vkCode).ToString();

                // Only printed characters are a single character long 
                if (output.Length == 1)
                {
                    // Could be simplified but this is super clear to read
                    if (Keyboard.IsKeyToggled(Keys.CapsLock))
                    {
                        if (Keyboard.IsKeyDown(Keys.LShiftKey) || Keyboard.IsKeyDown(Keys.RShiftKey))
                        {
                            output = output.ToLower();
                        }
                    }
                    else
                    {
                        if (!Keyboard.IsKeyDown(Keys.LShiftKey) && !Keyboard.IsKeyDown(Keys.RShiftKey))
                        {
                            output = output.ToLower();
                        }
                    }
                }
            }

            // Pad Key names (e.g. LMenu, not single typed characters like "A")
            if ((output.Length > 1) && (output != "\r\n"))
            {
                if (!padLeft)
                {
                    output = output.Insert(0, " ");
                }
                output += " ";
            }

            return output;
        }

        public static bool CheckAndFireTriggers(int vkCode, MoveType movement)
        {
            bool eatInput = false;

            // Triggers fire macros 
            if (Tube.Triggers != null && 
                Tube.Triggers.TryGetValue((Keys) vkCode, out Trigger trigger))
            {
                switch (trigger.Type)
                {
                    case TriggerType.Both:
                        eatInput = trigger.Fire();
                    break;

                    case TriggerType.Down:
                    if (MoveType.PRESS == movement)
                    {
                        eatInput = trigger.Fire();
                    }
                    break;

                    case TriggerType.Up:
                    if (MoveType.RELEASE == movement)
                    {
                        eatInput = trigger.Fire();
                    }
                    break;
                }
            }

            return eatInput;
        }

        public static void OnKeyDown(int vkCode)
        {
            if (!s_keysDown.Contains((VirtualKeyCode) vkCode))
            {
                s_keysDown.Add((VirtualKeyCode)vkCode);
                MainForm.UpdateKeys(s_keysDown);
            }

            if (MainForm.LogInput)
            {
                LOGGER.Debug("+" + (VirtualKeyCode)vkCode);

                string output;
                if (MainForm.RawKeyNames)
                {
                    output = "+" + (VirtualKeyCode)vkCode + " ";
                }
                else
                {
                    output = FormatKeyString(vkCode);
                }

                MainForm.AppendText(output);
            }
        }

        public static void OnKeyUp(int vkCode)
        {
            while (s_keysDown.Contains((VirtualKeyCode) vkCode))
            {
                s_keysDown.Remove((VirtualKeyCode) vkCode);
            }
            MainForm.UpdateKeys(s_keysDown);

            if (MainForm.LogInput)
            {
                LOGGER.Debug("-" + (VirtualKeyCode)vkCode);

                if (MainForm.RawKeyNames)
                {
                    MainForm.AppendText("-" + (VirtualKeyCode)vkCode + " ");
                }
            }
        }

        private static string FormatKeyString(int vkCode)
        {
            string output;
            if (keyMap.TryGetValue((Keys)vkCode, out string text))
            {
                output = text;
            }
            else
            {
                output = ((Keys)vkCode).ToString();

                // Only printed characters are a single character long 
                if (output.Length == 1)
                {
                    // Could be simplified but this is super clear to read
                    if (Keyboard.IsKeyToggled(Keys.CapsLock))
                    {
                        if (Keyboard.IsKeyDown(Keys.LShiftKey) || Keyboard.IsKeyDown(Keys.RShiftKey))
                        {
                            output = output.ToLower();
                        }
                    }
                    else
                    {
                        if (!Keyboard.IsKeyDown(Keys.LShiftKey) && !Keyboard.IsKeyDown(Keys.RShiftKey))
                        {
                            output = output.ToLower();
                        }
                    }
                }
            }

            // Pad Key names (e.g. LMenu, not single typed characters like "A")
            if ((output.Length > 1) && (output != "\r\n"))
            {
                if (!s_lastInsertWasSpace)
                {
                    output = output.Insert(0, " ");
                }
                output += " ";
            }

            // Set flag for next time this method is called
            s_lastInsertWasSpace
                = (output.EndsWith(" ") ||
                   output.EndsWith("\r\n"));

            return output;
        }
        private static void CreateDefaultContent()
        {
            //
            // Create macro with several actions bound to CTRL-Z
            //
            string macroName = "delayed-action";
            Macro macro = new Macro(macroName, 4000) 
                .AddAction(new ActionSound(10, "sound_tab_retreat.wav"))
                .AddAction(new ActionKey(10,    VirtualKeyCode.RETURN,  MoveType.PRESS))
                .AddAction(new ActionKey(10,    VirtualKeyCode.RETURN,  MoveType.RELEASE))
                ;
            Macros.Add(macroName, macro);
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
                    "Lorem ipsum dollar sit amet, Cogito Maud obliviscaris aliorum.",
                    10,     // delay MS
                    10));   // dwell time MS
            Macros.Add(macroName, macro);
            trigger = new Trigger(Keys.C, macroName);
            trigger.AddModifier(Keys.LControlKey);
            Triggers.Add(trigger.TriggerKey, trigger);

            //
            // Create a trigger that alternates between macros that play sound events
            //
            macroName = "sound-servomotor";
            macro = new Macro(macroName, 0);
            macro.AddAction(new ActionSound(0, "sound_servomotor.wav"));
            Macros.Add(macroName, macro);

            macroName = "sound-ahha";
            macro = new Macro(macroName, 0);
            macro.AddAction(new ActionSound(0, "ahha.wav"));
            Macros.Add(macroName, macro);

            trigger = new Trigger(Keys.S, new List<string> { "sound-servomotor", "sound-ahha" });
            trigger.AddModifier(Keys.LControlKey);
            Triggers.Add(trigger.TriggerKey, trigger);

            //
            // Schedule repeating sound every N MS
            //

            // TODO Repeated sound should play when trigger fires (currently only plays after first delay interval)
            macroName = "sound-tock";
            macro = new Macro(macroName, 0);
            macro.AddAction(new ActionSound(0, "sound_click_tock.wav"));
            Macros.Add(macroName, macro);

            macroName = "repeat-sound";
            macro = new Macro(macroName, 0);
            macro.AddAction(new ActionRepeat(3000, macroName, "sound-tock" ));
            Macros.Add(macroName, macro);

            trigger = new Trigger(Keys.Oemcomma, "repeat-sound");
            trigger.AddModifier(Keys.LMenu);
            Triggers.Add(trigger.TriggerKey, trigger);

            macroName = "repeat-sound-cancel";
            macro = new Macro(macroName, 0);
            macro.AddAction(new ActionCancel("repeat-sound"));
            Macros.Add(macroName, macro);

            trigger = new Trigger(Keys.OemPeriod, "repeat-sound-cancel");
            trigger.AddModifier(Keys.LMenu);
            Triggers.Add(trigger.TriggerKey, trigger);

            //
            // Macro bound to mouse button X1 / X2
            //
            macroName = "F10";
            macro = new Macro(macroName, 0);
            macro.AddAction(new ActionKey(50, VirtualKeyCode.F10, MoveType.CLICK));
            Macros.Add(macroName, macro);

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
            macro.AddAction(new ActionKey(0, VirtualKeyCode.SPACE, MoveType.PRESS));
            Macros.Add(macroName, macro);

            macroName = "toggle-up";
            macro = new Macro(macroName, 0);
            macro.AddAction(new ActionKey(0, VirtualKeyCode.SPACE, MoveType.RELEASE));
            Macros.Add(macroName, macro);

            trigger = new Trigger(Keys.Space, new List<string> {"toggle-down", null, "toggle-up", null}, TriggerType.Both, true);
            trigger.AddModifier(Keys.LControlKey);
            Triggers.Add(trigger.TriggerKey, trigger);

            // 
            // Create mouse movement
            //
            macroName = "mouse-nudge-left";
            macro = new Macro(macroName, 20);
            macro.AddAction(
                new ActionMouse(
                    0, 
                    ActionMouse.CoordinateMode.RELATIVE,
                    -1, 0));
            Macros.Add(macroName, macro);
            trigger = new Trigger(Keys.Left, macroName);
            trigger.AddModifier(Keys.LMenu);
            Triggers.Add(trigger.TriggerKey, trigger);

            macroName = "mouse-center";
            macro = new Macro(macroName, 20);
            macro.AddAction(
                new ActionMouse(
                    0, 
                    ActionMouse.CoordinateMode.ABSOLUTE, 
                    65535 / 2, 65535 / 2));
            Macros.Add(macroName, macro);
            trigger = new Trigger(Keys.Home, macroName);
            trigger.AddModifier(Keys.LMenu);
            Triggers.Add(trigger.TriggerKey, trigger);

            macroName = "mouse-origin";
            macro = new Macro(macroName, 20);
            macro.AddAction(
                new ActionMouse(
                    0, 
                    ActionMouse.CoordinateMode.PIXEL, 
                    1, 1));
            Macros.Add(macroName, macro);
            trigger = new Trigger(Keys.End, macroName);
            trigger.AddModifier(Keys.LMenu);
            Triggers.Add(trigger.TriggerKey, trigger);

            macroName = "mouse-click-nomove";
            macro = new Macro(macroName, 20);
            macro.AddAction(
                new ActionMouse(
                    0,
                    ActionMouse.ClickType.CLICK,
                    MouseButton.LeftButton));
            Macros.Add(macroName, macro);
            trigger = new Trigger(Keys.Delete, macroName);
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
    }
}

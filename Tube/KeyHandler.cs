using System;
using System.Collections.Generic;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WindowsInput.Native;
using static Glue.KeyInterceptor;

namespace Glue
{
    static class KeyHandler
    {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static bool s_lastInsertWasSpace = false;

        private static readonly SoundPlayer PLAYER = new SoundPlayer();
        
        // For friendly display of keys in GUI
        private static readonly Dictionary<Keys, string> keyMap = new Dictionary<Keys, string>();

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

        public static IntPtr HookCallback(
            int nCode, IntPtr wParam, IntPtr lParam)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            KBDLLHOOKSTRUCT kbd = (KBDLLHOOKSTRUCT) Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));

            // TODO find a way to detect key repeats from held keys - check KBDLLHOOKSTRUCT
            if (wParam == (IntPtr) KeyInterceptor.WM_KEYDOWN || wParam == (IntPtr) KeyInterceptor.WM_SYSKEYDOWN)
            {
                // This could be used to ignore any injected keystrokes - may make it an option
                // if (!kbd.flags.HasFlag(KBDLLHOOKSTRUCTFlags.LLKHF_INJECTED))

                // Don't do remap for keys injected by our own process
                if (!KeyWasFromGlue(kbd.dwExtraInfo))
                {
                    VirtualKeyCode keyRemapped = DoRemap((VirtualKeyCode) vkCode, ActionKey.Movement.PRESS);

                    if ((int) keyRemapped != vkCode)
                    {
                        // Eat keystroke

                        // MSDN: if the hook procedure processed the message, it may return a nonzero value to prevent 
                        // the system from passing the message to the rest of the hook chain or the target window 
                        // procedure.
                        return new IntPtr(1);
                    }

                    if (GlueTube.CheckAndFireTriggers(vkCode, ActionKey.Movement.PRESS))
                    {
                        // Eat keystroke if trigger tells us to do so
                        return new IntPtr(1);
                    }
                }

                LogKeyDown(vkCode);
            }

            if (wParam == (IntPtr) KeyInterceptor.WM_KEYUP || wParam == (IntPtr) KeyInterceptor.WM_SYSKEYUP)
            {
                if (!KeyWasFromGlue(kbd.dwExtraInfo))
                {
                    VirtualKeyCode keyRemapped = DoRemap((VirtualKeyCode) vkCode, ActionKey.Movement.RELEASE);

                    if ((int) keyRemapped != vkCode)
                    {
                        // Eat keystroke
                        return new IntPtr(1);
                    }

                    if (GlueTube.CheckAndFireTriggers(vkCode, ActionKey.Movement.RELEASE))
                    {
                        // Eat keystroke if trigger tells us to do so
                        return new IntPtr(1);
                    }
                }

                LogKeyUp(vkCode);
            }

            return new IntPtr(0);
        }

        private static bool KeyWasFromGlue(UIntPtr injectionId)
        {
            return injectionId.ToUInt32() == ActionKey.INJECTION_ID.ToInt32();
        }

        private static void LogKeyUp(int vkCode)
        {
            if (GlueTube.MainForm.LogInput)
            {
                LOGGER.Debug("-" + (VirtualKeyCode) vkCode);

                if (GlueTube.MainForm.RawKeyNames)
                {
                    GlueTube.MainForm.AppendText("-" + (VirtualKeyCode) vkCode + " ");
                }
            }
        }

        private static void LogKeyDown(int vkCode)
        {
            if (GlueTube.MainForm.LogInput)
            {
                LOGGER.Debug("+" + (VirtualKeyCode) vkCode);

                string output;
                if (GlueTube.MainForm.RawKeyNames)
                {
                    output = "+" + (VirtualKeyCode) vkCode + " ";
                }
                else
                {
                    output = FormatKeyString(vkCode);
                }

                GlueTube.MainForm.AppendText(output);
            }
        }

        private static VirtualKeyCode DoRemap(VirtualKeyCode inputKey, ActionKey.Movement movement)
        {
            if (GlueTube.KeyMap != null && GlueTube.KeyMap.TryGetValue(inputKey, out KeyMapEntry remap))
            {
                // Filter remapping to the given process name 
                // If empty process name is given, perform remap for all of them
                string inputFocusProcessName = "";
                if (remap.ProcessName != null && remap.ProcessName.Length != 0)
                {
                    inputFocusProcessName = ProcessInfo.GetProcessFileName(
                        ProcessInfo.GetInputFocusProcessId());

                    LOGGER.Debug(
                        "DoRemap inputKey = [" + inputKey 
                        + "] focus window = [" + inputFocusProcessName 
                        + "] remap process = [" + remap.ProcessName + "]");

                    // TODO process name filtering should be regex not .Contains()
                    if (!inputFocusProcessName
                        .ToLower()
                        .Contains(remap.ProcessName.ToLower())
                        )
                    {
                        return inputKey;
                    }
                }

                LOGGER.Debug("REMAPPED: " + inputKey + " -> " + remap.KeyNew);
                ActionKey actionKey = new ActionKey(remap.KeyNew, movement, ActionQueue.Now());
                actionKey.Play();

                return remap.KeyNew;
            }

            return inputKey;
        }

        private static string FormatKeyString(int vkCode)
        {
            string output;
            if (keyMap.TryGetValue((Keys) vkCode, out string text))
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
    }
}

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WindowsInput.Native;

namespace Glue
{
    static class KeyHandler
    {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static bool lastInsertWasSpace = false;

        // For friendly display of keys in GUI
        private static readonly Dictionary<Keys, String> keyMap = new Dictionary<Keys, String>();

        // TODO is there a better way to do static initialization for this class?
        public static void Initialize()
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

            // TODO find a way to detect key repeats from held keys - flag not avail in LL hook
            if (wParam == (IntPtr) KeyInterceptor.WM_KEYDOWN || wParam == (IntPtr) KeyInterceptor.WM_SYSKEYDOWN)
            {
                LogKeyDown(vkCode);

                VirtualKeyCode keyRemapped = DoRemap((VirtualKeyCode) vkCode, ActionKey.Movement.PRESS);
                if ((int) keyRemapped != vkCode)
                {
                    // MSDN: if the hook procedure processed the message, it may return a nonzero value to prevent 
                    // the system from passing the message to the rest of the hook chain or the target window 
                    // procedure.
                    return new IntPtr(1);
                }

                CheckAndFireTriggers(vkCode);
            }

            if (wParam == (IntPtr) KeyInterceptor.WM_KEYUP || wParam == (IntPtr) KeyInterceptor.WM_SYSKEYUP)
            {
                LogKeyUp(vkCode);

                VirtualKeyCode keyRemapped = DoRemap((VirtualKeyCode) vkCode, ActionKey.Movement.RELEASE);
                if ((int) keyRemapped != vkCode)
                {
                    return new IntPtr(1);
                }
            }

            return new IntPtr(0);
        }

        private static void LogKeyUp(int vkCode)
        {
            if (GlueTube.MainForm.LogInput)
            {
                LOGGER.Debug("- " + (VirtualKeyCode) vkCode);
            }
        }

        private static void LogKeyDown(int vkCode)
        {
            if (GlueTube.MainForm.LogInput)
            {
                LOGGER.Debug("+ " + (VirtualKeyCode) vkCode);

                String output;
                if (GlueTube.MainForm.RawKeyNames)
                {
                    output = ((VirtualKeyCode) vkCode).ToString() + " ";
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
            if (GlueTube.KeyMap.TryGetValue(inputKey, out KeyRemap remap))
            {
                // Filter remapping to the given process name 
                // If empty process name is given, perform remap for all of them
                String inputFocusProcessName = "";
                if (remap.ProcName.Length != 0)
                {
                    inputFocusProcessName = ProcessInfo.GetProcessFileName(
                        ProcessInfo.GetInputFocusProcessId());

                    if (!inputFocusProcessName
                        .ToLower()
                        .Contains(remap.ProcName.ToLower())
                        )
                    {
                        return inputKey;
                    }
                }

                ActionKey actionKey = new ActionKey(remap.KeyNew, movement, ActionQueue.Now());
                actionKey.Play();

                return remap.KeyNew;
            }

            return inputKey;
        }

        private static void CheckAndFireTriggers(int vkCode)
        {
            // Triggers fire macros (and other things)
            if (GlueTube.Triggers.TryGetValue((Keys) vkCode, out Trigger trigger))
            {
                if (trigger.AreModKeysActive())
                {
                    if (LOGGER.IsDebugEnabled)
                    {
                        int inputProcessId = ProcessInfo.GetInputFocusProcessId();
                        LOGGER.Debug("Input process Id: " + inputProcessId);
                        LOGGER.Debug("Input process name" + ProcessInfo.GetProcessFileName(inputProcessId));
                    }

                    trigger.Fire();
                }
            }
        }

        private static string FormatKeyString(int vkCode)
        {
            string output;
            if (keyMap.TryGetValue((Keys)vkCode, out String text))
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
                        if (Keyboard.IsKeyDown(Keys.LShiftKey))
                        {
                            output = output.ToLower();
                        }
                    }
                    else
                    {
                        if (!Keyboard.IsKeyDown(Keys.LShiftKey))
                        {
                            output = output.ToLower();
                        }
                    }
                }
            }

            // Pad Key names (not single typed characters
            // e.g. LMenu)
            if ((output.Length > 1) && (output != "\r\n"))
            {
                if (!lastInsertWasSpace)
                {
                    output = output.Insert(0, " ");
                }
                output += " ";
            }

            // TODO conditional should be set of white space 
            // rather than just space
            lastInsertWasSpace
                = (output.EndsWith(" ") ||
                   output.EndsWith("\r\n"));

            return output;
        }
    }
}

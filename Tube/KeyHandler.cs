using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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

        // Using enum from windows forms
        private static readonly Dictionary<Keys, Trigger> triggers = new Dictionary<Keys, Trigger>();

        // TODO is there a better way to do static initialization for this class?
        public static void Initialize()
        {
            //
            // TODO remove trigger and macro building test code
            //

            // Define macro
            Macro macro = new Macro(10) // Fire 10ms after triggered
                .AddAction(new ActionKey(VirtualKeyCode.VK_R, ActionKey.Type.PRESS, 10))
                .AddAction(new ActionKey(VirtualKeyCode.VK_R, ActionKey.Type.RELEASE, 10))

                .AddAction(new ActionKey(VirtualKeyCode.RETURN, ActionKey.Type.PRESS, 4000))
                .AddAction(new ActionKey(VirtualKeyCode.RETURN, ActionKey.Type.RELEASE, 4010))

                .AddAction(new ActionKey(VirtualKeyCode.VK_Q, ActionKey.Type.PRESS, 4020))
                .AddAction(new ActionKey(VirtualKeyCode.VK_Q, ActionKey.Type.RELEASE, 4030))
                ;

            // Bind macro to trigger (Ctrl-Z and possibly other modifiers)
            Trigger trigger = new Trigger(Keys.Z, macro);
            trigger.AddModifier(Keys.LControlKey);
            // trigger.AddModifier(Keys.S);
            // trigger.AddModifier(Keys.LMenu);
            triggers.Add(trigger.TriggerKey, trigger);

            macro = new Macro(2000);
            macro.AddAction(new ActionTyping("Type this in your pipe and smoke it!", 10, 10));

            trigger = new Trigger(Keys.C, macro);
            trigger.AddModifier(Keys.LControlKey);
            triggers.Add(trigger.TriggerKey, trigger);

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
                CheckAndFireTriggers(vkCode);
            }

            if (wParam == (IntPtr) KeyInterceptor.WM_KEYUP || wParam == (IntPtr) KeyInterceptor.WM_SYSKEYUP)
            {
                LogKeyUp(vkCode);
            }

            // MSDN: if the hook procedure processed the message, it may return a nonzero value to prevent 
            // the system from passing the message to the rest of the hook chain or the target window 
            // procedure.                     
            if (DoRemap(vkCode))        // Do this after logging if you want to see remapped keys as typed
            {
                // Eat keystroke if we're remapping it (it's been sent to output queue as another key)
                return new IntPtr(1);
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

        private static bool DoRemap(int vkCode)
        {
            // This code eats a keystroke and will be useful for key remapping
            if (vkCode == (int) VirtualKeyCode.VK_Z)
            {
                LOGGER.Debug("EATING Z!!");

                return true;
            }

            return false;
        }

        private static void CheckAndFireTriggers(int vkCode)
        {
            // Triggers fire macros (and other things)
            if (triggers.TryGetValue((Keys) vkCode, out Trigger trigger))
            {
                if (trigger.AreModKeysActive())
                {
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

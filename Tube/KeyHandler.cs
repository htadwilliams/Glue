using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Glue
{
    static class KeyHandler
    {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static bool lastInsertWasSpace = false;
        private static readonly Dictionary<Keys, String> keyMap = new Dictionary<Keys, String>();

        // TODO Move triggers to trigger manager
        private static readonly Dictionary<Keys, Trigger> triggers = new Dictionary<Keys, Trigger>();

        // TODO is there a better way to do static initialization for this class?
        public static void Initialize()
        {

            // TODO remove trigger and macro building test code
            Macro macro = new Macro(10);    // Fire this macro 10ms after trigger

            macro = macro.AddAction(new Action(ActionTypes.KEYBOARD_PRESS, 0, Keys.Z))
                         .AddAction(new Action(ActionTypes.KEYBOARD_RELEASE, 100, Keys.Z));

            // Bind macro to trigger Ctrl-C
            Trigger trigger = new Trigger(Keys.C, macro);
            trigger.AddModifier(Keys.LControlKey);
            // trigger.AddModifier(Keys.S);
            // trigger.AddModifier(Keys.LMenu);
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

            if (GlueTube.MainForm.LogInput)
            {
                if (wParam == (IntPtr)KeyInterceptor.WM_KEYDOWN || wParam == (IntPtr)KeyInterceptor.WM_SYSKEYDOWN)
                {
                    LOGGER.Debug("+ " + (Keys)vkCode);

                    Main mainForm = GlueTube.MainForm;
                    String output = FormatKeyString(vkCode);

                    mainForm.AppendText(output);
                }
            }

            // TODO Move test trigger code 
            if (triggers.TryGetValue((Keys)vkCode, out Trigger trigger))
            {
                if (trigger.AreModKeysActive())
                {
                    trigger.Fire();
                }
            }

            // This code eats a keystroke and may be useful for key remapping
            if (vkCode == (int)Keys.Z)
            {
                LOGGER.Debug("EATING Z!!");

                // MSDN: if the hook procedure processed the message, it 
                // may return a nonzero value to prevent the system from 
                // passing the message to the rest of the hook chain or the 
                // target window procedure.                     
                return new IntPtr(1);
            }

            if (GlueTube.MainForm.LogInput)
            {
                if (wParam == (IntPtr)KeyInterceptor.WM_KEYUP || wParam == (IntPtr)KeyInterceptor.WM_SYSKEYUP)
                {
                    LOGGER.Debug("- " + (Keys)vkCode);
                }
            }

            return new IntPtr(0);
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
                if (output.Length == 1)
                {
                    output = output.ToLower();
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

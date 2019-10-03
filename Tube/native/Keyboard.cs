using System;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace Glue.Native
{
    public abstract class Keyboard
    {
        private static KeyStates GetKeyStates(int virtualKeyCode)
        {
            KeyStates state = KeyStates.None;
            short retVal = GetKeyState(virtualKeyCode);

            // If the high-order bit is 1, the key is down
            // otherwise, it is up.
            if ((retVal & 0x8000) == 0x8000)
            {
                state |= KeyStates.Down;
            }

            // If the low-order bit is 1, the key is toggled.
            if ((retVal & 1) == 1)
            { 
                state |= KeyStates.Toggled;
            }

            return state;
        }

        public static bool IsKeyDown(int virtualKeyCode)
        {
            return KeyStates.Down == (GetKeyStates(virtualKeyCode) & KeyStates.Down);
        }

        public static bool IsKeyToggled(int virtualKeyCode)
        {
            return KeyStates.Toggled == (GetKeyStates(virtualKeyCode) & KeyStates.Toggled);
        }

        #region Win API Functions and Constants

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern short GetKeyState(int keyCode);

        #endregion
    }
}
using Glue.Actions;
using System;
using System.Runtime.InteropServices;
using WindowsInput.Native;
using static Glue.Native.MouseInterceptor;

namespace Glue
{
    class MouseHandler
    {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private enum XBUTTONS
        {
            XBUTTON1 = 0x10000,
            XBUTTON2 = 0x20000
        };

        public static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                MouseMessages mouseMessage = (MouseMessages) wParam;
                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT) Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));

                // Translate mouse button into key code and action (press / release)
                VirtualKeyCode keyCode = 0x00;
                ActionKey.Movement triggerMovement = ActionKey.Movement.PRESS;
                switch (mouseMessage)
                {
                    case MouseMessages.WM_XBUTTONDOWN:
                        keyCode = GetXButton(hookStruct);
                    break;

                    case MouseMessages.WM_LBUTTONDOWN:
                        keyCode = VirtualKeyCode.LBUTTON;
                    break;

                    case MouseMessages.WM_RBUTTONDOWN:
                        keyCode = VirtualKeyCode.RBUTTON;
                    break;

                    case MouseMessages.WM_XBUTTONUP:
                        keyCode=GetXButton(hookStruct);
                        triggerMovement = ActionKey.Movement.RELEASE;
                    break;

                    case MouseMessages.WM_LBUTTONUP:
                        keyCode = VirtualKeyCode.LBUTTON;
                        triggerMovement = ActionKey.Movement.RELEASE;
                    break;

                    case MouseMessages.WM_RBUTTONUP:
                        keyCode = VirtualKeyCode.RBUTTON;
                        triggerMovement = ActionKey.Movement.RELEASE;
                    break;
                }

                // Fire trigger the same way keyboard input does
                if (Tube.CheckAndFireTriggers((int) keyCode, triggerMovement))
                {
                    // Eat keystroke if trigger tells us to do so
                    return new IntPtr(1);
                }
            }

            // TODO implement mouse lock "safety" feature 
            // Freeze the mouse 
            // return new IntPtr(1);

            return new IntPtr(0);
        }

        private static VirtualKeyCode GetXButton(MSLLHOOKSTRUCT hookStruct)
        {
            VirtualKeyCode keyCode;
            XBUTTONS xButton = (XBUTTONS) hookStruct.mouseData;

            if (xButton == XBUTTONS.XBUTTON1)
            {
                keyCode = VirtualKeyCode.XBUTTON1;
            }
            else
            {
                keyCode = VirtualKeyCode.XBUTTON2;
            }

            return keyCode;
        }
    }
}

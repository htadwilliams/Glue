using System;
using System.Runtime.InteropServices;
using WindowsInput.Native;
using static Glue.MouseInterceptor;

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

        public static IntPtr HookCallback(
            int nCode, IntPtr wParam, IntPtr lParam)
        {
//            if (nCode >= 0 &&
//                MouseMessages.WM_LBUTTONDOWN == (MouseMessages)wParam)
            if (nCode >= 0 && MouseMessages.WM_MOUSEMOVE != (MouseMessages) wParam)
            {
                MouseMessages mouseMessage = (MouseMessages) wParam;
                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT) Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                VirtualKeyCode keyCode = 0x00;
                ActionKey.Movement triggerMovement = ActionKey.Movement.PRESS;

                switch (mouseMessage)
                {
                    case MouseMessages.WM_XBUTTONDOWN:
                        keyCode=GetXButton(hookStruct);
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

                if (GlueTube.CheckAndFireTriggers((int) keyCode, triggerMovement))
                {
                    // Eat keystroke if trigger tells us to do so
                    return new IntPtr(1);
                }

                LOGGER.Info(mouseMessage.ToString() + " " + hookStruct.pt.x + ", " + hookStruct.pt.y);
            }

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

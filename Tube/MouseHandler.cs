using Glue.Events;
using Glue.Native;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
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
                VirtualKeyCode keyCode;
                MouseButtons mouseButton;
                ButtonStates buttonState = ButtonStates.Press;

                // Switch could be refactored into a lookup table to make this cleaner
                // Translate mouse messages into something Glue can use
                switch (mouseMessage)
                {
                    case MouseMessages.WM_LBUTTONDBLCLK:
                    case MouseMessages.WM_LBUTTONDOWN:
                        keyCode = VirtualKeyCode.LBUTTON;
                        mouseButton = MouseButtons.Left;

                        LOGGER.Info(String.Format("Click at ({0}, {1})", hookStruct.pt.x, hookStruct.pt.y));
                    break;

                    case MouseMessages.WM_LBUTTONUP:
                        keyCode = VirtualKeyCode.LBUTTON;
                        mouseButton = MouseButtons.Left;
                        buttonState = ButtonStates.Release;
                    break;

                    case MouseMessages.WM_RBUTTONDOWN:
                    case MouseMessages.WM_RBUTTONDBLCLK:
                        keyCode = VirtualKeyCode.RBUTTON;
                        mouseButton = MouseButtons.Right;
                    break;

                    case MouseMessages.WM_RBUTTONUP:
                        keyCode = VirtualKeyCode.RBUTTON;
                        mouseButton = MouseButtons.Right;
                        buttonState = ButtonStates.Release;
                    break;

                    case MouseMessages.WM_MBUTTONDOWN:
                    case MouseMessages.WM_MBUTTONDBLCLK:
                        keyCode = VirtualKeyCode.MBUTTON;
                        mouseButton = MouseButtons.Middle;
                        buttonState = ButtonStates.Press;
                    break;

                    case MouseMessages.WM_MBUTTONUP:
                        keyCode = VirtualKeyCode.MBUTTON;
                        mouseButton = MouseButtons.Middle;
                        buttonState = ButtonStates.Release;
                    break;

                    case MouseMessages.WM_XBUTTONDOWN:
                    case MouseMessages.WM_XBUTTONDBLCLK:
                        keyCode = GetXButtonKeyCode(hookStruct);
                        mouseButton = GetXButtonMouse(hookStruct);
                    break;

                    case MouseMessages.WM_XBUTTONUP:
                        keyCode=GetXButtonKeyCode(hookStruct);
                        mouseButton = GetXButtonMouse(hookStruct);
                        buttonState = ButtonStates.Release;
                    break;

                    case MouseMessages.WM_MOUSEWHEEL:
                        // Not processing mouse wheel yet
                        return new IntPtr(0);

                    case MouseMessages.WM_MOUSEMOVE:
                    default:
                        keyCode = 0;
                        mouseButton = MouseButtons.None;
                        buttonState = ButtonStates.Release;
                    break;
                }

                // Mouse buttons are handled like keyboard keys
                if (mouseMessage != MouseMessages.WM_MOUSEMOVE)
                {
                    EventBus<EventKeyboard>.Instance.SendEvent(
                        null, 
                        new EventKeyboard((int) keyCode, buttonState));

                    if (Tube.TriggerManager.OnKeyboard((int) keyCode, buttonState))
                    {
                        // Eat mouse message if trigger says to do so
                        return new IntPtr(1);
                    }
                }

                // All mouse events go on the mouse bus 
                EventBus<EventMouse>.Instance.SendEvent(
                    null, 
                    new EventMouse(mouseButton, buttonState, hookStruct.pt.x, hookStruct.pt.y));
            }

            // Freeze the mouse if mouse safety is toggled
            if (Tube.MouseLocked)
            {
                return new IntPtr(1);
            }
            
            return new IntPtr(0);
        }

        private static VirtualKeyCode GetXButtonKeyCode(MSLLHOOKSTRUCT hookStruct)
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

        private static MouseButtons GetXButtonMouse(MSLLHOOKSTRUCT hookStruct)
        {
            MouseButtons mouseButton;
            XBUTTONS xButton = (XBUTTONS) hookStruct.mouseData;

            if (xButton == XBUTTONS.XBUTTON1)
            {
                mouseButton = MouseButtons.XButton1;
            }
            else
            {
                mouseButton = MouseButtons.XButton2;
            }

            return mouseButton;
        }
    }
}

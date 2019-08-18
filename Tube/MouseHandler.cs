﻿using Glue.Actions;
using System;
using System.Runtime.InteropServices;
using WindowsInput;
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
                ActionKey.MoveType triggerMovement = ActionKey.MoveType.PRESS;
                switch (mouseMessage)
                {
                    case MouseMessages.WM_XBUTTONDOWN:
                        keyCode = GetXButton(hookStruct);
                    break;

                    case MouseMessages.WM_LBUTTONDOWN:
                        LOGGER.Info(String.Format("Click at ({0}, {1})", hookStruct.pt.x, hookStruct.pt.y));
                        Tube.OnMouseClick(MouseButton.LeftButton, hookStruct.pt.x, hookStruct.pt.y);
                        keyCode = VirtualKeyCode.LBUTTON;
                    break;

                    case MouseMessages.WM_RBUTTONDOWN:
                        keyCode = VirtualKeyCode.RBUTTON;
                    break;

                    case MouseMessages.WM_XBUTTONUP:
                        keyCode=GetXButton(hookStruct);
                        triggerMovement = ActionKey.MoveType.RELEASE;
                    break;

                    case MouseMessages.WM_LBUTTONUP:
                        keyCode = VirtualKeyCode.LBUTTON;
                        triggerMovement = ActionKey.MoveType.RELEASE;
                    break;

                    case MouseMessages.WM_RBUTTONUP:
                        keyCode = VirtualKeyCode.RBUTTON;
                        triggerMovement = ActionKey.MoveType.RELEASE;
                    break;

                    case MouseMessages.WM_MOUSEMOVE:
                    default:
                        // TODO Spaghetti control! Add pub / sub for mouse and keyboard handlers
                        // This already exists for the hook interceptors, but the pattern should be
                        // extended for app level handlers too.
                        Tube.OnMouseMove(hookStruct.pt.x, hookStruct.pt.y);
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

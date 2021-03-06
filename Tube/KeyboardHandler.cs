﻿using Glue.Actions;
using Glue.Events;
using Glue.Native;
using NerfDX.Events;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using WindowsInput.Native;
using static Glue.Native.KeyInterceptor;

namespace Glue
{
    /// <summary>
    /// 
    /// Processes low level keyboard hook messages via KeyInterceptor.
    /// 
    /// </summary>
    /// 
    /// <remarks>
    /// 
    /// MSDN: If the hook procedure processed the message, it may return a 
    /// nonzero value to prevent the system from passing the message to the 
    /// rest of the hook chain or the target window procedure.
    /// 
    /// </remarks>

    static class KeyboardHandler
    {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static uint s_lastExtraInfo = UInt32.MaxValue;

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

                // Don't do remap for keys injected by Glue
                if (!KeyWasFromGlue(kbd.dwExtraInfo))
                {
                    VirtualKeyCode keyRemapped = DoRemap((VirtualKeyCode) vkCode, ButtonStates.Press);

                    if ((int) keyRemapped != vkCode)
                    {
                        // Eat keystroke if remapped
                        return new IntPtr(1);
                    }
                }
                else if (LOGGER.IsDebugEnabled)
                {
                    LOGGER.Debug("Not remapping key already injected from Glue: " + ((VirtualKeyCode) vkCode).ToString());
                }

                if (Properties.Settings.Default.LogInput)
                {
                    LOGGER.Info("+" + (VirtualKeyCode) vkCode);
                }

                if (BroadcastKeyboardEvent(vkCode, ButtonStates.Press))
                {
                    // Eat keystroke if any event handlers say so
                    return new IntPtr(1);
                }
            }

            if (wParam == (IntPtr) KeyInterceptor.WM_KEYUP || wParam == (IntPtr) KeyInterceptor.WM_SYSKEYUP)
            {
                if (LOGGER.IsDebugEnabled)
                {
                    // Only logging when extra info changes to reduce debug log spam
                    uint extraInfo = kbd.dwExtraInfo.ToUInt32();
                    if (s_lastExtraInfo != extraInfo)
                    {
                        LOGGER.Debug(String.Format("dwExtraInfo changed: {0:X} -> {1:X}", s_lastExtraInfo, extraInfo));
                        s_lastExtraInfo = extraInfo;
                    }
                }

                if (!KeyWasFromGlue(kbd.dwExtraInfo))
                {
                    VirtualKeyCode keyRemapped = DoRemap((VirtualKeyCode) vkCode, ButtonStates.Release);

                    if ((int) keyRemapped != vkCode)
                    {
                        // Eat original keystroke if remapped
                        return new IntPtr(1);
                    }
                }

                if (Properties.Settings.Default.LogInput)
                {
                    LOGGER.Info("-" + (VirtualKeyCode)vkCode);
                }

                if (BroadcastKeyboardEvent(vkCode, ButtonStates.Release))
                {
                    // Eat keystroke if trigger says so
                    return new IntPtr(1);
                }
            }

            return new IntPtr(0);
        }

        public static bool BroadcastKeyboardEvent(int vkCode, ButtonStates buttonState)
        {
            EventKeyboard broadcastEvent = new EventKeyboard(vkCode, buttonState);

            EventBus<EventKeyboard>.Instance.SendEvent(null, broadcastEvent);
            List<bool> eatInputResults = ReturningEventBus<EventKeyboard, bool>.Instance.SendEvent(null, broadcastEvent);

            if (null != eatInputResults)
            {
                return eatInputResults.Contains(true);
            }
            else return false;
        }

        private static bool KeyWasFromGlue(UIntPtr injectionId)
        {
            return injectionId.ToUInt32() == ActionKey.INJECTION_ID.ToInt32();
        }

        private static VirtualKeyCode DoRemap(VirtualKeyCode inputKey, ButtonStates movement)
        {
            if (Tube.KeyMap != null && Tube.KeyMap.TryGetValue(inputKey, out KeyboardRemapEntry remap))
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

                    if (!inputFocusProcessName
                        .ToLower()
                        .Contains(remap.ProcessName.ToLower())
                        )
                    {
                        return inputKey;
                    }
                }

                LOGGER.Debug("REMAPPED: " + inputKey + " -> " + remap.KeyCodeNew);
                ActionKey actionKey = new ActionKey(TimeProvider.GetTickCount(), remap.KeyCodeNew, movement);
                actionKey.Play();

                return remap.KeyCodeNew;
            }

            return inputKey;
        }
    }
}

using Glue.Actions;
using Glue.Events;
using Glue.Native;
using System;
using System.Runtime.InteropServices;
using WindowsInput.Native;
using static Glue.Native.KeyInterceptor;

namespace Glue
{
    static class KeyboardHandler
    {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
                    VirtualKeyCode keyRemapped = DoRemap((VirtualKeyCode) vkCode, ButtonStates.Press);

                    if ((int) keyRemapped != vkCode)
                    {
                        // Eat keystroke

                        // MSDN: if the hook procedure processed the message, it may return a nonzero value to prevent 
                        // the system from passing the message to the rest of the hook chain or the target window 
                        // procedure.
                        return new IntPtr(1);
                    }

                    if (Tube.TriggerManager.OnKeyboard(vkCode, ButtonStates.Press))
                    {
                        // Eat keystroke if trigger tells us to do so
                        return new IntPtr(1);
                    }
                }

                if (Properties.Settings.Default.LogInput)
                {
                    LOGGER.Info("+" + (VirtualKeyCode) vkCode);
                }

                EventBus<EventKeyboard>.Instance.SendEvent(null, new EventKeyboard(vkCode, ButtonStates.Press));
            }

            if (wParam == (IntPtr) KeyInterceptor.WM_KEYUP || wParam == (IntPtr) KeyInterceptor.WM_SYSKEYUP)
            {
                if (!KeyWasFromGlue(kbd.dwExtraInfo))
                {
                    VirtualKeyCode keyRemapped = DoRemap((VirtualKeyCode) vkCode, ButtonStates.Release);

                    if ((int) keyRemapped != vkCode)
                    {
                        // Eat keystroke
                        return new IntPtr(1);
                    }

                    if (Tube.TriggerManager.OnKeyboard(vkCode, ButtonStates.Release))
                    {
                        // Eat keystroke if trigger tells us to do so
                        return new IntPtr(1);
                    }
                }

                if (Properties.Settings.Default.LogInput)
                {
                    LOGGER.Info("-" + (VirtualKeyCode)vkCode);
                }

                EventBus<EventKeyboard>.Instance.SendEvent(null, new EventKeyboard(vkCode, ButtonStates.Release));
            }

            return new IntPtr(0);
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
                ActionKey actionKey = new ActionKey(TimeProvider.GetTickCount(), remap.KeyNew, movement);
                actionKey.Play();

                return remap.KeyNew;
            }

            return inputKey;
        }
    }
}

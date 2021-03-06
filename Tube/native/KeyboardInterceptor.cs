﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Glue.Native
{
    // Based on https://blogs.msdn.microsoft.com/toub/2006/05/03/low-level-keyboard-hook-in-c/
    // Thanks Stephen Toub!  
    //
    // Added additional delegate passed to Initialize() making this class cleaner to re-use
    //
    // Note that this could be initialized on a separate thread in a forms application, but that
    // thread would need to run a PeekMessage() loop or the hooks don't get called.  To do that
    // in .NET, the thread proc can use the built-in message loop easily by calling 
    // System.Windows.Forms.Application.DoEvents(); in the thread loop.
    //

    class KeyInterceptor
    {
        private static readonly LowLevelKeyboardProc s_proc = HookCallback; // registered as hook proc via SetHook
        private static LowLevelKeyboardProc s_handler = null;               // for specific implementation
        private static IntPtr s_hookID = IntPtr.Zero;

        public static void Initialize(LowLevelKeyboardProc lowLevelKeyboardProc)
        {
            s_hookID = SetHook(s_proc);
            s_handler = lowLevelKeyboardProc;
        }

        public static void Cleanup()
        {
            if (s_hookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx(s_hookID);
                s_hookID = IntPtr.Zero;
            }
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())

            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        public delegate IntPtr LowLevelKeyboardProc(
            int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(
            int nCode, IntPtr wParam, IntPtr lParam)
        {
            int vkCode = Marshal.ReadInt32(lParam);

            if (nCode >= 0)
            {
                IntPtr retval = s_handler(nCode, wParam, lParam);

                // If handler returns non-zero it's eating a keystroke!
                // Just return the non-zero.
                if ((int) retval != 0)
                {
                    return retval;
                }
            }

            // MSDN: If nCode is less than zero, the hook procedure must return 
            // the value returned by CallNextHookEx. 
            return CallNextHookEx(s_hookID, nCode, wParam, lParam);
        }

        #region Win API Functions and Constants

        [StructLayout(LayoutKind.Sequential)]
        public class KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public KBDLLHOOKSTRUCTFlags flags;
            public uint time;
            public UIntPtr dwExtraInfo;
        }

        [Flags]
        public enum KBDLLHOOKSTRUCTFlags : uint 
        {
            LLKHF_EXTENDED = 0x01,
            LLKHF_INJECTED = 0x10,
            LLKHF_ALTDOWN = 0x20,
            LLKHF_UP = 0x80,
        }

        public const int WH_KEYBOARD_LL    = 13;

        public const int WM_KEYDOWN        = 0x0100;
        public const int WM_SYSKEYDOWN     = 0x0104;

        public const int WM_KEYUP          = 0x0101;
        public const int WM_SYSKEYUP       = 0x0105;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        #endregion
    }
}
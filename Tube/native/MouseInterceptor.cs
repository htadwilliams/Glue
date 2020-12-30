using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Glue.Native
{
    public enum MouseMessages
    {
        WM_MOUSEMOVE        = 0x0200,

        WM_LBUTTONDOWN      = 0x0201,
        WM_LBUTTONUP        = 0x0202,
        WM_LBUTTONDBLCLK    = 0x0203,

        WM_RBUTTONDOWN      = 0x0204,
        WM_RBUTTONUP        = 0x0205,
        WM_RBUTTONDBLCLK    = 0x0206,

        WM_MBUTTONDOWN      = 0x0207,
        WM_MBUTTONUP        = 0x0208,
        WM_MBUTTONDBLCLK    = 0x0209,

        WM_MOUSEWHEEL       = 0x020A,

        WM_XBUTTONDOWN      = 0x020B,
        WM_XBUTTONUP        = 0x020C,
        WM_XBUTTONDBLCLK    = 0x020D,
    }

    public class MouseInterceptor
    {
        public delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static LowLevelMouseProc s_proc = HookCallback;
        private static LowLevelMouseProc s_handler = HookCallback;
        private static IntPtr s_hookID = IntPtr.Zero;

        public static void Initialize(LowLevelMouseProc lowLevelMouseProc)
        {
            s_hookID = SetHook(s_proc);
            s_handler = lowLevelMouseProc;
        }

        public static void Unhook()
        {
            if (s_hookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx(s_hookID);
                s_hookID = IntPtr.Zero;
            }
        }

        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private static IntPtr HookCallback(
            int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                IntPtr retval = s_handler(nCode, wParam, lParam);

                // If handler returns non-zero it's eating input.
                // Just return the non-zero.
                if ((int) retval != 0)
                {
                    return retval;
                }
            }

            return CallNextHookEx(s_hookID, nCode, wParam, lParam);
        }

        #region Win API Functions and Constants

        private const int WH_MOUSE_LL = 14;

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

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
using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Glue
{
    class ProcessInfo
    {
        private const int MAX_PATH = 2048;
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static int GetInputFocusProcessId()
        {
            uint inputProcessId = 0;

            IntPtr foregroundWindowHandle = GetForegroundWindow();

            if (0 != foregroundWindowHandle.ToInt64())
            {
                //if (LOGGER.IsDebugEnabled)
                //{
                //    LOGGER.Debug(String.Format("Foreground window handle: {0:X}", foregroundWindowHandle.ToInt64()));
                //}

                GetWindowThreadProcessId(foregroundWindowHandle, out inputProcessId);
            }

            return (int) inputProcessId;
        }

        public static string GetProcessFileName(int processId)
        {
            StringBuilder processFileName = new StringBuilder(MAX_PATH);

            IntPtr processHandle = OpenProcess(ProcessAccessFlags.All, true, processId);

            //if (LOGGER.IsDebugEnabled)
            //{
            //    LOGGER.Debug(String.Format("Process handle for id={0}: {1:X}", processId, processHandle.ToInt64()));
            //}

            if (0 != processHandle.ToInt64())
            {
                GetProcessImageFileName(processHandle, processFileName, MAX_PATH);
                CloseHandle(processHandle);
            }

            return processFileName.ToString();
        }

        #region Win API Functions and Constants

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError=true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VirtualMemoryOperation = 0x00000008,
            VirtualMemoryRead = 0x00000010,
            VirtualMemoryWrite = 0x00000020,
            DuplicateHandle = 0x00000040,
            CreateProcess = 0x000000080,
            SetQuota = 0x00000100,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            QueryLimitedInformation = 0x00001000,
            Synchronize = 0x00100000
        }
    
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(
             ProcessAccessFlags processAccess,
             bool bInheritHandle,
             int processId
        );

        public static IntPtr OpenProcess(int processId, ProcessAccessFlags flags)
        {
             return OpenProcess(flags, false, processId);
        }

        [DllImport("psapi.dll")]
        static extern uint GetProcessImageFileName(
            IntPtr hProcess,
            [Out] StringBuilder lpImageFileName,
            [In] [MarshalAs(UnmanagedType.U4)] int nSize
        );

        [DllImport("kernel32.dll", SetLastError=true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);

        #endregion
    }
}

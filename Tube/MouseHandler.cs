using System;
using System.Runtime.InteropServices;
using static Glue.MouseInterceptor;

namespace Glue
{
    class MouseHandler
    {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static IntPtr HookCallback(
            int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 &&
                MouseMessages.WM_LBUTTONDOWN == (MouseMessages)wParam)
            {
                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT) Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));

                LOGGER.Info(hookStruct.pt.x + ", " + hookStruct.pt.y);
            }

            return new IntPtr(0);
        }
    }
}

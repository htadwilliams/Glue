using System;
using System.Runtime.InteropServices;

namespace Glue.Native
{
    class TimeProvider
    {
        public static long GetTickCount()
        {
            // LOGGER.Debug("Now = " + GetTickCount64());

            // Commented out first attempt - may wish to play with it again
            // (long)(new TimeSpan(DateTime.Now.Ticks)).TotalMilliseconds;

            return (long) GetTickCount64();
        }

        #region Win API Functions and Constants

        [DllImport("kernel32")]
        extern static UInt64 GetTickCount64();

        #endregion
    }
}

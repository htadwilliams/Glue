using System;
using System.Runtime.InteropServices;

namespace Glue.Native
{
    public class TimeProvider
    {
        public static long GetTickCount()
        {
            // Commented out first attempt - may wish to play with it again
            // (long)(new TimeSpan(DateTime.Now.Ticks)).TotalMilliseconds;

            return (long) GetTickCount64();
        }

        public virtual long Now()
        {
            return TimeProvider.GetTickCount();
        }

        #region Win API Functions and Constants

        [DllImport("kernel32")]
        extern static UInt64 GetTickCount64();

        #endregion
    }
}

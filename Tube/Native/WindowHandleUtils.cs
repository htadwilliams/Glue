using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glue.Native
{
    public static class WindowHandleUtils
    {
        // Caret control isn't exposed by forms textbox 

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool HideCaret(IntPtr hWnd);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ShowCaret(IntPtr hWnd);
    }
}

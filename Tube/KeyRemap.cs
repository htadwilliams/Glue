using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput.Native;

namespace Glue
{
    class KeyRemap
    {
        public VirtualKeyCode KeyOld => this.keyOld;
        public VirtualKeyCode KeyNew => this.keyNew;
        public string ProcessName => this.processName;

        private readonly VirtualKeyCode keyOld;
        private readonly VirtualKeyCode keyNew;
        private readonly string processName;

        public KeyRemap(VirtualKeyCode keyOld, VirtualKeyCode keyNew, string procName)
        {
            this.keyOld=keyOld;
            this.keyNew=keyNew;
            this.processName=procName??throw new ArgumentNullException(nameof(procName));
        }
    }
}

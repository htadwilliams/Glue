using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using WindowsInput.Native;

namespace Glue
{
    [JsonObject(MemberSerialization.OptIn)]
    class KeyboardRemapEntry
    {
        public string ProcessName => this.processName;
        public VirtualKeyCode KeyCodeOld => keyCodeOld;
        public VirtualKeyCode KeyCodeNew => keyCodeNew;

        [JsonProperty]
        private readonly string keyOld;

        [JsonProperty]
        private readonly string keyNew;

        [JsonProperty]
        private readonly string processName;

        private readonly VirtualKeyCode keyCodeOld;
        private readonly VirtualKeyCode keyCodeNew;

        [JsonConstructor]
        public KeyboardRemapEntry(string keyOld, string keyNew, string procName)
        {
            this.keyOld = keyOld;
            this.keyNew = keyNew;
            this.keyCodeOld = (VirtualKeyCode) Keyboard.GetKey(keyOld).Keys;
            this.keyCodeNew = (VirtualKeyCode) Keyboard.GetKey(keyNew).Keys;
            this.processName=procName;
        }
    }
}

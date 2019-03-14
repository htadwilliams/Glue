using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using WindowsInput.Native;

namespace Glue
{
    [JsonObject(MemberSerialization.OptIn)]
    class KeyRemap
    {
        public VirtualKeyCode KeyOld => this.keyOld;
        public VirtualKeyCode KeyNew => this.keyNew;
        public string ProcessName => this.processName;

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        private readonly VirtualKeyCode keyOld;

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        private readonly VirtualKeyCode keyNew;

        [JsonProperty]
        private readonly string processName;

        public KeyRemap(VirtualKeyCode keyOld, VirtualKeyCode keyNew, string procName)
        {
            this.keyOld=keyOld;
            this.keyNew=keyNew;
            this.processName=procName;
        }
    }
}

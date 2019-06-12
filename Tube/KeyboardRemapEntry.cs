using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using WindowsInput.Native;

namespace Glue
{
    [JsonObject(MemberSerialization.OptIn)]
    class KeyboardRemapEntry
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

        [JsonConstructor]
        public KeyboardRemapEntry(VirtualKeyCode keyOld, VirtualKeyCode keyNew, string procName)
        {
            this.keyOld=keyOld;
            this.keyNew=keyNew;
            this.processName=procName;
        }
    }
}

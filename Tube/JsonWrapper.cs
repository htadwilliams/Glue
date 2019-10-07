using Newtonsoft.Json;
using System.Collections.Generic;
using System.Windows.Forms;
using WindowsInput.Native;

namespace Glue
{
    /// <summary>
    /// Encapsulates items serialized / deserialized to JSON so there's one root element 
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    internal class JsonWrapper
    {
        public List<Macro> Macros { get => macros; set => macros = value; }
        internal List<TriggerKeyboard> Triggers { get => triggers; set => triggers = value; }
        internal List<KeyboardRemapEntry> RemapKeys { get => remapKeys; set => remapKeys = value; }

        // Never put these tags on class properties. JSon should always serialize lower case names.
        [JsonProperty]
        private List<TriggerKeyboard> triggers;
        [JsonProperty]
        private List<KeyboardRemapEntry> remapKeys;
        [JsonProperty]
        private List<Macro> macros;

        /// <summary>
        /// Use this constructor to create wrapper before serialization
        /// </summary>
        /// 
        /// <param name="triggers"></param>
        /// <param name="keyMap"></param>
        /// <param name="macros"></param>
        public JsonWrapper(
            TriggerMap triggers,
            Dictionary<VirtualKeyCode, KeyboardRemapEntry> keyMap,
            Dictionary<string, Macro> macros)
        {
            this.Triggers = new List<TriggerKeyboard>();

            // Flatten trigger map into easily serializable list
            foreach (List<TriggerKeyboard> triggerList in triggers.Values)
            {
                this.Triggers.AddRange(triggerList);
            }

            this.RemapKeys = new List<KeyboardRemapEntry>(keyMap.Values);
            this.Macros = new List<Macro>(macros.Values);
        }

        /// <summary>
        /// Default constructor must be provided for deserialization
        /// </summary>
        public JsonWrapper()
        {
        }

        public TriggerMap GetTriggerMap()
        {
            TriggerMap triggerMap = new TriggerMap(this.Triggers.Count);

            foreach (TriggerKeyboard trigger in Triggers)
            {
                triggerMap.Add(trigger);
            }

            return triggerMap;
        }

        public Dictionary<string, Macro> GetMacroMap()
        {
            Dictionary<string, Macro> macroMap = new Dictionary<string, Macro>(Macros.Count);

            foreach (Macro macro in Macros)
            {
                macroMap.Add(macro.Name, macro);
            }

            return macroMap;
        }

        public Dictionary<VirtualKeyCode, KeyboardRemapEntry> GetKeyboardMap()
        {
            Dictionary<VirtualKeyCode, KeyboardRemapEntry> keyboardMap = new Dictionary<VirtualKeyCode, KeyboardRemapEntry>(RemapKeys.Count);

            foreach (KeyboardRemapEntry remapEntry in RemapKeys)
            {
                keyboardMap.Add(remapEntry.KeyOld, remapEntry);
            }

            return keyboardMap;
        }
    }
}

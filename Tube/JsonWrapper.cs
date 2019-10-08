using Glue.Triggers;
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
        public List<TriggerKeyboard> KeyboardTriggers { get => keyboardTriggers; set => keyboardTriggers = value; }
        public List<TriggerController> ControllerTriggers { get => controllerTriggers; set => controllerTriggers = value; }
        public List<KeyboardRemapEntry> RemapKeys { get => remapKeys; set => remapKeys = value; }

        // Never put these tags on class properties. JSon should always serialize lower case names.
        [JsonProperty]
        private List<Macro> macros;
        [JsonProperty]
        private List<TriggerKeyboard> keyboardTriggers;
        [JsonProperty]
        private List<TriggerController> controllerTriggers;
        [JsonProperty]
        private List<KeyboardRemapEntry> remapKeys;

        /// <summary>
        /// Use this constructor to create wrapper before serialization
        /// </summary>
        /// 
        /// <param name="triggers"></param>
        /// <param name="keyMap"></param>
        /// <param name="macros"></param>
        public JsonWrapper(
            TriggerManager triggerManager,
            Dictionary<VirtualKeyCode, KeyboardRemapEntry> keyMap,
            Dictionary<string, Macro> macros)
        {
            this.KeyboardTriggers = new List<TriggerKeyboard>();

            // Flatten keyboard trigger map into list - map can be re-built on 
            // deserialization.
            foreach (List<TriggerKeyboard> triggerList in triggerManager.KeyboardTriggers.Values)
            {
                this.KeyboardTriggers.AddRange(triggerList);
            }

            this.ControllerTriggers = triggerManager.ControllerTriggers;

            this.RemapKeys = new List<KeyboardRemapEntry>(keyMap.Values);
            this.Macros = new List<Macro>(macros.Values);
        }

        /// <summary>
        /// Default constructor must be provided for deserialization
        /// </summary>
        public JsonWrapper()
        {
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

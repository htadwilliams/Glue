using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Glue
{
    [JsonObject(MemberSerialization.OptIn)]
    class Trigger
    {
        public Keys TriggerKey => this.triggerKey;
        public bool EatInput => this.eatInput;
        public TriggerType Type => this.type;

        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [Flags]
        public enum TriggerType
        {
            Down = 0x01,
            Up   = 0x02,
            Both = 0x03,
        };

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        private readonly Keys triggerKey;

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        private readonly TriggerType type;

        [JsonProperty]
        private readonly List<string> macroNames = new List<string>();
        
        [JsonProperty (ItemConverterType = typeof(StringEnumConverter))]
        private readonly List<Keys> modKeys = new List<Keys>();

        [JsonProperty]
        private readonly bool eatInput;

        internal int indexMacroCurrent = 0;

        [JsonConstructor]
        public Trigger(Keys triggerKey, List<string> macroNames, TriggerType type, bool eatInput)
        {
            this.triggerKey = triggerKey;
            this.macroNames.AddRange(macroNames);
            this.type = type;
            this.eatInput = eatInput;
        }

        public Trigger(Keys triggerKey, List<string> macroNames, bool eatInput)
        {
            this.triggerKey = triggerKey;
            this.macroNames.AddRange(macroNames);
            this.eatInput = eatInput;
            this.type = TriggerType.Down;
        }

        public Trigger(Keys triggerKey, List<string> macroNames, TriggerType type)
        {
            this.triggerKey = triggerKey;
            this.macroNames.AddRange(macroNames);
            this.type = type;
        }

        public Trigger(Keys triggerKey, List<string> macroNames)
        {
            this.triggerKey = triggerKey;
            this.macroNames.AddRange(macroNames);
            this.type = TriggerType.Down;
        }

        public Trigger(Keys triggerKey, string macroName)
        {
            this.triggerKey = triggerKey;
            this.macroNames.Add(macroName);
            this.type = TriggerType.Down;
        }

        public Trigger AddModifier(Keys modKey)
        {
            this.modKeys.Add(modKey);
            return this;
        }

        public bool AreModKeysActive()
        {
            bool modKeysAreActive = true;

            foreach(Keys key in this.modKeys)
            {
                if (!Keyboard.IsKeyDown(key))
                {
                    modKeysAreActive = false;
                    break;
                }
            }

            return modKeysAreActive;
        }

        internal static string FormatSeparatedList<T>(List<T> list, string separator)
        {
            string formattedList = "";
            int itemCount = 0;
            foreach (T item in list)
            {
                formattedList += item.ToString();
                if (++itemCount < list.Count)
                {
                    formattedList += ", ";
                }
            }

            return formattedList;
        }

        internal bool Fire()
        {
            if (!AreModKeysActive())
            {
                return false;
            }

            if (LOGGER.IsInfoEnabled)
            {
                string message = "TRIGGER FIRED: triggerKey = [" + TriggerKey + "]";
                if (this.modKeys.Count > 0)
                {
                    message += " modKeys = [";
                    message += FormatSeparatedList(this.modKeys, ", ");
                    message += "]";
                }
                LOGGER.Info(message);
            }

            if (this.indexMacroCurrent >= this.macroNames.Count)
            {
                indexMacroCurrent = 0;
            }

            // null macro names are allowed in list for no-op
            if (null != macroNames[this.indexMacroCurrent])
            {
                GlueTube.PlayMacro(macroNames[this.indexMacroCurrent]);
            }
            this.indexMacroCurrent++;

            return this.eatInput;
        }
    }
}

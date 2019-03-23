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

        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        private readonly Keys triggerKey;

        [JsonProperty]
        private readonly List<string> macroNames = new List<string>();
        
        [JsonProperty (ItemConverterType = typeof(StringEnumConverter))]
        private readonly List<Keys> modKeys = new List<Keys>();

        [JsonConstructor]
        public Trigger(Keys triggerKey, List<String> macroNames)
        {
            this.triggerKey = triggerKey;
            this.macroNames.AddRange(macroNames);
        }

        public Trigger(Keys triggerKey, String macroName)
        {
            this.triggerKey = triggerKey;
            this.macroNames.Add(macroName);
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

        internal void Fire()
        {
            String message = "TRIGGER FIRED: triggerKey=" + TriggerKey;

            if (this.modKeys.Count > 0)
            {
                message += " modKeys={";

                int modCount = 0;
                foreach (Keys key in this.modKeys)
                {
                    message += key;
                    modCount++;
                    if (modCount < this.modKeys.Count)
                    {
                        message += ", ";
                    }
                }
                message += "}";
            }

            LOGGER.Info(message);

            foreach (String macroName in this.macroNames)
            {
                GlueTube.PlayMacro(macroName);
            }
        }
    }
}

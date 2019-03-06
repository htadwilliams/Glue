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
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        private readonly Keys triggerKey;

        // TODO Triggers refer to macros by name rather than containing instances of them
        [JsonProperty]
        private readonly Macro macro;
        
        [JsonProperty (ItemConverterType = typeof(StringEnumConverter))]
        private List<Keys> modKeys = new List<Keys>();

        public Keys TriggerKey => triggerKey;

        public Trigger(Keys triggerKey, Macro macro)
        {
            this.triggerKey = triggerKey;
            this.macro = macro;
        }

        public void AddModifier(Keys modKey)
        {
            modKeys.Add(modKey);
        }

        public bool AreModKeysActive()
        {
            bool modKeysAreActive = true;

            foreach(Keys key in modKeys)
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
            String message = "TRIGGER FIRED: triggerKey=" + triggerKey;

            if (modKeys.Count > 0)
            {
                message += " modKeys={";

                int modCount = 0;
                foreach (Keys key in modKeys)
                {
                    message += key;
                    modCount++;
                    if (modCount < modKeys.Count)
                    {
                        message += ", ";
                    }
                }
                message += "}";
            }

            LOGGER.Info(message);

            macro.Play();
        }
    }
}

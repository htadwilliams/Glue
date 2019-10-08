using Glue.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Glue.Triggers
{
    public class TriggerKeyboard : Trigger
    {
        public Keys TriggerKey => triggerKey;

        public List<Keys> ModKeys => modKeys;

        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        private readonly Keys triggerKey;

        [JsonProperty (ItemConverterType = typeof(StringEnumConverter))]
        private readonly List<Keys> modKeys = new List<Keys>();

        [JsonConstructor]
        public TriggerKeyboard(
            Keys triggerKey,
            ButtonStates buttonState, 
            List<string> macroNames, 
            bool eatInput
            ) : base(buttonState, macroNames, eatInput)
        {
            this.triggerKey = triggerKey;
        }

        public TriggerKeyboard(
            Keys triggerKey, 
            List<string> macroNames) : base(ButtonStates.Press, macroNames, false)
        {
            this.triggerKey = triggerKey;
        }

        public TriggerKeyboard(
            Keys triggerKey,
            string macroName) : base (ButtonStates.Press, macroName, false)
        {
            this.triggerKey = triggerKey;
        }

        public Trigger AddModifier(Keys modKey)
        {
            ModKeys.Add(modKey);
            return this;
        }

        private bool AreModKeysActive()
        {
            bool modKeysAreActive = true;

            foreach(Keys key in ModKeys)
            {
                if (!Keyboard.IsKeyDown(key))
                {
                    modKeysAreActive = false;
                    break;
                }
            }

            return modKeysAreActive;
        }

        public bool CheckAndFire()
        {
            if (!AreModKeysActive())
            {
                return false;
            }

            Fire();

            return EatInput;
        }
    }
}

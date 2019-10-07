using Glue.Event;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Glue
{
    public class KeyboardTrigger : Trigger
    {
        public ButtonStates Condition => this.buttonState;

        public Keys TriggerKey => triggerKey;

        public List<Keys> ModKeys => modKeys;

        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        private readonly Keys triggerKey;

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        private readonly ButtonStates buttonState;

        [JsonProperty (ItemConverterType = typeof(StringEnumConverter))]
        private readonly List<Keys> modKeys = new List<Keys>();

        [JsonConstructor]
        public KeyboardTrigger(Keys triggerKey, List<string> macroNames, ButtonStates buttonState, bool eatInput) : base(macroNames, eatInput)
        {
            this.triggerKey = triggerKey;
            this.buttonState = buttonState;
        }

        public KeyboardTrigger(Keys triggerKey, List<string> macroNames) : base(macroNames, false)
        {
            this.triggerKey = triggerKey;
            this.buttonState = ButtonStates.Press;
        }

        public KeyboardTrigger(Keys triggerKey, string macroName) : base (macroName, false)
        {
            this.triggerKey = triggerKey;
            this.buttonState = ButtonStates.Press;
        }

        public Trigger AddModifier(Keys modKey)
        {
            ModKeys.Add(modKey);
            return this;
        }

        public bool AreModKeysActive()
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

        internal bool CheckAndFire()
        {
            if (!AreModKeysActive())
            {
                return false;
            }

            if (this.indexMacroCurrent >= MacroNames.Count)
            {
                indexMacroCurrent = 0;
            }

            String macroName = MacroNames[this.indexMacroCurrent];

            if (LOGGER.IsInfoEnabled)
            {
                string message = "TRIGGER FIRED: triggerKey = [" + TriggerKey + "]";
                if (this.ModKeys.Count > 0)
                {
                    message += " modKeys = [";
                    message += Utils.FormatSeparatedList(ModKeys, ", ");
                    message += "]";
                }
                message += " macro = [" + macroName + "]";
                LOGGER.Info(message);
            }

            // null macro names are allowed in list - useful for key toggles or other 
            // no-op entries in macro "ripple" effect 
            if (null != macroName)
            {
                Tube.PlayMacro(macroName);
            }
            this.indexMacroCurrent++;

            return EatInput;
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class Trigger
    {
        public bool EatInput => this.eatInput;

        public List<string> MacroNames => macroNames;

        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [JsonProperty]
        private readonly List<string> macroNames = new List<string>();
        
        [JsonProperty]
        private readonly bool eatInput;

        internal int indexMacroCurrent = 0;

        [JsonConstructor]
        public Trigger(List<string> macroNames, bool eatInput)
        {
            this.macroNames.AddRange(macroNames);
            this.eatInput = eatInput;
        }

        public Trigger(string macroName, bool eatInput)
        {
            this.macroNames.Add(macroName);
            this.eatInput = eatInput;
        }
    }
}

using Glue.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Glue.Triggers
{
    public class TriggerKeyboard : Trigger
    {
        public Keys TriggerKey => triggerKey;
        public ButtonStates ButtonState => this.buttonState;
        public List<Keys> ModKeys => modKeys;

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        private readonly ButtonStates buttonState;

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
            ) : base(macroNames, eatInput)
        {
            this.Type = TriggerType.Keyboard;
            this.triggerKey = triggerKey;
            this.buttonState = buttonState;
        }

        protected override void SubscribeEvent()
        {
            ReturningEventBus<EventKeyboard, bool>.Instance.ReturningEventRecieved += OnEventKeyboard;
        }

        private bool OnEventKeyboard(object sender, EventKeyboard e)
        {
            if (
                TriggerKey == (Keys) e.VirtualKeyCode && 
                    (ButtonState == ButtonStates.Both || 
                    ButtonState == e.ButtonState))
            {
                if (AreModKeysActive())
                {
                    Fire();
                    return EatInput;
                }
            }

            return false;
        }

        public TriggerKeyboard(
            Keys triggerKey, 
            List<string> macroNames) : base(macroNames, false)
        {
            this.Type = TriggerType.Keyboard;
            this.triggerKey = triggerKey;
            this.buttonState = ButtonStates.Press;
        }

        public TriggerKeyboard(
            Keys triggerKey,
            string macroName) : base (macroName, false)
        {
            this.Type = TriggerType.Keyboard;
            this.triggerKey = triggerKey;
            this.buttonState = ButtonStates.Press;
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
    }
}

using Glue.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Glue.Triggers
{
    public class TriggerKeyboard : Trigger
    {
        public ButtonStates ButtonState => this.buttonState;

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        private readonly ButtonStates buttonState;

        [JsonProperty]
        private readonly string triggerKey;

        [JsonProperty]
        private readonly List<string> modKeys = new List<string>();

        // This class de/serializes display names only 
        private readonly Keys triggerKeyCode;
        private readonly List<Keys> modKeyCodes = new List<Keys>();
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [JsonConstructor]
        public TriggerKeyboard(
            string triggerKey,
            List<string> modKeys,
            ButtonStates buttonState,
            List<string> macroNames,
            bool eatInput
            ) : base(macroNames, eatInput)
        {
            this.Type = TriggerType.Keyboard;
            this.triggerKey = triggerKey;
            this.modKeys = modKeys;
            this.triggerKeyCode = Keyboard.GetKey(triggerKey).Keys;

            // Many triggers won't have them
            if (null != modKeys)
            {
                foreach (string modKeyName in modKeys)
                {
                    Key key = Keyboard.GetKey(modKeyName);
                    if (null == key)
                    {
                        LOGGER.Warn("Unknown key name: " + modKeyName);
                        key = Keyboard.GetKey(Keys.None);
                    }
                    this.modKeyCodes.Add(key.Keys);
                }
            }

            this.buttonState = buttonState;
        }

        public TriggerKeyboard(
            Keys triggerKeyCode,
            ButtonStates buttonState, 
            List<string> macroNames, 
            bool eatInput
            ) : base(macroNames, eatInput)
        {
            this.Type = TriggerType.Keyboard;
            this.triggerKeyCode = triggerKeyCode;
            this.triggerKey = Keyboard.GetKeyName((int) triggerKeyCode);
            this.buttonState = buttonState;
        }

        public TriggerKeyboard(
            Keys triggerKeyCode, 
            List<string> macroNames) : base(macroNames, false)
        {
            this.Type = TriggerType.Keyboard;
            this.triggerKeyCode = triggerKeyCode;
            this.triggerKey = Keyboard.GetKeyName((int) triggerKeyCode);
            this.buttonState = ButtonStates.Press;
        }

        public TriggerKeyboard(
            Keys triggerKeyCode,
            string macroName) : base (macroName, false)
        {
            this.Type = TriggerType.Keyboard;
            this.triggerKeyCode = triggerKeyCode;
            this.triggerKey = Keyboard.GetKeyName((int) triggerKeyCode);
            this.buttonState = ButtonStates.Press;
        }

        public Trigger AddModifier(Keys modKeyCode)
        {
            this.modKeyCodes.Add(modKeyCode);
            this.modKeys.Add(Keyboard.GetKeyName((int) modKeyCode));
            return this;
        }

        private bool AreModKeysActive()
        {
            bool modKeysAreActive = true;

            foreach(Keys key in this.modKeyCodes)
            {
                if (!Keyboard.IsKeyDown(key))
                {
                    modKeysAreActive = false;
                    break;
                }
            }

            return modKeysAreActive;
        }

        protected override void SubscribeEvent()
        {
            ReturningEventBus<EventKeyboard, bool>.Instance.ReturningEventRecieved += OnEventKeyboard;
        }

        protected override void UnsubscribeEvent()
        {
            ReturningEventBus<EventKeyboard, bool>.Instance.ReturningEventRecieved -= OnEventKeyboard;
        }

        private bool OnEventKeyboard(object sender, EventKeyboard e)
        {
            if (
                this.triggerKeyCode == (Keys)e.VirtualKeyCode &&
                    (ButtonState == ButtonStates.Both ||
                    ButtonState == e.ButtonState))
            {
                if (AreModKeysActive())
                {
                    // Return indicates whether or not to eat input, 
                    // not whether trigger fired
                    return Fire();
                }
            }

            return false;
        }
    }
}

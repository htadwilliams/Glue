using NerfDX.DirectInput;
using NerfDX.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Glue.Triggers
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TriggerControllerButton : TriggerController
    {
        [JsonProperty]
        private readonly int button;

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        private readonly ButtonValues buttonValue;

        public int Button => button;
        public ButtonValues ButtonValue => buttonValue;

        [JsonConstructor]
        public TriggerControllerButton(
            string namePart,
            int button,
            ButtonValues buttonValue, 
            List<string> macroNames) 
                
            : base(namePart, macroNames)
        {
            this.Type = TriggerType.ControllerButton;
            this.button = button;
            this.buttonValue = buttonValue;
        }

        public TriggerControllerButton(
            string namePart,
            int button, 
            ButtonValues buttonValue, 
            string macroName) 
                
            : base(namePart, macroName)
        {
            this.Type = TriggerType.ControllerButton;
            this.button = button;
            this.buttonValue = buttonValue;
        }

        protected override void OnEventController(object sender, BusEventArgs<EventController> e)
        {
            EventController eventController = e.BusEvent;

            if (eventController.Type == EventController.EventType.Button &&
                eventController.Button == Button &&
                eventController.ButtonValue == ButtonValue &&
                eventController.Joystick.Information.InstanceName.Contains(NamePart))
            {
                Fire();
            }
        }
    }
}

using NerfDX.DirectInput;
using NerfDX.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Glue.Triggers
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TriggerControllerPOV : TriggerController
    {
        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        private readonly POVStates povState;

        public POVStates POVState => povState;

        [JsonConstructor]
        public TriggerControllerPOV(
            string namePart,
            POVStates povState,
            List<string> macroNames) 
                
            : base(namePart, macroNames)
        {
            this.Type = TriggerType.ControllerPOV;
            this.povState = povState;
        }

        public TriggerControllerPOV(
            string namePart,
            POVStates povState,
            string macroName
            ) 
                
            : base(namePart, macroName)
        {
            this.Type = TriggerType.ControllerPOV;
            this.povState = povState;
        }

        protected override void OnEventController(object sender, BusEventArgs<EventController> e)
        {
            EventController eventController = e.BusEvent;

            if (eventController.Type == EventController.EventType.POV &&
                eventController.POVState == POVState &&
                eventController.Joystick.Information.InstanceName.Contains(NamePart))
            {
                Fire();
            }
        }
    }
}

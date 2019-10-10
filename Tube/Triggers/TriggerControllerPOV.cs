using Glue.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
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

        public override void CheckAndFire(EventController busEvent)
        {
            if (busEvent.Type == EventController.EventType.POV &&
                busEvent.POVState == POVState &&
                busEvent.Joystick.Information.InstanceName.Contains(NamePart))
            {
                Fire();
            }
        }
    }
}

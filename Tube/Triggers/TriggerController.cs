using Glue.Events;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Glue.Triggers
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TriggerController : Trigger
    {
        [JsonProperty]
        private readonly string namePart;
        [JsonProperty]
        private readonly int button;

        public string NamePattern => namePart;
        public int Button => button;

        [JsonConstructor]
        public TriggerController(
            ButtonStates buttonState, 
            List<string> macroNames, 
            int button, 
            string namePart) 
                
            : base(buttonState, macroNames, false)
        {
            this.button = button;
            this.namePart = namePart;
        }

        public TriggerController(
            ButtonStates buttonState, 
            string macroName, 
            int button, 
            string namePart) 
                
            : base(buttonState, macroName, false)
        {
            this.button = button;
            this.namePart = namePart;
        }

        internal void CheckAndFire(EventController busEvent)
        {
            if (Button == busEvent.Button && ButtonState == busEvent.ButtonState)
            {
                if (busEvent.Joystick.Information.InstanceName.Contains(namePart))
                {
                    Fire();
                }
            }
        }
    }
}

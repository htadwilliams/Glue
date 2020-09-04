using Glue.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using static Glue.Events.EventMouse;

namespace Glue.Triggers
{
    public class TriggerMouseWheel : Trigger
    {
        public WheelMoves WheelMove => wheelMove;
        
        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        private readonly WheelMoves wheelMove;

        [JsonConstructor]
        public TriggerMouseWheel(
            WheelMoves wheelMove,
            List<string> macroNames, 
            bool eatInput
            ) : base(macroNames, eatInput)
        {
            this.Type = TriggerType.MouseWheel;
            this.wheelMove = wheelMove;
        }

        public TriggerMouseWheel(
            WheelMoves wheelMove,
            string macroName, 
            bool eatInput
            ) : base(macroName, eatInput)
        {
            this.Type = TriggerType.MouseWheel;
            this.wheelMove = wheelMove;
        }

        protected override void SubscribeEvent()
        {
            ReturningEventBus<EventMouse, bool>.Instance.ReturningEventRecieved += OnEventMouse;
        }

        protected override void UnsubscribeEvent()
        {
            ReturningEventBus<EventMouse, bool>.Instance.ReturningEventRecieved -= OnEventMouse;
        }

        private bool OnEventMouse(object sender, EventMouse e)
        {
            if (e.WheelMove == WheelMove)
            {
                // Return indicates whether or not to eat input, 
                // not whether trigger fired
                return Fire();
            }

            return false;
        }
    }
}

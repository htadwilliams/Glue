using Glue.Events;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Glue.Triggers
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class TriggerController : Trigger
    {
        [JsonProperty]
        private readonly string namePart;
        public string NamePart => namePart;

        [JsonConstructor]
        public TriggerController(
            string namePart,
            List<string> macroNames) 
                
            : base(macroNames, false)
        {
            this.namePart = namePart;
        }

        public TriggerController(
            string namePart,
            string macroName) 
                
            : base(macroName, false)
        {
            this.namePart = namePart;
        }

        protected override void SubscribeEvent()
        {
            EventBus<EventController>.Instance.EventRecieved += OnEventController;
        }

        protected abstract void OnEventController(object sender, BusEventArgs<EventController> e);
    }
}

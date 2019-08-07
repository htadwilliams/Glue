using Glue.Actions.JsonContract;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Glue.Actions
{
    [JsonConverter(typeof(ActionConverter))]
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class Action
    {
        public long TimeTriggerMS
        {
            get => this.timeTriggerMS;
            set => this.timeTriggerMS = value;
        }

        public string Type
        {
            get => this.type;
            set => this.type=value;
        }

        public long TimeScheduledMS
        {
            get => this.timeScheduledMS;
            set => this.timeScheduledMS = value;
        }
        
        public string Name
        {
            get => this.name;
            set => this.name = value;
        }

        [JsonProperty]
        protected string type;

        [JsonProperty]
        // Used to schedule action relative to triggering event
        protected long timeTriggerMS;

        // Name of scheduled instance - used to cancel actions in queue
        protected string name;

        // Time scheduled for this action instance 
        protected long timeScheduledMS;

        public abstract void Play();

        // Actions may schedule multiple instances - see ActionTyping
        public abstract Action[] Schedule();

        public override string ToString()
        {
            return type;
        }
    }
}

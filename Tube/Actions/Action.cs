using Glue.Actions.JsonContract;
using Glue.PropertyIO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Glue.Actions
{
    public enum ActionType
    {
        KEY,
        TYPING,
        SOUND,
        MOUSE,
        REPEAT,
        CANCEL,
     }

    [JsonConverter(typeof(ActionConverter))]
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class Action
    {
        public long TimeTriggerMS { get => this.timeTriggerMS; set => this.timeTriggerMS = value; }

        public long TimeScheduledMS { get => this.timeScheduledMS; set => this.timeScheduledMS = value; }
        
        public string Name { get => this.name; set => this.name = value; }

        protected ActionType Type { get => type; set => type = value; }

        private const string TIME_SCHEDULE_MS = "timeScheduleMS";

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        private ActionType type;

        // TODO Times serialized to Json should be of type Duration instead of long
        // This would allow times to be specified in the same format as form entry
        // e.g. 4s 32ms
        [JsonProperty]
        // Used to schedule action relative to triggering event
        protected long timeTriggerMS;

        // Name of scheduled instance - used to cancel actions in queue
        protected string name;

        // Time scheduled for this action instance 
        protected long timeScheduledMS;

        public abstract void Play();

        // Actions may schedule multiple instances - see ActionTyping
        public abstract Action[] Schedule(long timeScheduled);

        protected Action(long timeTriggerMS)
        {
            this.timeTriggerMS = timeTriggerMS;
        }

        protected Action(Action copyFrom)
        {
            this.timeTriggerMS = copyFrom.timeTriggerMS;
            this.type = copyFrom.type;
        }

        public override string ToString()
        {
            return Type.ToString();
        }

        public virtual void FromProperties(PropertyBag propertyBag)
        {
            if (null != propertyBag && propertyBag.Count > 0)
            {
                if (propertyBag.TryGetProperty(TIME_SCHEDULE_MS, out PropertyDuration propertyDuration))
                {
                    this.TimeScheduledMS = propertyDuration.Value;
                }
            }
        }

        public virtual PropertyBag ToProperties(PropertyBag propertyBag)
        {
            if (null == propertyBag)
            {
                propertyBag = new PropertyBag();
            }

            propertyBag.Add(TIME_SCHEDULE_MS, new PropertyDuration(timeScheduledMS));

            return propertyBag;
        }
    }
}

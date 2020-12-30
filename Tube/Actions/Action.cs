using Glue.Actions.JsonContract;
using Glue.PropertyIO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.ComponentModel;

namespace Glue.Actions
{
    public enum ActionType
    {
        Keyboard,
        Typing,
        Sound,
        Mouse,
        Repeat,
        Cancel,
        MouseLock,
        Cmd,
     }

    public enum LockAction
    {
        Unlock = 0,
        Lock = 1,
        Toggle = 2,
    }

    [JsonConverter(typeof(ActionConverter))]
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class Action : IComparable
    {
        public long DelayMS { get => this.delayMS; set => this.delayMS = value; }

        public long ScheduledTick { get => this.scheduledTick; set => this.scheduledTick = value; }
        
        public string Name { get => this.name; set => this.name = value; }

        protected ActionType Type { get => type; set => type = value; }

        private const string DELAY_MS = "delayMS";
        private const long DELAY_DEFAULT_MS = 30;

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        private ActionType type;

        // TODO Times serialized to Json should be of type Duration instead of long
        // This would allow times to be specified in the same format as form entry
        // e.g. 4s 32ms
        [JsonProperty]
        [DefaultValue(DELAY_DEFAULT_MS)]
        // Used to schedule action relative to previous event
        protected long delayMS = DELAY_DEFAULT_MS;

        // Name of scheduled instance - used to cancel actions in queue
        protected string name;

        // Time scheduled for this action instance 
        protected long scheduledTick;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required by callback signature")]
        public void PlayWaitCallback(object context)
        {
            Play();
        }

        public abstract void Play();

        // Actions may schedule multiple instances - see ActionTyping
        public abstract Action[] Schedule(long scheduleFromTick);

        [JsonConstructor]
        protected Action(long delayMS)
        {
            this.delayMS = delayMS;
        }

        protected Action(Action copyFrom)
        {
            this.delayMS = copyFrom.delayMS;
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
                if (propertyBag.TryGetProperty(DELAY_MS, out PropertyDuration propertyDuration))
                {
                    this.DelayMS = propertyDuration.Value;
                }
            }
        }

        public virtual PropertyBag ToProperties(PropertyBag propertyBag)
        {
            if (null == propertyBag)
            {
                propertyBag = new PropertyBag();
            }

            propertyBag.Add(DELAY_MS, new PropertyDuration(this.delayMS));

            return propertyBag;
        }

        public int CompareTo(Object compareTo)
        {
            Action compareToAsAction = compareTo as Action;

            if (null == compareToAsAction) 
                return 1;
            else if (this.ScheduledTick > compareToAsAction.ScheduledTick)
                return 1;
            else if (this.ScheduledTick == compareToAsAction.ScheduledTick) 
                return 0;
            else // if (this.ScheduledTick < compareTo.ScheduledTick) 
                return -1;
        }
    }
}

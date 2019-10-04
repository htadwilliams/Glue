using Glue.PropertyIO;
using Newtonsoft.Json;

namespace Glue.Actions
{
    [JsonObject(MemberSerialization.OptIn)]
    class ActionCancel : Action
    {
        [JsonProperty]
        private string macroName;
        private const string MACRO_NAME = "macroName";

        public ActionCancel(string macroName) : base (0)
        {
            this.macroName = macroName;
            this.Type = ActionType.Cancel;
        }

        public override void Play()
        {
            Macro.Scheduler.Cancel(this.macroName);
        }

        public override Action[] Schedule(long scheduleFromTick)
        {
            ActionCancel scheduledCopy = new ActionCancel(macroName)
            {
                ScheduledTick = scheduleFromTick + this.DelayMS
            };

            return new Action[] {scheduledCopy};
        }

        public override string ToString()
        {
            return base.ToString() + " " + this.macroName;
        }

        public override void FromProperties(PropertyBag propertyBag)
        {
            base.FromProperties(propertyBag);

            if (null != propertyBag && propertyBag.Count > 0)
            {
                if (propertyBag.TryGetProperty(MACRO_NAME, out PropertyString property))
                {
                    this.macroName = property.StringValue;
                }
            }
        }

        public override PropertyBag ToProperties(PropertyBag propertyBag)
        {
            base.ToProperties(propertyBag);

            propertyBag.Add(MACRO_NAME, new PropertyString(this.macroName));

            return propertyBag;
        }
    }
}

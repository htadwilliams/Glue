using System;
using Glue.PropertyIO;
using Newtonsoft.Json;

namespace Glue.Actions
{
    [JsonObject(MemberSerialization.OptIn)]
    class ActionCancel : Action
    {
        [JsonProperty]
        private string macroName;

        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string MACRO_NAME = "macroName";

        public ActionCancel(string macroName) : base (0)
        {
            this.macroName = macroName;
            this.Type = ActionType.CANCEL;
        }

        public override void Play()
        {
            ActionQueueThread.Cancel(this.macroName);
        }

        public override Action[] Schedule(long timeScheduleFrom)
        {
            ActionCancel scheduledCopy = new ActionCancel(macroName)
            {
                TimeScheduledMS = timeScheduleFrom + this.TimeTriggerMS
            };

            if (LOGGER.IsDebugEnabled)
            {
                String message = String.Format("Scheduled at tick {0:n0} in {1:n0}ms: canceling macro {2}",
                    scheduledCopy.TimeScheduledMS,      // Absolute time scheduled to play
                    this.TimeTriggerMS,                 // Time relative to trigger
                    this.macroName);

                LOGGER.Debug(message);
            }

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

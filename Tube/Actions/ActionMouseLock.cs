using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Glue.Actions
{
    [JsonObject(MemberSerialization.OptIn)]
    class ActionMouseLock : Action
    {
        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        private readonly LockAction lockAction;

        public ActionMouseLock(LockAction lockAction) : base (0)
        {
            Type = ActionType.MouseLock;
            this.lockAction = lockAction;
        }

        public override void Play()
        {
            Glue.Tube.ActivateMouseLock(this.lockAction);
        }

        public override Action[] Schedule(long scheduleFromTick)
        {
            ActionMouseLock scheduledCopy = new ActionMouseLock(this.lockAction)
            {
                ScheduledTick = scheduleFromTick + this.DelayMS
            };

            return new Action[] {scheduledCopy};
        }

        public override string ToString()
        {
            return base.ToString() + "(" + this.lockAction + ")";
        }
    }
}

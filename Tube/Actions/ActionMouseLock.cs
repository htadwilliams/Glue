using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Glue.Actions
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ActionMouseLock : Action
    {
        public MouseLocks MouseLock{ get => mouseLock; set => mouseLock = value; }

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        private MouseLocks mouseLock;

        public ActionMouseLock(MouseLocks mouseLock) : base (0)
        {
            Type = ActionType.MouseLock;
            this.MouseLock = mouseLock;
        }

        public override void Play()
        {
            Glue.Tube.MouseLock = this.MouseLock;
        }

        public override Action[] Schedule(long scheduleFromTick)
        {
            ActionMouseLock scheduledCopy = new ActionMouseLock(this.MouseLock)
            {
                ScheduledTick = scheduleFromTick + this.DelayMS
            };

            return new Action[] {scheduledCopy};
        }

        public override string ToString()
        {
            return base.ToString() + " (" + this.MouseLock + ")";
        }
    }
}

using Glue.PropertyIO;
using Newtonsoft.Json;
using WindowsInput.Native;

namespace Glue.Actions
{
    [JsonObject(MemberSerialization.OptIn)]
    class ActionMouseLock : Action
    {
        //[JsonProperty]
        //private bool  lockMouse;
        //private const string LOCK = "lockMouse";

//        public ActionMouseLock(bool lockMouse) : base (0)
        public ActionMouseLock() : base (0)
        {
            this.Type = ActionType.MouseLock;
        }

        public override void Play()
        {
            Glue.Tube.ToggleMouseLock();
        }

        public override Action[] Schedule(long scheduleFromTick)
        {
            ActionMouseLock scheduledCopy = new ActionMouseLock()
            {
                ScheduledTick = scheduleFromTick + this.DelayMS
            };

            return new Action[] {scheduledCopy};
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}

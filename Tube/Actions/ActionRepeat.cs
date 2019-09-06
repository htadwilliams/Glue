using Newtonsoft.Json;

namespace Glue.Actions
{
    class ActionRepeat : Action
    {
        [JsonProperty]
        private readonly string macroRepeater;

        [JsonProperty]
        private readonly string macro;

        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ActionRepeat(long timeRepeatMS, string macroRepeater, string macro) 
            : base (timeRepeatMS)
        {
            this.macroRepeater = macroRepeater;
            this.macro = macro;
            this.Type = ActionType.REPEAT;
        }

        public override void Play()
        {
            // play macro that initiated the repeater
            // it'll schedules another instance of ActionRepeater (this)
            Tube.PlayMacro(macroRepeater);
        }

        public override Action[] Schedule(long timeScheduleFrom)
        {
            // play macro to be repeated 
            Tube.PlayMacro(macro);

            // schedule the repeat
            ActionRepeat scheduledCopy = new ActionRepeat(
                this.DelayMS,
                this.macroRepeater, 
                this.macro)
            {
                ScheduledTick = timeScheduleFrom + this.delayMS,
                Name = macroRepeater
            };

            return new Action[] {scheduledCopy};
        }

        public override string ToString()
        {
            return base.ToString() + " " + this.macro;
        }
    }
}

using System;
using Glue.Native;
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
            // play macro to be repeated
            Tube.PlayMacro(macro);

            // schedule another instance to be played later
            Tube.PlayMacro(macroRepeater);
        }

        public override Action[] Schedule(long timeScheduleFrom)
        {
            ActionRepeat scheduledCopy = new ActionRepeat(
                this.TimeTriggerMS,
                this.macroRepeater, 
                this.macro)
            {
                TimeScheduledMS = timeScheduleFrom + this.timeTriggerMS,
                Name = macroRepeater
            };

            if (LOGGER.IsDebugEnabled)
            {
                string message = String.Format("Scheduled at tick {0:n0} in {1:n0}ms: repeating macro {2}",
                    scheduledCopy.TimeScheduledMS,
                    this.TimeTriggerMS,
                    this.macro);

                LOGGER.Debug(message);
            }

            return new Action[] {scheduledCopy};
        }

        public override string ToString()
        {
            return base.ToString() + " " + this.macro;
        }
    }
}

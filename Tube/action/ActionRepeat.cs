using System;
using Glue.native;
using Newtonsoft.Json;

namespace Glue
{
    class ActionRepeat : Action
    {
        [JsonProperty]
        private readonly string macroRepeater;

        [JsonProperty]
        private readonly string macro;

        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ActionRepeat(long timeRepeatMS, string macroRepeater, string macro)
        {
            this.macroRepeater = macroRepeater;
            this.macro = macro;
            this.TimeTriggerMS = timeRepeatMS;
            this.Type = "REPEAT";
        }

        public override void Play()
        {
            // play macro to be repeated
            Tube.PlayMacro(macro);

            // schedule another instance to be played later
            Tube.PlayMacro(macroRepeater);
        }

        public override Action[] Schedule()
        {
            ActionRepeat scheduledCopy = new ActionRepeat(
                this.TimeTriggerMS,
                this.macroRepeater, 
                this.macro)
            {
                TimeScheduledMS = TimeProvider.GetTickCount() + this.timeTriggerMS,
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
    }
}

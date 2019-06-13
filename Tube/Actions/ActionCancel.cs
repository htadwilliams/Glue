using System;
using Glue.Native;
using Newtonsoft.Json;

namespace Glue.Actions
{
    class ActionCancel : Action
    {
        [JsonProperty]
        private readonly string macro;

        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ActionCancel(string macro)
        {
            this.macro = macro;
            this.Type = "CANCEL";
        }

        public override void Play()
        {
            ActionQueueThread.Cancel(this.macro);
        }

        public override Action[] Schedule()
        {
            ActionCancel scheduledCopy = new ActionCancel(macro)
            {
                TimeScheduledMS = TimeProvider.GetTickCount() + this.TimeTriggerMS
            };

            if (LOGGER.IsDebugEnabled)
            {
                String message = String.Format("Scheduled at tick {0:n0} in {1:n0}ms: canceling macro {2}",
                    scheduledCopy.TimeScheduledMS,      // Absolute time scheduled to play
                    this.TimeTriggerMS,                 // Time relative to trigger
                    this.macro);

                LOGGER.Debug(message);
            }

            return new Action[] {scheduledCopy};
        }
    }
}

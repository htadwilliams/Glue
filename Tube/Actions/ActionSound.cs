using System;
using System.Media;
using Glue.Native;
using Newtonsoft.Json;

namespace Glue.Actions
{
    public class ActionSound : Action
    {
        private static readonly SoundPlayer PLAYER = new SoundPlayer();
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string soundPath;

        [JsonProperty]
        public string SoundPath
        {
            get => this.soundPath;
            set => this.soundPath=value;
        }

        public ActionSound(long timeTriggerMS, string soundPath) : base(timeTriggerMS)
        {
            this.soundPath = soundPath;
            this.Type = ActionType.SOUND;
        }

        public override void Play()
        {
            PLAYER.SoundLocation = soundPath;
            try
            {
                PLAYER.Play();
            }
            catch (Exception e)
            {
                LOGGER.Error("Exception: " + e);
            }

            LOGGER.Debug("Playing sound: " + this.soundPath);
        }

        public override Action[] Schedule(long timeScheduleFrom)
        {
            ActionSound scheduledCopy = new ActionSound(this.TimeTriggerMS, this.soundPath)
            {
                TimeScheduledMS = timeScheduleFrom + this.TimeTriggerMS
            };

            if (LOGGER.IsDebugEnabled)
            {
                string message = String.Format("Scheduled at tick {0:n0} in {1:n0}ms: {2}",
                    scheduledCopy.TimeScheduledMS,
                    this.TimeTriggerMS,
                    this.soundPath);
                LOGGER.Debug(message);
            }

            return new Action[] {scheduledCopy};
        }

        public override string ToString()
        {
            return base.ToString() + " " + this.soundPath;
        }
    }
}

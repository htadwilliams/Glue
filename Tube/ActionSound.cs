using System;
using System.Media;
using Newtonsoft.Json;

namespace Glue
{
    public class ActionSound : Action
    {
        private static readonly SoundPlayer PLAYER = new SoundPlayer();
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private String soundPath;

        [JsonProperty]
        public string SoundPath
        {
            get => this.soundPath;
            set => this.soundPath=value;
        }

        public ActionSound(string soundPath)
        {
            this.soundPath = soundPath;
            this.Type = "SOUND";
        }

        public override void Play()
        {
            // TODO Sharing sound player instance isn't thread safe.
            // Either multiple instances of player need to be used or this 
            // needs a lock.
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

        public override Action[] Schedule()
        {
            ActionSound scheduledCopy = new ActionSound(this.soundPath);
            {
                TimeScheduledMS = ActionQueue.Now();
            };

            if (LOGGER.IsDebugEnabled)
            {
                String message = String.Format("Scheduled     now {0:n0}: {1}",
                    // Absolute time scheduled to play
                    this.TimeScheduledMS,           
                    this.soundPath);
                LOGGER.Debug(message);
            }

            return new Action[] {scheduledCopy};
        }
    }
}

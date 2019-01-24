using System;
using System.Media;

namespace Glue
{
    public class ActionSound : Action, IAction
    {
        private static readonly SoundPlayer PLAYER = new SoundPlayer();
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private String soundPath;

        public string SoundPath
        {
            get => this.soundPath;
            set => this.soundPath=value;
        }

        public ActionSound(string soundPath)
        {
            this.soundPath = soundPath;
        }

        public void Play()
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

        public IAction[] Schedule()
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

            return new IAction[] {scheduledCopy};
        }
    }
}

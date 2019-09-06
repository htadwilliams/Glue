using System;
using System.Collections.Generic;
using System.Media;
using Glue.Native;
using Newtonsoft.Json;

namespace Glue.Actions
{
    public class ActionSound : Action
    {
        private static readonly Dictionary<string, SoundPlayer> PLAYER_CACHE = new Dictionary<String, SoundPlayer>();

        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string soundPath;

        [JsonProperty]
        public string SoundPath
        {
            get => this.soundPath;
            set => this.soundPath=value;
        }

        public ActionSound(long timeDelayMS, string soundPath) : base(timeDelayMS)
        {
            this.soundPath = soundPath;
            this.Type = ActionType.SOUND;
        }

        protected SoundPlayer GetPlayer(string soundFileName)
        {
            if (!PLAYER_CACHE.TryGetValue(soundFileName, out SoundPlayer player))
            {
                player = new SoundPlayer();
                try
                {
                    player.SoundLocation = this.SoundPath;
                    player.Load();
                }
                catch (Exception e)
                {
                    // Nothing we can reasonably do if something goes wrong but log and continue
                    player = null;
                    LOGGER.Error("Error loading sound: " + e);
                }

                // player may be null if exception occurred - add it anyway so loading attempts don't continue
                PLAYER_CACHE.Add(soundFileName, player);
            }

            return player;
        }

        public override void Play()
        {
            SoundPlayer player = GetPlayer(this.SoundPath);
            if (null != player)
            {
                LOGGER.Debug("Playing sound: " + this.soundPath);
                try
                {
                    player.Play();
                }
                catch (Exception e)
                {
                    // Don't use this player in the future - it is broken
                    PLAYER_CACHE[soundPath] = null;
                    LOGGER.Error("Error playing sound: " + e);
                }
            }
            else
            {
                LOGGER.Warn("Not attempting to play bad/unloaded sound: " + this.soundPath);
            }
        }

        public override Action[] Schedule(long timeScheduleFrom)
        {
            ActionSound scheduledCopy = new ActionSound(this.DelayMS, this.soundPath)
            {
                ScheduledTick = timeScheduleFrom + this.DelayMS
            };

            return new Action[] {scheduledCopy};
        }

        public override string ToString()
        {
            return base.ToString() + " " + this.soundPath;
        }
    }
}

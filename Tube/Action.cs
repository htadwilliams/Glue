using System;

namespace Glue
{
    public abstract class Action : IAction
    {
        public long TimeScheduledMS
        {
            get => this.timeScheduledMS;
            set => this.timeScheduledMS=value;
        }

        private long timeScheduledMS;         // Time relative to triggering event

        public IAction[] Schedule()
        {
            throw new NotImplementedException();
        }

        public void Play()
        {
            throw new NotImplementedException();
        }

        public long GetScheduledTime()
        {
            throw new NotImplementedException();
        }
    }
}

using System;

namespace Glue
{
    public abstract class Action
    {
        public long TimeScheduledMS
        {
            get => this.timeScheduledMS;
            set => this.timeScheduledMS=value;
        }

        private long timeScheduledMS;         // Time relative to triggering event
    }
}

using System;
using System.Windows.Forms;

namespace Glue
{
    // TODO Action types may be better as a class hierarchy - refactor when added
    public enum ActionTypes
    {
        KEYBOARD_PRESS,
        KEYBOARD_RELEASE,

        // TODO implement advanced actions
        //JOYBUTTON_PRESS,
        //JOYUBUTTON_RELEASE,
        //MOUSE_PRESS,
        //MOUSE_RELEASE,
        //SOUND,
    };

    class Action
    {
        public long TimeTriggerMS => timeTriggerMS;
        public ActionTypes ActionType => actionType;
        public Keys Key => key;

        public long TimeScheduledMS { get => timeScheduledMS; set => timeScheduledMS = value; }

        private readonly long timeTriggerMS;         // Time relative to previous action
        private readonly ActionTypes actionType;
        private readonly Keys key;

        // Absolute time to fire, calculated when Action is scheduled 
        private long timeScheduledMS;

#if DEBUG
        private static long counter = 0;
        private long id;
#endif

        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Action(ActionTypes actionType, long timeTriggerMS, Keys key)
        {
            this.actionType = actionType;
            this.timeTriggerMS = timeTriggerMS;
            this.key = key;
#if DEBUG
            this.id = counter += 1;
#endif
        }

        // copy ctor
        public Action(Action previousAction)
        {
            this.actionType = previousAction.actionType;
            this.timeTriggerMS = previousAction.timeTriggerMS;
            this.key = previousAction.key;
#if DEBUG
            this.id = counter += 1;
#endif
        }

        public long Schedule()
        {
            this.timeScheduledMS = ActionQueue.Now() + this.timeTriggerMS;


#if DEBUG   // Using conditional here because id var won't be defined for RELEASE
            if (LOGGER.IsDebugEnabled)
            {
                String message = String.Format("'{0}' Scheduled at tick {1:n0} in {2}ms: {3}-{4}",
                    this.id.ToString().PadLeft(3, '0'),
                    this.timeScheduledMS,           // Absolute time scheduled to play
                    this.timeTriggerMS,             // Time relative to prevous action
                    this.Key,                   
                    this.ActionType);
                LOGGER.Debug(message);
            }
#endif
            return this.timeScheduledMS;
        }

        internal void Play()
        {
#if DEBUG
            // TODO trigger actual key presses!
            if (LOGGER.IsDebugEnabled)
            {
                long now = ActionQueue.Now();
                String message = String.Format("'{0}'   Played at tick {1:n0} dt {2}ms: {3}-{4}",
                    this.id.ToString().PadLeft(3, '0'),
                    now,                            // Time actually played
                    now - this.timeScheduledMS,     // Time delta (how late were we?)
                    this.Key,                       // 
                    this.ActionType);
                LOGGER.Debug(message);
#endif
            }
        }
    }
}

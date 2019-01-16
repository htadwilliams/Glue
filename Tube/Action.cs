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
        private readonly long timeTriggerMS;         // Time relative to previous action
        private readonly ActionTypes actionType;
        private readonly Keys key;

        private long timeScheduledMS;               // Absolute time to fire, calculated internally

        public long TimeTriggerMS => timeTriggerMS;
        public ActionTypes ActionType => actionType;
        public Keys Key => key;

        public long TimeScheduledMS { get => timeScheduledMS; set => timeScheduledMS = value; }

        public Action(ActionTypes actionType, long timeTriggerMS, Keys key)
        {
            this.actionType = actionType;
            this.timeTriggerMS = timeTriggerMS;
            this.key = key;
        }

        public long Schedule()
        {
            return this.timeScheduledMS = ActionQueue.Now() + this.timeTriggerMS;
        }
    }
}

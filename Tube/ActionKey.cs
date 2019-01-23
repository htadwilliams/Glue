using System;
using WindowsInput;
using WindowsInput.Native;

namespace Glue
{
    public class ActionKey : Action, IAction
    {
        public long TimeTriggerMS => timeTriggerMS;
        public Nullable<VirtualKeyCode> Key => key;

        public enum Type
        {
            PRESS,
            RELEASE
        }

        // TODO get rid of overloaded behavior caused by field state
        // Only one of these fields should be set at a given time
        private readonly Nullable<VirtualKeyCode> key;  // Generated if scheduled by this class (ActionKey)
        private Nullable<INPUT> input;                  // Generated if scheduled by ActionTyping with a string

        private readonly Type type;
        private readonly long timeTriggerMS;         // Time relative to triggering event

        private static readonly WindowsInputMessageDispatcher DISPATCHER = new WindowsInputMessageDispatcher();
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ActionKey(VirtualKeyCode key, Type type, long timeTriggerMS)
        {
            this.key = key;
            this.type = type;
            this.input = null;
            this.timeTriggerMS = timeTriggerMS;
        }

        internal ActionKey(INPUT input, Type type, long timeTriggerMS)
        {
            this.input=input;
            this.key = null;
            this.type = type;
            this.timeTriggerMS = timeTriggerMS;
        }

        public new IAction[] Schedule()
        {
            ActionKey scheduledCopy = new ActionKey((VirtualKeyCode) this.key, this.type, this.TimeTriggerMS)
            {
                TimeScheduledMS = ActionQueue.Now() + this.timeTriggerMS
            };

            if (LOGGER.IsDebugEnabled)
            {
                String message = String.Format("Scheduled at tick {0:n0} in {1}ms: {2}-{3}",
                    this.TimeScheduledMS,           // Absolute time scheduled to play
                    this.timeTriggerMS,             // Time relative to prevous action
                    this.key,                   
                    this.type);
                LOGGER.Debug(message);
            }

            return new IAction[] {scheduledCopy};
        }

        public new void Play()
        {
            INPUT[] inputs = null;
            if (null == this.input)
            {
                switch (this.type)
                {
                    case Type.PRESS:
                        inputs = new InputBuilder().AddKeyDown((VirtualKeyCode)this.key).ToArray();
                        break;
                    case Type.RELEASE:
                        inputs = new InputBuilder().AddKeyUp((VirtualKeyCode)this.key).ToArray();
                        break;
                }
            }
            else
            {
                inputs = new INPUT[1];
                inputs[0] = (INPUT) this.input;
            }
            DISPATCHER.DispatchInput(inputs);

            if (LOGGER.IsDebugEnabled)
            {
                long now = ActionQueue.Now();
                String message = String.Format("   Played at tick {0:n0} dt {1}ms: {2}-{3}",
                    now,                            // Time actually played
                    now - this.TimeScheduledMS,     // Time delta (how late were we?)
                    this.key,                       
                    this.type);
                LOGGER.Debug(message);
            }
        }
    }
}

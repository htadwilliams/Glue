using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using WindowsInput;
using WindowsInput.Native;

namespace Glue
{
    public class ActionKey : Action
    {
        public enum Movement
        {
            PRESS,
            RELEASE
        }

        public static readonly IntPtr INJECTION_ID = new IntPtr(0xD00D);
        public long TimeTriggerMS => timeTriggerMS;
        public Nullable<VirtualKeyCode> Key => key;

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        private readonly Movement movement;

        [JsonProperty]
        private readonly long timeTriggerMS;            // Time relative to triggering event

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        private readonly Nullable<VirtualKeyCode> key;  // Generated if scheduled by this class (ActionKey)
        private Nullable<INPUT> input;                  // Generated if scheduled by ActionTyping with a string

        private static readonly WindowsInputMessageDispatcher DISPATCHER = new WindowsInputMessageDispatcher();
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ActionKey(VirtualKeyCode key, Movement movement, long timeTriggerMS)
        {
            this.key = key;
            this.movement = movement;
            this.input = null;
            this.timeTriggerMS = timeTriggerMS;
            this.Type = "KEY";
        }

        internal ActionKey(INPUT input, Movement movement, long timeTriggerMS)
        {
            this.input=input;
            this.key = null;
            this.movement = movement;
            this.timeTriggerMS = timeTriggerMS;
        }

        public override Action[] Schedule()
        {
            ActionKey scheduledCopy = new ActionKey((VirtualKeyCode) this.key, this.movement, this.TimeTriggerMS)
            {
                TimeScheduledMS = ActionQueue.Now() + this.timeTriggerMS
            };

            if (LOGGER.IsDebugEnabled)
            {
                String message = String.Format("Scheduled at tick {0:n0} in {1}ms: {2}-{3}",
                    scheduledCopy.TimeScheduledMS,      // Absolute time scheduled to play
                    this.timeTriggerMS,                 // Time relative to prevous action
                    this.key,                   
                    this.movement);
                LOGGER.Debug(message);
            }

            return new Action[] {scheduledCopy};
        }

        public override void Play()
        {
            INPUT[] inputs = null;

            // Constructor only supplied virtual key code - create INPUT 
            if (null == this.input)
            {
                switch (this.movement)
                {
                    case Movement.PRESS:
                        inputs = new InputBuilder().AddKeyDown((VirtualKeyCode) this.key).ToArray();
                        break;
                    case Movement.RELEASE:
                        inputs = new InputBuilder().AddKeyUp((VirtualKeyCode) this.key).ToArray();
                        break;
                }

            }
            // Constructor supplied INPUT - use it instead
            else
            {
                inputs = new INPUT[1];
                inputs[0] = (INPUT) this.input;
            }

            // Stamp our injected input so our keyboard remapper doesn't try 
            // to remap it! We still want to remap other injected input (for 
            // example from remote desktop, or steam streaming)
            inputs[0].Data.Keyboard.ExtraInfo = INJECTION_ID;
            DISPATCHER.DispatchInput(inputs);

            if (LOGGER.IsDebugEnabled)
            {
                long now = ActionQueue.Now();
                String message = String.Format("   Played at tick {0:n0} dt {1}ms: {2}-{3}",
                    now,                            // Time actually played
                    now - this.TimeScheduledMS,     // Time delta (how late were we?)
                    this.key,                       
                    this.movement);
                LOGGER.Debug(message);
            }
        }
    }
}

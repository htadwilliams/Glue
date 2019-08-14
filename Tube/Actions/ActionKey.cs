using System;
using Glue.Native;
using Glue.PropertyIO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using WindowsInput;
using WindowsInput.Native;

namespace Glue.Actions
{
    public class ActionKey : Action
    {
        public enum MoveType
        {
            PRESS,
            RELEASE,
            CLICK
        }

        private const string MOVEMENT = "movement";
        public static readonly IntPtr INJECTION_ID = new IntPtr(0xD00D);
        public Nullable<VirtualKeyCode> Key => key;

        public MoveType Movement { get => movement; set => movement = value; }

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        private MoveType movement;

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        private Nullable<VirtualKeyCode> key;

        [JsonProperty]
        // Options - if specified will be used to schedule if MoveType is CLICK
        protected long timeClickMS = -1;

        // Generated only if scheduled by ActionTyping with a string
        private Nullable<INPUT> input;          

        private static readonly WindowsInputMessageDispatcher DISPATCHER = new WindowsInputMessageDispatcher();
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [JsonConstructor]
        public ActionKey(long timeTriggerMS, VirtualKeyCode key, MoveType movement) : base(timeTriggerMS)
        {
            this.key = key;
            this.Movement = movement;
            this.input = null;
            this.Type = ActionType.KEY;
        }

        internal ActionKey(long timeTriggerMS, INPUT input, MoveType movement) : base(timeTriggerMS)
        {
            this.input=input;
            this.key = null;
            this.Movement = movement;
            this.Type = ActionType.KEY;
        }

        public override Action[] Schedule(long timeScheduleFrom)
        {
            Action[] scheduledActions;
            if (this.Movement == MoveType.CLICK)
            {
                long timeClickMS = 
                    this.timeClickMS > 0 
                        ? this.timeClickMS 
                        : this.timeTriggerMS;

                scheduledActions = new Action[]
                {
                    new ActionKey(this.TimeTriggerMS, (VirtualKeyCode) this.key, MoveType.PRESS)
                    {
                        TimeScheduledMS = timeScheduleFrom + this.timeTriggerMS
                    },
                    new ActionKey(this.timeTriggerMS, (VirtualKeyCode) this.key, MoveType.RELEASE)
                    {

                        TimeScheduledMS = timeScheduleFrom + this.timeTriggerMS + timeClickMS
                    }
                };
            }
            else
            {
                scheduledActions = new Action[]
                {
                    new ActionKey(this.TimeTriggerMS, (VirtualKeyCode) this.key, this.Movement)
                    {
                        TimeScheduledMS = timeScheduleFrom + this.timeTriggerMS
                    }
                };
            }

            if (LOGGER.IsDebugEnabled)
            {
                foreach(ActionKey action in scheduledActions)
                {
                    string message = String.Format("Scheduled at tick {0:n0} in {1:n0}ms: {2}-{3}",
                        action.TimeScheduledMS,      // Absolute time scheduled to play
                        action.TimeScheduledMS - timeScheduleFrom,        // Time relative to NOW
                        action.Key,                   
                        action.Movement);
                    LOGGER.Debug(message);
                }
            }

            return scheduledActions;
        }

        public override void Play()
        {
            INPUT[] inputs = null;

            // Constructor only supplied virtual key code - create INPUT 
            if (null == this.input)
            {
                switch (this.Movement)
                {
                    case MoveType.PRESS:
                        inputs = new InputBuilder().AddKeyDown((VirtualKeyCode) this.key).ToArray();
                        break;
                    case MoveType.RELEASE:
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
                long now = TimeProvider.GetTickCount();
                string message = String.Format("   Played at tick {0:n0} dt {1}ms: {2}-{3}",
                    now,                            // Time actually played
                    now - this.TimeScheduledMS,     // Time delta (how late were we?)
                    this.key,                       
                    this.Movement);
                LOGGER.Debug(message);
            }
        }

        public override string ToString()
        {
            return base.ToString() + (Movement == MoveType.PRESS ? " +" : " -") + this.key;
        }

        public override void FromProperties(PropertyBag propertyBag)
        {
            base.FromProperties(propertyBag);

            if (null != propertyBag && propertyBag.Count > 0)
            {
                // TODO Add PropertyEnum using reflection for valid set of user inputs and parsing 
                if (propertyBag.TryGetProperty<PropertyString>(MOVEMENT, out PropertyString property))
                {
                    Enum.TryParse(property.StringValue, out this.movement);
                }

                if (propertyBag.TryGetProperty("virtualKeyCode", out property))
                {
                    if (Enum.TryParse(property.StringValue, out VirtualKeyCode keyTemp))
                    {
                        this.key = keyTemp;
                    }
                }
            }
        }

        public override PropertyBag ToProperties(PropertyBag propertyBag)
        {
            base.ToProperties(propertyBag);

            propertyBag.Add(MOVEMENT, new PropertyString(this.Movement.ToString()));
            propertyBag.Add("virtualKeyCode", new PropertyString(this.key.ToString()));

            return propertyBag;
        }
    }
}

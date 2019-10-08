using System;
using System.ComponentModel;
using Glue.Events;
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
        private const string MOVEMENT = "movement";
        public static readonly IntPtr INJECTION_ID = new IntPtr(0xD00D);
        public Nullable<VirtualKeyCode> Key => key;

        public ButtonStates Movement { get => movement; set => movement = value; }

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        private ButtonStates movement;

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        private Nullable<VirtualKeyCode> key;

        [JsonProperty]
        [DefaultValue (50)]
        // Options - if specified will be used to schedule if ButtonStates is CLICK
        protected long timeClickMS = 50;

        // Generated only if scheduled by ActionTyping with a string
        private Nullable<INPUT> input;          

        private static readonly WindowsInputMessageDispatcher DISPATCHER = new WindowsInputMessageDispatcher();
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [JsonConstructor]
        public ActionKey(long timeDelayMS, VirtualKeyCode key, ButtonStates movement) : base(timeDelayMS)
        {
            this.key = key;
            this.Movement = movement;
            this.input = null;
            this.Type = ActionType.Keyboard;
        }

        internal ActionKey(long timeDelayMS, INPUT input, ButtonStates movement) : base(timeDelayMS)
        {
            this.input=input;
            this.key = null;
            this.Movement = movement;
            this.Type = ActionType.Keyboard;
        }

        public override Action[] Schedule(long timeScheduleFrom)
        {
            Action[] scheduledActions;
            if (this.Movement == ButtonStates.Both)
            {
                long timeClickMS = 
                    this.timeClickMS > 0 
                        ? this.timeClickMS 
                        : this.delayMS;

                scheduledActions = new Action[]
                {
                    new ActionKey(this.DelayMS, (VirtualKeyCode) this.key, ButtonStates.Press)
                    {
                        ScheduledTick = timeScheduleFrom + this.delayMS
                    },
                    new ActionKey(timeClickMS, (VirtualKeyCode) this.key, ButtonStates.Release)
                    {
                        ScheduledTick = timeScheduleFrom + this.delayMS + timeClickMS
                    }
                };
            }
            else
            {
                scheduledActions = new Action[]
                {
                    new ActionKey(this.DelayMS, (VirtualKeyCode) this.key, this.Movement)
                    {
                        ScheduledTick = timeScheduleFrom + this.delayMS
                    }
                };
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
                    case ButtonStates.Press:
                        inputs = new InputBuilder().AddKeyDown((VirtualKeyCode) this.key).ToArray();
                        break;
                    case ButtonStates.Release:
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
                string message = String.Format("Played at tick {0:n0} dt {1:n0}ms: {2}-{3}",
                    now,                          // Time actually played
                    now - this.ScheduledTick,     // Time delta (how late were we?)
                    this.key,                       
                    this.Movement);
                LOGGER.Debug(message);
            }
        }

        public override string ToString()
        {
            string toString = base.ToString();

            Key key = Keyboard.GetKey((int) this.key);

            if (null != key)
            {
                toString += " " + ButtonStatesToString(this.Movement) + key.ToString();
            }

            return toString;
        }

        public static string ButtonStatesToString(ButtonStates ButtonStates)
        {
            string toString = "";

            switch (ButtonStates)
            {
                case ButtonStates.Both:
                    toString = "*";
                    break;

                case ButtonStates.Press:
                    toString = "+";
                    break;

                case ButtonStates.Release:
                    toString = "-";
                    break;

                default:
                    break;
            }

            return toString;
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

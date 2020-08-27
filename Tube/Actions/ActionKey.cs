using Glue.Events;
using Glue.Native;
using Glue.PropertyIO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.ComponentModel;
using WindowsInput;
using WindowsInput.Native;

namespace Glue.Actions
{
    public class ActionKey : Action
    {
        private const string MOVEMENT = "movement";
        private const string KEYNAME = "keyName";
        public static readonly IntPtr INJECTION_ID = new IntPtr(0xD00D);

        public Nullable<VirtualKeyCode> KeyCode => keyCode;
        public string KeyName => key;

        public ButtonStates Movement { get => movement; set => movement = value; }

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        private ButtonStates movement;

        [JsonProperty]
        private string key;

        [JsonProperty]
        [DefaultValue (50)]
        // Options - if specified will be used to schedule if ButtonStates is CLICK
        protected long timeClickMS = 50;

        private Nullable<VirtualKeyCode> keyCode;

        // Generated only if scheduled by ActionTyping with a string, or similar external action
        private Nullable<INPUT> input;          

        private static readonly WindowsInputMessageDispatcher DISPATCHER = new WindowsInputMessageDispatcher();
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [JsonConstructor]
        public ActionKey(long timeDelayMS, string key, ButtonStates movement) : base(timeDelayMS)
        {
            this.keyCode = (VirtualKeyCode) Keyboard.GetKey(key).Keys;
            this.key = key;
            this.movement = movement;
            this.input = null;
            this.Type = ActionType.Keyboard;
        }

        public ActionKey(long timeDelayMS, VirtualKeyCode virtualKeyCode, ButtonStates movement) : base(timeDelayMS)
        {
            this.key = Keyboard.GetKeyName((int) virtualKeyCode);
            this.keyCode = virtualKeyCode;
            this.Movement = movement;
            this.input = null;
            this.Type = ActionType.Keyboard;
        }

        //
        // Used by ActionTyping or similar other actions to schedule keyboard events 
        // Never to be serialized when constructed this way. Only scheduled.
        //
        internal ActionKey(long timeDelayMS, INPUT input, ButtonStates movement) : base(timeDelayMS)
        {
            this.input = input;
            this.keyCode = null;
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
                    new ActionKey(this.DelayMS, (VirtualKeyCode) this.keyCode, ButtonStates.Press)
                    {
                        ScheduledTick = timeScheduleFrom + this.delayMS
                    },
                    new ActionKey(timeClickMS, (VirtualKeyCode) this.keyCode, ButtonStates.Release)
                    {
                        ScheduledTick = timeScheduleFrom + this.delayMS + timeClickMS
                    }
                };
            }
            else
            {
                scheduledActions = new Action[]
                {
                    new ActionKey(this.DelayMS, (VirtualKeyCode) this.keyCode, this.Movement)
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

            // Use filter driver to simulate input
            if (null == this.input && Tube.IntercepterDriverWrapper.IsLoaded)
            {
                Glue.Key glueKey = Keyboard.GetKey((int) this.KeyCode);
                Interceptor.Keys simulatedDriverKey = glueKey.InterceptorKey;

                if (0 == simulatedDriverKey)
                {
                    LOGGER.Warn(
                        "Action attempted to simulate key that doesn't exist in the filter driver: virtualKeyCode = " 
                        + glueKey.Display + "simulatedDriverCode = " + simulatedDriverKey );
                }

                else
                {
                    Tube.IntercepterDriverWrapper.SendKey(
                        simulatedDriverKey, 
                        Movement == ButtonStates.Press 
                            ? Interceptor.KeyState.Down 
                            : Interceptor.KeyState.Up);
                }
            }
            // Use SendInput() API to simulate input
            else
            {
                // Constructor only supplied virtual key code - create INPUT 
                if (null == this.input)
                {
                    switch (this.Movement)
                    {
                        case ButtonStates.Press:
                            inputs = new InputBuilder().AddKeyDown((VirtualKeyCode) this.keyCode).ToArray();
                            break;
                        case ButtonStates.Release:
                            inputs = new InputBuilder().AddKeyUp((VirtualKeyCode) this.keyCode).ToArray();
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
            }

            if (LOGGER.IsDebugEnabled)
            {
                long now = TimeProvider.GetTickCount();
                string message = String.Format("Played at tick {0:n0} dt {1:n0}ms: {2}-{3}",
                    now,                          // Time actually played
                    now - this.ScheduledTick,     // Time delta (how late were we?)
                    this.KeyName,                       
                    this.Movement);
                LOGGER.Debug(message);
            }
        }

        public override string ToString()
        {
            string toString = base.ToString();

            if (null != this.keyCode)
            {
                Key key = Keyboard.GetKey((int) this.keyCode);

                if (null != key)
                {
                    toString += " " + ButtonStatesToString(this.Movement) + key.ToString();
                }
            }

            if (null != this.input)
            {
                toString += this.input.ToString();
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

        // 
        // TODO ponder making FromProperties all constructors that operate on property bags
        // 
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

                if (propertyBag.TryGetProperty(KEYNAME, out property))
                {
                    this.key = property.StringValue;
                    this.keyCode = (VirtualKeyCode) Keyboard.GetKey(property.StringValue).Keys;
                }
            }
        }

        public override PropertyBag ToProperties(PropertyBag propertyBag)
        {
            base.ToProperties(propertyBag);

            propertyBag.Add(MOVEMENT, new PropertyString(this.Movement.ToString()));
            propertyBag.Add(KEYNAME, new PropertyString(this.key));

            return propertyBag;
        }
    }
}

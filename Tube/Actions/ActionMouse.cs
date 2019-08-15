using System;
using System.ComponentModel;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using WindowsInput;

namespace Glue.Actions
{
    class ActionMouse : Action
    {
        public enum CoordinateMode
        {
            ABSOLUTE,
            ABSOLUTE_VIRTUAL_DESKTOP,
            RELATIVE,
        }

        public enum ActionButton
        {
            LEFT,
            MIDDLE,
            RIGHT,
        }

        public enum MoveType
        {
            MOVE,
            PRESS,
            RELEASE,
            CLICK,
        }

        [JsonConstructor]
        public ActionMouse(
            long timeDelayMS, 
            MoveType moveType, 
            CoordinateMode mode, 
            ActionButton button, 
            int moveX, 
            int moveY) : base(timeDelayMS)
        {
            this.moveType = moveType;
            this.mode = mode;
            this.button = button;
            this.moveX = moveX;
            this.moveY = moveY;
            this.Type = ActionType.MOUSE;
        }

        public static readonly IntPtr INJECTION_ID = new IntPtr(0xF00D);

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        private readonly MoveType moveType;

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        private readonly CoordinateMode mode;

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        private readonly ActionButton button;

        [JsonProperty]
        [DefaultValue(-1)]
        private readonly int moveX = -1;
        
        [JsonProperty]
        [DefaultValue(-1)]
        private readonly int moveY = -1;

        private static readonly WindowsInputMessageDispatcher DISPATCHER = new WindowsInputMessageDispatcher();
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ActionMouse(ActionMouse copyFrom) : base(copyFrom)
        {
            this.moveX = copyFrom.moveX;
            this.moveY = copyFrom.moveY;
            this.moveType = copyFrom.moveType;
            this.mode = copyFrom.mode;
            this.Type = copyFrom.Type;
        }

        public override Action[] Schedule(long timeScheduleFrom)
        {
            ActionMouse scheduledCopy = new ActionMouse(this)
            {
                ScheduledTick = timeScheduleFrom + this.delayMS
            };

            return new Action[] {scheduledCopy};
        }

        public static int NormalizeX(int xNative)
        {
            //
            //             xscreen                       
            // 65,535 *     -------             =       xnormalized     
            //            xresolution                 

            return 65535 * (xNative / Screen.PrimaryScreen.Bounds.Width);
        }

        public static int NormalizeY(int yNative)
        {
            return 65535 * (yNative / Screen.PrimaryScreen.Bounds.Height);
        }

        public override void Play()
        {
            /*
            INPUT[] inputs = null;

            switch (this.mode)
            {
                case CoordinateMode.ABSOLUTE:
                    inputs = new InputBuilder().AddAbsoluteMouseMovement(this.moveX, this.moveY).ToArray();
                    break;
                case CoordinateMode.ABSOLUTE_VIRTUAL_DESKTOP:
                    inputs = new InputBuilder().AddAbsoluteMouseMovementOnVirtualDesktop(this.moveX, this.moveY).ToArray();
                    break;
                case CoordinateMode.RELATIVE:
                    inputs = new InputBuilder().AddRelativeMouseMovement(this.moveX, this.moveY).ToArray();
                    break;

                default:

                    InputBuilder inputBuilder = new InputBuilder();
                    // Only use coordinates if they're specified
                    if ((this.moveX >= 0) && (this.moveY >= 0))
                    {
                        inputBuilder.AddAbsoluteMouseMovement(
                            NormalizeX(this.moveX), 
                            NormalizeY(this.moveY));
                    }
                    inputBuilder.AddMouseButtonClick(MouseButton.LeftButton);
                    inputs = inputBuilder.ToArray();

                    break;
                case Mode.CLICK_MIDDLE: 
                    inputs = new InputBuilder().AddMouseButtonClick(MouseButton.MiddleButton).ToArray();
                    break;
                case Mode.CLICK_RIGHT:
                    inputs = new InputBuilder().AddMouseButtonClick(MouseButton.RightButton).ToArray();
                    break;
            }

            DISPATCHER.DispatchInput(inputs);

            if (LOGGER.IsDebugEnabled)
            {
                long now = TimeProvider.GetTickCount();
                string message = String.Format("   Played at tick {0:n0} dt {1}ms: {4} ({2},{3})",
                    now,                            // Time actually played
                    now - this.TimeScheduledMS,     // Time delta (how late were we?)
                    this.moveX,
                    this.moveY,
                    this.movement);
                LOGGER.Debug(message);
            }
        */
        }
        public override string ToString()
        {
            return base.ToString() + " " + this.Type.ToString() + "(" + this.moveX + ", " + this.moveY + ")";
        }
    }
}

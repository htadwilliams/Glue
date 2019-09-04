using System;
using System.ComponentModel;
using System.Windows.Forms;
using Glue.Native;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using WindowsInput;

namespace Glue.Actions
{
    class ActionMouse : Action
    {
        public enum CoordinateMode
        {
            PIXEL,
            ABSOLUTE,
            ABSOLUTE_VIRTUAL_DESKTOP,
            RELATIVE,
            NONE,
        }

        public enum ClickType
        {
            PRESS,
            RELEASE,
            CLICK,
            DOUBLE,
            NONE
        }

        [JsonConstructor]
        public ActionMouse(
            long timeDelayMS, 
            ClickType clickType, 
            CoordinateMode mode, 
            MouseButton button, 
            int xButtonId,
            int moveX, 
            int moveY) : base(timeDelayMS)
        {
            this.clickType = clickType;
            this.mode = mode;
            this.button = button;
            this.xMove = moveX;
            this.yMove = moveY;
            this.xButtonId = xButtonId;
            this.Type = ActionType.MOUSE;
        }

        // Convenience constructors 
        public ActionMouse(
            long timeDelayMS,
            CoordinateMode mode,
            int moveX,
            int moveY
            ) 
            : this (timeDelayMS, ClickType.NONE, mode, MouseButton.LeftButton, -1, moveX, moveY)
        {
        }

        public ActionMouse(
            long timeDelayMS,
            ClickType clickType,
            MouseButton mouseButton
            ) 
            : this (timeDelayMS, clickType, CoordinateMode.NONE, mouseButton, -1, 0, 0)
        {
        }

        public ActionMouse(
            long timeDelayMS,
            ClickType clickType,
            int xButtonId) 
            : this (timeDelayMS, clickType, CoordinateMode.NONE, MouseButton.LeftButton, xButtonId, 0, 0)
        {
        }

        public ActionMouse(
            long timeDelayMS,
            ClickType clickType,
            CoordinateMode mode,
            MouseButton mouseButton,
            int xClick,
            int yClick) 
            : this (timeDelayMS, clickType, mode, mouseButton, -1, xClick, yClick)
        {
        }

        public ActionMouse(
            long timeDelayMS,
            ClickType clickType,
            CoordinateMode mode,
            int xButtonId,
            int xClick,
            int yClick) 
            : this (timeDelayMS, clickType, mode, MouseButton.LeftButton, xButtonId, xClick, yClick)
        {
        }

        public static readonly IntPtr INJECTION_ID = new IntPtr(0xF00D);

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        private readonly ClickType clickType;

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        private readonly CoordinateMode mode;

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        private readonly MouseButton button;

        [JsonProperty]
        [DefaultValue(0)]
        private readonly int xMove = 0;
        
        [JsonProperty]
        [DefaultValue(0)]
        private readonly int yMove = 0;

        [JsonProperty]
        [DefaultValue(-1)]
        private readonly int xButtonId = -1;

        private static readonly WindowsInputMessageDispatcher DISPATCHER = new WindowsInputMessageDispatcher();
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ActionMouse(ActionMouse copyFrom) : base(copyFrom)
        {
            this.xMove = copyFrom.xMove;
            this.yMove = copyFrom.yMove;
            this.clickType = copyFrom.clickType;
            this.mode = copyFrom.mode;
            this.Type = copyFrom.Type;
            this.button = copyFrom.button;
            this.xButtonId = copyFrom.xButtonId;
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
            // Force multiply before divide or else rounding errors 
            return (65535 * xNative) / Screen.PrimaryScreen.Bounds.Width;
        }

        public static int NormalizeY(int yNative)
        {
            return (65535 * yNative) / Screen.PrimaryScreen.Bounds.Height;
        }

        public override void Play()
        {
            InputBuilder inputBuilder = new InputBuilder(); 

            CoordinateMode modeActual = this.mode;
            int xActual = this.xMove;
            int yActual = this.yMove;

            // Absolute mode in pixels isn't supported by SendInput() API 
            // Glue supports it with this translation 
            if (this.mode == CoordinateMode.PIXEL)
            {
                modeActual = CoordinateMode.ABSOLUTE;
                xActual = NormalizeX(this.xMove);
                yActual = NormalizeY(this.yMove);
            }

            // Add movement if one is specified
            switch (modeActual)
            {
                case CoordinateMode.ABSOLUTE:
                    inputBuilder.AddAbsoluteMouseMovement(xActual, yActual);
                break;

                case CoordinateMode.ABSOLUTE_VIRTUAL_DESKTOP:
                    inputBuilder.AddAbsoluteMouseMovementOnVirtualDesktop(xActual, yActual);
                break;

                case CoordinateMode.RELATIVE:
                    inputBuilder.AddRelativeMouseMovement(xActual, yActual);
                break;

                case CoordinateMode.NONE:
                default:
                    // If none specified don't do anything
                break;
            }

            switch (this.clickType)
            {
                case ClickType.PRESS:
                    if (-1 != this.xButtonId)
                    {
                        inputBuilder.AddMouseXButtonDown(this.xButtonId);
                    }
                    else 
                    {
                        inputBuilder.AddMouseButtonDown(this.button);
                    }
                break;

                case ClickType.RELEASE:
                    if (-1 != this.xButtonId)
                    {
                        inputBuilder.AddMouseXButtonUp(this.xButtonId);
                    }
                    else 
                    {
                        inputBuilder.AddMouseButtonUp(this.button);
                    }
                break;

                case ClickType.DOUBLE:
                    if (-1 != this.xButtonId)
                    {
                        inputBuilder.AddMouseXButtonDoubleClick(this.xButtonId);
                    }
                    else 
                    {
                        inputBuilder.AddMouseButtonDoubleClick(this.button);
                    }
                break;

                case ClickType.CLICK:
                    if (-1 != this.xButtonId)
                    {
                        inputBuilder.AddMouseXButtonClick(this.xButtonId);
                    }
                    else 
                    {
                        inputBuilder.AddMouseButtonClick(this.button);
                    }
                break;

                case ClickType.NONE:
                default:
                    // Do nothing
                break;
            }

            if (LOGGER.IsDebugEnabled)
            {
                long now = TimeProvider.GetTickCount();
                string message = String.Format(
                    "   Played at tick {0:n0} dt {1:n0}ms: {2} {3} {4} {5} ({6:n0}, {7:n0})",
                    now,                          // Time actually played
                    now - this.ScheduledTick,     // Time delta (how late were we?)
                    this.clickType,
                    this.button,
                    this.xButtonId,
                    this.mode,                       
                    this.xMove,
                    this.yMove);
                LOGGER.Debug(message);
            }


            DISPATCHER.DispatchInput(inputBuilder.ToArray());
        }

        public override string ToString()
        {
            // TODO make ActionMouse.ToString() MUCH friendlier for display purposes
            return String.Format(
                "{0} {1} {2} {3} {4} ({5:n0}, {6:n0})", 
                base.ToString(),
                this.clickType,
                this.button,
                this.xButtonId,
                this.mode,
                this.xMove,
                this.yMove
                );
        }
    }
}

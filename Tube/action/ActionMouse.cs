using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glue.native;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using WindowsInput;
using WindowsInput.Native;

namespace Glue
{
    class ActionMouse : Action
    {
        public enum Movement
        {
            ABSOLUTE,
            ABSOLUTE_VIRTUAL_DESKTOP,
            RELATIVE
        }

        public static readonly IntPtr INJECTION_ID = new IntPtr(0xF00D);

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        private readonly Movement movement;

        [JsonProperty]
        private readonly int moveX;
        
        [JsonProperty]
        private readonly int moveY;

        private static readonly WindowsInputMessageDispatcher DISPATCHER = new WindowsInputMessageDispatcher();
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [JsonConstructor]
        public ActionMouse(Movement movement, int moveX, int moveY, long timeTriggerMS)
        {
            this.moveX = moveX;
            this.moveY = moveY;
            this.movement = movement;
            this.timeTriggerMS = timeTriggerMS;
            this.Type = "MOUSE";
        }

        public ActionMouse(ActionMouse copyFrom)
        {
            this.moveX = copyFrom.moveX;
            this.moveY = copyFrom.moveY;
            this.movement = copyFrom.movement;
            this.timeTriggerMS = copyFrom.TimeTriggerMS;
            this.Type = copyFrom.Type;
        }

        public override Action[] Schedule()
        {
            ActionMouse scheduledCopy = new ActionMouse(this)
            {
                TimeScheduledMS = TimeProvider.GetTickCount() + this.timeTriggerMS
            };

            if (LOGGER.IsDebugEnabled)
            {
                string message = String.Format("Scheduled at tick {0:n0} in {1}ms: {4} ({2},{3})",
                    scheduledCopy.TimeScheduledMS,      // Absolute time scheduled to play
                    this.timeTriggerMS,                 // Time relative to prevous action
                    this.moveX,
                    this.moveY,
                    this.movement);
                LOGGER.Debug(message);
            }

            return new Action[] {scheduledCopy};
        }

        public override void Play()
        {
            INPUT[] inputs = null;

            switch (this.movement)
            {
                case Movement.ABSOLUTE:
                    inputs = new InputBuilder().AddAbsoluteMouseMovement(this.moveX, this.moveY).ToArray();
                    break;

                case Movement.ABSOLUTE_VIRTUAL_DESKTOP:
                    inputs = new InputBuilder().AddAbsoluteMouseMovementOnVirtualDesktop(this.moveX, this.moveY).ToArray();
                    break;
                
                case Movement.RELATIVE:
                    inputs = new InputBuilder().AddRelativeMouseMovement(this.moveX, this.moveY).ToArray();
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
        }
    }
}

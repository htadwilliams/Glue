using System;
using SharpDX.DirectInput;

namespace Glue.Events
{
    public class EventController
    {
        public enum EventType
        {
            Unknown = 0,
            Button,
            POV,
            Axis,
        }

        public Joystick Joystick { get; }
        public JoystickUpdate JoystickUpdate { get; }

        /// <summary>
        /// Will always be -1 if EventType is not Button
        /// </summary>
        public int Button { get; } = -1;

        /// <summary>
        /// Will always be ButtonStates.Release if EventType is not Button
        /// </summary>
        public ButtonStates ButtonState { get; } = ButtonStates.Release;

        /// <summary>
        /// Will always be POVStates.Release if EventType is not POV
        /// </summary>
        public POVStates POVState { get; }

        /// <summary>
        /// Determined by JoystickUpdate.Offset as documented by MSDN (DirectX)
        /// </summary>
        public EventType Type { get; }

        public EventController(Joystick joystick, JoystickUpdate joystickUpdate) : base()
        {
            Joystick = joystick;
            JoystickUpdate = joystickUpdate;
            Type = GetEventType();
            POVState = GetPOVState();
            Button = GetButton();
            ButtonState = GetButtonState();
        }

        private ButtonStates GetButtonState()
        {
            if (Type == EventType.Button && ButtonValues.Press == (ButtonValues) JoystickUpdate.Value)
            {
                return ButtonStates.Press;
            }
            return ButtonStates.Release;
        }

        private EventType GetEventType()
        {
            EventType eventType = EventType.Unknown;

            if (IsButtonOffset(JoystickUpdate.Offset))
            {
                eventType = EventType.Button;
            }
            else if (DirectInputManager.OffsetsPOV.Contains(JoystickUpdate.Offset))
            {
                eventType = EventType.POV;
            }
            else if (DirectInputManager.OffsetsAxis.Contains(JoystickUpdate.Offset))
            {
                eventType = EventType.Axis;
            }

            return eventType;
        }

        private int GetButton()
        {
            return (int) (JoystickUpdate.Offset - JoystickOffset.Buttons0);
        }

        private POVStates GetPOVState()
        {
            if (Type == EventType.POV)
            {
                return (POVStates) JoystickUpdate.Value;
            }
            else
            {
                return POVStates.Release;
            }
        }

        private static bool IsButtonOffset(JoystickOffset offset)
        {
            return (
                offset >= JoystickOffset.Buttons0 && 
                offset <= JoystickOffset.Buttons127
                );
        }
    }
}

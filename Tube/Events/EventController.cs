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
            Hat,
            Axis,
        }

        public Joystick Joystick { get; }
        public JoystickUpdate JoystickUpdate { get; }
        public int Button { get; } = -1;
        public ButtonStates ButtonState { get; }
        public EventType Type { get; }

        public EventController(Joystick joystick, JoystickUpdate joystickUpdate) : base()
        {
            Joystick = joystick;
            JoystickUpdate = joystickUpdate;
            Type = EventTypeFromOffset(joystickUpdate.Offset);
            Button = GetButton(JoystickUpdate.Offset);

            ButtonState = joystickUpdate.Value == (int) ButtonValues.Press 
                ? ButtonStates.Press 
                : ButtonStates.Release;
        }

        private static EventType EventTypeFromOffset(JoystickOffset joystickOffset)
        {
            EventType eventType = EventType.Unknown;

            if (IsButtonOffset(joystickOffset))
            {
                eventType = EventType.Button;
            }
            else if (DirectInputManager.OffsetsHat.Contains(joystickOffset))
            {
                eventType = EventType.Hat;
            }
            else if (DirectInputManager.OffsetsAxis.Contains(joystickOffset))
            {
                eventType = EventType.Axis;
            }

            return eventType;
        }

        private static int GetButton(JoystickOffset offset)
        {
            return (int) (offset - JoystickOffset.Buttons0);
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

using System;
using SharpDX.DirectInput;

namespace Glue.Events
{
    public class EventController
    {
        public enum EventType
        {
            Button = 0,
            Hat = 1,
            Axis = 2,
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

            if (IsButton(joystickUpdate.Offset))
            {
                Type = EventType.Button;
                Button = GetButton(JoystickUpdate.Offset);
                ButtonState = joystickUpdate.Value == (int) ButtonValues.Press 
                    ? ButtonStates.Press 
                    : ButtonStates.Release;
            }
            else if (DirectInputManager.OffsetsHat.Contains(joystickUpdate.Offset))
            {
                Type = EventType.Hat;
            }
            else if (DirectInputManager.OffsetsAxis.Contains(joystickUpdate.Offset))
            {
                Type = EventType.Axis;
            }
        }

        private static int GetButton(JoystickOffset offset)
        {
            return (int) (offset - JoystickOffset.Buttons0);
        }

        private static bool IsButton(JoystickOffset offset)
        {
            return (
                offset >= JoystickOffset.Buttons0 && 
                offset <= JoystickOffset.Buttons127
                );
        }
    }
}

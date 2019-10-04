using SharpDX.DirectInput;

namespace Glue.Event
{
    public class EventController
    {
        public Joystick Joystick { get; }
        public JoystickUpdate JoystickUpdate { get; }

        public EventController(Joystick joystick, JoystickUpdate joystickUpdate) : base()
        {
            Joystick = joystick;
            JoystickUpdate = joystickUpdate;
        }
    }
}

using SharpDX.DirectInput;
using System;

namespace Glue.NerfDX
{
    /// <summary>
    /// Fired via EventBus when an update is generated from a controller
    /// </summary>
    public class ControllerEventArgs : EventArgs
    {
        public JoystickUpdate Update => update;
        public Joystick Joystick => joystick;

        private readonly JoystickUpdate update;
        private readonly Joystick joystick;

        public ControllerEventArgs(Joystick joystick, JoystickUpdate update)
        {
            this.update = update;
            this.joystick = joystick;
        }
    }
}

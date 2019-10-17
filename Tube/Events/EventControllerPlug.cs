using SharpDX.DirectInput;
using System.Collections.ObjectModel;

namespace Glue.Events
{
    public class EventControllersChanged : Event
    {
        public ReadOnlyCollection<Joystick> Joysticks { get; }

        public EventControllersChanged(ReadOnlyCollection<Joystick> joysticks)
        {
            Joysticks = joysticks;
        }
    }
}

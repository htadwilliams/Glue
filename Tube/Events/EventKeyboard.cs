using Glue.Native;
using System.Windows.Input;

namespace Glue.Events
{
    public class EventKeyboard
    {
        public int VirtualKeyCode { get; }
        public ButtonStates ButtonState { get; }

        public EventKeyboard(int virtualKeyCode, ButtonStates buttonState)
        {
            VirtualKeyCode = virtualKeyCode;
            ButtonState = buttonState;
        }
    }
}

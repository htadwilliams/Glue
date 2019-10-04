using Glue.Native;
using System.Windows.Input;

namespace Glue.Event
{
    public class EventKeyboard
    {
        public int VirtualKeyCode { get; }
        public ButtonStates ButtonState { get; }

        public EventKeyboard(int virtualKeyCode, ButtonStates buttonState) : base()
        {
            VirtualKeyCode = virtualKeyCode;
            ButtonState = buttonState;
        }
    }
}

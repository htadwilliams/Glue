using System.Windows.Forms;

namespace Glue.Event
{
    public class EventMouse
    {
        public int X { get; }
        public int Y { get; }
        public MouseButtons MouseButton { get; }

        public ButtonStates ButtonState { get; }

        public EventMouse(MouseButtons mouseButton, ButtonStates buttonState, int x, int y) : base()
        {
            MouseButton = mouseButton;
            ButtonState = buttonState;
            X = x;
            Y = y;
        }
    }
}

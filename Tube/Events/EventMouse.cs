using System.Windows.Forms;

namespace Glue.Events
{
    public class EventMouse
    {
        public enum WheelMoves
        {
            None = -1,
            Up = 0,
            Down = 1,
        }

        public int X { get; }
        public int Y { get; }
        public MouseButtons MouseButton { get; }
        public ButtonStates ButtonState { get; }
        public WheelMoves WheelMove { get; }

        public EventMouse(MouseButtons mouseButton, ButtonStates buttonState, WheelMoves wheelMove, int x, int y) : base()
        {
            MouseButton = mouseButton;
            ButtonState = buttonState;
            WheelMove = wheelMove;
            X = x;
            Y = y;
        }
    }
}

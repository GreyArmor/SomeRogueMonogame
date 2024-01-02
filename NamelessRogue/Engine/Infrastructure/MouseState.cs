using SharpDX;

public enum ButtonState
{
    Released, Pressed,
}

public struct MouseState
{
    public int X;
    public int Y;
    public bool RightPressed;
    public bool LeftPressed;
    public bool MiddlePressed;
    public int MouseWheelDelta;     
    public Point Position { get { return new Point(X, Y); } }
}
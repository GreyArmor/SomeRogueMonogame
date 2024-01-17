using Veldrid;

public enum ButtonState
{
    Released, Pressed,
}

public struct MouseState
{
    public MouseState(InputSnapshot snapshot)
    {
        X = (int)snapshot.MousePosition.X;
        Y = (int)snapshot.MousePosition.Y;
        RightPressed = snapshot.IsMouseDown(MouseButton.Right);
        LeftPressed  = snapshot.IsMouseDown(MouseButton.Left);
        MiddlePressed = snapshot.IsMouseDown(MouseButton.Middle);
        MouseWheelDelta = (int)snapshot.WheelDelta;
    }

    public int X;
    public int Y;
    public bool RightPressed;
    public bool LeftPressed;
    public bool MiddlePressed;
    public int MouseWheelDelta;     
    public Point Position { get { return new Point(X, Y); } }
}
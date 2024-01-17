using System;
using Veldrid;

public enum ButtonState
{
    Released, Pressed,
}

public struct MouseState : IEquatable<MouseState>
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

    public bool Equals(MouseState other)
    {
        return this == other; 
    }

    public static bool operator == (MouseState left, MouseState right)
    {
        return left.X == right.X && 
               left.Y == right.Y && 
               left.RightPressed == right.RightPressed &&
               left.LeftPressed == right.LeftPressed &&
               left.MiddlePressed == right.MiddlePressed &&
               left.MouseWheelDelta == right.MouseWheelDelta;
    }

    public static bool operator !=(MouseState left, MouseState right)
    {
        return !(left == right);
    }

}
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Utility;
using System;


public enum ButtonState
{
    Released, Pressed,
}

public class MouseState : IEquatable<MouseState>
{
    public MouseState(int x, int y, bool isRightPressed, bool isLeftPressed, bool isMiddlePressed, int middleMouseDelta)
    {
        X = x;
        Y =  y;
        RightPressed = isRightPressed;
        LeftPressed = isLeftPressed;
        MiddlePressed = isMiddlePressed;
        MouseWheelDelta = middleMouseDelta;
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
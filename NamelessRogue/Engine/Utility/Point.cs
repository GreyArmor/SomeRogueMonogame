using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Utility
{
    [DebuggerDisplay("{DebuggerDisplayString,nq}")]
    public struct Point : IEquatable<Point>
    {
        public int X;
        public int Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(Point other) => X.Equals(other.X) && Y.Equals(other.Y);
        public override bool Equals(object obj) => obj is Point p && Equals(p);
        public override int GetHashCode() => HashHelper.Combine(X.GetHashCode(), Y.GetHashCode());
        public override string ToString() => $"({X}, {Y})";

        public static bool operator ==(Point left, Point right) => left.Equals(right);
        public static bool operator !=(Point left, Point right) => !left.Equals(right);

        private string DebuggerDisplayString => ToString();
    }

    internal static class HashHelper
    {
        public static int Combine(int value1, int value2)
        {
            uint rol5 = ((uint)value1 << 5) | ((uint)value1 >> 27);
            return ((int)rol5 + value1) ^ value2;
        }
    }
}

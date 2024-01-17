using Veldrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace NamelessRogue.Engine.Utility
{
    public static class XnaPointExtensions
    {
        public static System.Drawing.Point ToPoint(this Point input)
        {
            return new System.Drawing.Point(input.X,input.Y);
        }

        public static Vector2 ToVector2(this Point input)
        {
            return new Vector2(input.X, input.Y);
        }
        public static Point Substract(this Point input, Point b)
        {
            return new Point(input.X - b.X, input.Y - b.Y);
        }

        public static Point Sum(this Point input, Point b)
        {
            return new Point(input.X + b.X, input.Y + b.Y);
        }

        public static Point ToPoint(this Vector2 input)
        {
            return new Point((int)input.X, (int)input.Y);
        }
    }
}

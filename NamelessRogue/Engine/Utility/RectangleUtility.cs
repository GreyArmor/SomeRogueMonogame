using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Utility
{
    public static class RectangleUtility
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="center"></param>
        /// <returns>
        /// topLeft, topright, bottomLeft, bottomRight
        /// </returns>
        public static Rectangle[] SplitRectangle(Rectangle rect, Point center)
        {
            int leftWidth = center.X - rect.Left;
            int rightWidth = rect.Right - center.X;
            int topHeight = center.Y - rect.Top;
            int bottomHeight = rect.Bottom - center.Y;

            Rectangle topLeft = new Rectangle(rect.Left, rect.Top, leftWidth, topHeight);
            Rectangle topRight = new Rectangle(center.X, rect.Top, rightWidth, topHeight);
            Rectangle bottomLeft = new Rectangle(rect.Left, center.Y, leftWidth, bottomHeight);
            Rectangle bottomRight = new Rectangle(center.X, center.Y, rightWidth, bottomHeight);

            return new[] { topLeft, topRight, bottomLeft, bottomRight };
        }

        public static Point[] RectangleCenters(Rectangle rect)
        {
            var center = rect.Center;
            return new[] { new Point(rect.Left, center.Y), new Point(rect.Right, center.Y), new Point(center.X, rect.Top), new Point(center.X, rect.Bottom) };
        }

        public static List<Point> GetRectangleOutline(Rectangle rect)
        {
            var points = new List<Point>();

            int left = rect.Left;
            int right = rect.Right;
            int top = rect.Top;
            int bottom = rect.Bottom;

            // Top edge (left to right)
            for (int x = left; x <= right; x++)
                points.Add(new Point(x, top));

            // Right edge (top+1 to bottom)
            for (int y = top + 1; y <= bottom; y++)
                points.Add(new Point(right, y));

            // Bottom edge (right-1 to left), only if height > 1
            if (bottom > top)
            {
                for (int x = right - 1; x >= left; x--)
                    points.Add(new Point(x, bottom));
            }

            // Left edge (bottom-1 to top+1), only if width > 1
            if (right > left)
            {
                for (int y = bottom - 1; y > top; y--)
                    points.Add(new Point(left, y));
            }

            return points;
        }
    }
}

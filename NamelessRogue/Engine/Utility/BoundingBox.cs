using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Components.ChunksAndTiles;

namespace NamelessRogue.Engine.Utility
{

    public class BoundingBox
    {
        private Point min;
        private Point max;
        Chunk leaf;
        Point getMin()
        {
            return min;
        }
        void setMin(Point min)
        {
            this.min = min;
        }
        Point getMax()
        {
            return max;
        }
        void setMax(Point max)
        {
            this.max = max;
        }

        public BoundingBox(Point min, Point max)
        {
            this.min = min;
            this.max = max;
        }

        public BoundingBox() { }

        public bool IsPointInside(int x, int y)
        {
            return x >= min.X && x <= max.X && y >= min.Y && y < max.Y;
        }

        public BoundingBox FromPoints(List<Point> points)
        {
            if (points == null || points.Count == 0)
            {
                throw new ArgumentException("Points list cannot be null or empty.");
            }
            int minX = int.MaxValue;
            int minY = int.MaxValue;
            int maxX = int.MinValue;
            int maxY = int.MinValue;
            foreach (var point in points)
            {
                if (point.X < minX) minX = point.X;
                if (point.Y < minY) minY = point.Y;
                if (point.X > maxX) maxX = point.X;
                if (point.Y > maxY) maxY = point.Y;
            }
            return new BoundingBox(new Point(minX, minY), new Point(maxX, maxY));
        }

        public bool Intersects(BoundingBox other)
        {
            return !(other.max.X < this.min.X ||
                     other.min.X > this.max.X ||
                     other.max.Y < this.min.Y ||
                     other.min.Y > this.max.Y);
        }
    }
}

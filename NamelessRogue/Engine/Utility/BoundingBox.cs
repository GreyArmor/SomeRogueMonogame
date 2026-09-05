using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;

namespace NamelessRogue.Engine.Utility
{

    public class BoundingBox
    {
        private Point min;
        private Point max;
        Chunk leaf;

        public Point Min { get { return min; } set { min = value; } }
        public Point Max { get { return max; } set { max = value; } }

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
        public BoundingBox GetExpanded(int expansionValue)
        {
            return new BoundingBox(new Point(min.X - expansionValue, min.Y - expansionValue), new Point(max.X + expansionValue, max.Y + expansionValue));
        }
        public bool Intersects(BoundingBox other)
        {
            return !(other.max.X < this.min.X ||
                     other.min.X > this.max.X ||
                     other.max.Y < this.min.Y ||
                     other.min.Y > this.max.Y);
        }

        public bool Neighboring(BoundingBox other)
        {
            var expanded = this.GetExpanded(1);
            other = other.GetExpanded(1);
            return expanded.Intersects(other);
        }
        internal static BoundingBox CreateMerged(BoundingBox mergedBounds, BoundingBox boundingBox)
        {
            int minX = Math.Min(mergedBounds.min.X, boundingBox.min.X);
            int minY = Math.Min(mergedBounds.min.Y, boundingBox.min.Y);
            int maxX = Math.Max(mergedBounds.max.X, boundingBox.max.X);
            int maxY = Math.Max(mergedBounds.max.Y, boundingBox.max.Y);
            return new BoundingBox(new Point(minX, minY), new Point(maxX, maxY));
        }
        public Rectangle ToRectangle()
        {
            int x = Min.X;
            int y = Min.Y;
            int width = Max.X - Min.X;
            int height = Max.Y - Min.Y;
            return new Rectangle(x, y, width, height);
        }
    }
}

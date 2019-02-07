using System;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Engine.Components.ChunksAndTiles;

namespace NamelessRogue.Engine.Engine.Utility
{
    [Serializable]
    public class BoundingBox {
        private Point min;
        private Point max;
        Chunk leaf;
        Point getMin() {
            return min;
        }
        void setMin(Point min) {
            this.min = min;
        }
        Point getMax() {
            return max;
        }
        void setMax(Point max) {
            this.max = max;
        }
	
        public BoundingBox(Point min, Point max)
        {
            this.min = min;
            this.max = max;
        }

        public bool IsPointInside(int x, int y)
        {
            return x >= min.X && x < max.X && y >= min.Y && y < max.Y;
        }
    }
}

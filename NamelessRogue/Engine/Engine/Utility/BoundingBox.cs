using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Engine.Components.ChunksAndTiles;

namespace NamelessRogue.Engine.Engine.Utility
{
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
	
        public bool isPointInside(int x, int y) {
            int bottomLeftX = min.X;
            int bottomLeftY = min.Y;
            int topRightX = max.X;
            int topRightY = max.Y;
		
            if(x>=bottomLeftX && x<topRightX&&y>=bottomLeftY&&y<topRightY){
                return true;
            }
            else{
                return false;
            }
        }
    }
}

using Microsoft.Xna.Framework;

namespace NamelessRogue.Engine.Engine.Components.Physical
{
    public class Position : Component {
        public Position(int x,int y)
        {
            p = new Point(x,y);
        }

        public Position()
        {
            p = new Point();
        }
        public Point p;
    }
}

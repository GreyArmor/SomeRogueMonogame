using Microsoft.Xna.Framework;

namespace NamelessRogue.Engine.Components.Physical
{
    public class Position : Component {
        public Position(int x,int y)
        {
            Point = new Point(x,y);
        }

        public Position()
        {
            Point = new Point();
        }
        public Point Point { get; set; }

        public override IComponent Clone()
        {
            return new Position(Point.X,Point.Y);
        }
    }
}

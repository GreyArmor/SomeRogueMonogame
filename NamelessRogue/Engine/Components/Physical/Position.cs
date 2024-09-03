using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Utility;

namespace NamelessRogue.Engine.Components.Physical
{
    public class Position : Component {

        public Vector3Int Point { get; set; }
        public Position(int x,int y, int z)
        {
           Point = new Vector3Int(x,y,z);
        }

        public int X { get { return Point.X; } }

        public int Y { get { return Point.Y; } }

        public int Z { get { return Point.Z; } }

        public Position()
        {
        }

        public override IComponent Clone()
        {
            return new Position(Point.X, Point.Y, Point.Z);
        }
    }
}

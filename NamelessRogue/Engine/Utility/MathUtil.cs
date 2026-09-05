using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Components.AI.Pathfinder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Point = Microsoft.Xna.Framework.Point;

namespace NamelessRogue.Engine.Utility
{
    public static class MathUtil
    {
        public static double AngleBetween(Vector2 vector1, Vector2 vector2)
        {
            double sin = vector1.X * vector2.Y - vector2.X * vector1.Y;
            double cos = vector1.X * vector2.X + vector1.Y * vector2.Y;

            return Math.Atan2(sin, cos) * (180 / Math.PI);
        }

        public static Dictionary<Direction, Vector3Int> DirectionVectors = new Dictionary<Direction, Vector3Int>
        {
            { Direction.North, new Vector3Int(0, -1, 0) },
            { Direction.South, new Vector3Int(0, 1, 0) },
            { Direction.East, new Vector3Int(1, 0, 0) },
            { Direction.West, new Vector3Int(-1, 0, 0) },
            { Direction.NorthEast, new Vector3Int(1, -1, 0) },
            { Direction.NorthWest, new Vector3Int(-1, -1, 0) },
            { Direction.SouthEast, new Vector3Int(1, 1, 0) },
            { Direction.SouthWest, new Vector3Int(-1, 1, 0) }
        };

        public static Dictionary<Direction, Point> DirectionPoints = new Dictionary<Direction, Point>
        {
            { Direction.North, new Point(0, -1) },
            { Direction.South, new Point(0, 1) },
            { Direction.East, new Point(1, 0) },
            { Direction.West, new Point(-1, 0) },
            { Direction.NorthEast, new Point(1, -1) },
            { Direction.NorthWest, new Point(-1, -1) },
            { Direction.SouthEast, new Point(1, 1) },
            { Direction.SouthWest, new Point(-1, 1) }
        };

        static Vector3Int GetDirectionVector(Direction direction)
        {
            if (DirectionVectors.TryGetValue(direction, out var vector))
            {
                return vector;
            }
            return Vector3Int.Zero;
        }
    }
}

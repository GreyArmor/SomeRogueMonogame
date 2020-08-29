using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.ChunksAndTiles;

namespace NamelessRogue.Engine.Engine.Utility
{
    public static class WorldLineDrawer
    {
  
        public static List<Tile> PlotLineAA(Point point0, Point point1, IWorldProvider world)
        {
            var result = new List<Tile>();
            int dx = Math.Abs(point1.X - point0.X), sx = point0.X < point1.X ? 1 : -1;
            int dy = Math.Abs(point1.Y - point0.Y), sy = point0.Y < point1.Y ? 1 : -1;
            int err = dx - dy, e2, x2; /* error value e_xy */

            int ed = (int) (dx + dy == 0 ? 1 : Math.Sqrt((float) dx * dx + (float) dy * dy));

            for (;;)
            {
                /* pixel loop */
                //setPixelAA(point0.X, point0.Y, 255 * abs(err - dx + dy) / ed);
                result.Add(world.GetTile(point0.X, point0.Y));
                e2 = err;
                x2 = point0.X;
                if (2 * e2 >= -dx)
                {
                    /* x step */
                    if (point0.X == point1.X) break;
                    if (e2 + dy < ed)
                    {
                        result.Add(world.GetTile(point0.X, point0.Y + sy));
                        // setPixelAA(point0.X, point0.Y + sy, 255 * (e2 + dy) / ed);
                    }

                    err -= dy;
                    point0.X += sx;
                }

                if (2 * e2 <= dy)
                {
                    /* y step */
                    if (point0.Y == point1.Y) break;
                    if (dx - e2 < ed)
                    {
                        result.Add(world.GetTile(x2 + sx, point0.Y + sy));
                        //  setPixelAA(x2 + sx, point0.Y, 255 * (dx - e2) / ed);
                    }

                    err += dx;
                    point0.Y += sy;
                }
            }

            return result;
        }


        public static List<Tile> BezierPath(Point[] points, IWorldProvider world)
        {
            var result = new List<Tile>();
            HashSet<Point> foundPoints = new HashSet<Point>();

            for (int i = 0; i < points.Length - 3; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    float t = j / 100f;

                    var p = GetPointOnBezierCurve(points[i].ToVector2(), points[i + 1].ToVector2(), points[i + 2].ToVector2(), points[i + 3].ToVector2(),t);
                    foundPoints.Add(p);

                }
            }


            var foundPointslist = foundPoints.ToList();
            for (int i = 0; i < foundPointslist.Count-1; i++)
            {
                result.AddRange(PlotLineAA(foundPointslist[i], foundPointslist[i + 1], world));
            }

            return result;
        }

        public static Point GetPointOnBezierCurve(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
        {
            float u = 1f - t;
            float t2 = t * t;
            float u2 = u * u;
            float u3 = u2 * u;
            float t3 = t2 * t;

            Vector2 result =
                (u3) * p0 +
                (3f * u2 * t) * p1 +
                (3f * u * t2) * p2 +
                (t3) * p3;

            return new Point((int)result.X, (int)result.Y);
        }


    }
}

        



    

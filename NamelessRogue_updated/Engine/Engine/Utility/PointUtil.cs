using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace NamelessRogue.Engine.Engine.Utility
{
    public class PointUtil {

        static int diagonalDistance(Point p0, Point p1) {
            int dx = p1.Y - p0.Y, dy = p1.X - p0.X;
            return Math.Max(Math.Abs(dx), Math.Abs(dy));
        }

        static Point roundPoint(Point p) {
            return new Point((int) Math.Round((double) p.Y), (int) Math.Round((double) p.X));
        }

        public static List<Point> getLine(Point p0, Point p1) {
            List<Point> points =  new List<Point>();
            int N = diagonalDistance(p0, p1);
            for (int step = 0; step <= N; step++) {
                float t = N == 0? (float) 0.0 : (float)step / N;
                points.Add(lerp_point(p0, p1, t));
            }
            return points;
        }

        static Point lerp_point(Point p0, Point p1, float t) {
            float x = lerp(p0.Y, p1.Y, t);
            float y = lerp(p0.X, p1.X, t);
            return new Point((int) Math.Round(x), (int) Math.Round(y));
        }

        static float lerp(float start, float end, float t) {
            return start + t * (end-start);
        }
    }
}

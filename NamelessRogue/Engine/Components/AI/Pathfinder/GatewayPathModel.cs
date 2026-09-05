using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.AI.Pathfinder
{
    internal class GatewayPathModel : IPathModel
    {
        public GatewayPathModel(Point pointA, Point pointB) {
            if (pointA == pointB)
            {
                throw new ArgumentException("Point A and Point B cannot be the same.");
            }
            if(Vector2.Distance(pointA.ToVector2(), pointB.ToVector2()) > 1.5f)
            {
                throw new ArgumentException("Point A and Point B must be adjacent.");
            }
            this.PointA = pointA;
            this.PointB = pointB;
        }
        public bool IsCalculated => true;

        public Point PointA { get; }
        public Point PointB { get; }

        public Point FinalPoint => throw new NotImplementedException();

        public void CalculateTo(Point to)
        {
            // No calculation needed for GatewayPathModel
        }

        public void ClearDebug()
        {}

        public void DrawDebug()
        {}

        public bool GetNextPoint(Point from, out Point? nextPoint)
        {            
            if(from == PointA)
            {
                nextPoint = PointB;
                return true;
            }
            else if (from == PointB)
            {
                nextPoint =  PointA;
                return true;
            }
            else
            {
                nextPoint = null;
                return false;
            }
        }        

        public void PaintWith(Direction direction)
        {
            throw new NotImplementedException();
        }
    }
}

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

        public void CalculateTo(Point to)
        {
            // No calculation needed for GatewayPathModel
        }

        public void ClearDebug()
        {}

        public void DrawDebug()
        {}

        public Point GetNextPoint(Point from)
        {            
            if(from == PointA)
            {
                return PointB;
            }
            else if (from == PointB)
            {
                return PointA;
            }
            else
            {
                throw new ArgumentException("The 'from' point must be either PointA or PointB.");
            }
        }

        public void PaintWith(FlowFieldDirection direction)
        {
            throw new NotImplementedException();
        }
    }
}

using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.AI.NonPlayerCharacter;
using NamelessRogue.Engine.Utility;
using NamelessRogue.shell;
using SharpDX.Direct3D9;
using System.Collections.Generic;

namespace NamelessRogue.Engine.Components.AI.Pathfinder
{
    internal class StraightLinePath : IPathModel
    {

        public List<Point> Points { get; set; } = new List<Point>();
        Dictionary<Point, Point> Nodes = new Dictionary<Point, Point>();
        private readonly IWorldProvider world;
        private readonly NamelessGame game;

        public StraightLinePath(Point from, IWorldProvider world, NamelessGame game)
        {
            From = from;
            this.world = world;
            this.game = game;
        }
        public bool IsCalculated { get; set; } = true;
        public Point To { get; private set; }
        public Point From { get; private set; }

        public Point FinalPoint { get; private set; }

        public void CalculateTo(Point to)
        {
            To = to;
            FinalPoint = to;
            AStarPathfinderSimple aStarPathfinderSimple = new AStarPathfinderSimple();
            Points = PointUtil.getLine(From, to);

            //foreach (Point point in Points)
            //{
            //    var neighbors = AllNeighborProviderFlowfield.GetNeighbors(point);
            //    foreach (var neighbor in neighbors)
            //    {
            //        Nodes[neighbor] = point;
            //    }
            //}

            // Points.Insert(0, From);
            for (int i = 0; i < Points.Count - 1; i++)
            {
                Point point = Points[i];
                Nodes[point] = Points[i + 1];
            }
            Nodes[Points[Points.Count - 1]] = Points[Points.Count - 1]; // Last point points to itself
        }

        public void ClearDebug()
        {
            // throw new NotImplementedException();
        }

        public void DrawDebug()
        {
            // throw new NotImplementedException();
        }

        public bool GetNextPoint(Point from, out Point? nextPoint)
        {
            if (Nodes.TryGetValue(from, out Point next))
            {
                nextPoint = next;
                return true;
            }
            nextPoint = null;
            return false;
        }

        public void PaintWith(FlowFieldDirection direction)
        {
            //throw new NotImplementedException();
        }
    }
}

using AStarNavigator;
using AStarNavigator.Algorithms;
using AStarNavigator.Providers;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.AI.NonPlayerCharacter;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.AI.Pathfinder
{
    internal class AStarListPath : IPathModel
    {

        public List<Point> Points { get; set; } = new List<Point>();
        int currentPointIndex = 0;
        private readonly IWorldProvider world;
        private readonly NamelessGame game;

        public AStarListPath(Point from, IWorldProvider world, NamelessGame game)
        {
            From = from;
            this.world = world;
            this.game = game;
        }
        public bool IsCalculated { get; set; } = true;
        public Point To { get; private set; }
        public Point From { get; private set; }

        public void CalculateTo(Point to)
        {
            To = to;
            AStarPathfinderSimple aStarPathfinderSimple = new AStarPathfinderSimple();
            Points = aStarPathfinderSimple.FindPath(From, to, world, game);
        }

        public void ClearDebug()
        {
           // throw new NotImplementedException();
        }

        public void DrawDebug()
        {
           // throw new NotImplementedException();
        }

        public Point GetNextPoint(Point from)
        {
            if (currentPointIndex < Points.Count)
            {
                var nextPoint = Points[currentPointIndex];
                currentPointIndex++;
                return nextPoint;
            }
            else
            {
                return from; // No more points, return the current position
            }
        }

        public void PaintWith(FlowFieldDirection direction)
        {
            //throw new NotImplementedException();
        }
    }
}

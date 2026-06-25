using AStarNavigator;
using AStarNavigator.Algorithms;
using AStarNavigator.Providers;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.shell;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Constants = NamelessRogue.Engine.Infrastructure.Constants;

namespace NamelessRogue.Engine.Components.AI.Pathfinder
{
	public class FlowRegionBlockedProvider : IBlockedProvider
	{
		private readonly IWorldProvider _worlldProvider;
		private readonly Point _destination;
		private readonly Point _start;
		private int counter = 0;
		private int maxSearches = 200;
		public FlowRegionBlockedProvider(IWorldProvider worlldProvider, Point destination, Point start)
		{
			_worlldProvider = worlldProvider;
			_destination = destination;
			_start = start;
		}

		public bool IsBlocked(AStarNavigator.Tile coord)
		{

			//counter++;

			//if (counter >= maxSearches)
			//{
			//	return true;
			//}
			var key = new Point((int)coord.X, (int)coord.Y);

            if (!_worlldProvider.GetChunks().ContainsKey(key))
			{
				if(key.X==5 && key.Y == 5)
				{
					key.ToString();
				}
                return true;
            }

   //         var chunk = _worlldProvider.GetChunks()[key];
			//if (chunk.NonGroundPassable)
			//{
			//	return true;
			//}

			var tile = _worlldProvider.GetTile((int)coord.X, (int)coord.Y, 0);
			if (counter >= maxSearches)
			{
				return true;
			}

			if (coord.X == _destination.X && coord.Y == _destination.Y)
			{
				return false;
			}

			if (coord.X == _start.X && coord.Y == _start.Y)
			{
				return false;
			}


			var isBlocked = !tile.IsPassable();
			return isBlocked;
		}
	}

	internal class FlowPathTuple
	{
		public FlowFieldPathModel ShortPath { get; set; }
		public FlowFieldPathModel FullPath { get; set; }
	}

	public class FlowFieldController
	{
		private readonly NamelessGame game;
		IWorldProvider world;
		int idCounter = 0;
		Dictionary<int, FlowPathTuple> currentPathModels = new Dictionary<int, FlowPathTuple>();
        Dictionary<string, int> waypointIdToPathId = new Dictionary<string, int>();

        public FlowFieldController(NamelessGame game, IWorldProvider world)
		{
			this.game = game;
			this.world = world;
		}

	
        //returns path id
        public int CalculateTo(Point to, Point from)
		{
			var realitychunks = world.GetRealityBubbleChunks();

            //ResetNodes();

          

            var toWorldPos = (to.ToVector2() / Constants.ChunkSize).ToPoint();
			var fromWorldPos = (from.ToVector2() / Constants.ChunkSize).ToPoint();

            TileNavigator navigator = new TileNavigator(
          new EmptyBlockedProvider(),
          new DiagonalNeighborProvider(),
          new PythagorasAlgorithm(),
          new ManhattanHeuristicAlgorithm());


            var path = navigator.Navigate(new Tile(fromWorldPos.X, fromWorldPos.Y), new Tile(toWorldPos.X, toWorldPos.Y));

			

			if (path == null)
			{
				return -1;
			}

			var pathOfPoints = path.Select(t => new Point((int)t.X, (int)t.Y)).ToList();

            var shortPathOfPoints = pathOfPoints.Take(3).ToList(); ;

			foreach (var point in pathOfPoints.ToList())
			{
				var neighbors = AllNeighborProviderFlowfield.GetNeighbors(point);
				pathOfPoints.AddRange(neighbors);
			}

			//foreach (var point in pathOfPoints.ToList())
			//{
			//	var neighbors = AllNeighborProviderFlowfield.GetNeighbors(point);
			//	pathOfPoints.AddRange(neighbors);
			//}


			foreach (var point in shortPathOfPoints.ToList())
			{
				var neighbors = AllNeighborProviderFlowfield.GetNeighbors(point);
				shortPathOfPoints.AddRange(neighbors);
			}

			//foreach (var point in shortPathOfPoints.ToList())
			//{
			//	var neighbors = AllNeighborProviderFlowfield.GetNeighbors(point);
			//	shortPathOfPoints.AddRange(neighbors);
			//}


			pathOfPoints = pathOfPoints.Distinct().ToList();
			shortPathOfPoints = shortPathOfPoints.Distinct().ToList();

			//clicked very closely to initial point, on the same chunk, so A* pathfinder returned an empty path
			if (!pathOfPoints.Any())
			{
				pathOfPoints.Insert(0, fromWorldPos);
			}

			if (!shortPathOfPoints.Any())
			{
				shortPathOfPoints = pathOfPoints;
			}

			Point closestShortPoint = shortPathOfPoints.First();

			foreach (var shortPathPoint in shortPathOfPoints)
			{
				var prevDistance = (closestShortPoint.ToVector2() - fromWorldPos.ToVector2()).LengthSquared();
				var newDist = (shortPathPoint.ToVector2() - fromWorldPos.ToVector2()).LengthSquared();

				if (newDist < prevDistance)
				{
					closestShortPoint = shortPathPoint;
				}
			}

			var centerOfClosestChunk = new Point(
				closestShortPoint.X * Constants.ChunkSize + (Constants.ChunkSize / 2),
				closestShortPoint.Y * Constants.ChunkSize + (Constants.ChunkSize / 2)
				);


			pathOfPoints = pathOfPoints.Where(p => realitychunks.ContainsKey(p)).ToList();
            shortPathOfPoints = shortPathOfPoints.Where(p => realitychunks.ContainsKey(p)).ToList();
            //foreach (var point in pathOfPoints.ToList())
            //{
            //    if (!realitychunks.ContainsKey(point))
            //    {
            //        return -1;
            //    }
            //}

			if(!pathOfPoints.Any())
			{
                return -1;
            }

			if(shortPathOfPoints.Count==0)
			{
				shortPathOfPoints = pathOfPoints;
			}

            //calculate long path asychronously
            var fullPath = new FlowFieldPathModel(game, pathOfPoints, world);

			var shortPath = new FlowFieldPathModel(game, shortPathOfPoints, world);

			var stopwatch = Stopwatch.StartNew();
			shortPath.ClaculateTo(centerOfClosestChunk);
			stopwatch.Stop();
			stopwatch.ToString();

			//Task.Factory.StartNew(() =>
			//{
				fullPath.ClaculateTo(to);
				fullPath.IsCalculated = true;
				fullPath.ClearDebug();
				fullPath.DrawDebug();
            //});

            idCounter++;
			currentPathModels.Add(idCounter, new FlowPathTuple() { FullPath = fullPath, ShortPath = shortPath });
			return idCounter;
		}
		
		public int CalculateToWaypoint(string waypointId, Point to, Point from)
		{
            var pathId = CalculateTo(to, from);
			waypointIdToPathId[waypointId] = pathId;
            return pathId;
        }

        public Point GetNextPoint(int pathId, Point from)
		{
			if (currentPathModels[pathId].FullPath.IsCalculated)
			{
				return currentPathModels[pathId].FullPath.GetNextPoint(from);
			}
			else
			{
				return currentPathModels[pathId].ShortPath.GetNextPoint(from);
			}
		}
	}
}

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

	public class PathfindingController
	{
		private readonly NamelessGame game;
		IWorldProvider world;
		int idCounter = 0;
		Dictionary<int, IPathModel> currentPathModels = new Dictionary<int, IPathModel>();
        Dictionary<string, int> waypointIdToPathId = new Dictionary<string, int>();

        public PathfindingController(NamelessGame game, IWorldProvider world)
		{
			this.game = game;
			this.world = world;
		}

        internal Dictionary<int, IPathModel> CurrentPathModels { get => currentPathModels; set => currentPathModels = value; }

		//returns path id
		public int CalculateTo(Point to, Point from)
		{
			var realityChunks = world.GetRealityBubbleChunks();

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

			foreach (var point in pathOfPoints.ToList())
			{
				var neighbors = AllNeighborProviderFlowfield.GetNeighbors(point);
				pathOfPoints.AddRange(neighbors);
			}


			foreach (var point in shortPathOfPoints.ToList())
			{
				var neighbors = AllNeighborProviderFlowfield.GetNeighbors(point);
				shortPathOfPoints.AddRange(neighbors);
			}

			foreach (var point in shortPathOfPoints.ToList())
			{
				var neighbors = AllNeighborProviderFlowfield.GetNeighbors(point);
				shortPathOfPoints.AddRange(neighbors);
			}


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


			pathOfPoints = pathOfPoints.Where(p => realityChunks.ContainsKey(p)).ToList();
			shortPathOfPoints = shortPathOfPoints.Where(p => realityChunks.ContainsKey(p)).ToList();
			//foreach (var point in pathOfPoints.ToList())
			//{
			//    if (!realitychunks.ContainsKey(point))
			//    {
			//        return -1;
			//    }
			//}

			if (!pathOfPoints.Any())
			{
				return -1;
			}

			if (shortPathOfPoints.Count == 0)
			{
				shortPathOfPoints = pathOfPoints;
			}

			//calculate long path asychronously
			var fullPath = new FlowFieldPathModel(game, pathOfPoints, world);

			var shortPath = new FlowFieldPathModel(game, shortPathOfPoints, world);

			var stopwatch = Stopwatch.StartNew();
			shortPath.CalculateTo(centerOfClosestChunk);
			stopwatch.Stop();
			stopwatch.ToString();

			Task.Factory.StartNew(() =>
			{
				fullPath.CalculateTo(to);
				fullPath.IsCalculated = true;
			});

			var flowPathTuple = new FlowPathTuple() { FullPath = fullPath, ShortPath = shortPath };

			idCounter++;
			currentPathModels.Add(idCounter, flowPathTuple);
			return idCounter;
		}


        public int SetDirectionForArea(FlowFieldDirection direction, Point min, Point max)
        {
            var realityChunks = world.GetRealityBubbleChunks();

            List<Point> pathOfPoints = new List<Point>();
            for (int x = min.X; x <= max.X; x++)
            {
                for (int y = min.Y; y <= max.Y; y++)
                {
                    pathOfPoints.Add(new Point(x, y));
                }
            }

            pathOfPoints = pathOfPoints.Where(p => realityChunks.ContainsKey(p)).ToList();

            if (pathOfPoints.Count == 0)
            {
                return -1;
            }

            var fullPath = new FlowFieldPathModel(game, pathOfPoints, world);
            fullPath.PaintWith(direction);

            var shortPath = fullPath;
            idCounter++;
            currentPathModels.Add(idCounter, new FlowPathTuple() { FullPath = fullPath, ShortPath = shortPath });
            return idCounter;
        }

        public int CalculateToForArea(Point to, Point min, Point max)
        {
            var toWorldPos = (to.ToVector2() / Constants.ChunkSize).ToPoint();
            List<Point> pathOfPoints = new List<Point>();
			for (int x = min.X; x <= max.X; x++)
			{
				for (int y = min.Y; y <= max.Y; y++)
				{
					pathOfPoints.Add(new Point(x, y));
                }
            }
            return CalculateForChunks(to, pathOfPoints);
        }

        public int CalculateToPointAStar(Point from, Point to)
        {
            AStarListPath aStarListPath = new AStarListPath(from, game.WorldProvider, game);
            aStarListPath.CalculateTo(to);
			            idCounter++;
            currentPathModels.Add(idCounter, aStarListPath);
            return idCounter;
        }

        public int CalculateToForAreas(Point to, List<Rectangle> areas)
        {
            var toWorldPos = (to.ToVector2() / Constants.ChunkSize).ToPoint();
            List<Point> pathOfPoints = new List<Point>();
            foreach (var area in areas)
            {
				for (int x = area.Left; x <= area.Right; x++)
				{
					for (int y = area.Top; y <= area.Bottom; y++)
					{
						pathOfPoints.Add(new Point(x, y));
					}
				}
            }
            return CalculateForChunks(to, pathOfPoints);
        }


        public int CalculateForChunks(Point to, List<Point> chunks)
        {
            var realityChunks = world.GetRealityBubbleChunks();

            var toWorldPos = (to.ToVector2() / Constants.ChunkSize).ToPoint();


			List<Point> pathOfPoints = chunks;

            pathOfPoints = pathOfPoints.Where(p => realityChunks.ContainsKey(p)).ToList();

            if (pathOfPoints.Count == 0)
            {
                return -1;
            }

            var fullPath = new FlowFieldPathModel(game, pathOfPoints, world);

            fullPath.CalculateTo(to);
            fullPath.IsCalculated = true;

            var shortPath = fullPath;
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

        public bool GetNextPoint(int pathId, Point from, out Point? nextPoint)
		{
            return currentPathModels[pathId].GetNextPoint(from, out nextPoint);
		}
	}
}

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

	//public class FlowFieldRegion
	//{
	//	public enum Neighbors : byte
	//	{
	//		NW, N, NE, W, E, SW, S, SE
	//	}

	//	public List<Neighbors> PassableNeighbors;

	//	public Point Coords;

	//}

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
			counter++;
			return false;
			var tile = _worlldProvider.GetTile((int)coord.X, (int)coord.Y);
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

	internal class FlowFieldModel
	{

		Point flowFieldWorldPosition;
		private readonly NamelessGame game;
		IWorldProvider world;
		int idCounter = 0;
		Dictionary<int, FlowPathTuple> currentPathModels = new Dictionary<int, FlowPathTuple>();

		//FlowFieldRegion[,] flowFieldRegions = null;

		public FlowFieldModel(NamelessGame game, IWorldProvider world)
		{
			this.game = game;
			this.world = world;

			navigator = new TileNavigator(
			new FlowRegionBlockedProvider(null, default(Point), default(Point)),
			new DiagonalNeighborProvider(),
			new PythagorasAlgorithm(),
			new ManhattanHeuristicAlgorithm());
		}

		void CalculatePassability()
		{

		}
		TileNavigator navigator;
		//returns path id
		public int ClaculateTo(Point to, Point from)
		{


			//ResetNodes();

			var toWorldPos = (to.ToVector2() / Constants.ChunkSize).ToPoint();
			var fromWorldPos = (from.ToVector2() / Constants.ChunkSize).ToPoint();

			var path = navigator.Navigate(new Tile(fromWorldPos.X, fromWorldPos.Y), new Tile(toWorldPos.X, toWorldPos.Y));


			var pathOfPoints = path.Select(t => new Point((int)t.X, (int)t.Y)).ToList();

			var shortPathOfPoints = pathOfPoints.Take(3).ToList(); ;

			foreach (var point in pathOfPoints.ToList())
			{
				var neighbors = DiagonalNeighborProviderFlowfield.GetNeighbors(point);
				pathOfPoints.AddRange(neighbors);
			}

			foreach (var point in pathOfPoints.ToList())
			{
				var neighbors = DiagonalNeighborProviderFlowfield.GetNeighbors(point);
				pathOfPoints.AddRange(neighbors);
			}


			foreach (var point in shortPathOfPoints.ToList())
			{
				var neighbors = DiagonalNeighborProviderFlowfield.GetNeighbors(point);
				shortPathOfPoints.AddRange(neighbors);
			}

			foreach (var point in shortPathOfPoints.ToList())
			{
				var neighbors = DiagonalNeighborProviderFlowfield.GetNeighbors(point);
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
				var prevDistance = (closestShortPoint.ToVector2() - fromWorldPos.ToVector2()).Length();
				var newDist = (shortPathPoint.ToVector2() - fromWorldPos.ToVector2()).Length();

				if (newDist > prevDistance)
				{
					closestShortPoint = shortPathPoint;
				}
			}

			var centerOfClosestChunk = new Point(
				closestShortPoint.X * Constants.ChunkSize + (Constants.ChunkSize / 2),
				closestShortPoint.Y * Constants.ChunkSize + (Constants.ChunkSize / 2)
				);

			//calculate long path asychronously
			var fullPath = new FlowFieldPathModel(game, pathOfPoints, world, flowFieldWorldPosition);

			var shortPath = new FlowFieldPathModel(game, shortPathOfPoints, world, flowFieldWorldPosition);

			var stopwatch = Stopwatch.StartNew();
			shortPath.ClaculateTo(centerOfClosestChunk);
			stopwatch.Stop();
			stopwatch.ToString();

			Task.Factory.StartNew(() =>
			{
				fullPath.ClaculateTo(to);
				fullPath.IsCalculated = true;
			});

			idCounter++;
			currentPathModels.Add(idCounter, new FlowPathTuple() { FullPath = fullPath, ShortPath = shortPath });
			return idCounter;
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

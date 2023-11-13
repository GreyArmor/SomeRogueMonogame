using AStarNavigator;
using AStarNavigator.Algorithms;
using AStarNavigator.Providers;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.shell;
using System.Collections.Generic;
using System.Linq;
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

	internal class FlowFieldModel
	{

		Point flowFieldWorldPosition;
		private readonly NamelessGame game;
		IWorldProvider world;
		int idCounter = 0;
		Dictionary<int, FlowFieldPathModel> currentPathModels = new Dictionary<int, FlowFieldPathModel>();

		//FlowFieldRegion[,] flowFieldRegions = null;

		public FlowFieldModel(NamelessGame game, IWorldProvider world)
		{
			this.game = game;
			this.world = world;
		}

		void CalculatePassability()
		{

		}

		//returns path id
		public int ClaculateTo(Point to, Point from)
		{

			//ResetNodes();

			var toWorldPos = (to.ToVector2()/Constants.ChunkSize).ToPoint();
			var fromWorldPos = (from.ToVector2() / Constants.ChunkSize).ToPoint(); ;
			var navigator = new TileNavigator(
				new FlowRegionBlockedProvider(this.world, toWorldPos, fromWorldPos),
				new DiagonalNeighborProvider(),
				new PythagorasAlgorithm(),
				new ManhattanHeuristicAlgorithm()
			);
			var path = navigator.Navigate(new Tile(fromWorldPos.X, fromWorldPos.Y), new Tile(toWorldPos.X, toWorldPos.Y));

			var pathOfPoints = path.Select(t => new Point((int)t.X, (int)t.Y)).ToList();

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


			pathOfPoints = pathOfPoints.Distinct().ToList();

			//clicked very closely to initial point, on the same chunk, so A* pathfinder returned an empty path
			if (!pathOfPoints.Any())
			{
				pathOfPoints.Insert(0, fromWorldPos);
			}
			var flowPath = new FlowFieldPathModel(game, pathOfPoints, world, flowFieldWorldPosition);

			flowPath.ClaculateTo(to);

			idCounter++;
			currentPathModels.Add(idCounter, flowPath);
			return idCounter;
		}

		public Point GetNextPoint(int pathId, Point from)
		{
			return currentPathModels[pathId].GetNextPoint(from);
		}
	}
}

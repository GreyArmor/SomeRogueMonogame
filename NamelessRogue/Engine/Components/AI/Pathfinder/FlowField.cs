using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using System.Collections.Generic;
using System.Linq;
using Constants = NamelessRogue.Engine.Infrastructure.Constants;

namespace NamelessRogue.Engine.Components.AI.Pathfinder
{
	internal class FlowFieldChunkModel : IFlowfield
	{
		private class FlowNode
		{
			public Point Next = new Point(-1, -1);
			public bool Occupied = false;
			public int IntegrationValue = 255;
			public int Cost = 1;
		}

		Point currentDestination;
		Point flowFieldWorldPosition;
		//used as the cost map
		IWorldProvider world;
		FlowNode[,] Nodes;

		public bool IsCalculated { get; internal set; }

		public FlowFieldChunkModel(IWorldProvider worldProvider)
		{
			world = worldProvider;
			var realityBubbleChunks = worldProvider.GetRealityBubbleChunks();
			var orderedChunks = realityBubbleChunks.Values.OrderBy(x => x.WorldPositionBottomLeftCorner.X + x.WorldPositionBottomLeftCorner.Y);
			var minChunk = orderedChunks.First();
			//var maxChunk = orderedChunks.Last();

			flowFieldWorldPosition = minChunk.WorldPositionBottomLeftCorner;

			//var flowFieldDimension = maxChunk.ChunkWorldMapLocationPoint - minChunk.ChunkWorldMapLocationPoint;

			var arrayDimension = Constants.ChunkSize * Constants.RealityBubbleRangeInChunks * 2;
			var flowFieldDimension = new Point(arrayDimension, arrayDimension);

			Nodes = new FlowNode[flowFieldDimension.X, flowFieldDimension.Y];
			for (int i = 0; i < arrayDimension; i++)
			{
				for (int j = 0; j < arrayDimension; j++)
				{
					Nodes[i, j] = new FlowNode();
				}
			}
		}

		void ResetNodes()
		{
			var arrayDimension = Constants.ChunkSize * Constants.RealityBubbleRangeInChunks * 2;
			for (int i = 0; i < arrayDimension; i++)
			{
				for (int j = 0; j < arrayDimension; j++)
				{
					Nodes[i, j].IntegrationValue = 255;
				}
			}
		}

		public void ClaculateTo(Point to)
		{

			ResetNodes();

			var toWorldPos = to - flowFieldWorldPosition;
			int arrayDimention = Nodes.GetLength(0);

			currentDestination = to;
			Queue<Point> closedPoints = new Queue<Point>();
			Queue<Point> openPoints = new Queue<Point>();
			openPoints.Enqueue(toWorldPos);


			//add impassable and cost Here

			//destination
			Nodes[toWorldPos.X, toWorldPos.Y] = new FlowNode() { IntegrationValue = 0, Cost = 0 };

			while (openPoints.Any())
			{
				var point = openPoints.Dequeue();
				var currentFlowNode = Nodes[point.X, point.Y];

				var neighbors = DiagonalNeighborProviderFlowfield.GetNeighbors(point);
				foreach (var neighborP in neighbors)
				{
					if (neighborP.X < 0 || neighborP.Y < 0 || neighborP.X >= arrayDimention || neighborP.Y >= arrayDimention)
					{
						continue;
					}
					var neighborNode = Nodes[neighborP.X, neighborP.Y];
					var integrationValue = neighborNode.Cost + currentFlowNode.IntegrationValue;
					if (integrationValue < neighborNode.IntegrationValue)
					{
						neighborNode.IntegrationValue = integrationValue;
						openPoints.Enqueue(neighborP);
					}
				}
			}


			for (int x = 0; x < arrayDimention; x++)
			{
				for (int y = 0; y < arrayDimention; y++)
				{
					var point = new Point(x, y);

					if (point == toWorldPos)
					{
						continue;
					}

					var neighbors = DiagonalNeighborProviderFlowfield.GetNeighbors(point);
					FlowNode bestCostFlowNode = null;
					Point bestPoint = new Point();
					foreach (var neighborP in neighbors)
					{
						if (neighborP.X < 0 || neighborP.Y < 0 || neighborP.X >= arrayDimention || neighborP.Y >= arrayDimention)
						{
							continue;
						}
						var neighborNode = Nodes[neighborP.X, neighborP.Y];
						if (bestCostFlowNode == null || bestCostFlowNode.IntegrationValue > neighborNode.IntegrationValue)
						{
							bestCostFlowNode = neighborNode;
							bestPoint = neighborP;
						}
					}
					Nodes[x, y].Next = bestPoint;
				}
			}
			IsCalculated = true;
		}

		public Point GetNextPoint(Point from)
		{
			var position = from - flowFieldWorldPosition;
			var next = Nodes[position.X, position.Y].Next;

			//TODO probably incorrect to do this, but for debug purposes leaving it like this
			if (next.X < 0)
			{
				return position;
			}

			return new Point(next.X + flowFieldWorldPosition.X, next.Y + flowFieldWorldPosition.Y);
		}
	}

	public static class DiagonalNeighborProviderFlowfield
	{
		private static readonly int[,] neighbors = new int[,]
		{
			{ 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 }, { -1, -1 }, { 1, -1 }, { 1, 1 }, { -1, 1 }
		};

		public static IEnumerable<Point> GetNeighbors(Point tile)
		{
			var result = new List<Point>();

			for (var i = 0; i < neighbors.GetLongLength(0); i++)
			{
				result.Add(new Point(
					x: tile.X + neighbors[i, 0],
					y: tile.Y + neighbors[i, 1]
				));
			}

			return result;
		}
	}

}

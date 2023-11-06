using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Systems._3DView;
using NamelessRogue.shell;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Constants = NamelessRogue.Engine.Infrastructure.Constants;
namespace NamelessRogue.Engine.Components.AI.Pathfinder
{
	internal class FlowFieldPathModel
	{
		string _key(int x, int y)
		{
			return x.ToString() + ':' + y.ToString();
		}

		string _keyP(Point p)
		{
			return _key(p.X, p.Y);
		}

		Point flowFieldWorldPosition;
		//used as the cost map
		IWorldProvider world;
		Dictionary<string, FlowNode> Nodes;

		public bool IsCalculated { get; internal set; }

		public FlowFieldPathModel(NamelessGame game, IEnumerable<Point> chunkPath, IWorldProvider worldProvider, Point worldPosition)
		{
			world = worldProvider;
			this.flowFieldWorldPosition = worldPosition;

			var chunks = new List<Chunk>();
			var realitychunks = worldProvider.GetRealityBubbleChunks();

			foreach (var chunkCoord in chunkPath)
			{
				if (realitychunks.TryGetValue(chunkCoord, out Chunk rbChunk))
				{
					chunks.Add(rbChunk);
				}
			}

			Nodes = new Dictionary<string, FlowNode>(chunks.Count * Constants.ChunkSize * Constants.ChunkSize);

			//fill the nodes
			foreach (var chunk in chunks)
			{

				//if (chunk != null)
				//{
				//	for (int i = 0; i < Infrastructure.Constants.ChunkSize; i++)
				//	{
				//		for (int j = 0; j < Infrastructure.Constants.ChunkSize; j++)
				//		{
				//			var tile = chunk.GetTileLocal(i, j);
				//			tile.Biome = Biomes.Mountain;
				//			tile.Terrain = TerrainTypes.Snow;


				//		}
				//	}
				//	UpdateChunkCommand updateChunkCommand = new UpdateChunkCommand(chunk.ChunkWorldMapLocationPoint);
				//	game.Commander.EnqueueCommand(updateChunkCommand);

				//}

				var location = chunk.WorldPositionBottomLeftCorner;
				for (int i = 0; i < Constants.ChunkSize; i++)
				{
					for (int j = 0; j < Constants.ChunkSize; j++)
					{
						var coordX = location.X + i;
						var coordY = location.Y + j;
						Nodes.Add(_key(coordX, coordY), new FlowNode()
						{
							Coordinate = new Point(coordX, coordY),
							Occupied = !world.GetTile(coordX,coordY).IsPassable()
						});
					}
				}
			}
		}

		void ResetNodes()
		{
			var arrayDimension = Constants.ChunkSize * Constants.RealityBubbleRangeInChunks * 2;
			foreach (var node in Nodes)
			{
				node.Value.IntegrationValue = int.MaxValue;
			}
		}

		public void ClaculateTo(Point to)
		{

			ResetNodes();

			var toWorldPos = to;
			//int arrayDimention = Nodes.GetLength(0);
			Queue<Point> closedPoints = new Queue<Point>();
			Queue<Point> openPoints = new Queue<Point>();
			openPoints.Enqueue(toWorldPos);


			//add impassable and cost Here

			//destination
			Nodes[_keyP(toWorldPos)] = new FlowNode() { IntegrationValue = 0, Cost = 0, Coordinate = toWorldPos };

			while (openPoints.Any())
			{
				var point = openPoints.Dequeue();
				var currentFlowNode = Nodes[_keyP(point)];

				var neighbors = DiagonalNeighborProviderFlowfield.GetNeighbors(point);
				foreach (var neighborP in neighbors)
				{
					if (!Nodes.ContainsKey(_keyP(neighborP)))
					{
						continue;
					}
					var neighborNode = Nodes[_keyP(neighborP)];
					if (neighborNode != null)
					{
						var integrationValue = neighborNode.Cost + currentFlowNode.IntegrationValue;
						if (integrationValue < neighborNode.IntegrationValue)
						{
							neighborNode.IntegrationValue = integrationValue;
							openPoints.Enqueue(neighborP);
						}
					}
				}
			}

			foreach (var node in Nodes)
			{
				var point = node.Value.Coordinate;

				if (point == toWorldPos)
				{
					continue;
				}

				var neighbors = DiagonalNeighborProviderFlowfield.GetNeighbors(point);
				FlowNode bestCostFlowNode = null;

				foreach (var neighborP in neighbors)
				{
					if (!Nodes.ContainsKey(_keyP(neighborP)))
					{
						continue;
					}
					var neighborNode = Nodes[_keyP(neighborP)];
					if (neighborNode != null && (bestCostFlowNode == null || (bestCostFlowNode.IntegrationValue > neighborNode.IntegrationValue && !neighborNode.Occupied)))
					{
						bestCostFlowNode = neighborNode;
					}
				}
				if (bestCostFlowNode != null)
				{
					Nodes[_keyP(point)].Next = bestCostFlowNode;
				}
				else
				{
					Nodes.ToString();
				}
			}
			IsCalculated = true;
		}

		public Point GetNextPoint(Point from)
		{
			//var s = Stopwatch.StartNew();
			//var position = from - flowFieldWorldPosition;
			var next = Nodes[_keyP(from)].Next;

			//TODO probably incorrect to do this, but for debug purposes leaving it like this
			if (next == null)
			{
				return from;
			}

			//s.Stop();
			//Debug.WriteLine(s.ElapsedMilliseconds);
			return new Point(next.Coordinate.X, next.Coordinate.Y);
		}
	}

}

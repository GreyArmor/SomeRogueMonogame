using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.shell;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.AccessControl;
using System.Windows.Forms;
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
		private int minX;
		private int minY;
		Dictionary<Point, FlowNode> Nodes;
		bool[,] avalabilityArray;
		int boolsWidth, boolsHeight;
		BoundingBox chunksBox;

		public bool IsCalculated { get; internal set; }

		public FlowFieldPathModel(NamelessGame game, IEnumerable<Point> chunkPath, IWorldProvider worldProvider, Point worldPosition)
		{
			world = worldProvider;
			this.flowFieldWorldPosition = worldPosition;

			var chunks = new List<Chunk>();
			var realitychunks = worldProvider.GetRealityBubbleChunks();

			foreach (var chunkCoord in chunkPath)
			{
				chunks.Add(realitychunks[chunkCoord]);
			}

		

			chunksBox = chunks.Select(x => x.Bounds).Aggregate((a, b) => { return BoundingBox.CreateMerged(a, b); });

			boolsWidth = (int)(chunksBox.Max.X - chunksBox.Min.X);
			boolsHeight = (int)(chunksBox.Max.Y - chunksBox.Min.Y);
			//make the availability map 1 tile wider from both size, to further optimize CalculateTo neighbor tile search
			bool[,] bools = new bool[boolsWidth+2, boolsHeight+2];

			minX = (int)chunksBox.Min.X;
			minY = (int)chunksBox.Min.Y;

			Nodes = new Dictionary<Point, FlowNode>(chunks.Count * Constants.ChunkSize * Constants.ChunkSize);

			//fill the nodes
			foreach (var chunk in chunks)
			{
				#region debug
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
				#endregion

				var location = chunk.WorldPositionBottomLeftCorner;
				for (int i = 0; i < Constants.ChunkSize; i++)
				{
					for (int j = 0; j < Constants.ChunkSize; j++)
					{
						var coordX = location.X + i;
						var coordY = location.Y + j;
						Nodes.Add(new Point(coordX, coordY), new FlowNode()
						{
							Coordinate = new Point(coordX, coordY),
							Occupied = /* !world.GetTile(coordX, coordY).IsPassableIgnoringCharacters() ||*/ world.GetTile(coordX, coordY, 0).Terrain == TerrainTypes.Water,
							IntegrationValue = int.MaxValue
						});


						bools[coordX - minX + 1 , coordY - minY + 1] = true;
					}
				}
			}

			avalabilityArray = bools;

		}

		void ResetNodes()
		{
			//var arrayDimension = Constants.ChunkSize * Constants.RealityBubbleRangeInChunks * 2;
			foreach (var node in Nodes)
			{
				node.Value.IntegrationValue = int.MaxValue;
			}
		}

		public void ClaculateTo(Point to)
		{
			var toWorldPos = to;
			Queue<Point> openPoints = new Queue<Point>();
			openPoints.Enqueue(toWorldPos);
			//destination
			Nodes[toWorldPos] = new FlowNode() { IntegrationValue = 0, Cost = 0, Coordinate = toWorldPos };


			bool _insodeBoundsOfArea(int arrayX, int arrayY)
			{
				return arrayX > 0 && arrayY > 0 && arrayX < boolsWidth && arrayY < boolsHeight;
			}
			while (openPoints.Any())
			{
				var point = openPoints.Dequeue();
				var currentFlowNode = Nodes[point];

				var neighbors = AllNeighborProviderFlowfield.GetNeighbors(point);
				foreach (var neighborP in neighbors)
				{
					var arrayX = neighborP.X - minX + 1;
					var arrayY = neighborP.Y - minY + 1;
					if (avalabilityArray[arrayX, arrayY])
					{
						var neighborNode = Nodes[neighborP];
						if (!neighborNode.Occupied)
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
			}
			foreach (var node in Nodes)
			{
				var point = node.Value.Coordinate;
				var neighbors = AllNeighborProviderFlowfield.GetNeighbors(point);
				FlowNode bestCostFlowNode = null;

				foreach (var neighborP in neighbors)
				{
					var arrayX = neighborP.X - minX + 1;
					var arrayY = neighborP.Y - minY + 1;
					if (avalabilityArray[arrayX, arrayY])
					{
						var neighborNode = Nodes[neighborP];

						if (neighborNode.Occupied)
						{
							continue;
						}

						if (bestCostFlowNode == null || (bestCostFlowNode.IntegrationValue > neighborNode.IntegrationValue))
						{
							bestCostFlowNode = neighborNode;
						}
					}
				}
				Nodes[point].Next = bestCostFlowNode;
			}

			Nodes[toWorldPos] = new FlowNode() { IntegrationValue = 0, Cost = 0, Coordinate = toWorldPos };

			IsCalculated = true;
		}


		//public void ClaculateToV1(Point to)
		//{

		//	//	ResetNodes();

		//	var toWorldPos = to;
		//	Queue<Point> openPoints = new Queue<Point>();
		//	openPoints.Enqueue(toWorldPos);


		//	//add impassable and cost Here

		//	//destination
		//	Nodes[_keyP(toWorldPos)] = new FlowNode() { IntegrationValue = 0, Cost = 0, Coordinate = toWorldPos };


		//	bool _insodeBoundsOfArea(int arrayX, int arrayY)
		//	{
		//		return arrayX > 0 && arrayY > 0 && arrayX < boolsWidth && arrayY < boolsHeight;
		//	}

		//	while (openPoints.Any())
		//	{
		//		var point = openPoints.Dequeue();
		//		var currentFlowNode = Nodes[_keyP(point)];

		//		var neighbors = DiagonalNeighborProviderFlowfield.GetNeighbors(point);
		//		foreach (var neighborP in neighbors)
		//		{

		//			var arrayX = neighborP.X - minX;
		//			var arrayY = neighborP.Y - minY;
		//			if (_insodeBoundsOfArea(arrayX, arrayY))
		//			{
		//				if (avalabilityArray[arrayX, arrayY])
		//				{   //if (!Nodes.ContainsKey(_keyP(neighborP)))
		//					//{
		//					//	continue;
		//					//}
		//					var neighborNode = Nodes[_keyP(neighborP)];
		//					if (!neighborNode.Occupied)
		//					{
		//						var integrationValue = neighborNode.Cost + currentFlowNode.IntegrationValue;

		//						if (integrationValue < neighborNode.IntegrationValue)
		//						{
		//							neighborNode.IntegrationValue = integrationValue;
		//							openPoints.Enqueue(neighborP);
		//						}
		//					}
		//				}
		//			}
		//		}
		//	}

		//	foreach (var node in Nodes)
		//	{
		//		var point = node.Value.Coordinate;

		//		if (point == toWorldPos)
		//		{
		//			continue;
		//		}

		//		var neighbors = DiagonalNeighborProviderFlowfield.GetNeighbors(point);
		//		FlowNode bestCostFlowNode = null;

		//		foreach (var neighborP in neighbors)
		//		{
		//			var arrayX = neighborP.X - minX;
		//			var arrayY = neighborP.Y - minY;
		//			if (_insodeBoundsOfArea(arrayX, arrayY))
		//			{
		//				if (avalabilityArray[arrayX, arrayY])
		//				{
		//					var neighborNode = Nodes[_keyP(neighborP)];

		//					if (neighborNode.Occupied)
		//					{
		//						continue;
		//					}

		//					if (bestCostFlowNode == null || (bestCostFlowNode.IntegrationValue > neighborNode.IntegrationValue))
		//					{
		//						bestCostFlowNode = neighborNode;
		//					}
		//				}
		//			}
		//		}
		//		//	if (bestCostFlowNode != null)
		//		//{
		//		Nodes[_keyP(point)].Next = bestCostFlowNode;
		//		//	}
		//		//else
		//		//{
		//		//	Nodes.ToString();
		//		//}
		//	}
		//	IsCalculated = true;
		//}


		public Point GetNextPoint(Point from)
		{
			//var s = Stopwatch.StartNew();
			//var position = from - flowFieldWorldPosition;
			var next = Nodes[from].Next;

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

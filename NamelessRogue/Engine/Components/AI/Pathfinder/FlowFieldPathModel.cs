using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Components.Environment;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Components.UI;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Systems.Ingame;
using NamelessRogue.Engine.Utility;
using NamelessRogue.shell;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.AccessControl;
using System.Windows.Forms;
using Constants = NamelessRogue.Engine.Infrastructure.Constants;
namespace NamelessRogue.Engine.Components.AI.Pathfinder
{
	public enum FlowFieldDirection : byte
	{
		None,
		North,
		South,
		East,
		West,
		NorthEast,
		NorthWest,
		SouthEast,
		SouthWest
    }

    internal class FlowFieldPathModel : IPathModel
    {
        public static Dictionary<FlowFieldDirection, Point> Directions = new Dictionary<FlowFieldDirection, Point>()
        {
            { FlowFieldDirection.None, new Point(0,0) },
            { FlowFieldDirection.North, new Point(0,-1) },
            { FlowFieldDirection.South, new Point(0,1) },
            { FlowFieldDirection.East, new Point(1,0) },
            { FlowFieldDirection.West, new Point(-1,0) },
            { FlowFieldDirection.NorthEast, new Point(1,-1) },
            { FlowFieldDirection.NorthWest, new Point(-1,-1) },
            { FlowFieldDirection.SouthEast, new Point(1,1) },
            { FlowFieldDirection.SouthWest, new Point(-1,1) }
        };

        string _key(int x, int y)
        {
            return x.ToString() + ':' + y.ToString();
        }

        string _keyP(Point p)
        {
            return _key(p.X, p.Y);
        }

        private NamelessGame game;

        //used as the cost map
        IWorldProvider world;
        private readonly List<Chunk> chunks;
        private int minX;
        private int minY;
        Dictionary<Point, FlowNode> Nodes;
        bool[,] avalabilityArray;
        int boolsWidth, boolsHeight;
        Microsoft.Xna.Framework.BoundingBox chunksBox;

        public bool IsCalculated { get; internal set; }

        bool _insideBoundsOfArea(int arrayX, int arrayY)
        {
            return arrayX > 0 && arrayY > 0 && arrayX < boolsWidth && arrayY < boolsHeight;
        }

        public FlowFieldPathModel(NamelessGame game, IEnumerable<Point> chunkPath, IWorldProvider worldProvider)
        {
            this.game = game;
            world = worldProvider;
            chunks = new List<Chunk>();
            var realitychunks = worldProvider.GetRealityBubbleChunks();

            foreach (var chunkCoord in chunkPath)
            {
                chunks.Add(realitychunks[chunkCoord]);
            }

            chunksBox = chunks.Select(x => x.Bounds).Aggregate((a, b) => { return Microsoft.Xna.Framework.BoundingBox.CreateMerged(a, b); });

            boolsWidth = (int)(chunksBox.Max.X - chunksBox.Min.X);
            boolsHeight = (int)(chunksBox.Max.Y - chunksBox.Min.Y);
            //make the availability map 1 tile wider from both size, to further optimize CalculateTo neighbor tile search
            bool[,] bools = new bool[boolsWidth + 2, boolsHeight + 2];

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
                        var tile = world.GetTile(coordX, coordY, 0);

                        Nodes.Add(new Point(coordX, coordY), new FlowNode()
                        {
                            Coordinate = new Point(coordX, coordY),
                            Occupied = !tile.IsPassableIgnoringCharacters() || tile.Terrain == TerrainTypes.Water,
                            IntegrationValue = int.MaxValue
                        });


                        bools[coordX - minX + 1, coordY - minY + 1] = true;
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


        public void PaintWith(FlowFieldDirection direction)
        {
            foreach (var node in Nodes)
            {
                var point = node.Value.Coordinate;
                var next = point + Directions[direction];
                node.Value.Next = Nodes.ContainsKey(next) ? Nodes[next] : null;
            }
            IsCalculated = true;
        }

        public void CalculateTo(Point to)
        {
            var toRealityPos = to;
            Queue<Point> openPoints = new Queue<Point>();
            openPoints.Enqueue(toRealityPos);
            //destination
            Nodes[toRealityPos] = new FlowNode() { IntegrationValue = 0, Cost = 0, Coordinate = toRealityPos };

            while (openPoints.Any())
            {
                var point = openPoints.Dequeue();
                var currentFlowNode = Nodes[point];

                var neighbors = AllNeighborProviderFlowfield.GetNeighbors(point);
                foreach (var neighborP in neighbors)
                {
                    var arrayX = neighborP.X - minX + 1;
                    var arrayY = neighborP.Y - minY + 1;
                    if (boolsWidth > arrayX && boolsHeight > arrayY && avalabilityArray[arrayX, arrayY])
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
                    if (boolsWidth > arrayX && boolsHeight > arrayY && avalabilityArray[arrayX, arrayY])
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

            Nodes[toRealityPos] = new FlowNode() { IntegrationValue = 0, Cost = 0, Coordinate = toRealityPos };




            IsCalculated = true;
        }

        List<IEntity> debugEntitiesFurniture = new List<IEntity>();
        List<IEntity> debugEntities = new List<IEntity>();
        public void DrawDebug()
        {
            void _addFurniture(string id, string descriptionName, bool occupiesTile, bool blocksVision)
            {
                Entity entity = new Entity();

                entity.AddComponent(new Drawable(id, new Engine.Utility.Color(1f)));

                entity.AddComponent(new Description(descriptionName, ""));
                if (occupiesTile)
                {
                    entity.AddComponent(new OccupiesTile());
                }
                if (blocksVision)
                {
                    entity.AddComponent(new BlocksVision());
                }
                entity.AddComponent(new Furniture());
                entity.AddComponent(new PhantomEntity());
                //FurnitureDictionary.Add(id, entity);
                debugEntitiesFurniture.Add(entity);
            }
            if (debugEntitiesFurniture.Count == 0)
            {
                _addFurniture("middleDir", "Middle Direction", false, false);
                _addFurniture("northDir", "North Direction", false, false);
                _addFurniture("southDir", "South Direction", false, false);
                _addFurniture("westDir", "West Direction", false, false);
                _addFurniture("eastDir", "East Direction", false, false);
                _addFurniture("nwDir", "Northwest Direction", false, false);
                _addFurniture("neDir", "Northeast Direction", false, false);
                _addFurniture("swDir", "Southwest Direction", false, false);
                _addFurniture("seDir", "Southeast Direction", false, false);
            }

            foreach (var node in Nodes)
            {
                var point = node.Value.Coordinate;
                var next = node.Value.Next;
                if (next != null)
                {
                    var direction = next.Coordinate - point;
                    string furnitureId = "middleDir";
                    if (direction.X == 0 && direction.Y == 0)
                    {
                        furnitureId = "middleDir";
                    }
                    if (direction.X == 0 && direction.Y == 1)
                    {
                        furnitureId = "southDir";
                    }
                    else if (direction.X == 0 && direction.Y == -1)
                    {
                        furnitureId = "northDir";
                    }
                    else if (direction.X == -1 && direction.Y == 0)
                    {
                        furnitureId = "westDir";
                    }
                    else if (direction.X == 1 && direction.Y == 0)
                    {
                        furnitureId = "eastDir";
                    }
                    else if (direction.X == -1 && direction.Y == 1)
                    {
                        furnitureId = "swDir";
                    }
                    else if (direction.X == 1 && direction.Y == 1)
                    {
                        furnitureId = "seDir";
                    }
                    else if (direction.X == -1 && direction.Y == -1)
                    {
                        furnitureId = "nwDir";
                    }
                    else if (direction.X == 1 && direction.Y == -1)
                    {
                        furnitureId = "neDir";
                    }
                    var entity = debugEntitiesFurniture.First(x => x.GetComponentOfType<Drawable>().ObjectID == furnitureId);
                    var entityClone = entity.CloneEntity();
                    entityClone.AddComponent(new Position(point.X, point.Y, 0));

                    debugEntities.Add(entityClone);
                    //game.AddEntity(entityClone);
                    var gameTile = world.GetTile(point.X, point.Y, 0);
                    gameTile.AddEntity((Entity)entityClone);
                }
            }

            foreach (var chunk in chunks)
            {
                game.Commander.EnqueueCommand(new UpdateVisualChunkCommand(new Vector3Int(chunk.ChunkWorldMapLocationPoint.X, chunk.ChunkWorldMapLocationPoint.Y, 0)));
            }
        }

        public void ClearDebug()
        {
            foreach (var entity in debugEntities)
            {
                var pos = entity.GetComponentOfType<Position>();
                var tile = world.GetTile(pos.X, pos.Y, pos.Z);
                tile.RemoveEntity((Entity)entity);
                game.RemoveEntity(entity);
            }
            debugEntities.Clear();
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

using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.AI.NonPlayerCharacter;
using NamelessRogue.Engine.Components.Environment;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Components.UI;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Systems.Ingame;
using NamelessRogue.Engine.Utility;
using NamelessRogue.shell;
using SharpDX.Direct3D9;
using System.Collections.Generic;
using System.Linq;

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

            var nodeList = Nodes.Values.ToArray();
            var len = nodeList.Length;
            for (int i = 0; i < nodeList.Length; i++) {
                var point = nodeList[i];
                var next = nodeList[(i+1)% len];
                if (next != null)
                {
                    var direction = next - point;
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

            foreach (var chunk in world.RealityChunks)
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

        public void PaintWith(Direction direction)
        {
            //throw new NotImplementedException();
        }
    }
}

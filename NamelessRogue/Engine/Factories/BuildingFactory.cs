using AStarNavigator.Providers;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.Timers;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.AI.Pathfinder;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Components.Environment;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Components.UI;
using NamelessRogue.Engine.Generation.Editor;
using NamelessRogue.Engine.Generation.Settlement;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Systems.Ingame;
using NamelessRogue.Engine.Utility;
using NamelessRogue.shell;
using RogueSharp.Random;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TiledCSPlus;
using TiledMap = TiledCSPlus.TiledMap;

namespace NamelessRogue.Engine.Factories
{
    public class BuildingFactory {
        const int tilemapTileSize = 32;
        static Dictionary<string, BuildingDataCache> buildingCreationCache = new Dictionary<string, BuildingDataCache>();

        public static Entity CreateDoor(int x, int y, int z, string objectId)
        {
            Entity door = new Entity();
            door.AddComponent(new Position(x, y, z));
            door.AddComponent(new Drawable("closed_" + objectId, new Engine.Utility.Color(1f, 1f, 1f)));
            door.AddComponent(new Description("Door", ""));
            door.AddComponent(new Door(objectId, false));
            door.AddComponent(new SimpleSwitch(true));
            door.AddComponent(new Interactable());
            door.AddComponent(new OccupiesTile());
            door.AddComponent(new BlocksVision());
            door.AddComponent(new Furniture());
            return door;
        }

        public static Entity CreateWindow(int x, int y, int z, string tileObjectType)
        {
            Entity window = new Entity();
            window.AddComponent(new Position(x, y, z));
            window.AddComponent(new Drawable(tileObjectType, new Engine.Utility.Color(1f, 1f, 1f), new Engine.Utility.Color()));
            window.AddComponent(new Description("Window", ""));
            window.AddComponent(new OccupiesTile());
            window.AddComponent(new Furniture());
            return window;
        }

        public static IEntity CreateBuilding(int worldSpaceX, int worldSpaceY, BuildingTemplateData data, NamelessGame namelessGame)
        {
            var realSpaceX = Constants.ChunkSize * worldSpaceX;
            var realSpaceY = Constants.ChunkSize * worldSpaceY;

            var diagonalNeighbors = new DiagonalNeighborProviderSelfIncluded();
            var straightNeighbors = new StraightNeighborProviderSelfIncluded();

            IEntity worldEntity = namelessGame.WorldTemplateEntity;
            IWorldProvider worldProvider = null;
            if (worldEntity != null)
            {
                worldProvider = worldEntity.GetComponentOfType<WorldTemplate>().WorldMap.Chunks;
            }
            var macroLocation = new MacroLocation()
            {
                Id = data.Id,
                ChunkPosition = new Vector3Int(worldSpaceX, worldSpaceY, 0),
                RealityPosition = new Vector3Int(realSpaceX, realSpaceY, 0),
                BoundingBox = new Microsoft.Xna.Framework.BoundingBox(new Vector3(realSpaceX, realSpaceY, 0), new Vector3(realSpaceX + data.Size.X, realSpaceY + data.Size.Y, 0)),
            };
            namelessGame.MacroNavigator.Locations.Add(macroLocation);

            IEntity building = new Entity();

            building.AddComponent(new Description(data.Name, data.Description));
            building.AddComponent(new Position(realSpaceX, realSpaceY, 0));

            Building buildingComponent = new Building();
            var tileset = new TiledTileset("Content\\Buildings\\tileset2.tsx");
            foreach (var buildingfloor in data.TiledFilePaths)
            {
                //var map = new TiledMap("Content\\Buildings\\ApartmentBlockFloor.tmx");

                var floorZ = buildingfloor.Floor;
                Vector3Int floorChunkPosition = new Vector3Int(worldSpaceX, worldSpaceY, floorZ);
                var mapPath = "Content\\" + buildingfloor.TiledFilePath.Path;
                var map = new TiledMap(mapPath);

                // Retrieving objects or layers can be done using Linq or a for loop
                var mainLayer = map.Layers.First(l => l.Name == "main");
                var animatedLayer = map.Layers.First(l => l.Name == "animated");
                var terrainLayer = map.Layers.FirstOrDefault(l => l.Name == "terrain");
                var objects = map.Layers.FirstOrDefault(l => l.Name == "objects")?.Objects;

                int buildingSize = map.Width;

                bool hasCache = buildingCreationCache.TryGetValue(mapPath, out var buildingCache);

                if (!hasCache)
                {
                    var newCache = new BuildingDataCache();
                    newCache.CalculateCache(buildingSize, realSpaceX, realSpaceY, floorZ, worldProvider, mainLayer, tileset);
                    buildingCreationCache.Add(mapPath, newCache);
                    buildingCache = newCache;
                }
                ////first determine which walls are corners and intersections

                for (int loopY = 0; loopY < buildingSize; loopY++)
                {
                    for (int loopX = 0; loopX < buildingSize; loopX++)
                    {
                        var gameTile = worldProvider.GetTile(realSpaceX + loopX, realSpaceY + loopY, floorZ);

                        if (terrainLayer != null)
                        {
                            var tileId = terrainLayer.Data[loopX + (loopY * buildingSize)];
                            if (tileId != 0)
                            {
                                var tile = tileset.Tiles.First(x => x.Id == tileId - 1);
                                var tileObjectType = tile.Properties[0].Value;
                                var terrainType = Enum.Parse<TerrainTypes>(tileObjectType);
                                gameTile = new Tile(terrainType, Biomes.None);
                                worldProvider.SetTile(realSpaceX + loopX, realSpaceY + loopY, floorZ, gameTile);
                            }
                        }

                        if (gameTile == null)
                        {
                            gameTile = new Tile(TerrainTypes.AsphaultPoor, Biomes.None);
                            worldProvider.SetTile(realSpaceX + loopX, realSpaceY + loopY, floorZ, gameTile);
                        }


                        //add static objects
                        {
                            var tileId = mainLayer.Data[loopX + (loopY * buildingSize)];
                            if (tileId != 0)
                            {
                                var tile = tileset.Tiles.First(x => x.Id == tileId - 1);
                                var tileObjectType = tile.Properties[0].Value;

                                gameTile.TilesetPosition = buildingCache.tilesetPositions[loopY, loopX];
                                switch (tileObjectType)
                                {
                                    case "nothingness":
                                        gameTile.Terrain = TerrainTypes.Nothingness;
                                        gameTile.Biome = Biomes.None;
                                        break;
                                    default:
                                        {
                                            if (tileObjectType.Contains("window"))
                                            {
                                                var entity = CreateWindow(realSpaceX + loopX, realSpaceY + loopY, floorZ, tileObjectType);
                                                gameTile.AddEntity(entity);
                                            }
                                            else if (tileObjectType.Contains("door"))
                                            {
                                                var entity = CreateDoor(realSpaceX + loopX, realSpaceY + loopY, floorZ, tileObjectType);
                                                gameTile.AddEntity(entity);

                                                if (tileObjectType.Contains("chainlink"))
                                                {
                                                    entity.RemoveComponentOfType<BlocksVision>();
                                                    entity.GetComponentOfType<Door>().IsTranslucent = true;
                                                }
                                            }
                                            else
                                            {
                                                var entity = TerrainFurnitureFactory.GetFurniture(tileObjectType);
                                                if (entity != null)
                                                {
                                                    gameTile.AddEntity(entity);
                                                }
                                            }
                                        }
                                        break;
                                }
                            }

                            if (tileId == 62)
                            {
                                gameTile.Terrain = TerrainTypes.AsphaultPoor;
                            }
                            if (tileId == 63)
                            {
                                gameTile.Terrain = TerrainTypes.PaintedAsphault;
                            }
                            gameTile.Biome = Biomes.None;
                        }
                        //add animated objects
                        {
                            var tileId = animatedLayer.Data[loopX + (loopY * buildingSize)];
                            if (tileId != 0)
                            {
                                var tile = tileset.Tiles.First(x => x.Id == tileId - 1);
                                var tileObjectType = tile.Properties[0].Value;

                                var entity = TerrainFurnitureFactory.GetAnimatedFurniture(tileObjectType);
                                if (entity != null)
                                {
                                    gameTile.AddEntity(entity);
                                    namelessGame.AddEntity(entity);
                                };
                            }
                        }
                    }
                }
                var apartmentBlockShopsRandomizer = new ApartmentShopsTerrainRandomizer();
                if (objects != null)
                {
                    foreach (var tileObject in objects)
                    {
                        var tilePosition = tileObject.Position / tilemapTileSize;



                        if (tileObject.Type == TiledObjectType.Polygon && tileObject.Class == "pedestrianMovementPolygon")
                        {
                          //  macroLocation.Type = LocationType.Building;
                           // if (onlyOnePath)
                            {
                             //   onlyOnePath = false;
                                List<MacroNode> waypoinMacroNodetList = new List<MacroNode>();
                                NodeType[] nodeTypes = new NodeType[] { NodeType.PedestrianEntranceNW, NodeType.PedestrianEntranceNE, NodeType.PedestrianEntranceSE, NodeType.PedestrianEntranceSW, };
                                //create nodes
                                for (int i = 0; i < tileObject.Polygon.Points.Count(); i++)
                                {
                                    var point = (tileObject.Polygon.Points[i] + tileObject.Position) / tilemapTileSize;

                                    point = point + new System.Numerics.Vector2(macroLocation.RealityPosition.X, macroLocation.RealityPosition.Y);

                                    var macroNodeType = nodeTypes[i];
                                                                     
                                    waypoinMacroNodetList.Add(new MacroNode()
                                    {
                                        Id = $@"pedestrianWaypoint{i}_x{realSpaceX}_y{realSpaceY}",
                                        RealityPosition = new Vector3Int(realSpaceX + (int)point.X, realSpaceY + (int)point.Y, floorZ),   
                                        Type = macroNodeType
                                    });
                                  //  break;
                                }


                                //for (int i = 0; i < tileObject.Polygon.Points.Count(); i++)
                                //{
                                //    var point = (tileObject.Polygon.Points[i] + tileObject.Position) / tilemapTileSize;
                                //    var waypoint = new Position(realSpaceX + (int)point.X, realSpaceY + (int)point.Y, floorZ);

                                //   // var oppositeWaypoint = waypoinMacroNodetList.Except(waypoinMacroNodetList[i].Neighbors).FirstOrDefault();

                                //    var waypointId = ("waypoint" + waypoint.Point.ToPoint());

                                //    var buildingMin = new Point(worldSpaceX - 1, worldSpaceY - 1);
                                //    var buildingMax = new Point((worldSpaceX + (int)(data.Size.X / Constants.ChunkSize) + 1), (worldSpaceY + (int)(data.Size.Y / Constants.ChunkSize) + 1));


                                //    var flowId = namelessGame.FlowFieldController.CalculateToForArea(waypoint.Point.ToPoint(), buildingMin, buildingMax);
                                //    waypoinMacroNodetList[i].FlowFieldId = flowId;
                                //    //   break;
                                //}

                                macroLocation.InternalNodes.AddRange(waypoinMacroNodetList);
                               // macroLocation.Entrances.AddRange(waypoinMacroNodetList);
                            }
                        }

                        if (tileObject.Type == TiledObjectType.Point)
                        {
                            if (tileObject.Class == "waypoint")
                            {

                            }
                            var npc_id = tileObject.Properties[0].Value;
                            var hasCharacter = CharacterFactory.CharacterDataById.TryGetValue(npc_id, out var characterData);

                            if (hasCharacter)
                            {
                                var gameTile = worldProvider.GetTile(realSpaceX + (int)tilePosition.X, realSpaceY + (int)tilePosition.Y, floorZ);

                                if (characterData.RandomName)
                                {
                                  //  if (BuildingFactory.onlyOne)
                                    {
                                     //   BuildingFactory.onlyOne = false;
                                        var randomValue = namelessGame.CurrentGame.GlobalRandom.Next(0, 100);
                                        if (randomValue < 50)
                                        {
                                            var character = CharacterFactory.CreateCharacterFromData(namelessGame, new Vector3Int(realSpaceX + (int)tilePosition.X, realSpaceY + (int)tilePosition.Y, 0), characterData);
                                        }
                                    }
                                }
                                else
                                {
                                    var character = CharacterFactory.CreateCharacterFromData(namelessGame, new Vector3Int(realSpaceX + (int)tilePosition.X, realSpaceY + (int)tilePosition.Y, 0), characterData);
                                    namelessGame.AddEntity(character);
                                    gameTile.AddEntity(character);
                                }
                            }
                        }
                        else if (tileObject.Type == TiledObjectType.Rectangular)
                        {
                            List<MacroNode> waypointMacroNodesList = new List<MacroNode>();

                            var rectZise = tileObject.Size / tilemapTileSize;
                            var isRandomizer = tileObject.Name == "TerrainRandomizer";
                            if (isRandomizer)
                            {
                                var randomizerRect = new Rectangle((int)tilePosition.X + realSpaceX, (int)tilePosition.Y + realSpaceY, (int)rectZise.X, (int)rectZise.Y);
                                apartmentBlockShopsRandomizer.Randomize(worldProvider, randomizerRect, floorZ, namelessGame.CurrentGame.GlobalRandom);
                            }
                            //if(false)
                            else if (tileObject.Class == "pedestrian_crossing")
                            {                               

                                var rect = new Rectangle(
                                    (int)(tileObject.Position.X / tilemapTileSize + realSpaceX), 
                                    (int)(tileObject.Position.Y / tilemapTileSize + realSpaceY), 
                                    (int)(tileObject.Size.X / tilemapTileSize), 
                                    (int)(tileObject.Size.Y / tilemapTileSize));

                                namelessGame.MacroNavigator.Crossings.Add(macroLocation);

                                bool vertical = tileObject.Properties.FirstOrDefault(x=>x.Name == "direction").Value == "vertical";
                                if(vertical)
                                {
                                    //var centerTop = new Vector3Int(rect.Center.X, rect.Top, floorZ);
                                    //var centerBottom = new Vector3Int(rect.Center.X, rect.Bottom, floorZ);

                                    //var flowIdTop = namelessGame.FlowFieldController.SetDirectionForArea(FlowFieldDirection.North, rect.Location, rect.Location + rect.Size);
                                    //var flowIdBottom = namelessGame.FlowFieldController.SetDirectionForArea(FlowFieldDirection.South, rect.Location, rect.Location + rect.Size);

                                    //waypointMacroNodesList.Add(new MacroNode()
                                    //{
                                    //    Id = $@"pedestrian_crossing_top_x{realSpaceX}_y{realSpaceY}",
                                    //    RealityPosition = centerTop,
                                    //    FlowFieldId = flowIdTop
                                    //});

                                    //waypointMacroNodesList.Add(new MacroNode()
                                    //{
                                    //    Id = $@"pedestrian_crossing_bottom_x{realSpaceX}_y{realSpaceY}",
                                    //    RealityPosition = centerBottom,
                                    //    FlowFieldId = flowIdBottom
                                    //});


                                    macroLocation.Type = LocationType.CrossingVertical;

                                }
                                else
                                {
                                    //var centerLeft = new Vector3Int(rect.Left, rect.Center.Y, floorZ);
                                    //var centerRight = new Vector3Int(rect.Right, rect.Center.Y, floorZ);
                                    //var flowIdLeft = namelessGame.FlowFieldController.SetDirectionForArea(FlowFieldDirection.West, rect.Location, rect.Location + rect.Size);
                                    //var flowIdRight = namelessGame.FlowFieldController.SetDirectionForArea(FlowFieldDirection.East, rect.Location, rect.Location + rect.Size);
                                    //waypointMacroNodesList.Add(new MacroNode()
                                    //{
                                    //    Id = $@"pedestrian_crossing_left_x{realSpaceX}_y{realSpaceY}",
                                    //    RealityPosition = centerLeft,
                                    //    FlowFieldId = flowIdLeft
                                    //});
                                    //waypointMacroNodesList.Add(new MacroNode()
                                    //{
                                    //    Id = $@"pedestrian_crossing_right_x{realSpaceX}_y{realSpaceY}",
                                    //    RealityPosition = centerRight,
                                    //    FlowFieldId = flowIdRight,
                                    //});

                                    macroLocation.Type = LocationType.CrossingHorizontal;
                                }
                              
                            }
                        }
                       

                    }
                }
            }
            building.AddComponent(buildingComponent);
            return building;

        }   

        //public static bool onlyOne = true;
        //public static bool onlyOnePath = true;
    }
}

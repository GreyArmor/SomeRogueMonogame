using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AStarNavigator.Providers;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.Timers;
using NamelessRogue.Engine.Abstraction;
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
using TiledCSPlus;
using TiledMap = TiledCSPlus.TiledMap;

namespace NamelessRogue.Engine.Factories
{

    public class BuildingDataCache
    {
        static DiagonalNeighborProviderSelfIncluded diagonalNeighbors = new DiagonalNeighborProviderSelfIncluded();
        static StraightNeighborProviderSelfIncluded straightNeighbors = new StraightNeighborProviderSelfIncluded();
        public bool[,] postProcessingArray;
        public string[,] tilesetPositions;
        public void CalculateCache(int buildingSize, int realSpaceX, int realSpaceY, int floorZ, IWorldProvider worldProvider, TiledLayer mainLayer, TiledTileset tileset)
        {
            postProcessingArray = new bool[buildingSize, buildingSize];
            tilesetPositions = new string[buildingSize, buildingSize];

            for (int loopY = 0; loopY < buildingSize; loopY++)
            {
                for (int loopX = 0; loopX < buildingSize; loopX++)
                {
                    var gameTile = worldProvider.GetTile(realSpaceX + loopX, realSpaceY + loopY, floorZ);
                    var tileId = mainLayer.Data[loopX + (loopY * buildingSize)];
                    if (tileId != 0)
                    {
                        var tile = tileset.Tiles.First(x => x.Id == tileId - 1);
                        var tileObjectType = tile.Properties[0].Value;
                        if (tileObjectType == "wall" || tileObjectType == "wall_brick" || tileObjectType == "door" || tileObjectType == "window")
                        {
                            postProcessingArray[loopY, loopX] = true;
                        }
                    }
                }
            }

            for (int loopY = 0; loopY < buildingSize; loopY++)
            {
                for (int loopX = 0; loopX < buildingSize; loopX++)
                {
                    var cellValue = postProcessingArray[loopY, loopX];

                    if (!cellValue)
                    {
                        continue;
                    }

                    var diagonalCells = diagonalNeighbors.GetNeighbors(new AStarNavigator.Tile(loopX, loopY)).ToList();
                    var straightCells = straightNeighbors.GetNeighbors(new AStarNavigator.Tile(loopX, loopY)).ToList();
                    string tilesetPosition = "";
                    foreach (var neighbor in diagonalCells)
                    {
                        //throw away outside of bounds tiles
                        if (neighbor.X < 0 || neighbor.Y < 0 || neighbor.X == buildingSize || neighbor.Y == buildingSize)
                        {
                            tilesetPosition += "0";
                        }
                        else
                        {
                            // also throw away all diagonal tiles
                            if (straightCells.Contains(neighbor) && postProcessingArray[(int)neighbor.Y, (int)neighbor.X])
                            {
                                tilesetPosition += "1";
                            }
                            else
                            {
                                tilesetPosition += "0";
                            }
                        }
                    }
                    tilesetPositions[loopY, loopX] = tilesetPosition;
                }
            }

        }
    }
    public class BuildingFactory {

        static Dictionary<string, BuildingDataCache> buildingCreationCache = new Dictionary<string, BuildingDataCache>();

        public static Entity CreateDoor(int x, int y, int z, string objectId)
        {
            Entity door  = new Entity();
            door.AddComponent(new Position(x, y, z));
            door.AddComponent(new Drawable(objectId, new Engine.Utility.Color(1f, 1f, 1f)));
            door.AddComponent(new Description("Door",""));
            door.AddComponent(new Door());
            door.AddComponent(new SimpleSwitch(true));
            door.AddComponent(new Interactable());
            door.AddComponent(new OccupiesTile());
            door.AddComponent(new BlocksVision());
            door.AddComponent(new Furniture());
            return door;
        }

        public static Entity CreateWindow(int x, int y, int z, string tileObjectType)
        {
            Entity window  = new Entity();
            window.AddComponent(new Position(x, y, z));
            window.AddComponent(new Drawable(tileObjectType, new Engine.Utility.Color(1f, 1f, 1f), new Engine.Utility.Color()));
            window.AddComponent(new Description("Window",""));
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

            IEntity worldEntity = namelessGame.TimelineEntity;
            IWorldProvider worldProvider = null;
            if (worldEntity != null)
            {
                worldProvider = worldEntity.GetComponentOfType<WorldTemplate>().WorldMap.Chunks;
            }
   
            IEntity building = new Entity();

            building.AddComponent(new Description(data.Name, data.Description));
            building.AddComponent(new Position(realSpaceX, realSpaceY, 0));

            Building buildingComponent = new Building();
            var tileset = new TiledTileset("Content\\Buildings\\tileset2.tsx");
            foreach (var buildingfloor in data.TiledFilePaths)
            {
                //var map = new TiledMap("Content\\Buildings\\ApartmentBlockFloor.tmx");

                var floorZ = buildingfloor.Floor;
                var mapPath = "Content\\" + buildingfloor.TiledFilePath.Path;
                var map = new TiledMap(mapPath);              

                // Retrieving objects or layers can be done using Linq or a for loop
                var mainLayer = map.Layers.First(l => l.Name == "main");
                var animatedLayer = map.Layers.First(l => l.Name == "animated");
                int buildingSize = 64;

                bool hasCache = buildingCreationCache.TryGetValue(mapPath, out var buildingCache);
                
                if(!hasCache)
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

                        //if (gameTile == null)
                        //{
                        //    gameTile = new Tile(TerrainTypes.AsphaultPoor, Biomes.None);
                        //    worldProvider.SetTile(realSpaceX + loopX, realSpaceY + loopY, floorZ, gameTile);
                        //}
                        //else
                        //{
                        //    gameTile.Terrain = TerrainTypes.FloorGrate;
                        //}

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
                                    case "door":
                                    case "door_brick":
                                        {
                                            var entity = CreateDoor(realSpaceX + loopX, realSpaceY + loopY, floorZ, tileObjectType);
                                            gameTile.AddEntity(entity);
                                        }
                                        break;
                                    case "window":
                                    case "window_brick":
                                        {
                                            var entity = CreateWindow(realSpaceX + loopX, realSpaceY + loopY, floorZ, tileObjectType);
                                            gameTile.AddEntity(entity);
                                        }
                                        break;
                                    default:
                                        {
                                            var entity = TerrainFurnitureFactory.GetFurniture(tileObjectType);
                                            if (entity != null)
                                            {
                                                gameTile.AddEntity(entity);
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
            }
            building.AddComponent(buildingComponent);
            return building;

        }
    }
}

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
using SharpDX.Direct3D11;
using TiledCSPlus;
using TiledMap = TiledCSPlus.TiledMap;

namespace NamelessRogue.Engine.Factories
{
    public class BuildingFactory {
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
                worldProvider = worldEntity.GetComponentOfType<TimeLine>().CurrentTimelineLayer.Chunks;
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
                var map = new TiledMap("Content\\"+buildingfloor.TiledFilePath.Path);              

                // Retrieving objects or layers can be done using Linq or a for loop
                var mainLayer = map.Layers.First(l => l.Name == "main");
                var animatedLayer = map.Layers.First(l => l.Name == "animated");
                int buildingSize = 60;

                var postProcessingArray = new bool[buildingSize, buildingSize];
                var tilesetPositions = new string[buildingSize, buildingSize];
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

                if (false)
                {
                    for (int loopY = 0; loopY < buildingSize; loopY++)
                    {
                        for (int loopX = 0; loopX < buildingSize; loopX++)
                        {
                            Debug.Write(postProcessingArray[loopY, loopX] ? 1 : 0);
                        }
                        Debug.Write("\n");
                    }
                }

                ////first determine which walls are corners and intersections
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

                for (int loopY = 0; loopY < buildingSize; loopY++)
                {
                    for (int loopX = 0; loopX < buildingSize; loopX++)
                    {
                        var gameTile = worldProvider.GetTile(realSpaceX + loopX, realSpaceY + loopY, floorZ);

                        if (gameTile == null)
                        {
                            gameTile = new Tile(TerrainTypes.AsphaultPoor, Biomes.None);
                            worldProvider.SetTile(realSpaceX + loopX, realSpaceY + loopY, floorZ, gameTile);
                        }
                        else
                        {
                            gameTile.Terrain = TerrainTypes.FloorGrate;
                        }

                        //add static objects
                        {
                            var tileId = mainLayer.Data[loopX + (loopY * buildingSize)];
                            if (tileId != 0)
                            {
                                var tile = tileset.Tiles.First(x => x.Id == tileId - 1);
                                var tileObjectType = tile.Properties[0].Value;
                                switch (tileObjectType)
                                {
                                    case "wall":
                                    case "wall_brick":
                                        {
                                            var wall = TerrainFurnitureFactory.GetFurniture(tileObjectType);
                                            var drawable = wall.GetComponentOfType<Drawable>();
                                            drawable.TilesetPosition = tilesetPositions[loopY, loopX];
                                            gameTile.AddEntity(wall);
                                            namelessGame.AddEntity(wall);
                                        }
                                        break;
                                    case "door":
                                    case "door_brick":
                                        {
                                            var entity = CreateDoor(realSpaceX + loopX, realSpaceY + loopY, floorZ, tileObjectType);
                                            var drawable = entity.GetComponentOfType<Drawable>();
                                            drawable.TilesetPosition = tilesetPositions[loopY, loopX];
                                            gameTile.AddEntity(entity);
                                            namelessGame.AddEntity(entity);
                                        }
                                        break;
                                    case "window":
                                    case "window_brick":
                                        {
                                            var entity = CreateWindow(realSpaceX + loopX, realSpaceY + loopY, floorZ, tileObjectType);
                                            var drawable = entity.GetComponentOfType<Drawable>();
                                            drawable.TilesetPosition = tilesetPositions[loopY, loopX];
                                            gameTile.AddEntity(entity);
                                            namelessGame.AddEntity(entity);
                                        }
                                        break;
                                    default:
                                        {

                                            var entity = TerrainFurnitureFactory.GetFurniture(tileObjectType);
                                            if (entity != null)
                                            {
                                                gameTile.AddEntity(entity);
                                                namelessGame.AddEntity(entity);
                                            }
                                        }
                                        break;
                                }
                            }
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

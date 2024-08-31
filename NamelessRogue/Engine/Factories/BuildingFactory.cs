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
        public static Entity CreateDoor(int x, int y)
        {
            Entity door  = new Entity();
            door.AddComponent(new Position(x, y));
            door.AddComponent(new Drawable("Door", new Engine.Utility.Color(0.7,0.7,0.7)));
            door.AddComponent(new Description("Door",""));
            door.AddComponent(new Door());
            door.AddComponent(new SimpleSwitch(true));
            door.AddComponent(new OccupiesTile());
            door.AddComponent(new BlocksVision());
            door.AddComponent(new Furniture());
            return door;
        }

        public static Entity CreateWindow(int x, int y)
        {
            Entity window  = new Entity();
            window.AddComponent(new Position(x, y));
            window.AddComponent(new Drawable("Window", new Engine.Utility.Color(0.9,0.9,0.9), new Engine.Utility.Color()));
            window.AddComponent(new Description("Window",""));
            window.AddComponent(new OccupiesTile());
            window.AddComponent(new Furniture());
            return window;
        }

        public static IEntity CreateDummyBuilding(int x, int y, NamelessGame namelessGame)
        {
            var diagonalNeighbors = new DiagonalNeighborProviderSelfIncluded();
            var straightNeighbors = new StraightNeighborProviderSelfIncluded();

            IEntity worldEntity = namelessGame.TimelineEntity;
            IWorldProvider worldProvider = null;
            if (worldEntity != null)
            {
                worldProvider = worldEntity.GetComponentOfType<TimeLine>().CurrentTimelineLayer.Chunks;
            }

            IEntity building = new Entity();

            building.AddComponent(new Description("",""));
            building.AddComponent(new Position(x,y));

            Building buildingComponent = new Building();

            var map = new TiledMap("Content\\Buildings\\ApartmentBlockFloor.tmx");
            var tileset = new TiledTileset("Content\\Buildings\\tileset2.tsx");

            // Retrieving objects or layers can be done using Linq or a for loop
            var myLayer = map.Layers.First(l => l.Name == "main");
            int buildingSize = 60;

            var postProcessingArray = new bool[buildingSize, buildingSize];
            var tilesetPositions = new string[buildingSize, buildingSize];
            for (int loopY = 0; loopY < buildingSize; loopY++)
            {
                for (int loopX = 0; loopX < buildingSize; loopX++)
                {
                    var gameTile = worldProvider.GetTile(x + loopX, y + loopY);
                    var tileId = myLayer.Data[loopX + (loopY * buildingSize)];
                    if(tileId != 0)
                    {
                        var tile = tileset.Tiles[tileId-1];
                        var tileObjectType = tile.Properties[0].Value;
                        if (tileObjectType == "wall" || tileObjectType == "door" || tileObjectType == "window")
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
                    Debug.Write(postProcessingArray[loopY, loopX]?1:0);
                }
                Debug.Write("\n");
            }

                    ////first determine which walls are corners and intersections
            for (int loopY = 0; loopY < buildingSize; loopY++)
            {
                for (int loopX = 0; loopX < buildingSize; loopX++)
                {
                    var cellValue = postProcessingArray[loopY, loopX];

                    if(!cellValue)
                    {
                        continue;
                    }

                    var diagonalCells = diagonalNeighbors.GetNeighbors(new AStarNavigator.Tile(loopX, loopY)).ToList();
                    var straightCells = straightNeighbors.GetNeighbors(new AStarNavigator.Tile(loopX, loopY)).ToList();
                    string tilesetPosition = "";
                    foreach (var neighbor in diagonalCells)
                    {
                        //throw away outside of bounds tiles
                        if (neighbor.X < 0 || neighbor.Y < 0 || neighbor.X == buildingSize || neighbor.Y == buildingSize )
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


            //then determine which walls are left, right, top, bottom and which are internal
            for (int i = 0; i < buildingSize; i++)
            {
                for (int j = 0; j < buildingSize; j++)
                {
                    
                }
            }


            for (int loopY = 0; loopY < buildingSize; loopY++)
            {
                for (int loopX = 0; loopX < buildingSize; loopX++)
                {
                    var gameTile = worldProvider.GetTile(x + loopX, y + loopY);
                    var tileId = myLayer.Data[loopX + (loopY * buildingSize)];
                    if (tileId != 0)
                    {
                        var tile = tileset.Tiles[tileId - 1];
                        var tileObjectType = tile.Properties[0].Value;
                        switch (tileObjectType)
                        {
                            case "wall":
                                {
                                    var wall = TerrainFurnitureFactory.WallEntity;
                                    var drawable = wall.GetComponentOfType<Drawable>();
                                    drawable.TilesetPosition = tilesetPositions[loopY, loopX];
                                    gameTile.AddEntity(wall);
                                    namelessGame.AddEntity(wall);
                                }
                                break;
                            case "door":
                                {
                                    var entity = CreateDoor(x + loopX, y + loopY);
                                    var drawable = entity.GetComponentOfType<Drawable>();
                                    drawable.TilesetPosition = tilesetPositions[loopY, loopX];
                                    gameTile.AddEntity(entity);
                                    namelessGame.AddEntity(entity);
                                }
                                break;
                            case "window":
                                {
                                    var entity = CreateWindow(x + loopX, y + loopY);
                                    var drawable = entity.GetComponentOfType<Drawable>();
                                    drawable.TilesetPosition = tilesetPositions[loopY, loopX];
                                    gameTile.AddEntity(entity);
                                    namelessGame.AddEntity(entity);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            building.AddComponent(buildingComponent);
            return building;

        }


        public static IEntity CreateBuilding(int x, int y, Generation.Settlement.BuildingBlueprint blueprint, NamelessGame namelessGame, IWorldProvider worldProvider, InternalRandom random)
        {
            IEntity building = new Entity();

            building.AddComponent(new Description("", ""));
            building.AddComponent(new Position(x, y));

            Building buildingComponent = new Building();

            

            for (int i = 0; i < blueprint.Matrix.Length; i++)
            {
                for (int j = 0; j < blueprint.Matrix[i].Length; j++)
                {
                    var tile = worldProvider.GetTile(x + j, y + i);
                    tile.Terrain = TerrainTypes.Road;
                    tile.Biome = Biomes.None;

                    var bluepringCell = blueprint.Matrix[i][j];
                    switch (bluepringCell)
                    {
                        case BlueprintCell.Wall:
                        {
                            AddToTileAndGame(tile, TerrainFurnitureFactory.WallEntity, namelessGame);
                                break;
                        }
                        case BlueprintCell.Door:
                        {
                            IEntity door = CreateDoor(x + j, y + i);
                            buildingComponent.getBuildingParts().Add(door);
                            AddToTileAndGame(tile, door, namelessGame);
                            break;

                        }
                        case BlueprintCell.Window:
                        {
                            AddToTileAndGame(tile, TerrainFurnitureFactory.WindowEntity, namelessGame);
                            break;
                        }
                        case BlueprintCell.Bed:
                        {
                            AddToTileAndGame(tile, TerrainFurnitureFactory.BedEntity, namelessGame);
                            break;
                        }
                        case BlueprintCell.IndoorsFurniture:
                        {
                            AddToTileAndGame(tile, TerrainFurnitureFactory.BedEntity, namelessGame);
                            break;
                        }

                    }


                }
            }


            {
                //BSPTree tree = new BSPTree(new Rectangle(x,y,width,height));

                //tree.Split(random,3);

                //{
                //    BSPTree child = tree.ChildA;
                //    for (int i = 0; i < child.Bounds.Width; i++)
                //    {
                //        for (int j = 0; j < child.Bounds.Height; j++)
                //        {
                //            var tile = worldProvider.GetTile(child.Bounds.X + i, child.Bounds.Y + j);
                //            tile.Terrain = TerrainLibrary.Terrains[TerrainTypes.Road];
                //            tile.Biome = BiomesLibrary.Biomes[Biomes.None];

                //            if (i == 0 || j == 0 || i == child.Bounds.Width - 1 || j == child.Bounds.Height - 1)
                //            {
                //                if (i == child.Bounds.Width / 2)
                //                {
                //                    IEntity door = CreateDoor(child.Bounds.X + i, child.Bounds.Y + j);
                //                    buildingComponent.getBuildingParts().Add(door);
                //                    namelessGame.GetEntities().Add(door);
                //                    tile.getEntitiesOnTile().Add((Entity)door);
                //                }
                //                else
                //                {
                //                    tile.getEntitiesOnTile().Add(TerrainFurnitureFactory.WallEntity);
                //                }
                //            }

                //        }
                //    }
                //}

                //{
                //    BSPTree child = tree.ChildB;
                //    for (int i = 0; i < child.Bounds.Width; i++)
                //    {
                //        for (int j = 0; j < child.Bounds.Height; j++)
                //        {
                //            var tile = worldProvider.GetTile(child.Bounds.X + i, child.Bounds.Y + j);
                //            tile.Terrain = TerrainLibrary.Terrains[TerrainTypes.Road];
                //            tile.Biome = BiomesLibrary.Biomes[Biomes.None];

                //            if (i == 0 || j == 0 || i == child.Bounds.Width - 1 || j == child.Bounds.Height - 1)
                //            {
                //                if (i == child.Bounds.Width / 2)
                //                {
                //                    IEntity door = CreateDoor(child.Bounds.X + i, child.Bounds.Y + j);
                //                    buildingComponent.getBuildingParts().Add(door);
                //                    namelessGame.GetEntities().Add(door);
                //                    tile.getEntitiesOnTile().Add((Entity)door);
                //                }
                //                else
                //                {
                //                    tile.getEntitiesOnTile().Add(TerrainFurnitureFactory.WallEntity);
                //                }
                //            }

                //        }
                //    }
                //}
            }

            building.AddComponent(buildingComponent);
            return building;

        }

        private static void AddToTileAndGame(Tile tile, IEntity entity, NamelessGame namelessGame)
        {
            namelessGame.AddEntity(entity);
            tile.AddEntity((Entity)entity);
        }

    }
}

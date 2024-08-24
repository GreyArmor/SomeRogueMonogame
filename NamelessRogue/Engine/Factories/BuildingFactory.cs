using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
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
using NamelessRogue.Engine.Utility;
using NamelessRogue.shell;
using RogueSharp.Random;
using TiledCSPlus;
using TiledMap = TiledCSPlus.TiledMap;

namespace NamelessRogue.Engine.Factories
{
    public class BuildingFactory {
        public static Entity CreateDoor(int x, int y)
        {
            Entity door  = new Entity();
            door.AddComponent(new Position(x, y));
            door.AddComponent(new Drawable("Door", new Engine.Utility.Color(0.7,0.7,0.7), new Engine.Utility.Color()));
            door.AddComponent(new Description("Door",""));
            door.AddComponent(new Door());
            door.AddComponent(new SimpleSwitch(true));
            door.AddComponent(new OccupiesTile());
            door.AddComponent(new BlocksVision());
            door.AddComponent(new Furniture());
            return door;
        }

        public static Entity CreateWindow(int x, int y, NamelessGame namelessGame)
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
            var tileset = new TiledTileset("Content\\Buildings\\symbols.tsx");

            // Retrieving objects or layers can be done using Linq or a for loop
            var myLayer = map.Layers.First(l => l.Name == "main");

            for (int i = 0; i < 64; i++)
            {
                for (int j = 0; j<64; j++)
                {

                    var gameTile = worldProvider.GetTile(x + i, y + j);

                    var tileId = myLayer.Data[j + (i * 64)];
                    if(tileId != 0)
                    {
                        var tile = tileset.Tiles[tileId-1];
                        var tileObjectType = tile.Properties[0].Value;
                        if (tileObjectType != "")
                        {
                            switch (tileObjectType)
                            {
                                case "wall":
                                    var wall = TerrainFurnitureFactory.WallEntity;
                                    gameTile.AddEntity(wall);
                                    namelessGame.AddEntity(wall);
                                    break;
                                case "door":
                                    {
                                        var entity = CreateDoor(x + i, y + j);
                                        gameTile.AddEntity(entity);
                                        namelessGame.AddEntity(entity);
                                    }
                                    break;
                                case "window":
                                    {
                                        var entity = CreateWindow(x + i, y + j, namelessGame);
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
            }


            // You can use the helper methods to get useful information to generate maps
            if (map.IsTileFlippedDiagonal(myLayer, 3, 5))
            {
                // Do something
            }


            //for (int i = 0;i< width; i++)
            //{
            //    for (int j = 0;j< height; j++)
            //    {
            //        var tile = worldProvider.GetTile(x + i, y + j);
            //        tile.Terrain = TerrainTypes.Road;
            //        tile.Biome = Biomes.None;
            //        if (i == 0 || j == 0 || i == width - 1 || j == height - 1)
            //        {
            //            if (i == width / 2)
            //            {
            //                IEntity door = CreateDoor(x + i, y + j);
            //                buildingComponent.getBuildingParts().Add(door);
            //                namelessGame.AddEntity(door);
            //                tile.AddEntity((Entity) door);
            //            }
            //            else
            //            {
            //                var wall = TerrainFurnitureFactory.WallEntity;
            //                tile.AddEntity(wall);
            //                namelessGame.AddEntity(wall);
            //            }
            //        }
            //    }
            //}
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

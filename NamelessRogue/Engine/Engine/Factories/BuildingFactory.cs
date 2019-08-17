using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Engine.Components.Environment;
using NamelessRogue.Engine.Engine.Components.Interaction;
using NamelessRogue.Engine.Engine.Components.Physical;
using NamelessRogue.Engine.Engine.Components.Rendering;
using NamelessRogue.Engine.Engine.Components.UI;
using NamelessRogue.Engine.Engine.Generation.Settlement;
using NamelessRogue.Engine.Engine.Generation.World;
using NamelessRogue.Engine.Engine.Infrastructure;
using NamelessRogue.Engine.Engine.Utility;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Factories
{
    public class BuildingFactory {
        public static IEntity CreateDoor(int x, int y)
        {
            IEntity door  = new Entity();
            door.AddComponent(new Position(x, y));
            door.AddComponent(new Drawable('C', new Engine.Utility.Color(0.7,0.7,0.7), new Engine.Utility.Color()));
            door.AddComponent(new Description("Door",""));
            door.AddComponent(new Door());
            door.AddComponent(new SimpleSwitch(true));
            door.AddComponent(new OccupiesTile());
            door.AddComponent(new BlocksVision());
            return door;
        }

        public static IEntity CreateWindow(int x, int y, NamelessGame namelessGame)
        {
            IEntity window  = new Entity();
            window.AddComponent(new Position(x, y));
            window.AddComponent(new Drawable('O', new Engine.Utility.Color(0.9,0.9,0.9), new Engine.Utility.Color()));
            window.AddComponent(new Description("Window",""));
            window.AddComponent(new OccupiesTile());
            return window;
        }

        public static IEntity CreateDummyBuilding(int x, int y, int width ,int height, NamelessGame namelessGame)
        {

            IEntity worldEntity = namelessGame.GetEntityByComponentClass<TimeLine>();
            IChunkProvider worldProvider = null;
            if (worldEntity != null)
            {
                worldProvider = worldEntity.GetComponentOfType<TimeLine>().CurrentTimelineLayer.Chunks;
            }

            IEntity building = new Entity();

            building.AddComponent(new Description("",""));
            building.AddComponent(new Position(x,y));

            Building buildingComponent = new Building();

            for (int i = 0;i< width; i++)
            {
                for (int j = 0;j< height; j++)
                {
                    var tile = worldProvider.GetTile(x + i, y + j);
                    tile.Terrain = TerrainLibrary.Terrains[TerrainTypes.Road];
                    tile.Biome = BiomesLibrary.Biomes[Biomes.None];
                    if (i == 0 || j == 0 || i == width - 1 || j == height - 1)
                    {
                        if (i == width / 2)
                        {
                            IEntity door = CreateDoor(x + i, y + j);
                            buildingComponent.getBuildingParts().Add(door);
                            namelessGame.GetEntities().Add(door);
                            tile.getEntitiesOnTile().Add((Entity) door);
                        }
                        else
                        {
                            tile.getEntitiesOnTile().Add(TerrainFurnitureFactory.WallEntity);
                        }
                    }
                }



            }
            building.AddComponent(buildingComponent);
            return building;

        }


        public static IEntity CreateBuilding(int x, int y, BuildingBlueprint blueprint, NamelessGame namelessGame, IChunkProvider worldProvider, Random random)
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
                    tile.Terrain = TerrainLibrary.Terrains[TerrainTypes.Road];
                    tile.Biome = BiomesLibrary.Biomes[Biomes.None];

                    var bluepringCell = blueprint.Matrix[i][j];
                    switch (bluepringCell)
                    {
                        case BlueprintCell.Wall:
                        {
                            tile.getEntitiesOnTile().Add(TerrainFurnitureFactory.WallEntity);
                            break;
                        }
                        case BlueprintCell.Door:
                        {
                            IEntity door = CreateDoor(x + j, y + i);
                            buildingComponent.getBuildingParts().Add(door);
                            namelessGame.GetEntities().Add(door);
                            tile.getEntitiesOnTile().Add((Entity) door);
                            break;

                        }
                        case BlueprintCell.Window:
                        {
                            tile.getEntitiesOnTile().Add(TerrainFurnitureFactory.WindowEntity);
                            break;
                        }
                        case BlueprintCell.Bed:
                        {
                            tile.getEntitiesOnTile().Add(TerrainFurnitureFactory.BedEntity);
                            break;
                        }
                        case BlueprintCell.IndoorsFurniture:
                        {
                            tile.getEntitiesOnTile().Add(TerrainFurnitureFactory.BedEntity);
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

    }
}

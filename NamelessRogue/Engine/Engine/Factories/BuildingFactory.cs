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


        public static IEntity CreateTiltedBuilding(int x, int y, int width, int height, NamelessGame namelessGame, IChunkProvider worldProvider)
        {
            IEntity building = new Entity();

            building.AddComponent(new Description("", ""));
            building.AddComponent(new Position(x, y));

            Building buildingComponent = new Building();

            var shiftedY = y - 5;

            var points = new List<Point> { };

            //points.Add(new Point(x + (width / 2), y));
            //points.Add(new Point(x , y + (height / 2)));
            //points.Add(new Point(x + (width / 2), y + height));
           // points.Add(new Point(x + width, y+(height/2)));



            //points.Add(new Point(x, y));
            //points.Add(new Point(x, y + height));
            //points.Add(new Point(x + (width), y + height));
            //points.Add(new Point(x + width, y));

            //var reversedpoints = points.ToArray();
            //points.AddRange(reversedpoints.Reverse());


            // WorldLineDrawer.BezierPath(points.ToArray(),worldProvider, TerrainFurnitureFactory.WallEntity);
           // List<Tile> tiles = new List<Tile>();
           // for (int i = 0; i < points.Count - 1; i++)
           // {
           //     tiles.AddRange(WorldLineDrawer.PlotLineAA(points[i], points[i + 1], worldProvider));
           // }
           //// tiles.AddRange(WorldLineDrawer.PlotLineAA(points[3], points[0], worldProvider));

           // foreach (var tile in tiles)
           // {
           //    tile.getEntitiesOnTile().Add(TerrainFurnitureFactory.WallEntity); 
           // }
            




            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
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

    }
}

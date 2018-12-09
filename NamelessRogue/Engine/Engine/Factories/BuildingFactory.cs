using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Engine.Components.Environment;
using NamelessRogue.Engine.Engine.Components.Interaction;
using NamelessRogue.Engine.Engine.Components.Physical;
using NamelessRogue.Engine.Engine.Components.Rendering;
using NamelessRogue.Engine.Engine.Components.UI;
using NamelessRogue.Engine.Engine.Infrastructure;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Factories
{
    public class BuildingFactory {
        public static IEntity CreateDoor(int x, int y)
        {
            IEntity door  = new Entity();
            door.AddComponent(new Position(x, y));
            door.AddComponent(new Drawable('C', new Engine.Utility.Color(0.7,0.7,0.7)));
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
            window.AddComponent(new Drawable('O', new Engine.Utility.Color(0.9,0.9,0.9)));
            window.AddComponent(new Description("Window",""));
            window.AddComponent(new OccupiesTile());
            return window;
        }

        public static IEntity CreateDummyBuilding(int x, int y, int widthHeight, NamelessGame namelessGame)
        {

            IEntity worldEntity = namelessGame.GetEntityByComponentClass<ChunkData>();
            IChunkProvider worldProvider = null;
            if (worldEntity != null)
            {
                worldProvider = worldEntity.GetComponentOfType<ChunkData>();
            }

            IEntity building = new Entity();

            building.AddComponent(new Description("Window",""));
            building.AddComponent(new Position(x,y));

            Building buildingComponent = new Building();

            for (int i = 0;i<widthHeight; i++)
            {
                for (int j = 0;j<widthHeight; j++)
                {
                    var tile = worldProvider.GetTile(x + i, y + j);
                    tile.SetTerrainType(TerrainTypes.Nothingness);
                    if (i == 0 || j == 0 || i == widthHeight - 1 || j == widthHeight - 1)
                    {
                        if (i == widthHeight / 2)
                        {
                            IEntity door = CreateDoor(x + i, y + j);
                            buildingComponent.getBuildingParts().Add(door);
                            namelessGame.GetEntities().Add(door);
                            tile.getEntitiesOnTile().Add(door);
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

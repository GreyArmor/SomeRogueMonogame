using NamelessRogue.Engine.Abstraction;
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
            return door;
        }

        public static IEntity CreateWall(int x, int y)
        {
            IEntity wall  = new Entity();
            wall.AddComponent(new Position(x, y));
            wall.AddComponent(new Drawable('#', new Engine.Utility.Color(0.9,0.9,0.9)));
            wall.AddComponent(new Description("Wall",""));
            wall.AddComponent(new OccupiesTile());
            return wall;
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
            IEntity building = new Entity();

            building.AddComponent(new Description("Window",""));
            building.AddComponent(new Position(x,y));

            Building buildingComponent = new Building();

            for (int i = 0;i<widthHeight; i++)
            {
                for (int j = 0;j<widthHeight; j++)
                {
                    if(i==0 || j==0 || i==widthHeight-1 || j==widthHeight-1) {
                        if (i == widthHeight / 2) {
                            IEntity door = CreateDoor(x + i, y + j);
                            buildingComponent.getBuildingParts().Add(door);
                            namelessGame.GetEntities().Add(door);
                        } else {
                            IEntity wall = CreateWall(x + i, y + j);
                            buildingComponent.getBuildingParts().Add(wall);
                            namelessGame.GetEntities().Add(wall);
                        }
                    }
                }



            }
            building.AddComponent(buildingComponent);
            return building;

        }
    }
}

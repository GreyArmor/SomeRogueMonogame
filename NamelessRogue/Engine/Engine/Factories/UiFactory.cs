 

using GeonBit.UI;
using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Engine.UiScreens;
using NamelessRogue.shell;
using Entity = NamelessRogue.Engine.Engine.Infrastructure.Entity;

/**
 * Created by Admin on 16.06.2017.
 */
namespace NamelessRogue.Engine.Engine.Factories
{
    public class UiFactory
    {
        public static Hud HudInstance { get; private set; }
        public static WorldBoardScreen WorldBoardScreen { get; private set; }

        public static void CreateHud(NamelessGame game)
        {
            HudInstance = new Hud(game);
        }

        public static void CreateWorldBoardScreen(NamelessGame game)
        {
            WorldBoardScreen = new WorldBoardScreen(game);
        }
    }
}

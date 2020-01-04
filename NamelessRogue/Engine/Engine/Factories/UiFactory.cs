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

        public static MainMenuScreen MainMenuScreen { get; private set; }

        public static InventoryScreen InventoryScreen { get; private set; }
        public static void CreateHud(NamelessGame game)
        {
            HudInstance = new Hud(game);
        }

        public static void CreateWorldBoardScreen(NamelessGame game)
        {
            WorldBoardScreen = new WorldBoardScreen(game);
        }

        public static void CreateMainMenuScreen(NamelessGame game)
        {
            MainMenuScreen = new MainMenuScreen(game);
        }

        public static void CreateInventoryScreen(NamelessGame game)
        {
            InventoryScreen = new InventoryScreen(game);
        }

    }
}

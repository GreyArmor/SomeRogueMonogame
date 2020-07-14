using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Context;
using NamelessRogue.Engine.Engine.Input;
using NamelessRogue.Engine.Engine.Systems;
using NamelessRogue.Engine.Engine.Systems.Ingame;
using NamelessRogue.Engine.Engine.Systems.Inventory;
using NamelessRogue.Engine.Engine.Systems.MainMenu;
using NamelessRogue.Engine.Engine.Systems.Map;
using NamelessRogue.Engine.Engine.Systems.PickUpItems;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Factories
{
    public class ContextFactory
    {
        private static GameContext IngameContext;
        public static GameContext GetIngameContext(NamelessGame game)
        {

            if (IngameContext != null)
            {
                return IngameContext;
            }
            else
            {
     ; 
                var systems = new List<ISystem>();
                systems.Add(new InputSystem(new IngameKeyIntentTraslator(), game));
                systems.Add(new IngameIntentSystem());
                systems.Add(new AiSystem());
                systems.Add(new TurnManagementSystem());
                systems.Add(new CombatSystem());
                systems.Add(new InventorySystem());
                systems.Add(new SwitchSystem());
                systems.Add(new DamageHandlingSystem());
                systems.Add(new DeathSystem());
                systems.Add(new HudSystem());
                systems.Add(new ChunkManagementSystem());
              
                var renderingSystem = new RenderingSystem(game.GetSettings());
                var uiSystem = new UIRenderSystem();

                // create and init the UI manager
                UiFactory.CreateHud(game);

                IngameContext = new GameContext(systems, new List<ISystem>() {uiSystem, renderingSystem},
                    UiFactory.HudInstance);

                return IngameContext;
            }
        }

        private static GameContext WorldBoardContext;
        public static GameContext GetWorldBoardContext(NamelessGame game)
        {

            if (WorldBoardContext != null)
            {
                return WorldBoardContext;
            }
            else
            {
                var renderingSystem = new MapRenderingSystem(game.GetSettings(), game.WorldSettings);
                var systems = new List<ISystem>();
                systems.Add(new InputSystem(new WorldMapKeyIntentTranslator(), game));
                systems.Add(new WorldBoardIntentSystem());
                systems.Add(new WorldBoardScreenSystem(renderingSystem));
              
                var uiSystem = new UIRenderSystem();

                // create and init the UI manager
                UiFactory.CreateWorldBoardScreen(game);
                WorldBoardContext = new GameContext(systems, new List<ISystem>() { uiSystem, renderingSystem }, UiFactory.WorldBoardScreen);
                return WorldBoardContext;
            }
        }


        private static GameContext mainMenuContext;
        public static GameContext GetMainMenuContext(NamelessGame game)
        {

            if (mainMenuContext != null)
            {
                return mainMenuContext;
            }
            else
            {
                var systems = new List<ISystem>();
                systems.Add(new InputSystem(new MainMenuKeyIntentTranslator(),game ));
                systems.Add(new MainMenuScreenSystem());
                var uiSystem = new UIRenderSystem();

                // create and init the UI manager
                UiFactory.CreateMainMenuScreen(game);
                mainMenuContext = new GameContext(systems, new List<ISystem>() { uiSystem }, UiFactory.MainMenuScreen);
                return mainMenuContext;
            }
        }


        private static GameContext inventoryContext;
        public static GameContext GetInventoryContext(NamelessGame game)
        {

            if (inventoryContext != null)
            {
                return inventoryContext;
            }
            else
            {
                var systems = new List<ISystem>();
                systems.Add(new InputSystem(new InventoryKeyIntentTranslator(), game));
                systems.Add(new InventoryScreenSystem());
                systems.Add(new InventorySystem());
                var uiSystem = new UIRenderSystem();

                // create and init the UI manager
                UiFactory.CreateInventoryScreen(game);
                inventoryContext = new GameContext(systems, new List<ISystem>() { uiSystem }, UiFactory.InventoryScreen);
                return inventoryContext;
            }
        }


        private static GameContext pickUpContext;
        public static GameContext GetPickUpItemContext(NamelessGame game)
        {

            if (pickUpContext != null)
            {
                return pickUpContext;
            }
            else
            {
                var systems = new List<ISystem>();
                systems.Add(new InputSystem(new PickUpKeyIntentTranslator(), game));
                systems.Add(new PickUpItemSystem());
                systems.Add(new InventorySystem());
                var uiSystem = new UIRenderSystem();

                // create and init the UI manager
                UiFactory.CreatePickUpItemsScreenn(game);
                pickUpContext = new GameContext(systems, new List<ISystem>() { uiSystem }, UiFactory.PickUpItemsScreen);
                return pickUpContext;
            }
        }

        internal static void InitAllContexts(NamelessGame game)
        {
            GetIngameContext(game).ContextScreen.Hide();
            GetInventoryContext(game).ContextScreen.Hide(); ;
            GetMainMenuContext(game).ContextScreen.Hide(); ;
            GetPickUpItemContext(game).ContextScreen.Hide(); ;
            GetWorldBoardContext(game).ContextScreen.Hide(); ;
        }
    }
}

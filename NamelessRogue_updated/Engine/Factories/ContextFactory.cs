using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Context;
using NamelessRogue.Engine.Input;
using NamelessRogue.Engine.Systems;
using NamelessRogue.Engine.Systems._3DView;
using NamelessRogue.Engine.Systems.Ingame;
using NamelessRogue.Engine.Systems.Inventory;
using NamelessRogue.Engine.Systems.MainMenu;
using NamelessRogue.Engine.Systems.Map;
using NamelessRogue.Engine.Systems.PickUpItems;
using NamelessRogue.Engine.UI;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Factories
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
                systems.Add(new Camera3DSystem(game));
                systems.Add(new TerrainClickSystem());
			  //  systems.Add(new ChunkManagementSystem());
				systems.Add(new Chunk3DManagementSystem());
                systems.Add(new SoundPlaySystem());
              
               // var renderingSystem = new RenderingSystem(game.GetSettings());

                var renderingSystem = new RenderingSystem3D(game.GetSettings());
                var uiSystem = new UIRenderSystem(game);


                IngameContext = new GameContext(systems, new List<ISystem>() {uiSystem, renderingSystem},
                    UIController.Instance.HudScreen);

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
                systems.Add(new SoundPlaySystem());

                var uiSystem = new UIRenderSystem(game);

                // create and init the UI manager
                WorldBoardContext = new GameContext(systems, new List<ISystem>() { uiSystem, renderingSystem }, UIController.Instance.MapScreen);
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
                systems.Add(new SoundPlaySystem());
                var uiSystem = new UIRenderSystem(game);

                // create and init the UI manager
                mainMenuContext = new GameContext(systems, new List<ISystem>() { uiSystem }, UIController.Instance.MainMenu);
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
                systems.Add(new SoundPlaySystem());
                var uiSystem = new UIRenderSystem(game);

                inventoryContext = new GameContext(systems, new List<ISystem>() { uiSystem }, UIController.Instance.InventoryScreen);
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
                systems.Add(new PickUpItemScreenSystem());
                systems.Add(new InventorySystem());
                systems.Add(new SoundPlaySystem());
                var uiSystem = new UIRenderSystem(game);

                pickUpContext = new GameContext(systems, new List<ISystem>() { uiSystem }, UIController.Instance.MainMenu);
                return pickUpContext;
            }
        }


        private static GameContext worldGenContext;
        public static GameContext GetWorldGenContext(NamelessGame game)
        {

            if (worldGenContext != null)
            {
                return worldGenContext;
            }
            else
            {
                var systems = new List<ISystem>();
                systems.Add(new InputSystem(new MainMenuKeyIntentTranslator(), game));
                systems.Add(new WorldGenSystem());
                systems.Add(new SoundPlaySystem());
                var uiSystem = new UIRenderSystem(game);

                worldGenContext = new GameContext(systems, new List<ISystem>() { uiSystem }, UIController.Instance.WorldGenScreen);
                return worldGenContext;
            }
        }


        internal static void InitAllContexts(NamelessGame game)
        {
            GetIngameContext(game);
            GetInventoryContext(game);
            GetMainMenuContext(game);
            GetPickUpItemContext(game);
            GetWorldBoardContext(game);
		}

        internal static void ReleaseAllContexts(NamelessGame game)
        {
            IngameContext = null;
            inventoryContext = null;
            mainMenuContext = null;
            pickUpContext = null;
            WorldBoardContext = null;
        }

    }
}

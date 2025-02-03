using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Context;
using NamelessRogue.Engine.Input;
using NamelessRogue.Engine.Systems;
using NamelessRogue.Engine.Systems.Editors;
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
				systems.Add(new ChunkManagementSystem());
				systems.Add(new InputSystem(new IngameKeyIntentTraslator(), game));
                systems.Add(new IngameIntentSystem());

                systems.Add(new PlayerMovementSystem());
                systems.Add(new InteractSystem());
                systems.Add(new AbilitySystem());
                systems.Add(new VisibilitySystem());
                systems.Add(new TargetingSystem());
                
                       
                systems.Add(new InventorySystem());
                systems.Add(new EquipSystem());
                systems.Add(new FireWeaponSystem());
                systems.Add(new ProjectileSystem());
				systems.Add(new TurnManagementSystem());
                systems.Add(new AiSystem());
                systems.Add(new FlowFieldMovementSystem());


                systems.Add(new ConsumableSystem());
                systems.Add(new ModifierSystem());
                systems.Add(new CombatSystem());
                systems.Add(new FireSystem());

                systems.Add(new SwitchSystem());
                systems.Add(new DamageHandlingSystem());
                systems.Add(new DeathSystem());
                systems.Add(new HudSystem());
                systems.Add(new CharacterAnimationSystem());
                systems.Add(new SoundPlaySystem());
              
               // var renderingSystem = new RenderingSystem(game.GetSettings());

                var renderingSystem = new RenderingSystem(game.GetSettings());
                var uiSystem = new UIRenderSystem(game);


				IngameContext = new GameContext(systems, new List<ISystem>() { renderingSystem,  uiSystem, new HudElementsRenderingSystem(game.Settings) },
                    UIContainer.Instance.HudScreen, "InGame");

                return IngameContext;
            }
        }

        private static GameContext IngameContextMenu;
        public static GameContext GetIngameContextMenu(NamelessGame game)
        {

            if (IngameContextMenu != null)
            {
                return IngameContextMenu;
            }
            else
            {
                var systems = new List<ISystem>();
                systems.Add(new InputSystem(new IngameKeyIntentTraslator(), game));
                systems.Add(new IngameIntentSystem());
                systems.Add(new InteractSystem());
                systems.Add(new VisibilitySystem());
                systems.Add(new EquipSystem());
                systems.Add(new TurnManagementSystem());
                systems.Add(new HudSystem());
                systems.Add(new SoundPlaySystem());

                var renderingSystem = new RenderingSystem(game.GetSettings());
                var uiSystem = new UIRenderSystem(game);


                IngameContext = new GameContext(systems, new List<ISystem>() { renderingSystem, uiSystem, new HudElementsRenderingSystem(game.Settings) },
                    UIContainer.Instance.HudScreen, "InGame");

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
                WorldBoardContext = new GameContext(systems, new List<ISystem>() { uiSystem, renderingSystem }, UIContainer.Instance.MapScreen, "WorldMap");
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
                var backgroundSystem = new MainMenuBackgroundRenderingSystem(game);
			
				// create and init the UI manager
				mainMenuContext = new GameContext(systems, new List<ISystem>() { backgroundSystem, uiSystem }, UIContainer.Instance.MainMenu, "MainMenu");
                return mainMenuContext;
            }
        }

        private static GameContext editorsPickerContext;
        public static GameContext GetEditorsPickerContext(NamelessGame game)
        {

            if (editorsPickerContext != null)
            {
                return editorsPickerContext;
            }
            else
            {
                var systems = new List<ISystem>();
     //           systems.Add(new InputSystem(new MainMenuKeyIntentTranslator(), game));
                systems.Add(new EditorsPickerScreenSystem());
                systems.Add(new SoundPlaySystem());
                var uiSystem = new UIRenderSystem(game);
                var backgroundSystem = new MainMenuBackgroundRenderingSystem(game);

                // create and init the UI manager
                editorsPickerContext = new GameContext(systems, new List<ISystem>() { backgroundSystem, uiSystem }, UIContainer.Instance.EditorsPickerScreen, "MainMenu");
                return editorsPickerContext;
            }
        }

        private static GameContext editorItemContext;
        public static GameContext GetEditorItemContext(NamelessGame game)
        {

            if (editorItemContext != null)
            {
                return editorItemContext;
            }
            else
            {
                var systems = new List<ISystem>();
                systems.Add(new EditorItemScreenSystem());
                systems.Add(new SoundPlaySystem());
                var uiSystem = new UIRenderSystem(game);
                var backgroundSystem = new MainMenuBackgroundRenderingSystem(game);
                // create and init the UI manager
                editorItemContext = new GameContext(systems, new List<ISystem>() { backgroundSystem, uiSystem }, UIContainer.Instance.EditorItemScreen, "MainMenu");
                return editorItemContext;
            }
        }

        private static GameContext editorCharacterContext;
        internal static GameContext GetEditorCharacterContext(NamelessGame game)
        {
            if (editorCharacterContext != null)
            {
                return editorCharacterContext;
            }
            else
            {
                var systems = new List<ISystem>();
                systems.Add(new EditorCharacterScreenSystem());
                systems.Add(new SoundPlaySystem());
                var uiSystem = new UIRenderSystem(game);
                var backgroundSystem = new MainMenuBackgroundRenderingSystem(game);
                // create and init the UI manager
                editorCharacterContext = new GameContext(systems, new List<ISystem>() { backgroundSystem, uiSystem, new EditorCharacterScreenSpriteRenderSystem() }, UIContainer.Instance.EditorCharacterScreen, "MainMenu");
                return editorCharacterContext;
            }
        }
        private static GameContext editorBuffContext;
        internal static GameContext GetEditorBuffContext(NamelessGame game)
        {
            if (editorBuffContext != null)
            {
                return editorBuffContext;
            }
            else
            {
                var systems = new List<ISystem>();
                systems.Add(new EditorBuffScreenSystem());
                systems.Add(new SoundPlaySystem());
                var uiSystem = new UIRenderSystem(game);
                var backgroundSystem = new MainMenuBackgroundRenderingSystem(game);
                // create and init the UI manager
                editorBuffContext = new GameContext(systems, new List<ISystem>() { backgroundSystem, uiSystem}, UIContainer.Instance.EditorBuffScreen, "MainMenu");
                return editorBuffContext;
            }
        }

        private static GameContext editorAbilityContext;
        internal static GameContext GetEditorAbilityContext(NamelessGame game)
        {
            if (editorAbilityContext != null)
            {
                return editorAbilityContext;
            }
            else
            {
                var systems = new List<ISystem>();
                systems.Add(new EditorAbilityScreenSystem());
                systems.Add(new SoundPlaySystem());
                var uiSystem = new UIRenderSystem(game);
                var backgroundSystem = new MainMenuBackgroundRenderingSystem(game);
                // create and init the UI manager
                editorAbilityContext = new GameContext(systems, new List<ISystem>() { backgroundSystem, uiSystem }, UIContainer.Instance.EditorAbilityScreen, "MainMenu");
                return editorAbilityContext;
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
                systems.Add(new ConsumableSystem());
                systems.Add(new EquipSystem());
                systems.Add(new ModifierSystem());
                systems.Add(new SoundPlaySystem());
                var uiSystem = new UIRenderSystem(game);

                inventoryContext = new GameContext(systems, new List<ISystem>() { uiSystem }, UIContainer.Instance.InventoryScreen, "");
                return inventoryContext;
            }
        }

        private static GameContext abilityScreenContext;
        public static GameContext GetAbilityScreenContext(NamelessGame game)
        {

            if (abilityScreenContext != null)
            {
                return abilityScreenContext;
            }
            else
            {
                var systems = new List<ISystem>();
                systems.Add(new InputSystem(new AbilityKeyIntentTranslator(), game));
                systems.Add(new AbilityScreenSystem());
                systems.Add(new ModifierSystem());
                systems.Add(new SoundPlaySystem());
                var uiSystem = new UIRenderSystem(game);

                abilityScreenContext = new GameContext(systems, new List<ISystem>() { uiSystem }, UIContainer.Instance.AbilityScreen, "");
                return abilityScreenContext;
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

                pickUpContext = new GameContext(systems, new List<ISystem>() { uiSystem }, UIContainer.Instance.MainMenu, "");
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

                worldGenContext = new GameContext(systems, new List<ISystem>() { uiSystem }, UIContainer.Instance.WorldGenScreen, "NewWorld");
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
        private static GameContext EditorDialogContext;
        public static GameContext GetEditorDialogContext(NamelessGame game)
        {

            if (EditorDialogContext != null)
            {
                return EditorDialogContext;
            }
            else
            {
                var systems = new List<ISystem>();
                systems.Add(new InputSystem(new MainMenuKeyIntentTranslator(), game));
                systems.Add(new EditorDialogScreenSystem());
                systems.Add(new SoundPlaySystem());
                var uiSystem = new UIRenderSystem(game);
                var backgroundSystem = new MainMenuBackgroundRenderingSystem(game);

                // create and init the UI manager
                EditorDialogContext = new GameContext(systems, new List<ISystem>() { backgroundSystem, uiSystem }, UIContainer.Instance.EditorDialogScreen, "Dialog");
                return EditorDialogContext;
            }
        }

    }
}

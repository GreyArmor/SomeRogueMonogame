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
using NamelessRogue.Engine.Systems.WorldGen;
using NamelessRogue.Engine.UI;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Factories
{
    public class ContextFactory
    {
        private static List<ISystem> ingameLogicSystems = null;
        private static List<ISystem> ingameRenderSystems = null;
        private static GameContext IngameContext;
        public static GameContext GetIngameContext(NamelessGame game)
        {

            if (IngameContext != null)
            {
                return IngameContext;
            }
            else
            {
                //i put these sustems in a list to use with other contexts that overlay over ingame context; example: dialog system

                var ingameIntentSystem = new IngameIntentSystem();

                ingameLogicSystems = new List<ISystem>();
				ingameLogicSystems.Add(new ChunkManagementSystem());
				ingameLogicSystems.Add(new InputSystem(new IngameKeyIntentTraslator(), game));
                ingameLogicSystems.Add(ingameIntentSystem);
                ingameLogicSystems.Add(new PlayerMovementSystem());
                ingameLogicSystems.Add(new InteractSystem());
                ingameLogicSystems.Add(new AbilitySystem());
                ingameLogicSystems.Add(new VisibilitySystem());
                ingameLogicSystems.Add(new TargetingSystem());   
                ingameLogicSystems.Add(new InventorySystem());
                ingameLogicSystems.Add(new EquipSystem());
                ingameLogicSystems.Add(new FireWeaponSystem());
                ingameLogicSystems.Add(new ProjectileSystem());
				ingameLogicSystems.Add(new TurnManagementSystem());
                ingameLogicSystems.Add(new StreetlightsSystem());
                ingameLogicSystems.Add(new AiSystem());
                ingameLogicSystems.Add(new FlowFieldMovementSystem());
                ingameLogicSystems.Add(new ConsumableSystem());
                ingameLogicSystems.Add(new ModifierSystem());
                ingameLogicSystems.Add(new CombatSystem());
                ingameLogicSystems.Add(new FireSystem());
                ingameLogicSystems.Add(new SwitchSystem());
                ingameLogicSystems.Add(new DamageHandlingSystem());
                ingameLogicSystems.Add(new DeathSystem());
                ingameLogicSystems.Add(new HudSystem());
                ingameLogicSystems.Add(new CharacterAnimationSystem());
                ingameLogicSystems.Add(new SoundPlaySystem());

                // var renderingSystem = new RenderingSystem(game.GetSettings());
                ingameRenderSystems = new List<ISystem>();
                var renderingSystem = new RenderingSystem(game.GetSettings());
                var uiSystem = new UIRenderSystem(game);

                ingameRenderSystems.Add(renderingSystem);
                ingameRenderSystems.Add(uiSystem);
                ingameRenderSystems.Add(new HudElementsRenderingSystem(game.Settings));
                ingameRenderSystems.Add(new ProjectileRendringSystem());
                ingameRenderSystems.Add(new StreetlightsRenderingSystem());
                ingameRenderSystems.Add(new SFXSystem());

				IngameContext = new GameContext(ingameLogicSystems.ToList(), ingameRenderSystems, UIContainer.Instance.HudScreen, "InGame");
                ingameLogicSystems.Remove(ingameIntentSystem);
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

                IngameContext = new GameContext(systems.ToList(), new List<ISystem>() { renderingSystem, uiSystem, new HudElementsRenderingSystem(game.Settings) }, UIContainer.Instance.HudScreen, "InGame");

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
                WorldBoardContext = new GameContext(systems, new List<ISystem>() { renderingSystem, uiSystem }, UIContainer.Instance.MapScreen, "WorldMap");
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
                systems.Add(new WorldGenerationSystem());
                systems.Add(new SoundPlaySystem());
                var uiSystem = new UIRenderSystem(game);

                worldGenContext = new GameContext(systems, new List<ISystem>() { uiSystem }, UIContainer.Instance.WorldGenScreen, "NewWorld");
                return worldGenContext;
            }
        }

        private static GameContext worldGenerationProgressContext;
        public static GameContext GetWorldGenerationProgressContext(NamelessGame game)
        {

            if (worldGenerationProgressContext != null)
            {
                return worldGenerationProgressContext;
            }
            else
            {
                var systems = new List<ISystem>();
                systems.Add(new InputSystem(new MainMenuKeyIntentTranslator(), game));
                systems.Add(new WorldGenerationProgressSystem());
                systems.Add(new SoundPlaySystem());
                var uiSystem = new UIRenderSystem(game);

                worldGenerationProgressContext = new GameContext(systems, new List<ISystem>() { uiSystem }, UIContainer.Instance.WorldGenerationProgressUI, "NewWorld");
                return worldGenerationProgressContext;
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
            editorAbilityContext = null;
            editorBuffContext = null;
            editorCharacterContext  = null;
            editorItemContext = null;
            editorsPickerContext = null;
            worldGenContext = null;
            DialogContext = null;

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

                EditorDialogContext = new GameContext(systems, new List<ISystem>() { backgroundSystem, uiSystem }, UIContainer.Instance.EditorDialogScreen, "Dialog");
                return EditorDialogContext;
            }
        }

        private static GameContext EditorQuestContext;
        public static GameContext GetEEditorQuestContext(NamelessGame game)
        {

            if (EditorQuestContext != null)
            {
                return EditorQuestContext;
            }
            else
            {
                var systems = new List<ISystem>();
                systems.Add(new InputSystem(new MainMenuKeyIntentTranslator(), game));
                systems.Add(new EditorQuestScreenSystem());
                systems.Add(new SoundPlaySystem());
                var uiSystem = new UIRenderSystem(game);
                var backgroundSystem = new MainMenuBackgroundRenderingSystem(game);

                EditorQuestContext = new GameContext(systems, new List<ISystem>() { backgroundSystem, uiSystem }, UIContainer.Instance.EditorQuestScreen, "Dialog");
                return EditorQuestContext;
            }
        }



        private static GameContext DialogContext;
        public static GameContext GetDialogContext(NamelessGame game)
        {

            if (DialogContext != null)
            {
                return DialogContext;
            }
            else
            {
                var systems = new List<ISystem>();
                systems.Add(new InputSystem(new IngameKeyIntentTraslator(), game));
                systems.Add(new DialogIntentSystem());
                systems.Add(new DialogSystem());
                systems.Add(new DialogScreenSystem());
                systems.Add(new SoundPlaySystem());
                var uiSystem = new UIRenderSystem(game);
                var renderingSystem = new RenderingSystem(game.GetSettings());

                DialogContext = new GameContext(systems, new List<ISystem>() { renderingSystem, uiSystem }, new List<IBaseGuiScreen>() { UIContainer.Instance.DialogScreen, UIContainer.Instance.HudScreen, } , "Dialog");
                return DialogContext;
            }
        }

        private static GameContext PickupItemsDialogContext;
        public static GameContext GetPickupItemsDialogContext(NamelessGame game)
        {

            if (PickupItemsDialogContext != null)
            {
                return PickupItemsDialogContext;
            }
            else
            {
                var systems = new List<ISystem>();
                systems.Add(new InputSystem(new IngameKeyIntentTraslator(), game));
                systems.Add(new PickOptionDialogIntentSystem());
                systems.Add(new PickOptionItemPickupDialogSystem());
                systems.Add(new PickOptionDialogScreenSystem());
                systems.Add(new InventorySystem());
                systems.Add(new SoundPlaySystem());
                var uiSystem = new UIRenderSystem(game);
                var renderingSystem = new RenderingSystem(game.GetSettings());

                PickupItemsDialogContext = new GameContext(systems, new List<ISystem>() { renderingSystem, uiSystem }, new List<IBaseGuiScreen>() { UIContainer.Instance.PickOptionDialogScreen, UIContainer.Instance.HudScreen, }, "Dialog");
                return PickupItemsDialogContext;
            }
        }


        private static GameContext TradeScreenContext;
        public static GameContext GetTradeScreenContext(NamelessGame game)
        {

            if (TradeScreenContext != null)
            {
                return TradeScreenContext;
            }
            else
            {
                var systems = new List<ISystem>();
                systems.Add(new InputSystem(new TradeScreenIntentTranslator(), game));
                systems.Add(new TradeScreenIntentSystem());
                systems.Add(new TradeScreenSystem());
                systems.Add(new InventorySystem());
                systems.Add(new SoundPlaySystem());
                var uiSystem = new UIRenderSystem(game);
                TradeScreenContext = new GameContext(systems, new List<ISystem>() { uiSystem }, new List<IBaseGuiScreen>() { UIContainer.Instance.TradeScreen }, "Trade");
                return TradeScreenContext;
            }
        }

        private static GameContext editorLocationContext;
        public static GameContext GetEditorLocationContext(NamelessGame game)
        {

            if (editorLocationContext != null)
            {
                return editorLocationContext;
            }
            else
            {
                var systems = new List<ISystem>();
                systems.Add(new EditorLocationScreenSystem());
                systems.Add(new SoundPlaySystem());
                var uiSystem = new UIRenderSystem(game);
                var backgroundSystem = new MainMenuBackgroundRenderingSystem(game);
                // create and init the UI manager
                editorLocationContext = new GameContext(systems, new List<ISystem>() { backgroundSystem, uiSystem }, UIContainer.Instance.EditorLocationScreen, "MainMenu");
                return editorLocationContext;
            }
        }

        private static GameContext newGamePickWorldContext;
        public static GameContext GetNewGamePickWorldContext(NamelessGame game)
        {
            if (newGamePickWorldContext != null)
            {
                return newGamePickWorldContext;
            }
            else
            {
                var systems = new List<ISystem>();
                systems.Add(new NewGamePickWorldScreenSystem());
                systems.Add(new SoundPlaySystem());
                var uiSystem = new UIRenderSystem(game);
                var backgroundSystem = new MainMenuBackgroundRenderingSystem(game);
                // create and init the UI manager
                newGamePickWorldContext = new GameContext(systems, new List<ISystem>() { backgroundSystem, uiSystem }, UIContainer.Instance.NewGamePickWorldScreen, "MainMenu");
                return newGamePickWorldContext;
            }
        }

        private static GameContext characterCreationScreenContext;
        public static GameContext GetCharacterCreationScreenContext(NamelessGame game)
        {
            if (characterCreationScreenContext != null)
            {
                return characterCreationScreenContext;
            }
            else
            {
                var systems = new List<ISystem>();
                systems.Add(new CharacterCreationScreenSystem());
                systems.Add(new SoundPlaySystem());
                var uiSystem = new UIRenderSystem(game);
                var backgroundSystem = new MainMenuBackgroundRenderingSystem(game);
                characterCreationScreenContext = new GameContext(systems, new List<ISystem>() { backgroundSystem, uiSystem }, UIContainer.Instance.CharacterCreationScreen, "MainMenu");
                return characterCreationScreenContext;
            }
        }

        private static GameContext newGameGenerationProgressContext;
        public static GameContext GetNewGameGenerationProgressContext(NamelessGame game)
        {
            if (newGameGenerationProgressContext != null)
            {
                return newGameGenerationProgressContext;
            }
            else
            {
                var systems = new List<ISystem>();
                systems.Add(new NewGameGenerationScreenSystem());
                systems.Add(new SoundPlaySystem());
                var uiSystem = new UIRenderSystem(game);
                var backgroundSystem = new MainMenuBackgroundRenderingSystem(game);
                newGameGenerationProgressContext = new GameContext(systems, new List<ISystem>() { backgroundSystem, uiSystem }, UIContainer.Instance.NewGameGenerationProgressScreen, "MainMenu");
                return newGameGenerationProgressContext;
            }
        }

    }
}

using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Text;

namespace NamelessRogue.Engine.UI
{
	public class UIContainer
	{
		public static UIContainer Instance { get; private set; } = null;

		public MainMenuScreen MainMenu { get; set; }
		public IngameScreen HudScreen { get; set; }
		public MapScreen MapScreen { get; set; }
		public InventoryScreen InventoryScreen { get; set; }

        public DialogScreen DialogScreen { get; set; }

        public AbilityScreen AbilityScreen { get; set; }

        public EditorsPickerScreen EditorsPickerScreen { get; set; }
        public EditorItemScreen EditorItemScreen { get; set; }
        public EdirtorLocationScreen EditorLocationScreen { get; set; }
        public EditorCharacterScreen EditorCharacterScreen { get; set; }
		public EditorDialogScreen EditorDialogScreen { get; set; }

        public EditorBuffScreen EditorBuffScreen { get; set; }   
		public EditorAbilityScreen EditorAbilityScreen { get;  set; }

        public EditorQuestScreen EditorQuestScreen { get; set; }
        public WorldGenerationUI WorldGenScreen { get; set; }

        public WorldGenerationProgressUI WorldGenerationProgressUI { get; set; }
        public PickOptionDialogScreen PickOptionDialogScreen { get; internal set; }
        public NewGamePickWorldScreen NewGamePickWorldScreen { get; set; }

        public CharacterCreationScreen CharacterCreationScreen { get; set; }

        public NewGameGenerationProgressScreen NewGameGenerationProgressScreen { get; set; }

        public TradeScreen TradeScreen { get; set; }

        public UIContainer(NamelessGame game)
		{
			if (Instance != null)
			{
				throw new Exception("Attempted to create multiple instances of a singleton class UIContainer");
			}

			MainMenu = new MainMenuScreen(game);

		    HudScreen = new IngameScreen(game);
			MapScreen = new MapScreen(game);
			InventoryScreen = new InventoryScreen(game);
			WorldGenScreen = new WorldGenerationUI(game);
            EditorsPickerScreen = new EditorsPickerScreen(game);
            EditorItemScreen = new EditorItemScreen(game);
            EditorCharacterScreen = new EditorCharacterScreen(game);
            EditorBuffScreen = new EditorBuffScreen(game);
            EditorAbilityScreen = new EditorAbilityScreen(game);
            AbilityScreen = new AbilityScreen(game);
			EditorDialogScreen = new EditorDialogScreen(game);
            DialogScreen = new DialogScreen(game);
            PickOptionDialogScreen = new PickOptionDialogScreen(game);
            TradeScreen = new TradeScreen(game);
            EditorQuestScreen = new EditorQuestScreen(game);
            EditorLocationScreen = new EdirtorLocationScreen(game);
            WorldGenerationProgressUI = new WorldGenerationProgressUI(game);
            NewGamePickWorldScreen = new NewGamePickWorldScreen(game);
            CharacterCreationScreen = new CharacterCreationScreen(game);
            NewGameGenerationProgressScreen = new NewGameGenerationProgressScreen(game);
            Instance = this;

		}
	}
}

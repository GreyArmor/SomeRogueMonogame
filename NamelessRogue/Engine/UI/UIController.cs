using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Text;

namespace NamelessRogue.Engine.UI
{
	public class UIController
	{


		public static UIController Instance { get; private set; } = null;

		public MainMenuScreen MainMenu { get; set; }
		public IngameScreen HudScreen { get; set; }
		public MapScreen MapScreen { get; set; }
		public InventoryScreen InventoryScreen { get; set; }

		public WorldGenerationUI WorldGenScreen { get; set; }

		public UIController(NamelessGame game)
		{
			if (Instance != null)
			{
				throw new Exception("Attempted to create multiple instances of a singleton class UIControlle");
			}

			MainMenu = new MainMenuScreen(game);

		    HudScreen = new IngameScreen(game);
			MapScreen = new MapScreen(game);
			InventoryScreen = new InventoryScreen(game);
			WorldGenScreen = new WorldGenerationUI(game);
			Instance = this;

		}
	}
}

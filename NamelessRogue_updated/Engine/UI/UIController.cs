using NamelessRogue.Engine.ViewModels;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Text;

namespace NamelessRogue.Engine.UI
{
	public class UIController
	{


		public static UIController Instance { get; private set; } = null;

		public BaseScreen MainMenu { get; set; }
		public BaseScreen HudScreen { get; set; }
		public BaseScreen MapScreen { get; set; }
		public BaseScreen InventoryScreen { get; set; }

		public UIController(NamelessGame game)
		{
			if (Instance != null)
			{
				throw new Exception("Attempted to create multiple instances of a singleton class UIControlle");
			}

			MainMenu = new MainMenuScreen(game);

			//HudScreen = new HudScreen();
			//MapScreen = new MapScreen();
			//InventoryScreen = new InventoryScreen();

			Instance = this;

		}
	}
}

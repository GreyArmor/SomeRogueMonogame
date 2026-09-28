using Gum.Forms.Controls;
using Gum.Mvvm;
using Gum.Wireframe;
using NamelessRogue.Components.Neon.Controls;
using NamelessRogue.Engine.UI.GumScreens.Commands.MainMenu;
using NamelessRogue.Screens;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Button = NamelessRogue.Components.Neon.Controls.Button;

namespace NamelessRogue.Content.GumScreens.GumViewModels
{
	public class MainMenuViewModel : ViewModel
	{
		internal MainMenuViewModel(NamelessGame game, MainScreen mainManuScreen) {
			mainManuScreen.NewGameButton.Click += (_,_) => { game.Commander.EnqueueCommand(new MainMenuNewGameCommand()); };
			mainManuScreen.LoadGameButton.Click += (_, _) => { game.Commander.EnqueueCommand(new MainMenuLoadGameCommand()); };
			mainManuScreen.WorldGenButton.Click += (_, _) => { game.Commander.EnqueueCommand(new MainMenuWorldGenCommand()); };
			mainManuScreen.OptionsButton.Click += (_, _) => { game.Commander.EnqueueCommand(new MainMenuOptionCommand()); };
			mainManuScreen.EditorsButton.Click += (_, _) => { game.Commander.EnqueueCommand(new MainMenuEditorsCommand()); };
			mainManuScreen.ExitButton.Click += (_, _) => { game.Commander.EnqueueCommand(new MainMenuExitCommand()); };

		}
	}
}

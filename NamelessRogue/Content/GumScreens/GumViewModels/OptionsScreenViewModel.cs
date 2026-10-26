using Gum.Mvvm;
using NamelessRogue.Engine.UI.GumScreens.Commands.MainMenu;
using NamelessRogue.Screens;
using NamelessRogue.shell;

namespace NamelessRogue.Content.GumScreens.GumViewModels
{
	public class OptionsScreenViewModel : ViewModel
	{
		internal OptionsScreenViewModel(NamelessGame game, OptionsScreen optionsScreen)
		{
			optionsScreen.BackButton.Click += (_, _) => { game.Commander.EnqueueCommand(new OptionsScreenBackCommand()); };
		}
	}
}

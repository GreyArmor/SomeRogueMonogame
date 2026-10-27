using Microsoft.Xna.Framework;
using NamelessRogue.Content.GumScreens.GumViewModels;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.UI.GumScreens.Commands.MainMenu;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;

namespace NamelessRogue.Engine.Systems.Ingame
{
	public class CharacterStatsScreenSystem : BaseSystem
	{
		private CharacterStatsScreenViewModel viewModel;

		public CharacterStatsScreenSystem(CharacterStatsScreenViewModel viewModel)
		{
			this.viewModel = viewModel;
		}

		public override HashSet<Type> Signature { get; } = new HashSet<Type>();

		public override void Update(GameTime gameTime, NamelessGame namelessGame)
		{
			viewModel.UpdateValues();
			while (namelessGame.Commander.DequeueCommand(out CharacterStatsScreenBackCommand command))
			{
				namelessGame.ContextToSwitch = ContextFactory.GetIngameContext(namelessGame);
			}
		}
	}
}

using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.UI.GumScreens.Commands.MainMenu;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;

namespace NamelessRogue.Engine.Systems.Ingame
{
	public class OptionsScreenSystem : BaseSystem
	{
		public override HashSet<Type> Signature { get; } = new HashSet<Type>();

		public override void Update(GameTime gameTime, NamelessGame namelessGame)
		{
			while (namelessGame.Commander.DequeueCommand(out OptionsScreenBackCommand command))
			{
				namelessGame.ContextToSwitch = ContextFactory.GetMainMenuContext(namelessGame);
			}
		}
	}
}

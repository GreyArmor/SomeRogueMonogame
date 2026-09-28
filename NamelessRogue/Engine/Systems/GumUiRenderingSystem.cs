using Gum;
using Gum.DataTypes;
using Gum.Forms.Controls;
using Gum.Managers;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using NamelessRogue.Content.GumScreens.GumViewModels;
using NamelessRogue.Engine.Context;
using NamelessRogue.Engine.UI;
using NamelessRogue.Screens;
using NamelessRogue.shell;
using RenderingLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Systems
{
	internal class GumUiRenderingSystem : BaseSystem
	{
		NamelessGame game;
		GumService GumUI => GumService.Default;

		public GumUiRenderingSystem(NamelessGame game, GameContext context, string gumUiScreenId)
		{
			Context = context;
			this.game = game;
			var gumProject = GumUI.Initialize(game, $@"GumScreens/GameScreens.gumx");

			var screen = new MainScreen();
			screen.AddToRoot();

			MainMenuViewModel mainMenuViewModel = new MainMenuViewModel(game, screen);
			screen.BindingContext = mainMenuViewModel;
		}

		public override HashSet<Type> Signature { get; } = new HashSet<Type>();
		public GameContext Context { get; }

		public override void Update(GameTime gameTime, NamelessGame namelessGame)
		{
			GumUI.Update(gameTime);
			GumUI.Draw();

		}
	}
}

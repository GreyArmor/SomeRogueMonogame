using Gum;
using Gum.DataTypes;
using Gum.Forms;
using Gum.Forms.Controls;
using Gum.Managers;
using Gum.Mvvm;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;
using NamelessRogue.Components.Neon.Controls;
using NamelessRogue.Content.GumScreens.GumViewModels;
using NamelessRogue.Engine.Components.Interaction;
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
using Button = Gum.Forms.Controls.Button;

namespace NamelessRogue.Engine.Systems
{
	internal class GumUiRenderingSystem : BaseSystem
	{
		NamelessGame game;
		GumService GumUI => GumService.Default;

		public GumUiRenderingSystem(NamelessGame game, GameContext context, FrameworkElement screen, ViewModel viewModel)
		{
			Context = context;
			this.game = game;			
			screen.BindingContext = viewModel;

			foreach (var child in screen.Descendants())
			{
				if (child is Button button)
				{
					button.Click += (_,_)=> { game.Commander.EnqueueCommand(new PlaySoundCommand("ButtonClick", false, 0.5f)); };
				}
			}

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

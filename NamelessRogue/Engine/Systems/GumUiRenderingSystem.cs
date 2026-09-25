using Gum;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.UI;
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
		private GraphicalUiElement _rootElement = null;
		GumService GumUI => GumService.Default;
		public GumUiRenderingSystem(NamelessGame game)
		{
			this.game = game;
			var gumProject = GumUI.Initialize(game,	$@"GumScreens/GameScreens.gumx");
			var screen = gumProject.Screens.Find(item => item.Name == "Neon/DemoScreenGum");
			var screenRuntime = screen.ToGraphicalUiElement();
			screenRuntime.AddToRoot();

		}
		public override HashSet<Type> Signature { get; } = new HashSet<Type>();

		public override void Update(GameTime gameTime, NamelessGame namelessGame)
		{
			GumUI.Update(gameTime);
			GumUI.Draw();

		}
	}
}

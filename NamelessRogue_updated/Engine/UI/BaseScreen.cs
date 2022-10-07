using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Text;

namespace NamelessRogue.Engine.UI
{

	public abstract class BaseScreen : IBaseGuiScreen
	{
		protected NamelessGame game;
		protected System.Numerics.Vector2 uiSize;
		public BaseScreen(NamelessGame game)
		{
			this.game = game;
			uiSize = new System.Numerics.Vector2(game.GetActualWidth(), game.GetActualHeight());
		}
		public abstract void DrawLayout();
	}
}

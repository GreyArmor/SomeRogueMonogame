using ImGuiNET;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Sounds;
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

		public bool ButtonWithSound(String text, System.Numerics.Vector2 size)
		{
			var clicked = ImGui.Button(text, size);
			if (clicked) { game.Commander.EnqueueCommand(new PlaySoundCommand(Sound.ButtonClick, 0.5f)); }
			return clicked;
		}

	}
}

using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Text;

namespace NamelessRogue.Engine.ViewModels
{
	public class HudViewModel
	{
		private NamelessGame game;

		public HudViewModel(NamelessGame game)
		{
			this.game = game;
		}
	}
}

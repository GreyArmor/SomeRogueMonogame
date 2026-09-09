using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Environment
{
	internal class StreetlightsStatusKeeper : Component
	{
		public bool VerticalMovementAllowed { get; set; } = false;

		public void Switch()
		{
			VerticalMovementAllowed = !VerticalMovementAllowed;
		}
		public StreetlightsStatusKeeper() { }
		public override IComponent Clone()
		{
			return new StreetlightsStatusKeeper();
		}

	}
}

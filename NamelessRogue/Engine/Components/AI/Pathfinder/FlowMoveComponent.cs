using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.AI.Pathfinder
{
	internal class FlowMoveComponent : Component
	{
		public FlowMoveComponent() { }

		public FlowMoveComponent(Point To)
		{
			this.To = To;
		}

		public Point To { get; set; }
		public bool FinishedMoving { get; set; }
	}
}

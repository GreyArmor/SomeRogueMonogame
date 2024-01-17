using Veldrid;
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
		//this id is received after calculation
		public int PathId { get; set; }
		public Point To { get; set; }
		public bool FinishedMoving { get; set; } = true;
	}
}

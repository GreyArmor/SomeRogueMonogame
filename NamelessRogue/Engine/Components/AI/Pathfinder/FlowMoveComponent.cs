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
		//this id is received after calculation
		public int CurrentPathIndex { get; set; }
		public List<int> PathChain {get; set;} = new List<int>();
		public int PathId { get { return PathChain.Count > 0 ? PathChain[CurrentPathIndex] : -1; } }
        public Point To { get; set; }
		public bool FinishedMoving { get; set; } = true;

		public MacroNode CurrentMacroNode { get; set; } = null;
    }
}

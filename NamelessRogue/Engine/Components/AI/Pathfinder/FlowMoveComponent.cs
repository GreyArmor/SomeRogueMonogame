using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.AI.Pathfinder
{


	public enum PedestrianAiState
	{
		Start, Wait, Move, WaitForCrossing, CrossingMove, Finished
	}
	internal class FlowMoveComponent : Component
	{
		private int currentPathIndex;

		public FlowMoveComponent() { }

		public FlowMoveComponent(Point To)
		{
			this.To = To;
		}
		//this id is received after calculation
		public int CurrentPathIndex { get; set; }
		public List<int> PathChain {get; set;} = new List<int>();
		public List<MacroNode> NodeChain { get; set; } = new List<MacroNode>();
		public int PathId { get { return PathChain.Count > 0 && CurrentPathIndex < PathChain.Count ? PathChain[CurrentPathIndex] : -1; } }
        public Point To { get; set; }
        public MacroNode CurrentMacroNode { get; set; } = null;
		public PedestrianAiState CurrentState { get; set; } = PedestrianAiState.Start;
		public string Log { get; internal set; }
	}
}

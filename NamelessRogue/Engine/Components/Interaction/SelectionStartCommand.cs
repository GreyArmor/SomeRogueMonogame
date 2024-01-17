using Veldrid;
using NamelessRogue.Engine.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Interaction
{
	internal class SelectionCommand : ICommand
	{
		public SelectionCommand(SelectionState state, Point cursorPosition)
		{
			State = state;
			CursorPosition = cursorPosition;
		}

		public SelectionState State { get; }
		public Point CursorPosition { get; }
	}
}

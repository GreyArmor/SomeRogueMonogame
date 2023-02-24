using NamelessRogue.Engine.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;

namespace NamelessRogue.Engine.Systems._3DView
{

	public enum MoveType
	{
		None,
		Left,
		Right,
		Forward,
		Backward,
	}
	internal class MoveCamera3dCommand : ICommand
	{
		public List<MoveType> MovesToMake { get; } = new List<MoveType>();
		public MoveCamera3dCommand(params MoveType[] moves)
		{
			MovesToMake.AddRange(moves);
		}
	}
}

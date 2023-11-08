using Microsoft.Xna.Framework;
using MonoGame.Extended;
using NamelessRogue.Engine.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Interaction
{
	internal class GroupMoveCommand : ICommand
	{
		public GroupMoveCommand(string groupTag, Point previousPoint, Point nextPoint)
		{
			GroupTag = groupTag;
			PreviousPoint = previousPoint;
			NextPoint = nextPoint;
		}

		public string GroupTag { get; }
		public Point PreviousPoint { get; }
		public Point NextPoint { get; }
	}
}

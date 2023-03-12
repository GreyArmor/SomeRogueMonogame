using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.AI.Pathfinder
{
	internal interface IFlowfield
	{
		void ClaculateTo(Point to);
		Point GetNextPoint(Point from);
	}
}

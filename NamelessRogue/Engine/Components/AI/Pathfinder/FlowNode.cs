using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Utility;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace NamelessRogue.Engine.Components.AI.Pathfinder
{
	public class FlowNode
	{
		public FlowNode Next;
		public Point Coordinate = new Point(-1, -1);
		public bool Occupied = false;
		public int IntegrationValue = 10000;
		public int Cost = 1;
		public List<string> neighbors;
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Physical
{
	internal class Position3D : Component
	{
		public Vector3 Position { get; set; }
		public Vector3 Normal { get; set; }
		public Position3D(Vector3 position, Vector3 normal)
		{
			Position = position;
			Normal = normal;
		}

		public Position3D() { }
	}
}

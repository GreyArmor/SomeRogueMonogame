using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Physical
{
	internal class Position3D : Component
	{
		public Vector3 Position { get; set; }
		//the unit looks in this direction;
		public Vector2 Normal { get; set; }
		public Position3D(Vector3 position, Vector2 normal)
		{
			Position = position;
			Normal = normal;
		}

		public Position3D() { }
	}
}

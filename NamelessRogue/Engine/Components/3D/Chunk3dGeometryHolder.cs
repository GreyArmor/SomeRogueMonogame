using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NamelessRogue.Engine.Components._3D
{
	public class Chunk3dGeometryHolder : Component
	{
		public Dictionary<Point, Geometry3D> ChunkGeometries { get; set; } = new Dictionary<Point, Geometry3D>();
	}
}

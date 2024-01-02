using SharpDX;
using System;
using System.Collections.Generic;
using System.Text;

namespace NamelessRogue.Engine.Components._3D
{
	public class Chunk3dGeometryHolder : Component
	{
		public Dictionary<Point, Tuple<Geometry3D, TerrainGeometry3D>> ChunkGeometries { get; set; } = new Dictionary<Point, Tuple<Geometry3D, TerrainGeometry3D>>();
	}
}

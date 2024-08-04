using NamelessRogue.Engine.Systems.Ingame;
using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using NamelessRogue.Engine.Utility;

namespace NamelessRogue.Engine.Components
{
	public class Geometry3D : Component, IDisposable
	{
		//should not be here, but for test its alright, this collection contains tile indexes for underlying chunk, for each triangle index, useful for collision detection
		public List<Point> TriangleTerrainAssociation { get; set; }
		public List<Vector3> Vertices { get; set; }
		public List<int> Indices { get; set; }
		public int TriangleCount { get; set; }
		public object Buffer { get; set; }
		public object  IndexBuffer { get; set; }
		public BoundingBox3D Bounds;
		public object  Material { get; set; }

		public object WorldTextureSet { get; set; }

		public void Dispose()
		{
			//Buffer?.Dispose();
			//IndexBuffer?.Dispose();
			//Material?.Dispose();
			//Indices?.Clear();
			//Vertices?.Clear();
			//TriangleTerrainAssociation?.Clear();
		}
	}
}

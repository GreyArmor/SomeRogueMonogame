using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NamelessRogue.Engine.Systems.Ingame;
using System;
using System.Collections.Generic;
using System.Text;

namespace NamelessRogue.Engine.Components
{
	public class Geometry3D : Component
	{
		public int TriangleCount { get; set; }
		public VertexBuffer Buffer { get; set; }
		public IndexBuffer IndexBuffer { get; set; }

		public IndexBuffer WirefraveIndexBuffer { get; set; }
		public BoundingBox Bounds { get; set; }

	}
}

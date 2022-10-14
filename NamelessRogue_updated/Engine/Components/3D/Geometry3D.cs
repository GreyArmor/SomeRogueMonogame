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
		public List<Vertex> Positions { get; set; } = new List<Vertex>();
		public List<int> Indices { get; set; } = new List<int>();
		public List<Vector2> TextureCoords { get; set; } = new List<Vector2>();

		public VertexBuffer Buffer { get; set; }
		public IndexBuffer IndexBuffer { get; set; }

	}
}

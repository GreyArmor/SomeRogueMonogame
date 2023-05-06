﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NamelessRogue.Engine.Systems.Ingame;
using System;
using System.Collections.Generic;
using System.Text;

namespace NamelessRogue.Engine.Components
{
	public class Geometry3D : Component, IDisposable
	{
		//should not be here, but for test its alright, this collection contains tile indexes for underlying chunk, for each triangle index, useful for collision detection
		public List<Point> TriangleTerrainAssociation { get; set; }
		public List<Vector3> Vertices { get; set; }
		public List<int> Indices { get; set; }
		public int TriangleCount { get; set; }
		public VertexBuffer Buffer { get; set; }
		public IndexBuffer IndexBuffer { get; set; }
		public BoundingBox Bounds { get; set; }
		public Texture2D Material { get; set; }

		public void Dispose()
		{
			Buffer?.Dispose();
			IndexBuffer?.Dispose();
			Material?.Dispose();
			Indices?.Clear();
			Vertices?.Clear();
			TriangleTerrainAssociation?.Clear();
		}
	}
}

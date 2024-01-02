using SharpDX;
using NamelessRogue.Engine.Systems.Ingame;
using System;
using System.Collections.Generic;
using System.Text;
using FbxSharp;
using SharpDX.Direct3D11;
using Vector3 = SharpDX.Vector3;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace NamelessRogue.Engine.Components
{
	public class Geometry3D : Component, IDisposable
	{
		//should not be here, but for test its alright, this collection contains tile indexes for underlying chunk, for each triangle index, useful for collision detection
		public List<Point> TriangleTerrainAssociation { get; set; }
		public List<Vector3> Vertices { get; set; }
		public List<int> Indices { get; set; }
		public int TriangleCount { get; set; }
		public Buffer Buffer { get; set; }
		public Buffer IndexBuffer { get; set; }
		public BoundingBox Bounds;
		public Texture2D Material { get; set; }
		public ShaderResourceView MaterialResourceView { get; set; }
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

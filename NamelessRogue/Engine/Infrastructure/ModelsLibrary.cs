using Assimp.Configs;
using Assimp;
using Microsoft.Xna.Framework.Graphics;
using NamelessRogue.Engine.Components;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NamelessRogue.Engine.Systems.Ingame;
using Microsoft.Xna.Framework;
using System.Drawing;

namespace NamelessRogue.Engine.Infrastructure
{
	public static class ModelsLibrary
	{
		public static Dictionary<string, Geometry3D> Models = new Dictionary<string, Geometry3D>();
		static ModelsLibrary() { }

		public static void Initialize(NamelessGame game)
		{
			//void _addModel(string id, string path, Matrix modelTrandform) {

			//	AssimpContext importer = new AssimpContext();

			//	NormalSmoothingAngleConfig config = new NormalSmoothingAngleConfig(175f);
			//	importer.SetConfig(config);

			//	LogStream logstream = new LogStream(delegate (String msg, String userData)
			//	{
			//		Console.WriteLine(msg);
			//	});
			//	logstream.Attach();
			//	Scene model = importer.ImportFile(path, PostProcessPreset.TargetRealTimeMaximumQuality);
			

			//	Geometry3D geometry = new Geometry3D();

			//	var vertices = new Queue<Vertex3D>();
			//	var positions = new Queue<Vector3>();
			//	var mesh = model.Meshes[0];
				
			//	for (int i = 0; i < mesh.Vertices.Count; i++)
			//	{
			//		var vec = mesh.Vertices[i];
					
				
			//		var normal = new Vector3(mesh.Normals[i].X, mesh.Normals[i].Y, mesh.Normals[i].Z);
			//		var vecConverted = Vector3.Transform(new Microsoft.Xna.Framework.Vector3(vec.X, vec.Y, vec.Z), modelTrandform);
			//		Vector4 colorConverted = new Vector4(1,1,1,1);
			//		if (mesh.VertexColorChannels[0].Any())
			//		{
			//			var color = mesh.VertexColorChannels[0][i];
			//			colorConverted = new Vector4(color.R, color.G, color.B, color.A);
			//		}
			//		vertices.Enqueue(new Vertex3D(vecConverted, colorConverted, colorConverted, Vector2.Zero, normal));
			//		positions.Enqueue(vecConverted);
			//	}

			//	geometry.Vertices = positions.ToList();
			//	geometry.Indices = mesh.GetIndices().ToList();
			//	geometry.Bounds = Microsoft.Xna.Framework.BoundingBox.CreateFromPoints(positions);

			//	//result.TriangleTerrainAssociation = tileTriangleAssociations;

			//	geometry.Buffer = new Microsoft.Xna.Framework.Graphics.VertexBuffer(game.GraphicsDevice, RenderingSystem3D.VertexDeclaration, vertices.Count, Microsoft.Xna.Framework.Graphics.BufferUsage.None);
			//	geometry.Buffer.SetData<Vertex3D>(vertices.ToArray());
			//	geometry.IndexBuffer = new Microsoft.Xna.Framework.Graphics.IndexBuffer(game.GraphicsDevice, Microsoft.Xna.Framework.Graphics.IndexElementSize.ThirtyTwoBits, geometry.Indices.Count, Microsoft.Xna.Framework.Graphics.BufferUsage.None);
			//	geometry.IndexBuffer.SetData<int>(geometry.Indices.ToArray());
			//	geometry.TriangleCount = geometry.Indices.Count/3;

			//	importer.Dispose();; 

			//	Models.Add(id, geometry);
			//}
			//Matrix rotationX = Matrix.CreateRotationX(MathHelper.ToRadians(90f));
			//var modelShiftTree1 = Matrix.CreateTranslation(0.5f, 0.5f, 0);
			//var modelShift = Matrix.CreateTranslation(0.5f, 0.5f, 1.5f);
			//_addModel("smallTree", "Content\\Models\\smallTree.fbx", modelShift);
			//_addModel("tree", "Content\\Models\\tree.fbx", modelShift);
			//_addModel("tree1", "Content\\Models\\tree1.fbx", rotationX * modelShiftTree1);
			//_addModel("cube", "Content\\Models\\cube.fbx", Matrix.CreateScale(0.01f));

		}
	}
}

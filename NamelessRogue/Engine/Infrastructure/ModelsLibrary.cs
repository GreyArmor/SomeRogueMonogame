using Assimp.Configs;
using Assimp;

using NamelessRogue.Engine.Components;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NamelessRogue.Engine.Systems.Ingame;
using SharpDX;
using System.Drawing;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace NamelessRogue.Engine.Infrastructure
{
	public static class ModelsLibrary
	{
		public static Dictionary<string, Geometry3D> Models = new Dictionary<string, Geometry3D>();
		static ModelsLibrary() { }

		public static void Initialize(NamelessGame game)
		{
			void _addModel(string id, string path, Matrix modelTrandform) {

				AssimpContext importer = new AssimpContext();

				NormalSmoothingAngleConfig config = new NormalSmoothingAngleConfig(175f);
				importer.SetConfig(config);

				LogStream logstream = new LogStream(delegate (String msg, String userData)
				{
					Console.WriteLine(msg);
				});
				logstream.Attach();
				Scene model = importer.ImportFile(path, PostProcessPreset.TargetRealTimeMaximumQuality);
			

				Geometry3D geometry = new Geometry3D();

				var vertices = new Queue<Vertex3D>();
				var positions = new Queue<Vector3>();
				var mesh = model.Meshes[0];
				
				for (int i = 0; i < mesh.Vertices.Count; i++)
				{
					var vec = mesh.Vertices[i];
					
				
					var normal = new Vector3(mesh.Normals[i].X, mesh.Normals[i].Y, mesh.Normals[i].Z);
					var vecConverted = (Vector3)Vector3.Transform(new Vector3(vec.X, vec.Y, vec.Z), modelTrandform);
					Vector4 colorConverted = new Vector4(1,1,1,1);
					if (mesh.VertexColorChannels[0].Any())
					{
						var color = mesh.VertexColorChannels[0][i];
						colorConverted = new Vector4(color.R, color.G, color.B, color.A);
					}
					vertices.Enqueue(new Vertex3D(vecConverted, colorConverted, colorConverted, Vector2.Zero, normal));
					positions.Enqueue(vecConverted);
				}

				geometry.Vertices = positions.ToList();
				geometry.Indices = mesh.GetIndices().ToList();
				geometry.Bounds = BoundingBox.FromPoints(positions.ToArray());

				//result.TriangleTerrainAssociation = tileTriangleAssociations;

				geometry.Buffer = Buffer.Create(game.GraphicsDevice, SharpDX.Direct3D11.BindFlags.VertexBuffer, vertices.ToArray());
				geometry.IndexBuffer = Buffer.Create(game.GraphicsDevice, SharpDX.Direct3D11.BindFlags.IndexBuffer, geometry.Indices.ToArray());
				geometry.TriangleCount = geometry.Indices.Count/3;

				importer.Dispose();; 

				Models.Add(id, geometry);
			}
			Matrix rotationX = Matrix.RotationX(MathUtil.DegreesToRadians(90f));
			var modelShiftTree1 = Matrix.Translation(0.5f, 0.5f, 0);
			var modelShift = Matrix.Translation(0.5f, 0.5f, 1.5f);
			_addModel("smallTree", "Content\\Models\\smallTree.fbx", modelShift);
			_addModel("tree", "Content\\Models\\tree.fbx", modelShift);
			_addModel("tree1", "Content\\Models\\tree1.fbx", rotationX * modelShiftTree1);
			_addModel("cube", "Content\\Models\\cube.fbx", Matrix.Scaling(0.01f));

		}
	}
}

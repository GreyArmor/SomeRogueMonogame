using DynamicData;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Components;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Systems.Ingame;
using NamelessRogue.Engine.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace NamelessRogue.Engine._3DUtility
{
	public static class ChunkGeometryGeneratorWeb
	{

		static List<Vector3> _points = new List<Vector3>();
		static List<int> _indices = new List<int>();

		static ChunkGeometryGeneratorWeb()
		{
			_points.Add(new Vector3(0, 0, 0));
			_points.Add(new Vector3(1, 0, 0));

			_indices.AddRange(new int[6] { 0, 1, 2, 2, 3, 0 });
		}

		/// <summary>
		/// Generate 3D model for a chunk
		/// </summary>
		/// <param name="chunkToGenerate"></param>
		/// <param name="chunks"></param>
		/// <returns></returns>
		/// 
		static bool firstTime = true;
		static Point originalPointForTest;
		static Matrix moveToZero = Matrix.CreateTranslation(-(new Vector3(1, 1, 0) / 2));
		static Matrix invertedMoveToZero = Matrix.Invert(moveToZero);
		static Matrix scaleDown = Matrix.CreateScale(0.01f);
		//precalculated out of loop

		public static Geometry3D GenerateChunkModel(Game namelessGame, Point chunkToGenerate, ChunkData chunks, TileAtlasConfig atlasConfig)
		{
			var result = new Geometry3D();

			var chunk = chunks.Chunks[chunkToGenerate];
			chunk.Activate();

			//activate neighbors
			var sp = new Point(chunkToGenerate.X, chunkToGenerate.Y + 1);
			var southChunk = chunks.Chunks[sp];
			var wp = new Point(chunkToGenerate.X + 1, chunkToGenerate.Y);
			var westChunk = chunks.Chunks[wp];

			southChunk.Activate();
			westChunk.Activate();

			//chunks.RealityBubbleChunks.Add(sp, southChunk);
		//	chunks.RealityBubbleChunks.Add(wp, westChunk);


			var currentCorner = chunk.WorldPositionBottomLeftCorner;
			if (firstTime)
			{
				firstTime = false;
				originalPointForTest = currentCorner;
			}

			var tiles = chunk.GetChunkTiles();
			List<Vertex3D> vertices = new List<Vertex3D>();
			List<int> indices = new List<int>();
			var resolution = Constants.ChunkSize;
			Matrix scaleDown = Matrix.Identity;
			var transformedPoints = new List<Vector3>();
			var colors = new List<Vector4>();
			const float worldHeight = 2500;
			for (int x = 0; x < resolution+1; x++)
			{
				for (int y = 0; y < resolution+1; y++)
				{
					Tile tile = chunks.GetTile(chunk.WorldPositionBottomLeftCorner.X + x, chunk.WorldPositionBottomLeftCorner.Y + y);
					float elevation = (float)tile.Elevation;
					transformedPoints.Add(Vector3.Transform(
						new Vector3(
						x + currentCorner.X - originalPointForTest.X, 
						y + currentCorner.Y - originalPointForTest.Y,
						((elevation) * worldHeight)- worldHeight), scaleDown));
					var tileColor = TerrainLibrary.Terrains[tile.Terrain].Representation.CharColor;
					colors.Add(tileColor.ToVector4());
				}
			}
			int triangleCount = 0;
			for (int i = 0; i <= resolution; i++)
			{
				for (int j = 0; j < resolution; j++)
				{
					var index = (i * resolution) + j;
					indices.Add(index);
					indices.Add(index + 1);
					indices.Add(index + resolution + 1 );

					indices.Add(index + resolution + 1);
					indices.Add(index + resolution);
					indices.Add(index);
					triangleCount += 2;
				}
			}

			for (int i = 0; i < transformedPoints.Count; i++)
			{
				Vector3 point = transformedPoints[i];
				var vertex = new Systems.Ingame.Vertex3D(point,
				colors[i],
				colors[i], new Vector2(0, 0), Vector3.UnitZ);
				vertices.Add(vertex);
			}


			result.Buffer = new Microsoft.Xna.Framework.Graphics.VertexBuffer(namelessGame.GraphicsDevice, RenderingSystem3D.VertexDeclaration, vertices.Count, Microsoft.Xna.Framework.Graphics.BufferUsage.None);
			result.Buffer.SetData<Vertex3D>(vertices.ToArray());
			result.IndexBuffer = new Microsoft.Xna.Framework.Graphics.IndexBuffer(namelessGame.GraphicsDevice, Microsoft.Xna.Framework.Graphics.IndexElementSize.ThirtyTwoBits, indices.Count, Microsoft.Xna.Framework.Graphics.BufferUsage.None);
			result.IndexBuffer.SetData<int>(indices.ToArray());
			result.TriangleCount = triangleCount;
			return result;
		}

		static Random random = new Random();
		public static Geometry3D GenerateChunkModelTiles(Game namelessGame, Point chunkToGenerate, ChunkData chunks, TileAtlasConfig atlasConfig)
		{

			var result = new Geometry3D();

			var chunk = chunks.Chunks[chunkToGenerate];
			chunk.Activate();

			//activate neighbors
			var sp = new Point(chunkToGenerate.X, chunkToGenerate.Y + 1);
			var southChunk = chunks.Chunks[sp];
			var ep = new Point(chunkToGenerate.X + 1, chunkToGenerate.Y);
			var eastChunk = chunks.Chunks[ep];

			southChunk.Activate();
			eastChunk.Activate();

			//chunks.RealityBubbleChunks.Add(sp, southChunk);
			//	chunks.RealityBubbleChunks.Add(wp, westChunk);


			var currentCorner = chunk.WorldPositionBottomLeftCorner;
			if (firstTime)
			{
				firstTime = false;
				originalPointForTest = currentCorner;
			}

			var tiles = chunk.GetChunkTiles();
			Queue<Vertex3D> vertices = new Queue<Vertex3D>();
			Queue<int> indices = new Queue<int>();
			var resolution = Constants.ChunkSize;
			Matrix scaleDown = Matrix.CreateScale(0.001f);
			var transformedPoints = new Queue<Vector3>();
			var colors = new Queue<Vector4>();
			const float worldHeight = 1300;

			//for terrain collion detection
			var tileTriangleAssociations = new List<Point>();

			//bool first = true;
			for (int x = 0; x < resolution; x++)
			{
				for (int y = 0; y < resolution; y++)
				{
					Tile tile = chunks.GetTile(chunk.WorldPositionBottomLeftCorner.X + x, chunk.WorldPositionBottomLeftCorner.Y + y);
					Tile tileE = chunks.GetTile(chunk.WorldPositionBottomLeftCorner.X + x + 1, chunk.WorldPositionBottomLeftCorner.Y + y);
					Tile tileS = chunks.GetTile(chunk.WorldPositionBottomLeftCorner.X + x, chunk.WorldPositionBottomLeftCorner.Y + y + 1);
					Tile tileSE = chunks.GetTile(chunk.WorldPositionBottomLeftCorner.X + x + 1, chunk.WorldPositionBottomLeftCorner.Y + y + 1);

					float elevation = (float)tile.Elevation;
					float elevationE = (float)tileE.Elevation;
					float elevationS = (float)tileS.Elevation;
					float elevationSE = (float)tileSE.Elevation;

					void AddPoint(int x, int y, float pointElevation, Tile tile)
					{
						pointElevation = pointElevation - 0.5f;
						bool _isMountain(Tile t)
						{
							return t.Terrain == TerrainTypes.LightRocks || t.Terrain == TerrainTypes.HardRocks || t.Terrain == TerrainTypes.Snow;
						}

						float elevation = (pointElevation) * worldHeight;
						elevation = MathF.Pow(elevation,2) * 0.005f;
						transformedPoints.Enqueue(Vector3.Transform(
							new Vector3(
							x + currentCorner.X - originalPointForTest.X,
							y + currentCorner.Y - originalPointForTest.Y,
							elevation), scaleDown));
						var tileColor = TerrainLibrary.Terrains[tile.Terrain].Representation.CharColor;
						//colors.Add(first? new Vector4(100,0,0,100):tileColor.ToVector4());
						//first = false;
						colors.Enqueue(tileColor.ToVector4() * (random.Next(7, 9) / 10f));
					}

				

					//Tile nwTile = chunks.GetTile(chunk.WorldPositionBottomLeftCorner.X + x - 1, chunk.WorldPositionBottomLeftCorner.Y + y - 1);
					//Tile neTile = chunks.GetTile(chunk.WorldPositionBottomLeftCorner.X + x + 1, chunk.WorldPositionBottomLeftCorner.Y + y - 1);
					//Tile swTile = chunks.GetTile(chunk.WorldPositionBottomLeftCorner.X + x - 1, chunk.WorldPositionBottomLeftCorner.Y + y + 1);
					//Tile seTile = chunks.GetTile(chunk.WorldPositionBottomLeftCorner.X + x + 1, chunk.WorldPositionBottomLeftCorner.Y + y + 1);

					//float _getElevation(Tile neighbor, float currentElevation) { 
					//	return neighbor.Biome == Biomes.None ? elevation : (float)((elevation + neighbor.Elevation) / 2f);
					//}

					//float nwElevation = _getElevation(nwTile, elevation);
					//float neElevation = _getElevation(neTile, elevation);
					//float swElevation = _getElevation(swTile, elevation);
					//float seElevation = _getElevation(seTile, elevation);


					AddPoint(x, y, elevation, tile);
					AddPoint(x + 1, y, elevationE, tileE);
					AddPoint(x, y + 1, elevationS, tileS);
					AddPoint(x + 1, y + 1, elevationSE, tileSE);

					for (int i = 0; i < 6; i++)
					{
						tileTriangleAssociations.Add(new Point(x, y));
					}			
				}
			}

			int triangleCount = 0;
			for (int i = 0; i < transformedPoints.Count - 3; i += 4)
			{
				var index = i;

				indices.Enqueue(index);
				indices.Enqueue(index + 1);
				indices.Enqueue(index + 2);

				indices.Enqueue(index + 3);
				indices.Enqueue(index + 2);
				indices.Enqueue(index + 1);

				triangleCount += 2;
			}
			var points = transformedPoints.ToList();
			while (transformedPoints.Any())
			{
				Vector3 point = transformedPoints.Dequeue();
				var color = colors.Dequeue();
				var vertex = new Systems.Ingame.Vertex3D(point,
				color,
				color, new Vector2(0, 0), Vector3.UnitZ);
				vertices.Enqueue(vertex);
			}

			result.Vertices = points;
			result.Indices = indices.ToList();
			result.Bounds = Microsoft.Xna.Framework.BoundingBox.CreateFromPoints(points);

			result.TriangleTerrainAssociation = tileTriangleAssociations;

			result.Buffer = new Microsoft.Xna.Framework.Graphics.VertexBuffer(namelessGame.GraphicsDevice, RenderingSystem3D.VertexDeclaration, vertices.Count, Microsoft.Xna.Framework.Graphics.BufferUsage.None);
			result.Buffer.SetData<Vertex3D>(vertices.ToArray());
			result.IndexBuffer = new Microsoft.Xna.Framework.Graphics.IndexBuffer(namelessGame.GraphicsDevice, Microsoft.Xna.Framework.Graphics.IndexElementSize.ThirtyTwoBits, indices.Count, Microsoft.Xna.Framework.Graphics.BufferUsage.None);
			result.IndexBuffer.SetData<int>(indices.ToArray());
			result.TriangleCount = triangleCount;
			return result;
		}
	}

}

using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Components;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Systems.Ingame;
using NamelessRogue.Engine.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace NamelessRogue.Engine._3DUtility
{
	enum SurfaceType {
		North, South, East, West, Top, Bottom
	}
	public static class ChunkGeometryGeneratorTiled
	{

		static List<Vector3> _points = new List<Vector3>();
		static List<int> _indices = new List<int>();

		static ChunkGeometryGeneratorTiled()
		{
			_points.Add(new Vector3(0, 0, 0));
			_points.Add(new Vector3(1, 0, 0));
			_points.Add(new Vector3(1, 1, 0));
			_points.Add(new Vector3(0, 1, 0));

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
		static Matrix moveToZero = Matrix.CreateTranslation(-(new Vector3(1,1,0) / 2));
		static Matrix invertedMoveToZero = Matrix.Invert(moveToZero);
		static Matrix scaleDown = Matrix.CreateScale(0.01f);
		//precalculated out of loop
		static Matrix northRot = Matrix.CreateRotationX(MathHelper.ToRadians(90f));
		static Matrix northTR = Matrix.CreateTranslation(new Vector3(0, -0.5f, 0.5f));

		static Matrix southRot = Matrix.CreateRotationX(MathHelper.ToRadians(90f));
		static Matrix southTR = Matrix.CreateTranslation(new Vector3(0, 0.5f, 0.5f));

		static Matrix eastRot = Matrix.CreateRotationX(MathHelper.ToRadians(90f)) * Matrix.CreateRotationZ(MathHelper.ToRadians(90f));
		static Matrix eastTR = Matrix.CreateTranslation(new Vector3(-0.5f, 0, 0.5f));

		static Matrix westRot = Matrix.CreateRotationX(MathHelper.ToRadians(90f)) * Matrix.CreateRotationZ(MathHelper.ToRadians(90f));
		static Matrix westTR = Matrix.CreateTranslation(new Vector3(0.5f, 0, 0.5f));

		public static Geometry3D GenerateChunkModel(Game namelessGame, Point chunkToGenerate, ChunkData chunks, TileAtlasConfig atlasConfig)
		{

		

			var result = new Geometry3D();
			var chunk = chunks.Chunks[chunkToGenerate];
			chunk.Activate();

			var currentCorner = chunk.WorldPositionBottomLeftCorner;
			if (firstTime)
			{
				firstTime = false;
				originalPointForTest = currentCorner;
			}

				

			var tiles = chunk.GetChunkTiles();


			int geometryCounter = 0;
			List<Vertex3D> vertices = new List<Vertex3D>();
			List<int> indices = new List<int>();
			var light = new Vector3(0, 0, 1);
			// TODO:unoptimized
			void AddSurface(SurfaceType type, float surfaceHeight, float neighborHeight, AtlasTileData atlasTileData, Vector2 tilePostion, Utility.Color tileColor)
			{


				Vector3 originalNormal = Vector3.UnitZ;
				Matrix rotation = Matrix.Identity;
				Matrix translation = Matrix.Identity;
				const float worldHeight = 2500;
				var sideHeight = Math.Abs(surfaceHeight - neighborHeight);
				//if (sideHeight < 0.2f) sideHeight = 0.2f;
				var sideMatrix = Matrix.CreateTranslation(new Vector3(0, 0, 1 - sideHeight));
				sideMatrix = Matrix.Identity;
				switch (type)
				{
					case SurfaceType.Top:
					case SurfaceType.Bottom:
					
						break;
					case SurfaceType.North:
						rotation = northRot;
						translation = northTR;
						//tileColor = new Utility.Color(0, 0, 255);						
						break;
					case SurfaceType.South:
						rotation = southRot;
						translation = southTR;
						//tileColor = new Utility.Color(0, 128, 128);
						break;
					case SurfaceType.East:
						rotation = eastRot;
						translation = eastTR;
						//tileColor = new Utility.Color(255, 0, 0);
						break;
					case SurfaceType.West:
						rotation = westRot;
						translation = westTR;
						//tileColor = new Utility.Color(0, 255, 0);
						break;

				}

				var elevationMatrix = Matrix.CreateTranslation(new Vector3(tilePostion.X, tilePostion.Y, ((surfaceHeight) * worldHeight) - worldHeight));

				var finalMatrix = moveToZero * rotation * translation * elevationMatrix * invertedMoveToZero * sideMatrix * scaleDown;
				//add transformed position to geometry
				var transformedPoints = new Queue<Vector3>();

				transformedPoints.Enqueue(Vector3.Transform(_points[0], finalMatrix));
				transformedPoints.Enqueue(Vector3.Transform(_points[1], finalMatrix));

				if (type == SurfaceType.Top || type == SurfaceType.Bottom)
				{
					transformedPoints.Enqueue(Vector3.Transform(_points[2], finalMatrix));
					transformedPoints.Enqueue(Vector3.Transform(_points[3], finalMatrix));
				}
				else
				{
					var p2 = _points[2]; p2.Y = -sideHeight * worldHeight;
					var p3 = _points[3]; p3.Y = -sideHeight * worldHeight;
					transformedPoints.Enqueue(Vector3.Transform(p2, finalMatrix));
					transformedPoints.Enqueue(Vector3.Transform(p3, finalMatrix));
				}

				var transformedNormal = Vector3.TransformNormal(originalNormal, finalMatrix);

				transformedNormal.Normalize();

				var textureCoordCounter = 1;
				foreach (var point in transformedPoints)
				{
					
					Vector2 textCoord = default(Vector2);
					switch (textureCoordCounter)
					{
						case 1:
							textCoord = new Vector2(0, 0);
							break;
						case 2:
							textCoord = new Vector2(1, 0);
							break;
						case 3:
							textCoord = new Vector2(1, 1);
							break;
						case 4:
							textureCoordCounter = 1;
							textCoord = new Vector2(0, 1);
							break;
						default:
							textureCoordCounter = 1;
							break;
					}
					var vetex = new Systems.Ingame.Vertex3D(point, 
						new Vector4(tileColor.Red, tileColor.Green, tileColor.Blue, 1), 
						new Vector4(tileColor.Red, tileColor.Green, tileColor.Blue, 1), textCoord, transformedNormal);
					textureCoordCounter++;
					vertices.Add(vetex);
				}

				foreach (var index in _indices)
				{
					indices.Add(index + geometryCounter);
				}
				geometryCounter += 4;
			}
			var resolution = Constants.ChunkSize;
			for (int x = 0; x < resolution; x++)
			{
				for (int y = 0; y < resolution; y++)
				{
					var currentTile = tiles[x][y];

					//var atlasData = atlasConfig.CharacterToTileDictionary[TerrainLibrary.Terrains[currentTile.Terrain].Representation.Representation];

					var atlasData = new AtlasTileData(15, 15);
					var sideData = new AtlasTileData(11, 13);
					Tile north, south, east, west;

					north = chunks.GetTile(chunk.WorldPositionBottomLeftCorner.X + x, chunk.WorldPositionBottomLeftCorner.Y + y - 1);
					south = chunks.GetTile(chunk.WorldPositionBottomLeftCorner.X + x, chunk.WorldPositionBottomLeftCorner.Y + y + 1);
					east = chunks.GetTile(chunk.WorldPositionBottomLeftCorner.X + x - 1, chunk.WorldPositionBottomLeftCorner.Y + y);
					west = chunks.GetTile(chunk.WorldPositionBottomLeftCorner.X + x + 1, chunk.WorldPositionBottomLeftCorner.Y + y);

					var shift = new Vector2(x + currentCorner.X - originalPointForTest.X, y + currentCorner.Y - originalPointForTest.Y);
					var tileColor = TerrainLibrary.Terrains[currentTile.Terrain].Representation.CharColor;
				

					bool nth, sth;
					nth = sth = false;

					//if (x == 6 && y == 6)
					//{
					//	currentTile.Elevation += 0.2f;
					//}
					if (currentTile.Elevation > 0.7) {
						currentTile.ToString();
					}

					if (north.Elevation - currentTile.Elevation < 0)
					{
						AddSurface(SurfaceType.North, (float)currentTile.Elevation, (float)north.Elevation, atlasData, shift, tileColor);
						nth = true;
					}
					if (south.Elevation - currentTile.Elevation < 0)
					{
						AddSurface(SurfaceType.South, (float)currentTile.Elevation, (float)south.Elevation, atlasData, shift, tileColor);
						sth = true;
					}
					if (east.Elevation - currentTile.Elevation < 0)
					{
						AddSurface(SurfaceType.East, (float)currentTile.Elevation, (float)east.Elevation, atlasData, shift, tileColor);
					}
					if (west.Elevation - currentTile.Elevation < 0)
					{
						AddSurface(SurfaceType.West, (float)currentTile.Elevation, (float)west.Elevation, atlasData, shift, tileColor);
					}
					if (nth && sth)
					{
					//	tileColor = new Utility.Color(255, 0, 2	55);
					}
					AddSurface(SurfaceType.Top, (float)currentTile.Elevation, (float)currentTile.Elevation, atlasData, shift, tileColor);

				}
			}

			result.Buffer = new Microsoft.Xna.Framework.Graphics.VertexBuffer(namelessGame.GraphicsDevice, RenderingSystem3D.VertexDeclaration, vertices.Count, Microsoft.Xna.Framework.Graphics.BufferUsage.None);
			result.Buffer.SetData<Vertex3D>(vertices.ToArray());
			result.IndexBuffer = new Microsoft.Xna.Framework.Graphics.IndexBuffer(namelessGame.GraphicsDevice, Microsoft.Xna.Framework.Graphics.IndexElementSize.ThirtyTwoBits, indices.Count, Microsoft.Xna.Framework.Graphics.BufferUsage.None);
			result.IndexBuffer.SetData<int>(indices.ToArray());
			result.TriangleCount = vertices.Count - 1 / 3;
			return result;
		}
	}
}

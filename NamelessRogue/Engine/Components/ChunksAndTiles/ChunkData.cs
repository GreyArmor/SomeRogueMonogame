
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Generation;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Serialization;
using NamelessRogue.Engine.Utility;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace NamelessRogue.Engine.Components.ChunksAndTiles
{
	[SkipClassGeneration]
	public class ChunkData : IWorldProvider
	{
		private Dictionary<Point, Chunk> chunks;

		private Dictionary<Point, Chunk> realityBubbleChunks;
		public List<Chunk> RealityChunks { get; set; } = new List<Chunk>();
		private WorldSettings worldSettings;
		private WorldBoard worldBoard;

		public ChunkData(WorldSettings settings, WorldBoard worldBoard)
		{
			Id = Guid.NewGuid();
			chunks = new Dictionary<Point, Chunk>();
			realityBubbleChunks = new Dictionary<Point, Chunk>();
			worldSettings = settings;
			this.worldBoard = worldBoard;
			initWorld();
		}

		public ChunkData()
		{
		}

		void initWorld()
		{
			int offfset = 0;
			for (int x = offfset; x < ChunkResolution + offfset; x++)
			{
				for (int y = offfset; y < ChunkResolution + offfset; y++)
				{
					CreateChunk(x, y);
				}
			}
		}

		public void CreateChunk(int x, int y)
		{
			Chunk newChunk = new Chunk(new Point(x * Constants.ChunkSize, y * Constants.ChunkSize), new Point(x, y), this);
			//newChunk.FillWithTiles(terrainGenerator);
			chunks.Add(new Point(x, y), newChunk);
		}

		public void RemoveChunk(int x, int y)
		{
			chunks.Remove(new Point(x, y));
		}


		public Tile GetTileAt(Point p)
		{
			return GetTile(p.X, p.Y);
		}

		//TODO: we need to implement quick iteration by using bounding box trees;
		public int ChunkResolution { get; set; } = WorldGenConstants.Resolution;
		public WorldBoard WorldBoard { get => worldBoard; set => worldBoard = value; }
		public Dictionary<Point, Chunk> Chunks { get => chunks; set => chunks = value; }
		public Dictionary<Point, Chunk> RealityBubbleChunks { get => realityBubbleChunks; set => realityBubbleChunks = value; }
		public WorldSettings WorldSettings { get => worldSettings; set => worldSettings = value; }

		public Tile GetTile(int x, int y)
		{

			Chunk chunkOfPoint = null;

			int chunkX = x / Constants.ChunkSize;
			int chunkY = y / Constants.ChunkSize;


			realityBubbleChunks.TryGetValue(new Point(chunkX, chunkY), out chunkOfPoint);

			if (chunkOfPoint == null)
			{
				return new Tile(TerrainTypes.Nothingness, Biomes.None, new Point(-1, -1), 0.5);
			}
			var result = chunkOfPoint.GetTile(x, y);
			return result;
			//return new Tile(TerrainTypes.Nothingness, Biomes.None, new Point(-1, -1), 0.5); ;

		}
		public bool SetTile(int x, int y, Tile tile)
		{
			Chunk chunkOfPoint = null;

			int chunkX = x / Constants.ChunkSize;
			int chunkY = y / Constants.ChunkSize;


			realityBubbleChunks.TryGetValue(new Point(chunkX, chunkY), out chunkOfPoint);


			if (chunkOfPoint == null)
			{
				return false;
			}

			chunkOfPoint.SetTile(x, y, tile);
			return true;
		}


		public TerrainGenerator GetWorldGenerator()
		{
			return worldSettings.TerrainGen;
		}

		public InternalRandom GetGlobalRandom()
		{
			return worldSettings.GlobalRandom;
		}

		public Dictionary<Point, Chunk> GetRealityBubbleChunks()
		{
			return realityBubbleChunks;
		}

		public Dictionary<Point, Chunk> GetChunks()
		{
			return chunks;
		}

		Guid Id;


		public Guid GetId()
		{
			return Id;
		}


		public void ClearTheWay(IEntity entity, Point moveTo)
		{
			Position position = entity.GetComponentOfType<Position>();
			if (position != null)
			{
				Tile oldTile = this.GetTile(position.Point.X, position.Point.Y);
				oldTile.RemoveEntity((Entity)entity);
			}
		}

		public void AddEntityToNewLocation(IEntity entity, Point moveTo)
		{

			Position position = entity.GetComponentOfType<Position>();
			if (position != null)
			{
				Tile newTile = this.GetTile(moveTo.X, moveTo.Y);
				newTile.AddEntity((Entity)entity);

				var new3dNormal = new Vector2(position.Point.X - moveTo.X, position.Point.Y - moveTo.Y);
				position.Point = new Point(moveTo.X, moveTo.Y);
				var pos3d = entity.GetComponentOfType<Position3D>();
				if (pos3d != null)
				{
					pos3d.Tile = newTile;
					pos3d.Position = new Vector3(moveTo.X, moveTo.Y, newTile.ElevationVisual);
					pos3d.Normal = new3dNormal;

					pos3d.WorldPosition = null;
				}
			}
		}

		public bool MoveEntity(IEntity entity, Point moveTo)
		{
			Position position = entity.GetComponentOfType<Position>();
			if (position != null)
			{
				IWorldProvider worldProvider = this;

				Tile oldTile = worldProvider.GetTile(position.Point.X, position.Point.Y);
				Tile newTile = worldProvider.GetTile(moveTo.X, moveTo.Y);

				if (newTile.IsPassable())
				{
					oldTile.RemoveEntity((Entity)entity);
					newTile.AddEntity((Entity)entity);

					var new3dNormal = new Vector2(position.Point.X - moveTo.X, position.Point.Y - moveTo.Y);


					position.Point = new Point(moveTo.X, moveTo.Y);

					var pos3d = entity.GetComponentOfType<Position3D>();

					if (pos3d != null)
					{
						pos3d.Tile = newTile;
						pos3d.Position = new Vector3(moveTo.X, moveTo.Y, newTile.ElevationVisual);
						pos3d.Normal = new3dNormal;

						pos3d.WorldPosition = null;
					}

					return true;
				}
			}

			return false;
		}
	}
}

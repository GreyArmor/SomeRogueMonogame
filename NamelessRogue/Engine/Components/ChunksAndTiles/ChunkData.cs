using Assimp;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Generation;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Serialization;
using NamelessRogue.Engine.Utility;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;

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


		//TODO: we need to implement quick iteration by using bounding box trees;
		public int ChunkResolution { get; set; } = WorldGenConstants.Resolution;
		public WorldBoard WorldBoard { get => worldBoard; set => worldBoard = value; }
		public Dictionary<Point, Chunk> Chunks { get => chunks; set => chunks = value; }
		public Dictionary<Point, Chunk> RealityBubbleChunks { get => realityBubbleChunks; set => realityBubbleChunks = value; }
		public WorldSettings WorldSettings { get => worldSettings; set => worldSettings = value; }

		public Tile GetTile(int x, int y, int z)
		{

			Chunk chunkOfPoint = null;

			int chunkX = x / Constants.ChunkSize;
			int chunkY = y / Constants.ChunkSize;


			realityBubbleChunks.TryGetValue(new Point(chunkX, chunkY), out chunkOfPoint);

			if (chunkOfPoint == null)
			{
				return new Tile(TerrainTypes.Nothingness, Biomes.None, new Point(-1, -1), 0.5);
			}
			var result = chunkOfPoint.GetTile(x, y, z);
			return result;
			//return new Tile(TerrainTypes.Nothingness, Biomes.None, new Point(-1, -1), 0.5); ;

		}
		public bool SetTile(int x, int y, int z, Tile tile)
		{
			Chunk chunkOfPoint = null;

			int chunkX = x / Constants.ChunkSize;
			int chunkY = y / Constants.ChunkSize;


			realityBubbleChunks.TryGetValue(new Point(chunkX, chunkY), out chunkOfPoint);


			if (chunkOfPoint == null)
			{
				return false;
			}

			chunkOfPoint.SetTile(x, y, z, tile);
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

		public void AddEntityToNewLocation(IEntity entity, int x, int y, int z)
		{

			Position position = entity.GetComponentOfType<Position>();
			if (position != null)
			{
				Tile newTile = this.GetTile(x, y, z);
				newTile.AddEntity((Entity)entity);
				position.Point = new Vector3Int(x, y, z);
			}
		}
        public bool MoveEntity(IEntity entity, Vector3Int moveTo)
		{
			return MoveEntity(entity, moveTo.X, moveTo.Y, moveTo.Z);
        }

        public bool MoveEntity(IEntity entity, int x, int y, int z)
		{
			Position position = entity.GetComponentOfType<Position>();
			if (position != null)
			{

                IWorldProvider worldProvider = this;

				Tile oldTile = worldProvider.GetTile(position.Point.X, position.Point.Y, position.Point.Z);
				Tile newTile = worldProvider.GetTile(x, y, z);

				if (newTile.IsPassable())
				{
					oldTile.RemoveEntity((Entity)entity);
					newTile.AddEntity((Entity)entity);
					position.Point = new Vector3Int(x, y, z);
                    return true;
				}
			}

			return false;
		}
	}
}

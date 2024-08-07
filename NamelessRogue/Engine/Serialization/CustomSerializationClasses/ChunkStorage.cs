﻿using FlatSharp.Attributes;

using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Serialization.AutogeneratedSerializationClasses;
using NamelessRogue.Engine.Serialization.SerializationIfrastructure;
using NamelessRogue.Engine.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace NamelessRogue.Engine.Serialization.CustomSerializationClasses
{
	[FlatBufferTable]
	public class ChunkStorage : IStorage<Chunk>
	{
		[FlatBufferItem(1)] public PointStorage ChunkWorldMapLocationPoint { get; set; }
		[FlatBufferItem(2)] public BoundingBoxStorage Bounds { get; set; }
		[FlatBufferItem(3)] public TileStorage[] Tiles { get; set; }
		[FlatBufferItem(4)] public int ChunkResolution { get; set; }
		[FlatBufferItem(5)] public PointStorage WorldPositionBottomLeftCorner { get; set; }

		public void FillFrom(Chunk component)
		{
			ChunkWorldMapLocationPoint = component.ChunkWorldMapLocationPoint;
			Bounds = new BoundingBox3D(component.Bounds.Min, component.Bounds.Max);
			WorldPositionBottomLeftCorner = component.WorldPositionBottomLeftCorner;
			var tiles = component.GetChunkTiles();
			if (tiles != null)
			{
				ChunkResolution = Constants.ChunkSize;
				Tiles = new TileStorage[ChunkResolution * ChunkResolution];

				for (int i = 0; i < ChunkResolution; i++)
				{
					for (int j = 0; j < ChunkResolution; j++)
					{
						Tiles[i * ChunkResolution + j] = tiles[i][j];
					}
				}
			}			
		}

		public void FillTo(Chunk component)
		{
			component.ChunkWorldMapLocationPoint = ChunkWorldMapLocationPoint;
			component.WorldPositionBottomLeftCorner = WorldPositionBottomLeftCorner;
			component.Bounds = Bounds;
			if (Tiles != null)
			{
				Tile[][] tiles;
				tiles = new Tile[Tiles.Length][];
				for (var index = 0; index < Tiles.Length; index++)
				{
					tiles[index] = new Tile[ChunkResolution];
				}

				for (int i = 0; i < ChunkResolution; i++)
				{
					for (int j = 0; j < ChunkResolution; j++)
					{
						tiles[i][j] = Tiles[i* ChunkResolution + j];
					}
				}
				component.SetChunkTiles(tiles);
			}

			//obviously, it was not generated right now
			component.JustCreated = false;
		}
	}
}

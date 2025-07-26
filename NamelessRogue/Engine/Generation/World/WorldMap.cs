using Microsoft.Xna.Framework;
using MonoGame.Extended;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Utility;
using System;
using System.Collections.Generic;

namespace NamelessRogue.Engine.Generation.World
{

	public class TileForGeneration
	{
        public double fillValue;
        public int x;
		public int y;
		public bool isWater;
		public TileForGeneration parent;
        public int X { get => x; set => x = value; }
        public int Y { get => y; set => y = value; }
        public bool IsWater { get => isWater; set => isWater = value; }
        public double FillValue { get => fillValue; set => fillValue = value; }
    }

	public class TileForPainting
	{
        public int x;
        public int y;
        public bool isWater;
        public List<Waypoints> WaterBorderLines { get; set; } = new List<Waypoints>();
        public List<Waypoints> Roads { get; set; } = new List<Waypoints>();
        public int X { get => x; set => x = value; }
        public int Y { get => y; set => y = value; }
        public bool IsWater { get => isWater; set => isWater = value; }
    }

	public class Waypoints
	{
		public List<Vector2> Points { get; set; } = new List<Vector2>();
	}


    public class WorldMap
    {
        public int Resolution { get; set; }
        public WorldTile[,] WorldTiles { get; set; }
        public ChunkData Chunks { get; set; }
		public List<Waypoints> RiverBorderLines { get; set; } = new List<Waypoints>();
		public List<Waypoints> Roads { get; set; } = new List<Waypoints>();
        public TileForPainting[][] TerrainFeatures { get; set; }
 

        public WorldMap(int resolution)
        {
            WorldTiles = new WorldTile[resolution, resolution];
            			
			TerrainFeatures = new TileForPainting[resolution][];
			for (int i = 0; i < resolution; i++)
			{
				TerrainFeatures[i] = new TileForPainting[resolution];
			}

			RiverBorderLines = new List<Waypoints>();
            Resolution = resolution;            
        }

		public WorldMap()
		{
		}
	}
}

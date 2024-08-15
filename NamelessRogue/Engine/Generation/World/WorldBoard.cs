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
	}

	public class TileForPainting
	{
		public int x;
		public int y;
		public bool isWater;
		public List<Waypoints> WaterBorderLines { get; set; } = new List<Waypoints>();
        public List<Waypoints> Roads { get; set; } = new List<Waypoints>();
    }

	public class Waypoints
	{
		public List<Vector2> Points { get; set; } = new List<Vector2>();
	}

	public class CityPart {

        public Matrix Transform { get; set; } = new Matrix();

		public Point Center { get; set; }

		public List<Waypoints> Roads { get; set; } = new List<Waypoints>();

		Utility.BoundingBox Bounds { get; set; } = new Utility.BoundingBox();
	}


    public class WorldBoard
    {
        public DateTime CurrentTime { get; set; }
        public WorldTile[,] WorldTiles { get; set; }
        public ChunkData Chunks { get; set; }
		public float[][] ElevationMap { get => elevationMap; set => elevationMap = value; }
		public bool[][] RiverMap { get => riverMap; set => riverMap = value; }
		public bool[][] RiverBorderMap { get => riverBorderMap; set => riverBorderMap = value; }
		public List<Waypoints> RiverBorderLines { get; set; } = new List<Waypoints>();

		public List<Waypoints> Roads { get; set; } = new List<Waypoints>();

        public List<CityPart> CityParts { get; set; }


		//used for river/lake generation
		private float[][] elevationMap;
		private bool[][] riverMap;
		private bool[][] riverBorderMap;
        private bool[][] roadsMap;
        public TileForPainting[][] TerrainFeatures { get; set; }


        public WorldBoard(int width, int height, int age)
        {
            WorldTiles = new WorldTile[width, height];
            CurrentTime = new DateTime(2050, 5, 14);


			var resolution = WorldGenConstants.Resolution;

			ElevationMap = new float[resolution][];
			for (int i = 0; i < resolution; i++)
			{
				ElevationMap[i] = new float[resolution];
			}

			RiverMap = new bool[resolution][];
			for (int i = 0; i < resolution; i++)
			{
				RiverMap[i] = new bool[resolution];
			}

			RiverBorderMap = new bool[resolution][];
			for (int i = 0; i < resolution; i++)
			{
				RiverBorderMap[i] = new bool[resolution];
			}

			TerrainFeatures = new TileForPainting[resolution][];
			for (int i = 0; i < resolution; i++)
			{
				TerrainFeatures[i] = new TileForPainting[resolution];
			}

			RiverBorderLines = new List<Waypoints>();
			CityParts = new List<CityPart>();
		}

		public WorldBoard()
		{
		}
	}
}

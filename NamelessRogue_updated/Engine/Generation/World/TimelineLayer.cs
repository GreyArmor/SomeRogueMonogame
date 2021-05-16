 

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Components;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue_updated.Engine.Serialization;
using Newtonsoft.Json;

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

	public class TileForInlandWaterConnectivity
	{
		public int x;
		public int y;
		public bool isWater;
		public List<WaterBorderLine> WaterBorderLines { get; set; } = new List<WaterBorderLine>();
	}

	public class WaterBorderLine
	{
		public List<Point> Points { get; set; }
	}

	[SkipClassGeneration]
	public class TimelineLayer
    {
        public int Age { get; }
        //[JsonIgnore]
        public WorldTile[,] WorldTiles { get; set; }
        public List<Civilization> Civilizations { get; set; }
        public List<Region> Continents { set; get; }
        public List<Region> Islands { set; get; }
        public List<Region> Mountains { set; get; }
        public List<Region> Forests { get; set; }
        public List<Region> Deserts { get; set; }
        public List<Region> Swamps { get; set; }
        [JsonIgnore]
        public ChunkData Chunks { get; set; }
		public double[][] ElevationMap { get => elevationMap; set => elevationMap = value; }
		public bool[][] RiverMap { get => riverMap; set => riverMap = value; }
		public bool[][] RiverBorderMap { get => riverBorderMap; set => riverBorderMap = value; }
		public List<WaterBorderLine> BorderLines { get; internal set; }


		//used for river/lake generation
		private double[][] elevationMap;
		private bool[][] riverMap;
		private bool[][] riverBorderMap;
		public TileForInlandWaterConnectivity[][] InlandWaterConnectivity;




        public TimelineLayer(int width, int height, int age)
        {
            WorldTiles = new WorldTile[width, height];
            Age = age;
            Civilizations = new List<Civilization>();
            Continents = new List<Region>();


			var resolution = WorldGenConstants.Resolution;

			ElevationMap = new double[resolution][];
			for (int i = 0; i < resolution; i++)
			{
				ElevationMap[i] = new double[resolution];
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

			InlandWaterConnectivity = new TileForInlandWaterConnectivity[resolution][];
			for (int i = 0; i < resolution; i++)
			{
				InlandWaterConnectivity[i] = new TileForInlandWaterConnectivity[resolution];
			}

			BorderLines = new List<WaterBorderLine>();
		}
    }
}

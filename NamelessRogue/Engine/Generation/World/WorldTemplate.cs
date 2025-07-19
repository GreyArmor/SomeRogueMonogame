using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using NamelessRogue.Engine.Components;
using NamelessRogue.Engine.Serialization;
using NamelessRogue.Engine.UI;
using NamelessRogue.Engine.Utility;

namespace NamelessRogue.Engine.Generation.World
{

    public enum WaterLevel
    {
        NoWater,
        Desert,
        Rivers,
        Coastal,
        CoastalWithRivers
    }

    public enum WorldSizeName
    {
        Tiny,
        Small,
        Medium,
        Big,
        Large,       
    }

    [SkipClassGeneration]
    public class WorldTemplate : Component
    {
        public int Seed { get; set; }
        public string Name { get; set; }
        public WorldSizeName WorldSizeName { get; set; }
        public Vector2 WorldSize { get; set; }
        public WaterLevel WaterType { get; set;}
        public int NeighbouringCityCount {get; set;}
        public DateTime CurrentTime { get; set; }

        public Vector2 CitySize { get; set; }

        public WorldTemplate(int seed)
        {
            Seed = seed;
        }

        public WorldTemplate(WorldGenerationParameters parameters)
        {
            Seed = parameters.seed.GetHashCode();
            Name = parameters.name;
            WorldSizeName = parameters.worldSize;
            WorldSize = new Vector2(parameters.worldSizeValue);
            WaterType = parameters.waterLevel;
            NeighbouringCityCount = parameters.neighboringCityCount;
            CurrentTime = parameters.time;
        }

        public WorldMap WorldMap { get; set; }

        public override IComponent Clone()
        {
            throw new NotImplementedException();
        }
    }
}

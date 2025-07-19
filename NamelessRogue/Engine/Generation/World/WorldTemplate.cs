using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using NamelessRogue.Engine.Components;
using NamelessRogue.Engine.Serialization;
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
        public InternalRandom GlobalRandom { get; }
        public Vector2 WorldSize { get; set; }

        public Vector2 CitySize { get; set; }

        public int WaterLevel { get; set; }

        public WaterLevel WaterType { get; set;}

        public int NeighbouringCityCount {get; set;}

        public DateTime CurrentTime { get; set; }

        public WorldTemplate(int seed)
        {
            Seed = seed;

        }
        public WorldMap WorldMap { get; set; }

        public override IComponent Clone()
        {
            throw new NotImplementedException();
        }
    }
}

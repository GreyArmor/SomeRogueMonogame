using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NamelessRogue.Engine.Engine.Generation.World;

namespace NamelessRogue.Engine.Engine.Generation
{
    public class WorldSettings
    {
        public WorldSettings(int seed)
        {
            Seed = seed;
            GlobalRandom = new Random(seed);
            TerrainGenerator terrainGenerator;
            TerrainGen = new TerrainGenerator(GlobalRandom);
        }

        public TerrainGenerator TerrainGen { get; }
        public int Seed { get; }
        public Random GlobalRandom { get; }
    }
}

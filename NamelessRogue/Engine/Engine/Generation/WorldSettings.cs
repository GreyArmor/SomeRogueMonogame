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
        public WorldSettings(int seed, int worldBoardWidth, int worldBoardHeight)
        {
            Seed = seed;
            WorldBoardWidth = worldBoardWidth;
            WorldBoardHeight = worldBoardHeight;
            GlobalRandom = new Random(seed);
            TerrainGen = new TerrainGenerator(GlobalRandom);
            WorldMapScale = 1;
            ContinentTilesPerCivilization = 1500;
            NamesGenerator = new NamesGenerator();
        }

        public int ContinentTilesPerCivilization { get; set; }
        public NamesGenerator NamesGenerator { get; }

        //TODO: not implemented currently
        public float WorldMapScale { get; set; }

        public TerrainGenerator TerrainGen { get; }
        public int Seed { get; }
        public int WorldBoardWidth { get; }
        public int WorldBoardHeight { get; }
        public Random GlobalRandom { get; }
    }
}

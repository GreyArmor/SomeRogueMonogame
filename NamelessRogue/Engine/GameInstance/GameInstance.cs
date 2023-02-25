using NamelessRogue.Engine.Generation.World;

namespace NamelessRogue.Engine.GameInstance
{
    public class GameInstance {
        int seed;
        TerrainGenerator terrainGen;
        HistoryGenerator historyGen;

        public int Turn { get; set; }


    }
}

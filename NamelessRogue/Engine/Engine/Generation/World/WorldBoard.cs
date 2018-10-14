 

namespace NamelessRogue.Engine.Engine.Generation.World
{
    public class WorldBoard {
        WorldDictionaryTile[,] worldDictionaryTiles;
        public WorldBoard(int width, int height)
        {
            worldDictionaryTiles = new WorldDictionaryTile[width,height];
        }
    }
}

using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.ChunksAndTiles;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Generation.World
{
    public abstract class WorldTileBluepring
    {

        public Tile[][] MatrixToFillWith { get; set; }
        public abstract void FillTheWorld(NamelessGame game,IChunkProvider worldProvider, WorldTile tile);
    }
}
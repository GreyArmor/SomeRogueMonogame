using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Utility;
using System.Collections.Generic;

namespace NamelessRogue.Engine.Generation.World
{
    public interface ITerrainGenerator
    {
        List<Waypoints> BorderLines { get; set; }
        InternalRandom Random { get; set; }

        float GetHeightNoise(int x, int y, float scale);
        Tile GetTileWithoutTerrainFeatures(int x, int y, float scale);
        void Init(InternalRandom random);
    }
}
using NamelessRogue.Engine.Components.ChunksAndTiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Generation.World.TerrainFeatures
{
    public interface ITerrainFeature
    {
        string Name { get; }
        void Draw(Chunk chunkToDrawOn);
    }
}

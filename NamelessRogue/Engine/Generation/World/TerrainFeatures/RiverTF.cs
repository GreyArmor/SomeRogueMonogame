using NamelessRogue.Engine.Components.ChunksAndTiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Generation.World.TerrainFeatures
{
    internal class RiverTF : ITerrainFeature
    {
        public string Name => "River";

        public void Draw(Chunk chunkToDrawOn)
        {
            throw new NotImplementedException();
        }
    }
}

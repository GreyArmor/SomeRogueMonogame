using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NamelessRogue.Engine.Engine.Components.Rendering;
using NamelessRogue.Engine.Engine.Generation.World;

namespace NamelessRogue.Engine.Engine.Infrastructure
{
    public class Terrain
    {
        public TerrainTypes Type { get; set; }
        public Drawable Representation { get; set; }

        public Terrain(TerrainTypes type, Drawable representation)
        {
            Type = type;
            Representation = representation;
        }
    }

    public class Biome
    {
        public Biomes Type { get; set; }
        public Drawable Representation { get; set; }

        public Biome(Biomes type, Drawable representation)
        {
            Type = type;
            Representation = representation;
        }
    }
}

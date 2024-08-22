using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Utility;

namespace NamelessRogue.Engine.Generation.World
{
    public class BiomesLibrary
    {

        public static Dictionary<Biomes, Biome> Biomes { get; set; }

        static BiomesLibrary()
        {
            Biomes = new Dictionary<Biomes, Biome>();

            Biomes.Add(World.Biomes.Sea,
                new Biome(World.Biomes.Sea, new Drawable("Sea", new Color(0, 255, 255), new Color(0, 0, 1f))));
            Biomes.Add(World.Biomes.Lake,
                new Biome(World.Biomes.Lake, new Drawable("Lake", new Color(0, 255, 255), new Color(0, 0, 1f))));

            Biomes.Add(World.Biomes.River,
                new Biome(World.Biomes.River, new Drawable("River", new Color(0, 255, 255), new Color(0, 0, 1f))));


            Biomes.Add(World.Biomes.Plains,
                new Biome(World.Biomes.Plains, new Drawable("Plains", new Color(0, 0.8f, 0), new Color())));
          
            Biomes.Add(World.Biomes.None,
                new Biome(World.Biomes.None, new Drawable("None", new Color(), new Color())));

        }
    }
}

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
                new Biome(World.Biomes.Sea, new Drawable('~', new Color(0, 255, 255), new Color(0, 0, 1f))));
            Biomes.Add(World.Biomes.Lake,
                new Biome(World.Biomes.Lake, new Drawable('~', new Color(0, 255, 255), new Color(0, 0, 1f))));

            Biomes.Add(World.Biomes.River,
                new Biome(World.Biomes.River, new Drawable('~', new Color(0, 255, 255), new Color(0, 0, 1f))));


            Biomes.Add(World.Biomes.Plains,
                new Biome(World.Biomes.Plains, new Drawable('.', new Color(0, 0.8f, 0), new Color())));
            Biomes.Add(World.Biomes.Savannah,
                new Biome(World.Biomes.Savannah, new Drawable('.', new Color(218 / 255f, 165 / 255f, 32 / 255f), new Color())));


            Biomes.Add(World.Biomes.Mountain,
                new Biome(World.Biomes.Mountain, new Drawable('A', new Color(0.5f, 0.5f, 0.5f), new Color())));
            Biomes.Add(World.Biomes.Hills,
                new Biome(World.Biomes.Hills, new Drawable('h', new Color(0, 0.8, 0), new Color())));

            Biomes.Add(World.Biomes.Beach,
                new Biome(World.Biomes.Beach, new Drawable('~', new Color(0.5f, 0.5f, 0), new Color())));
            Biomes.Add(World.Biomes.Desert,
                new Biome(World.Biomes.Desert, new Drawable('~', new Color(1f,1f, 0), new Color())));

            Biomes.Add(World.Biomes.SnowDesert,
                new Biome(World.Biomes.SnowDesert, new Drawable('~', new Color(1f, 1f, 1f), new Color())));

            Biomes.Add(World.Biomes.Forest,
                new Biome(World.Biomes.Forest, new Drawable('f', new Color(0f, 0.5f, 0f), new Color())));

            Biomes.Add(World.Biomes.Jungle,
                new Biome(World.Biomes.Jungle, new Drawable('j', new Color(0f, 0.5f, 0f), new Color())));

            Biomes.Add(World.Biomes.Tundra,
                new Biome(World.Biomes.Tundra, new Drawable('t', new Color(0, 255, 255), new Color())));


            Biomes.Add(World.Biomes.Swamp,
                new Biome(World.Biomes.Swamp, new Drawable('s', new Color(0.2, 0.2, 0.2), new Color())));

            Biomes.Add(World.Biomes.None,
                new Biome(World.Biomes.None, new Drawable(' ', new Color(), new Color())));

        }
    }
}

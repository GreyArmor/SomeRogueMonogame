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

    public static class TerrainLibrary
    {

        public static Dictionary<TerrainTypes, Terrain> Terrains { get; set; }

        static TerrainLibrary()
        {

            Terrains = new Dictionary<TerrainTypes, Terrain>();

            Terrains.Add(TerrainTypes.Dirt,
                new Terrain(TerrainTypes.Dirt,
                    new Drawable("Dirt", new Color(1f, 1f, 0), new Color())));

            Terrains.Add(TerrainTypes.Road,
                new Terrain(TerrainTypes.Road, new Drawable("Road", new Color(0.5, 0.5, 0.5), new Color())));

            Terrains.Add(TerrainTypes.Water,
                new Terrain(TerrainTypes.Water, new Drawable("Water", new Color(0, 0, 255), new Color(0, 255, 255))));

            Terrains.Add(TerrainTypes.Grass,
                new Terrain(TerrainTypes.Grass, new Drawable("Grass", new Color(0, 0.4f, 0), new Color())));

            Terrains.Add(TerrainTypes.HardRocks,
                new Terrain(TerrainTypes.HardRocks, new Drawable("HardRocks", new Color(0.2, 0.2, 0.2), new Color())));

            Terrains.Add(TerrainTypes.Asphault,
                new Terrain(TerrainTypes.Asphault, new Drawable("Asphault", new Color(0.8, 0.8, 0.8), new Color())));

            Terrains.Add(TerrainTypes.Sidewalk,
                new Terrain(TerrainTypes.Sidewalk, new Drawable("Sidewalk", new Color(0.6, 0.6, 0.6), new Color())));

            Terrains.Add(TerrainTypes.PaintedAsphault,
                new Terrain(TerrainTypes.PaintedAsphault, new Drawable("PaintedAsphault", new Color(1f, 1f, 1f), new Color())));


			Terrains.Add(TerrainTypes.Nothingness,
                new Terrain(TerrainTypes.Nothingness, new Drawable("Nothingness", new Color(), new Color())));
        }
    }
}

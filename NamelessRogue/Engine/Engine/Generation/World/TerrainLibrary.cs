﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NamelessRogue.Engine.Engine.Components.Rendering;
using NamelessRogue.Engine.Engine.Infrastructure;
using NamelessRogue.Engine.Engine.Utility;

namespace NamelessRogue.Engine.Engine.Generation.World
{

    public static class TerrainLibrary
    {

        public static Dictionary<TerrainTypes, Terrain> Terrains { get; set; }

        static TerrainLibrary()
        {

            Terrains = new Dictionary<TerrainTypes, Terrain>();

            Terrains.Add(TerrainTypes.Dirt,
                new Terrain(TerrainTypes.Dirt,
                    new Drawable('.', new Color(1f, 1f, 0), new Color())));

            Terrains.Add(TerrainTypes.Road,
                new Terrain(TerrainTypes.Road, new Drawable('.', new Color(0.5, 0.5, 0.5), new Color())));

            Terrains.Add(TerrainTypes.Water,
                new Terrain(TerrainTypes.Water, new Drawable('~', new Color(0, 0, 255), new Color(0, 1, 1))));

            Terrains.Add(TerrainTypes.Grass,
                new Terrain(TerrainTypes.Grass, new Drawable('.', new Color(0, 0.8f, 0), new Color())));

            Terrains.Add(TerrainTypes.HardRocks,
                new Terrain(TerrainTypes.HardRocks, new Drawable('.', new Color(0.2, 0.2, 0.2), new Color())));

            Terrains.Add(TerrainTypes.Rocks,
                new Terrain(TerrainTypes.Rocks, new Drawable('.', new Color(0.8, 0.8, 0.8), new Color())));

            Terrains.Add(TerrainTypes.LightRocks,
                new Terrain(TerrainTypes.LightRocks, new Drawable('.', new Color(0.5, 0.5, 0.5), new Color())));

            Terrains.Add(TerrainTypes.Sand,
                new Terrain(TerrainTypes.Sand, new Drawable('~', new Color(1f, 1f, 0), new Color())));

            Terrains.Add(TerrainTypes.Snow,
                new Terrain(TerrainTypes.Snow, new Drawable('~', new Color(1f, 1f, 1f), new Color())));

            Terrains.Add(TerrainTypes.Nothingness,
                new Terrain(TerrainTypes.Nothingness, new Drawable(' ', new Color(), new Color())));
        }
    }
}

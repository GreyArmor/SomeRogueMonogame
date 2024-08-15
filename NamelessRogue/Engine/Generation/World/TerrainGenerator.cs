using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Generation.Noise;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Utility;
using Color = NamelessRogue.Engine.Utility.Color;
using System.Drawing;
using Point = Microsoft.Xna.Framework.Point;
using VoronoiLib.Structures;
using VoronoiLib;
using RogueSharp.Random;

/**
* Created by Admin on 04.11.2017.
*/
namespace NamelessRogue.Engine.Generation.World
{
    public class TerrainGenerator : ITerrainGenerator
    {

        public List<SimplexNoise> TerrainNoises;
        public SimplexNoise ForestsNoise;
        public SimplexNoise LakesNoise;
        public SimplexNoise SwampNoise;
        public SimplexNoise DesertNoise;
        public SimplexNoise TemperatureNoise;
        int lenght = 100;
        int layer1 = 3000,layer2 = 1000,layer3 = 1000;
        public InternalRandom Random { get; set; }
        public List<Waypoints> BorderLines { get; set; } = new List<Waypoints>();

        public TerrainGenerator(InternalRandom random)
        {
            Init(random);
        }

        public void Init(InternalRandom random)
        {
            Random = random;
            TerrainNoises = new List<SimplexNoise>();
            SimplexNoise noise1 = new SimplexNoise(layer1, 0.5, random);
            SimplexNoise noise2 = new SimplexNoise(layer2, 0.5, random);
            SimplexNoise noise3 = new SimplexNoise(layer3, 0.5, random);


            ForestsNoise = new SimplexNoise(200, 0.75, random);
            LakesNoise = new SimplexNoise(200, 0.75, random);
            SwampNoise = new SimplexNoise(200, 0.75, random);
            DesertNoise = new SimplexNoise(200, 0.75, random);

            TemperatureNoise = new SimplexNoise(200, 0.75, random);

            TerrainNoises.Add(noise1);
            //TerrainNoises.Add(noise2);
            //TerrainNoises.Add(noise3);
        }

        public TerrainGenerator()
        {
        }

        public float GetHeightNoise(int x, int y, float scale)
        {
            double dX = (double)x / scale;
            double dY = (double)y / scale;
            int resolutionZoomed = (int)(WorldGenConstants.Resolution * scale);
            int borderthickness = 0;

            double noise = 0;
            foreach (SimplexNoise s in TerrainNoises)
            {
                noise += s.getNoise(dX, dY);
            }

            noise /= TerrainNoises.Count;


            double result = 1 - (0.5 * (1 + noise));

            if (x < borderthickness || y < borderthickness || x > resolutionZoomed - borderthickness || y > resolutionZoomed - borderthickness)
            {

                int iDist = x > resolutionZoomed - borderthickness ? resolutionZoomed - x : x;
                int jDist = y > resolutionZoomed - borderthickness ? resolutionZoomed - y : y;
                int edgePosition = iDist > jDist ? jDist : iDist;
                //System.out.print(String.format("edgePosition = {0}\n", edgePosition));
                result *= (float)edgePosition / (resolutionZoomed / 10);
            }
            return (float)result;
        }

        public Tile GetTileWithoutTerrainFeatures(int x, int y, float scale)
        {
            double dX = (double)x / scale;
            double dY = (double)y / scale;
            // System.out.print("X ="+dX+"Y =" +dY +"\n");
            int resolutionZoomed = (int)(WorldGenConstants.Resolution * scale);

            var terrainElevation = GetHeightNoise(x, y, scale);

            if (terrainElevation <= 0.5)
            {
                terrainElevation = 0.5f;
            }

            double forest = 1 - (0.5 * (1 + ForestsNoise.getNoise(dX, dY)));
            double swamp = 1 - (0.5 * (1 + SwampNoise.getNoise(dX, dY)));
            // double lake = 1 - (0.5 * (1 + LakesNoise.getNoise(dX, dY)));
            double desert = 1 - (0.5 * (1 + DesertNoise.getNoise(dX, dY)));

            double temperature = 0.5 - (0.5 * (1 + TemperatureNoise.getNoise(dX, dY)));
            temperature = temperature / 20;
            Tuple<Terrain, Biome> terrinBiome =
                TileNoiseInterpreter.GetTerrain(terrainElevation, forest, swamp, desert, temperature, resolutionZoomed, x, y);
            return new Tile(terrinBiome.Item1.Type, terrinBiome.Item2.Type, new Point(x, y), terrainElevation);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Engine.Generation.Noise;
using NamelessRogue.Engine.Engine.Infrastructure;

/**
 * Created by Admin on 04.11.2017.
 */
namespace NamelessRogue.Engine.Engine.Generation.World
{
    public class TerrainGenerator {


        public List<SimplexNoise> TerrainNoises;
        public int Seed;
        private int forestSeed;
        int resolution=1000;
        int layer1 = 300, layer2 = 600, layer3 = 900;

        public TerrainGenerator(Random random)
        {
            TerrainNoises =  new List<SimplexNoise>();
            // Constants.
            //Const.

            SimplexNoise noise1 = new SimplexNoise( layer1,0.5, random);
            SimplexNoise noise2 = new SimplexNoise( layer2,0.5, random);
            SimplexNoise noise3 = new SimplexNoise( layer3,0.5, random);


            TerrainNoises.Add(noise1);
            TerrainNoises.Add(noise2);
            TerrainNoises.Add(noise3);



            double xStart=0;
            double XEnd=1000;
            double yStart=0;
            double yEnd=1000;


            int borderthickness = resolution/10;



            double[,] result=new double[resolution,resolution];

            for(int i=0;i<resolution;i++){
                for(int j=0;j<resolution;j++) {

                    int x = (int) (xStart + i * ((XEnd - xStart) / resolution));
                    int y = (int) (yStart + j * ((yEnd - yStart) / resolution));
                    double noise = 0;
                    foreach (SimplexNoise s in TerrainNoises) {
                        noise += s.getNoise(x, y);
                    }
                    noise /= TerrainNoises.Count;
                    result[i,j] = 1d - (0.5d * (1d + noise));

                }
            }

            for(int i=0;i<resolution;i++) {
                for (int j = 0; j < resolution; j++) {
                    if (i < borderthickness || j < borderthickness || i > resolution - borderthickness || j > resolution - borderthickness) {

                        int iDist = i > resolution - borderthickness ? resolution - i : i;
                        int jDist = j >  resolution - borderthickness ? resolution - j : j;
                        int edgePosition = iDist > jDist ? jDist : iDist;
                        result[i,j] *= (float) edgePosition / (resolution/10);
                    }
                    if (result[i,j] <= 0.5) {
                        result[i,j] = 0;
                    }
                }
            }

            //for (int i = 0; i < resolution; i++)
            //{
            //    for (int j = 0; j < resolution; j++)
            //    {
            //        if (result[i, j] > 0.6f)
            //        {
            //            result[i, j].ToString();
            //        }
            //    }
            //}
            
          // ImageWriter.greyWriteImage(result, resolution);

        }

        public Tile GetTile(int x, int y, float scale)
        {
            double dX = (double)x/ scale;
            double dY = (double)y/ scale;
            // System.out.print("X ="+dX+"Y =" +dY +"\n");
            int resolutionZoomed = (int) (resolution * scale);
            int borderthickness = resolutionZoomed/10;

            double noise = 0;
            foreach (SimplexNoise s in TerrainNoises) {
                noise += s.getNoise(dX, dY);
            }

            noise /= TerrainNoises.Count;
            double result = 1 - (0.5 * (1 + noise));

            if (x < borderthickness || y < borderthickness || x > resolutionZoomed - borderthickness || y > resolutionZoomed - borderthickness) {

                int iDist = x > resolutionZoomed - borderthickness ? resolutionZoomed - x : x;
                int jDist = y >  resolutionZoomed - borderthickness ? resolutionZoomed - y : y;
                int edgePosition = iDist > jDist ? jDist : iDist;
                //System.out.print(String.format("edgePosition = {0}\n", edgePosition));
                result *= (float) edgePosition / (resolutionZoomed/10);
            }
            if (result <= 0.5) {
                result = 0;
            }
            return new Tile(TileNoiseInterpreter.GetTerrain(result), new Point(x,y));
        }
    }
}

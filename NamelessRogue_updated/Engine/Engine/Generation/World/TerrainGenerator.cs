using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Engine.Generation.Noise;
using NamelessRogue.Engine.Engine.Infrastructure;
using NamelessRogue.Engine.Engine.Utility;
using Color = NamelessRogue.Engine.Engine.Utility.Color;
using System.Drawing;
using Point = Microsoft.Xna.Framework.Point;

/**
* Created by Admin on 04.11.2017.
*/
namespace NamelessRogue.Engine.Engine.Generation.World
{
    public class TerrainGenerator {

        //used for river/lake generation
        public double[][] ElevationMap; 
        public bool[][] RiverMap;
        public bool[][] RiverBorderMap;
        public TileForInlandWaterConnectivity[][] InlandWaterConnectivity;


        public List<SimplexNoise> TerrainNoises;
        public SimplexNoise ForestsNoise;
        public SimplexNoise LakesNoise;
        public SimplexNoise SwampNoise;
        public SimplexNoise DesertNoise;
        public SimplexNoise TemperatureNoise;
        int resolution=1000;
        int layer1 = 300, layer2 = 600, layer3 = 900;

        public class TileForGeneration
        {
            public double fillValue;
            public int x;
            public int y;
            public bool isWater;
        }

        public class TileForInlandWaterConnectivity
        {
            public int x;
            public int y;
            public bool isWater;
            public List<WaterBorderLine> WaterBorderLines { get; set; } = new List<WaterBorderLine>();
        }

        public class WaterBorderLine
        {
            public List<Point> Points { get; set; }
        }

        public List<WaterBorderLine> BorderLines { get; } = new List<WaterBorderLine>();

        public TerrainGenerator(Random random)
        {
            TerrainNoises =  new List<SimplexNoise>();
            // Constants.
            //Const.

            SimplexNoise noise1 = new SimplexNoise( layer1,0.5, random);
            SimplexNoise noise2 = new SimplexNoise( layer2,0.5, random);
            SimplexNoise noise3 = new SimplexNoise( layer3,0.5, random);


            ForestsNoise = new SimplexNoise(200, 0.75, random);
            LakesNoise = new SimplexNoise(200, 0.75, random);
            SwampNoise = new SimplexNoise(200, 0.75, random);
            DesertNoise = new SimplexNoise(200, 0.75, random);

            TemperatureNoise = new SimplexNoise(200, 0.75, random);

            TerrainNoises.Add(noise1);
            TerrainNoises.Add(noise2);
            TerrainNoises.Add(noise3);




            //generate elevation map for river gen
            ElevationMap = new double[resolution][];
            for (int i = 0; i < resolution; i++)
            {
                ElevationMap[i] = new double[resolution];
                for (int j = 0; j < resolution; j++)
                {
                    //fill it with terrain heght with current noises using resolution
                    ElevationMap[i][j] = GetHeightNoise(i,j,1);

                    //if (ElevationMap[i][j] < 0.5)
                    //{
                    //    ElevationMap[i][j] = 1;
                    //}

                }
            }

            //copy elevationArray
            var fillArray = new TileForGeneration[resolution][];
            for (int i = 0; i < resolution; i++)
            {
                fillArray[i] = new TileForGeneration[resolution];
                for (int j = 0; j < resolution; j++)
                {
                    fillArray[i][j] = new TileForGeneration() {fillValue = ElevationMap[i][j], x = i, y = j, isWater = false};
                }
            }


            var landmassWithoutWater =
                fillArray.SelectMany(arr => arr).Select(x => x).Where(tile => !tile.isWater);

            var topHighPlaces = landmassWithoutWater.OrderByDescending(x => x.fillValue).Take(landmassWithoutWater.Count()/10);

            var randomlyTakenHighPlacesStartingPositions = topHighPlaces.OrderBy(x => random.Next()).Take(100).ToList();

            foreach (var randomlyTakenHighPlace in randomlyTakenHighPlacesStartingPositions)
            {
                randomlyTakenHighPlace.isWater = true;
                randomlyTakenHighPlace.fillValue = 1;
            }

            for (int i = 0; i < resolution; i++)
            {

                for (int j = 0; j < resolution; j++)
                {
                    if (fillArray[i][j].fillValue < TileNoiseInterpreter.SeaLevelThreshold)
                    {
                        fillArray[i][j].fillValue = 1;
                    }
                }
            }


            bool[][] initialWater = new bool[resolution][];
            for (int i = 0; i < resolution; i++)
            {
                initialWater[i] = new bool[resolution];
                for (int j = 0; j < resolution; j++)
                {
                    initialWater[i][j] = fillArray[i][j].isWater;
                }
            }

          

            bool changeOccured = true;
            var neighbors = new Queue<TileForGeneration>();
            int counter = 0;
            while (changeOccured)
            {
                changeOccured = false;
                //dont iterate over map borders, its always water anyway
                for (int i = 1; i < resolution-1; i++)
                {
                    for (int j = 1; j < resolution - 1; j++)
                    {
                        var currentCell = fillArray[i][j];
                        if (currentCell.isWater)
                        {
                            neighbors.Clear();
                            GetNeighbours(fillArray, neighbors, i, j);

                            foreach (var neighbor in neighbors)
                            {
                                if (neighbor.isWater)
                                {
                                    neighbor.fillValue = 1;
                                }
                            }

                            var minFillTile = neighbors.OrderBy(x => x.fillValue).First();

                            if (minFillTile.fillValue < fillArray[i][j].fillValue)
                            {
                                currentCell.fillValue = minFillTile.fillValue;
                                fillArray[minFillTile.x][minFillTile.y].isWater = true;
                                fillArray[minFillTile.x][minFillTile.y].fillValue = 1;
                                changeOccured = true;
                            }
                       }
                    }
                }

                counter++;
            }

            RiverMap = new bool[resolution][];
            for (int i = 0; i < resolution; i++)
            {
                RiverMap[i] = new bool[resolution];
                for (int j = 0; j < resolution; j++)
                {
                    RiverMap[i][j] = fillArray[i][j].isWater;
                }
            }

            RiverBorderMap = new bool[resolution][];
            var riverBorderMapCopyForCalcultaion = new bool[resolution][]; 
            for (int i = 0; i < resolution; i++)
            {
                RiverBorderMap[i] = new bool[resolution];
                riverBorderMapCopyForCalcultaion[i] = new bool[resolution];
                for (int j = 0; j < resolution; j++)
                {
                    if (RiverMap[i][j])
                    {
                        bool borderedByAnythingBesidesWater = false;
                        for (int k = i-1; k < i + 2; k++)
                        {
                            for (int l = j-1; l < j+2; l++)
                            {
                                if (!RiverMap[k][l])
                                {
                                    borderedByAnythingBesidesWater = true;
                                }
                            }
                        }

                        if (borderedByAnythingBesidesWater)
                        {
                            RiverBorderMap[i][j] = true;
                            riverBorderMapCopyForCalcultaion[i][j] = true;
                        }
                    }
                }
            }


            List<List<TileForGeneration>> borderLines = new List<List<TileForGeneration>>();

            var startingPoints = randomlyTakenHighPlacesStartingPositions.ToList();

            AnalyzeAndAddLines(startingPoints, borderLines, riverBorderMapCopyForCalcultaion);

            var pointsNotConnectedToStartingPoints = new List<TileForGeneration>();

            for (int i = 0; i < resolution; i++)
            {
                for (int j = 0; j < resolution; j++)
                {
                    if (RiverBorderMap[i][j])
                    {
                        pointsNotConnectedToStartingPoints.Add(fillArray[i][j]);
                    }
                }
            }


            AnalyzeAndAddLines(pointsNotConnectedToStartingPoints, borderLines, riverBorderMapCopyForCalcultaion);

            InlandWaterConnectivity = new TileForInlandWaterConnectivity[resolution][];

            for (int i = 0; i < resolution; i++)
            {
                InlandWaterConnectivity[i] = new TileForInlandWaterConnectivity[resolution]; 
                for (int j = 0; j < resolution; j++)
                {
                    InlandWaterConnectivity[i][j] = new TileForInlandWaterConnectivity()
                    {
                        x=i, y=j, isWater = RiverMap[i][j]
                    };
                }
            }

            foreach (var borderLine in borderLines)
            {
                var line = new WaterBorderLine() {Points = borderLine.Select(p => new Point(p.x, p.y)).ToList()};
                BorderLines.Add(line);
                foreach (var p in line.Points)
                {
                    InlandWaterConnectivity[p.X][p.Y].WaterBorderLines.Add(line);
                }
            }

			ImageWriter.WaterWriteImage(RiverMap, ElevationMap, resolution, "C:\\11\\RiverMap.png", new Color(1, 0, 0, 1f));

			ImageWriter.WaterWriteImage(RiverBorderMap, ElevationMap, resolution, "C:\\11\\RiverBorderMap.png", new Color(1, 0, 0, 1f));

			ImageWriter.RiverBordersWriteImage(borderLines, ElevationMap, 1000, "C:\\11\\riverBordersLines.png");

		}

        private void AnalyzeAndAddLines(List<TileForGeneration> startingPoints, List<List<TileForGeneration>> borderLines, bool[][] riverBorderMapCopyForCalcultaion)
        {
            for (int i = 0; i < startingPoints.Count; i++)
            {
                var start = startingPoints[i];

                var newLine = new List<TileForGeneration>();

                var neighboursForStartingPoint = GetNeighbours(riverBorderMapCopyForCalcultaion, start.x, start.y);


                if (!neighboursForStartingPoint.Any())
                {
                    continue;
                }

                newLine.Add(start);
                riverBorderMapCopyForCalcultaion[start.x][start.y] = false;

                bool lastPointHasNighboors = true;
                while (true)
                {
                    var lastPoint = newLine.Last();
                    var neighboursForLineFormation = new Queue<TileForGeneration>();
                    GetNeighbours(riverBorderMapCopyForCalcultaion, lastPoint.x, lastPoint.y).ForEach(x => neighboursForLineFormation.Enqueue(x));

                    if (!neighboursForLineFormation.Any())
                    {
                        break;
                    }

                    while (neighboursForLineFormation.Any())
                    {
                        var tile = neighboursForLineFormation.Dequeue();
                        riverBorderMapCopyForCalcultaion[tile.x][tile.y] = false;
                        newLine.Add(tile);
                    }
                }

                foreach (var tileForGenerationInLine in newLine)
                {
                    var neighbours = GetNeighbours(riverBorderMapCopyForCalcultaion, tileForGenerationInLine.x, tileForGenerationInLine.y);
                    if (neighbours.Any())
                    {
                        startingPoints.Add(tileForGenerationInLine);
                    }
                }

                borderLines.Add(newLine);
            }
        }

        public void GetNeighbours(TileForGeneration[][] fill, Queue<TileForGeneration> destinationCollection, int x, int y)
        {
            for (int i = x-1; i < x + 2; i++)
            {
                for (int j = y-1; j < y + 2; j++)
                {
                    if (!(x == i && y == j))
                    {
                        var old = fill[i][j];
                        destinationCollection.Enqueue(new TileForGeneration() { fillValue = old.fillValue, x = old.x, y = old.y , isWater = old.isWater});
                    }
                }
            }
        }

        public List<TileForGeneration> GetNeighbours(bool[][] borderMap, int x, int y)
        {
            var result = new List<TileForGeneration>();
            for (int i = x - 1; i < x + 2; i++)
            {
                for (int j = y - 1; j < y + 2; j++)
                {
                    if (!(x == i && y == j))
                    {
                        if (borderMap[i][j])
                        {
                            result.Add(new TileForGeneration(){x = i, y = j});
                        }
                       
                    }
                }
            }

            return result;
        }

        public int Resolution
        {
            get { return resolution; }
            set { resolution = value; }
        }

        public double GetHeightNoise(int x, int y, float scale)
        {
            double dX = (double)x / scale;
            double dY = (double)y / scale;
            int resolutionZoomed = (int)(resolution * scale);
            int borderthickness = resolutionZoomed / 10;

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
            return result;
        }

        public Tile GetTileWithoutRiverWater(int x, int y, float scale)
        {
            double dX = (double)x / scale;
            double dY = (double)y / scale;
            // System.out.print("X ="+dX+"Y =" +dY +"\n");
            int resolutionZoomed = (int) (resolution * scale);
            int borderthickness = resolutionZoomed/10;

            var terrainElevation = GetHeightNoise(x, y, scale);

            if (terrainElevation <= 0.5)
            {
                terrainElevation = 0;
            }

            double forest = 1 - (0.5 * (1 + ForestsNoise.getNoise(dX,dY)));
            double swamp = 1 - (0.5 * (1 + SwampNoise.getNoise(dX, dY)));
           // double lake = 1 - (0.5 * (1 + LakesNoise.getNoise(dX, dY)));
            double desert = 1 - (0.5 * (1 + DesertNoise.getNoise(dX, dY)));

            double temperature = 0.5 - (0.5 * (1 + TemperatureNoise.getNoise(dX,dY)));
            temperature = temperature / 20;
            Tuple<Terrain, Biome> terrinBiome =
                TileNoiseInterpreter.GetTerrain(terrainElevation, forest, swamp, desert, temperature, resolutionZoomed, x, y);
            return new Tile(terrinBiome.Item1, terrinBiome.Item2, new Point(x,y), terrainElevation);
        }
    }
}

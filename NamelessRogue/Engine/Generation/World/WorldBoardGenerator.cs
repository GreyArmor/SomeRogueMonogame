using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Markov;
using RogueSharp.Random;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.Generation.Noise;
using NamelessRogue.Engine.Generation.World.Meta;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Utility;
using NamelessRogue.shell;
using VoronoiLib;
using VoronoiLib.Structures;
using Color = NamelessRogue.Engine.Utility.Color;
using NamelessRogue.Engine.Generation.World.Denizens;
using MonoGame.Extended;
using SharpDX.Direct3D9;
using AStarNavigator.Algorithms;
using AStarNavigator.Providers;
using AStarNavigator;
using static NamelessRogue.Engine.Components.AI.NonPlayerCharacter.AStarPathfinderSimple;
using System.Reflection.Metadata;
using NamelessRogue.Engine.Generation.World.TerrainFeatures;

namespace NamelessRogue.Engine.Generation.World
{
    public class WorldBoardGenerator
    {
        static Markov.MarkovChain<char> continentNamesChain = new MarkovChain<char>(2);

        static WorldBoardGenerator()
        {
            string continentNames =
                "Africa Europa Asia America Australia Antarctica Atlantis? Lemuria? Hyboria Westeros Mu Middleearth Essos Assail Genabackis Jacuruku Oceania Khorvaire Valinor Neverland Ingas Mist Faerun Everice Crimea Absolom Alcatraz Atuan Banoi Britannula Caereon Caspiar Crusoeland Carlotta Dargenk Dressrosa Esme Eureka Ember Felimath Fraxos Flyspeck Gaea Ganae Galuga Gristol Havnor Hoenn Iwako Kokovoko Kiloran Kalimdor Lea Monde Maple Myst Navarone Nibelia Numenor Okishima Orange Pharmaul Paradise Skira Skypiea Summerisle Tolaria Innistrad Dominaria Unova Utopia Vanutu Warbler Yew Yin Zandia Zolon Zou";
            foreach (var s in continentNames.Split(' '))
            {
                continentNamesChain.Add(s);
            }


        }
        public static class GenerationUtility
        {
            public static void AnalyzeAndAddLines(List<TileForGeneration> startingPoints, bool[][] riverBorderMapCopyForCalcultaion, out List<List<TileForGeneration>> borderLines)
            {
                borderLines = new List<List<TileForGeneration>>();
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

            public static void GetNeighbours(TileForGeneration[][] fill, Queue<TileForGeneration> destinationCollection, int x, int y, int searchRange)
            {
                for (int i = x - searchRange; i < x + searchRange + 1; i++)
                {
                    for (int j = y - searchRange; j < y + searchRange + 1; j++)
                    {
                        if (!(x == i && y == j))
                        {
                            var old = fill[i][j];
                            destinationCollection.Enqueue(new TileForGeneration() { fillValue = old.fillValue, x = old.x, y = old.y, isWater = old.isWater });
                        }
                    }
                }
            }

            public static List<TileForGeneration> GetNeighbours(bool[][] borderMap, int x, int y)
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
                                result.Add(new TileForGeneration() { x = i, y = j });
                            }

                        }
                    }
                }

                return result;
            }
        }


        public static void PopulateWithInitialData(WorldBoard board, NamelessGame game)
        {
            var resolution = WorldGenConstants.Resolution;

            var random = new InternalRandom(game.WorldSettings.GlobalRandom.Next());

            InitBoardWithoutFeatures(board, game, resolution);

            //generate elevation map for river gen
            for (int i = 0; i < resolution; i++)
            {
                for (int j = 0; j < resolution; j++)
                {
                    //fill it with terrain heght with current noises using resolution
                    board.ElevationMap[i][j] = game.WorldSettings.TerrainGen.GetHeightNoise(i, j, 1);
                }
            }

            //copy elevationArray
            var fillArray = new TileForGeneration[resolution][];
            for (int i = 0; i < resolution; i++)
            {
                fillArray[i] = new TileForGeneration[resolution];
                for (int j = 0; j < resolution; j++)
                {
                    fillArray[i][j] = new TileForGeneration() { fillValue = board.ElevationMap[i][j], x = i, y = j, isWater = false };

                }
            }





            //board.CityParts = new List<CityPart>();

            var testCityPart = new CityPart();

            testCityPart.Center = new Microsoft.Xna.Framework.Point(200, 200);

            

            //generate random roads here

          

            for (int i = 0; i < 10; i++)
            {
                if (i % 2 == 0)
                {
                    Vector2 startV = new Vector2((float)i, 0);
                    Vector2 endV = new Vector2((float)i, 10);
                    CreateRoad(board, testCityPart.Center.ToVector2(), startV, endV);
                }
            }

            for (int i = 0; i < 10; i++)
            {
                if (i % 2 == 0)
                {
                    Vector2 startV = new Vector2(0, i);
                    Vector2 endV = new Vector2(10, i);
                    CreateRoad(board, testCityPart.Center.ToVector2(), startV, endV);
                }
            }

            //  board.CityParts.Add(testCityPart);

            OldRiverGeneration(board, game, resolution, random, fillArray);
        }

        private static void CreateRoad(WorldBoard board, Vector2 center, Vector2 startV, Vector2 endV)
        {

            var navigator = new TileNavigator(
             new EmptyBlockedProvider(),         
             new DiagonalNeighborProvider(),    
             new PythagorasAlgorithm(),          
             new ManhattanHeuristicAlgorithm()   
             );

            Waypoints road = new Waypoints();

            var start = new AStarNavigator.Tile(startV.X, startV.Y);
            var end = new AStarNavigator.Tile(endV.X, endV.Y);

            var result = navigator.Navigate(start, end);

            foreach (var point in result)
            {
                road.Points.Add(new Vector2((float)point.X, (float)point.Y));
            }

            foreach (var point in road.Points.ToList())
            {
                for (int x = (int)(point.X - 2); x <= point.X + 2; x++)
                {
                    for (int y = (int)(point.Y - 2); y <= point.Y + 2; y++)
                    {
                        road.Points.Add(new Vector2(x, y));
                    }
                }
            }

            var transformedRoad = new Waypoints();
            foreach (var point in road.Points)
            {
                transformedRoad.Points.Add(point + center);
            }
            board.Roads.Add(transformedRoad);

            var roadTF = new RoadTF(new Vector2((float)start.X, (float)start.Y) + center, new Vector2((float)end.X, (float)end.Y) + center);

            foreach (var p in transformedRoad.Points)
            {
                var chunkPoint = new Microsoft.Xna.Framework.Point((int)p.X, (int)p.Y);
                board.Chunks.GetChunks()[chunkPoint].TerrainFeatures.Add(roadTF);
                board.TerrainFeatures[(int)Math.Round(p.X, 0)][(int)(int)Math.Round(p.Y, 0)].Roads.Add(transformedRoad);
            }
        }

       
        private static void InitBoardWithoutFeatures(WorldBoard board, NamelessGame game, int resolution)
        {
            for (int x = 0; x < game.WorldSettings.WorldBoardWidth; x++)
            {
                for (int y = 0; y < game.WorldSettings.WorldBoardHeight; y++)
                {
                    var worldTile = new WorldTile(new Microsoft.Xna.Framework.Point(x, y));
                    var tile = game.WorldSettings.TerrainGen.GetTileWithoutTerrainFeatures(x, y, (float)game.WorldSettings.WorldBoardWidth / resolution);
                    worldTile.Terrain = tile.Terrain;
                    worldTile.Biome = tile.Biome;
                    board.WorldTiles[x, y] = worldTile;
                }
            }

            for (int i = 0; i < resolution; i++)
            {
                for (int j = 0; j < resolution; j++)
                {
                    board.TerrainFeatures[i][j] = new TileForPainting()
                    {
                        x = i,
                        y = j,
                    };
                }
            }
        }

        private static void OldRiverGeneration(WorldBoard board, NamelessGame game, int resolution, InternalRandom random, TileForGeneration[][] fillArray)
        {
            List<FortuneSite> points = new List<FortuneSite>();
            LinkedList<VEdge> edges = new LinkedList<VEdge>();

            for (var i = 0; i < resolution; i++)
            {
                points.Add(new FortuneSite(
                    random.Next(0, resolution - 1),
                    random.Next(0, resolution - 1)));
            }

            //uniq the points
            points.Sort((p1, p2) =>
            {
                if (p1.X.ApproxEqual(p2.X))
                {
                    if (p1.Y.ApproxEqual(p2.Y))
                        return 0;
                    if (p1.Y < p2.Y)
                        return -1;
                    return 1;
                }
                if (p1.X < p2.X)
                    return -1;
                return 1;
            });

            var unique = new List<FortuneSite>(points.Count / 2);
            var last = points.First();
            unique.Add(last);
            for (var index = 1; index < points.Count; index++)
            {
                var point = points[index];
                if (!last.X.ApproxEqual(point.X) ||
                    !last.Y.ApproxEqual(point.Y))
                {
                    unique.Add(point);
                    last = point;
                }
            }
            points = unique;

            edges = FortunesAlgorithm.Run(points, 0, 0, resolution - 1, resolution - 1);

            //VEdge.Start is a VPoint with location VEdge.Start.X and VEdge.End.Y
            //VEdge.End is the ending point for the edge
            //FortuneSite.Neighbors contains the site's neighbors in the Delaunay Triangulation


            var waterBitmap = new Bitmap(resolution, resolution);

            var graphics = Graphics.FromImage(waterBitmap);

            Pen whitePen = new Pen(System.Drawing.Color.White, 1);

            var edgesByTheSea = edges.Where(
                x =>
                    board.ElevationMap[(int)x.Start.X][(int)x.Start.Y] < TileNoiseInterpreter.SeaLevelThreshold &&
                    board.ElevationMap[(int)x.End.X][(int)x.End.Y] >= TileNoiseInterpreter.SeaLevelThreshold ||

                    board.ElevationMap[(int)x.End.X][(int)x.End.Y] < TileNoiseInterpreter.SeaLevelThreshold &&
                    board.ElevationMap[(int)x.Start.X][(int)x.Start.Y] >= TileNoiseInterpreter.SeaLevelThreshold
                );

            var allInlandEdges = edges.Where(
                x =>
                    board.ElevationMap[(int)x.End.X][(int)x.End.Y] >= TileNoiseInterpreter.SeaLevelThreshold &&
                    board.ElevationMap[(int)x.Start.X][(int)x.Start.Y] >= TileNoiseInterpreter.SeaLevelThreshold
                ).ToList();

            //randommly remove some rivers;
            var cullingChance = 15;
            var culledEdges = allInlandEdges.Where(edge => random.Next(1, 101) > cullingChance).ToList();
            culledEdges.AddRange(edgesByTheSea.Where(edge => random.Next(1, 101) > cullingChance));

            //remove edges collapsed to one point
            culledEdges = culledEdges.Where(x => x.Start.X != x.End.X || x.Start.Y != x.End.Y).ToList();

            var finalEdges = new List<VEdge>();
            finalEdges.AddRange(culledEdges);

            foreach (var edge in finalEdges)
            {

                var pointCountForLerpAndRandomization = 7;

                var listVectorEdgePoints = new List<Vector2>();
                var startVector = new Vector2((float)edge.Start.X, (float)edge.Start.Y);
                var endVector = new Vector2((float)edge.End.X, (float)edge.End.Y);
                var perpendicular = (endVector - startVector);
                perpendicular.Normalize();
                perpendicular = new Vector2(perpendicular.Y, -perpendicular.X);



                listVectorEdgePoints.Add(startVector);

                var riverWiggle = 1;

                for (int i = 1; i < pointCountForLerpAndRandomization - 1; i++)
                {
                    var newPoint = Vector2.Lerp(startVector, endVector, ((float)i / (pointCountForLerpAndRandomization - 1))) + (perpendicular * (random.Next(-riverWiggle, riverWiggle)));
                    listVectorEdgePoints.Add(newPoint);
                }
                listVectorEdgePoints.Add(endVector);

                graphics.DrawCurve(whitePen, listVectorEdgePoints.Select(x => x.ToPoint().ToPoint()).ToArray());
            }

            for (int x = 3; x < resolution - 3; x++)
            {
                for (int y = 3; y < resolution - 3; y++)
                {
                    if (waterBitmap.GetPixel(x, y).R > 0)
                    {
                        //fillArray[x][y].isWater = true;

                        if (board.ElevationMap[x][y] >= TileNoiseInterpreter.SeaLevelThreshold)
                        {
                            fillArray[x][y].isWater = true;
                        }
                        else
                        {
                            Queue<TileForGeneration> neighbours = new Queue<TileForGeneration>();
                            GenerationUtility.GetNeighbours(fillArray, neighbours, x, y, 2);
                            if (neighbours.Any(n => board.ElevationMap[n.x][n.y] >= TileNoiseInterpreter.SeaLevelThreshold))
                            {
                                fillArray[x][y].isWater = true;
                            }
                        }

                    }
                }
            }


            for (int i = 0; i < resolution; i++)
            {
                for (int j = 0; j < resolution; j++)
                {
                    board.RiverMap[i][j] = fillArray[i][j].isWater;
                }
            }

            var riverBorderMapCopyForCalcultaion = new bool[resolution][];
            for (int i = 0; i < resolution; i++)
            {
                riverBorderMapCopyForCalcultaion[i] = new bool[resolution];
                for (int j = 0; j < resolution; j++)
                {
                    if (board.RiverMap[i][j])
                    {
                        bool borderedByAnythingBesidesWater = false;
                        //not really a radius, more like an side lenght of a square
                        var searchRadius = 3;
                        if (i > searchRadius && i < resolution - searchRadius && j > searchRadius && j < resolution - searchRadius)
                        {
                            for (int k = i - searchRadius; k < i + searchRadius + 1; k++)
                            {
                                for (int l = j - searchRadius; l < j + searchRadius + 1; l++)
                                {
                                    if (!board.RiverMap[k][l])
                                    {
                                        borderedByAnythingBesidesWater = true;
                                    }
                                }
                            }
                        }

                        if (borderedByAnythingBesidesWater)
                        {
                            board.RiverBorderMap[i][j] = true;
                            riverBorderMapCopyForCalcultaion[i][j] = true;
                        }
                    }
                }
            }


            var pointsNotConnectedToStartingPoints = new List<TileForGeneration>();

            for (int i = 0; i < resolution; i++)
            {
                for (int j = 0; j < resolution; j++)
                {
                    if (board.RiverBorderMap[i][j])
                    {
                        pointsNotConnectedToStartingPoints.Add(fillArray[i][j]);
                    }
                }
            }

            GenerationUtility.AnalyzeAndAddLines(pointsNotConnectedToStartingPoints, riverBorderMapCopyForCalcultaion, out var borderLines);

            for (int i = 0; i < resolution; i++)
            {
                for (int j = 0; j < resolution; j++)
                {
                    board.TerrainFeatures[i][j].isWater = board.RiverMap[i][j];
                }
            }

            foreach (var borderLine in borderLines)
            {
                var line = new Waypoints() { Points = borderLine.Select(p => new Vector2(p.x, p.y)).ToList() };
                board.RiverBorderLines.Add(line);
                foreach (var p in line.Points)
                {
                    board.TerrainFeatures[(int)p.X][(int)p.Y].WaterBorderLines.Add(line);
                }
            }
#if false
			ImageWriter.WaterWriteImage(board.RiverMap, board.ElevationMap, resolution, "C:\\11\\RiverMap.png", new Color(1, 0, 0, 1f));

			ImageWriter.WaterWriteImage(board.RiverBorderMap, board.ElevationMap, resolution, "C:\\11\\RiverBorderMap.png", new Color(1, 0, 0, 1f));

			ImageWriter.RiverBordersWriteImage(borderLines, board.ElevationMap, resolution, "C:\\11\\riverBordersLines.png");
#endif

            for (int x = 0; x < game.WorldSettings.WorldBoardWidth; x++)
            {
                for (int y = 0; y < game.WorldSettings.WorldBoardHeight; y++)
                {
                    var worldTile = board.WorldTiles[x, y];

                    if (board.RiverMap[x][y] && worldTile.Terrain != TerrainTypes.Water)
                    {
                        worldTile.Terrain = TerrainTypes.Water;
                        worldTile.Biome = Biomes.River;
                    }

                }
            }
        }

        public class WorldGenerationParameters
        {
            public string StrSeed { get; set; }
            int Seed { get; set; }
            public Vector2 Size { get; set; }
            public int SimulationSizeInYears { get; set; }

            //if empty then generate the name procedurally
            string Name { get; set; } = "";
            List<RaceId> RacesOfDenizens { get; set; }
            public WorldGenerationParameters() { }
        }
    }

}

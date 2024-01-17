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
using Veldrid;
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
using System.Numerics;
using Point = Veldrid.Point;

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
				for (int i = x - searchRange; i < x + searchRange+1; i++)
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


			for (int x = 0; x < game.WorldSettings.WorldBoardWidth; x++)
			{
				for (int y = 0; y < game.WorldSettings.WorldBoardHeight; y++)
				{
					var worldTile = new WorldTile(new Point(x, y));
					var tile = game.WorldSettings.TerrainGen.GetTileWithoutRiverWater(x, y, (float)game.WorldSettings.WorldBoardWidth / resolution);
					worldTile.Terrain = tile.Terrain;
					worldTile.Biome = tile.Biome;

					board.WorldTiles[x, y] = worldTile;
				}
			}


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

			//remove desert rivers with high probability
			var cullingDesertChance = 90;

			//if not in desert biome ignore, if in desert, cull with high probability
			culledEdges = culledEdges.Where(edge => 
			(board.WorldTiles[(int)edge.Start.X, (int)edge.Start.Y].Biome.HasFlag(Biomes.Desert|Biomes.Jungle|Biomes.Savannah) && 
			board.WorldTiles[(int)edge.End.X, (int)edge.End.Y].Biome.HasFlag(Biomes.Desert | Biomes.Jungle | Biomes.Savannah)) 
			|| random.Next(1, 101) < cullingDesertChance).ToList();

			var finalEdges = new List<VEdge>();
			finalEdges.AddRange(culledEdges);

			foreach (var edge in finalEdges)
			{

				var pointCountForLerpAndRandomization = 7;

				var listVectorEdgePoints = new List<Vector2>();
				var startVector = new Vector2((float)edge.Start.X, (float)edge.Start.Y);
				var endVector = new Vector2((float)edge.End.X, (float)edge.End.Y);
				var perpendicular = (endVector - startVector);
				Vector2.Normalize(perpendicular);
				perpendicular = new Vector2(perpendicular.Y, -perpendicular.X);



				listVectorEdgePoints.Add(startVector);

				var riverWiggle = 3;

				for (int i = 1; i < pointCountForLerpAndRandomization - 1; i++)
				{
					var newPoint = Vector2.Lerp(startVector, endVector, ((float)i / (pointCountForLerpAndRandomization - 1))) + (perpendicular * (random.Next(-riverWiggle, riverWiggle)));
					listVectorEdgePoints.Add(newPoint);
				}
				listVectorEdgePoints.Add(endVector);

				graphics.DrawCurve(whitePen, listVectorEdgePoints.Select(x => x.ToPoint().ToPoint()).ToArray());
			}

			for (int x = 3; x < resolution-3; x++)
			{
				for (int y = 3; y < resolution-3; y++)
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
							for (int k = i - searchRadius; k < i + searchRadius+1; k++)
							{
								for (int l = j - searchRadius; l < j + searchRadius+1; l++)
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
					board.InlandWaterConnectivity[i][j] = new TileForInlandWaterConnectivity()
					{
						x = i,
						y = j,
						isWater = board.RiverMap[i][j]
					};
				}
			}

			foreach (var borderLine in borderLines)
			{
				var line = new WaterBorderLine() { Points = borderLine.Select(p => new Point(p.x, p.y)).ToList() };
				board.BorderLines.Add(line);
				foreach (var p in line.Points)
				{
					board.InlandWaterConnectivity[p.X][p.Y].WaterBorderLines.Add(line);
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
		public static void PlaceInitialCivilizations(WorldBoard timelineLayer, NamelessGame game)
        {
            var worldSettingsContinentTilesPerCivilization = game.WorldSettings.ContinentTilesPerCivilization;

            foreach (var worldBoardContinent in timelineLayer.Continents)
            {
                var civNumber = worldBoardContinent.SizeInTiles / worldSettingsContinentTilesPerCivilization;

                var continentTiles = new Queue<WorldTile>();

                foreach (var worldBoardWorldTile in timelineLayer.WorldTiles)
                {
                    if (worldBoardWorldTile.Continent == worldBoardContinent && worldBoardWorldTile.Terrain!=TerrainTypes.Water && worldBoardWorldTile.Owner == null)
                    {
                        continentTiles.Enqueue(worldBoardWorldTile);
                    }
                }
                //randomize list
                continentTiles = new Queue<WorldTile>(continentTiles.OrderBy(
                    (o) =>
                {
                    return (game.WorldSettings.GlobalRandom.Next() % continentTiles.Count);
                }));

				InternalRandom random = new InternalRandom(game.WorldSettings.GlobalRandom.Next(int.MaxValue - 1));

                for (int i = 0; i < civNumber; i++)
                {
                    bool civNameUnique = false;
                    string civName = "";
                    while (!civNameUnique)
                    {
                        civName = game.WorldSettings.NamesGenerator.GetCountryName(random).FirstCharToUpper();
                        civNameUnique = timelineLayer.Civilizations.All(x => x.Name != civName);
                    }

                    CultureTemplate culture = game.WorldSettings.CultureTemplates[
                        random.Next(game.WorldSettings.CultureTemplates.Count-1)];

                    var civilization = new Civilization(civName, new Color((float)random.Next(0,100)/100,
								(float)random.Next(0, 100) / 100,
								(float)random.Next(0, 100) / 100, 1f),
                        culture);
                    timelineLayer.Civilizations.Add(civilization);


                    var firstSettlement = new Settlement()
                    {
                            Name = civilization.CultureTemplate.GetTownName(random)
                    };

                    civilization.Settlements.Add(firstSettlement);



                    WorldTile tile = null;
                    bool noNeighbooringCivs = false;
                    const int squareToCheck = 10;
                    const int radiusToClaim = 7;
                    while (!noNeighbooringCivs)
                    {

                        if (!continentTiles.Any())
                        {
                            break;
                        }
                        
                        tile = continentTiles.Dequeue();
                        noNeighbooringCivs = true;
                        for (int x = tile.WorldBoardPosiiton.X - squareToCheck; x <= tile.WorldBoardPosiiton.X + squareToCheck; x++)
                        {
                            for (int y = tile.WorldBoardPosiiton.Y - squareToCheck; y <= tile.WorldBoardPosiiton.Y + squareToCheck; y++)
                            {
                                if (timelineLayer.WorldTiles[x, y].Owner != null)
                                {
                                    noNeighbooringCivs = false;
                                }
                            }
                        }
                    }


                    //if there is no tiles to place civilizations then break the loop
                    if (!continentTiles.Any())
                    {
                        break;
                    }


                    timelineLayer.Civilizations.Add(civilization);

                    tile.Settlement = firstSettlement;




                    for (int x = tile.WorldBoardPosiiton.X - radiusToClaim; x <= tile.WorldBoardPosiiton.X + radiusToClaim; x++)
                    {
                        for (int y = tile.WorldBoardPosiiton.Y - radiusToClaim; y <= tile.WorldBoardPosiiton.Y + radiusToClaim; y++)
                        {

                            var dX = x - tile.WorldBoardPosiiton.X;
                            var dY = y - tile.WorldBoardPosiiton.Y;

                            if (dX * dX + dY * dY <= (radiusToClaim))
                            {
                                timelineLayer.WorldTiles[x, y].Owner = civilization;
                            }
							
                        }
                    }
                }

               // var names = timelineLayer.Civilizations.Select(x => x.Name).Aggregate((s1, s2) => s1 + ", " + s2);
            }

        }

        public static void AnalizeLandmasses(WorldBoard timelineLayer, NamelessGame game)
        {
            var s = Stopwatch.StartNew();
            InternalRandom rand = game.WorldSettings.GlobalRandom;
            List<Region> regions =  GetRegions(timelineLayer, game, rand, (tile => tile.Biome != Biomes.Sea &&
                                                        tile.Continent == null), (tile, region) => tile.Continent = region,() => { return new string(continentNamesChain.Chain(game.WorldSettings.GlobalRandom.Next()).ToArray()).FirstCharToUpper(); });

            List<Region> forests = GetRegions(timelineLayer, game, rand, (tile => tile.Biome == Biomes.Forest && tile.LandmarkRegion==null), (tile, region) => tile.LandmarkRegion = region, () => { return new string(continentNamesChain.Chain(game.WorldSettings.GlobalRandom.Next(int.MaxValue - 1)).ToArray()).FirstCharToUpper() + " forest"; });
            List<Region> deserts = GetRegions(timelineLayer, game, rand, (tile => tile.Biome == Biomes.Desert && tile.LandmarkRegion == null), (tile, region) => tile.LandmarkRegion = region, () => { return new string(continentNamesChain.Chain(game.WorldSettings.GlobalRandom.Next(int.MaxValue - 1)).ToArray()).FirstCharToUpper() + " desert"; });
            List<Region> mountains = GetRegions(timelineLayer, game, rand, (tile => tile.Biome == Biomes.Mountain && tile.LandmarkRegion == null), (tile, region) => tile.LandmarkRegion = region, () => { return new string(continentNamesChain.Chain(game.WorldSettings.GlobalRandom.Next(int.MaxValue - 1)).ToArray()).FirstCharToUpper() + " mountain"; });
            List<Region> swamps = GetRegions(timelineLayer, game, rand, (tile => tile.Biome == Biomes.Swamp && tile.LandmarkRegion == null), (tile, region) => tile.LandmarkRegion = region, () => { return new string(continentNamesChain.Chain(game.WorldSettings.GlobalRandom.Next(int.MaxValue - 1)).ToArray()).FirstCharToUpper() + " swamp"; });
            s.Stop();
            timelineLayer.Continents = regions.Where(x => x.SizeInTiles >= 2000).ToList();
            timelineLayer.Islands = regions.Where(x => x.SizeInTiles < 2000).ToList();
            timelineLayer.Forests = forests.ToList();
            timelineLayer.Deserts = deserts.ToList();
            timelineLayer.Mountains = mountains.ToList();
            timelineLayer.Swamps = swamps.ToList();
        }

        private static List<Region> GetRegions(WorldBoard timelineLayer, NamelessGame game, InternalRandom rand, Func<WorldTile,bool> searchCriterion, Action<WorldTile,Region> onFoundRegion, Func<string> nameGenerator)
        {
            List<Region> regions = new List<Region>();
            var searchPoint = new Point();

            for (; searchPoint.X < game.WorldSettings.WorldBoardWidth; searchPoint.X++)
            {
                for (searchPoint.Y = 0; searchPoint.Y < game.WorldSettings.WorldBoardHeight; searchPoint.Y++)
                {
                    if (searchCriterion(timelineLayer.WorldTiles[searchPoint.X, searchPoint.Y]))
                    {
                        Region r = new Region();
                        r.Name = nameGenerator();
                        r.Color = new Color(rand.Next(0,255), rand.Next(0, 255), rand.Next(0, 255), 1f);
                        regions.Add(r);

                        var firstNode = timelineLayer.WorldTiles[searchPoint.X, searchPoint.Y];

                        var floodList = new Queue<WorldTile>();
                        floodList.Enqueue(firstNode);
                        onFoundRegion(firstNode, r);            
                        r.SizeInTiles++;

                        void AddToFloodList(int x, int y, Region region)
                        {
                            if (searchCriterion(timelineLayer.WorldTiles[x, y]))
                            {
                                floodList.Enqueue(timelineLayer.WorldTiles[x, y]);
                                onFoundRegion(timelineLayer.WorldTiles[x, y],region);
                                region.SizeInTiles++;
                            }
                        }

                        while (floodList.Any())
                        {
                            var elem = floodList.Dequeue();
                            
                            AddToFloodList(elem.WorldBoardPosiiton.X - 1, elem.WorldBoardPosiiton.Y, r);
                            AddToFloodList(elem.WorldBoardPosiiton.X + 1, elem.WorldBoardPosiiton.Y, r);
                            AddToFloodList(elem.WorldBoardPosiiton.X, elem.WorldBoardPosiiton.Y - 1, r);
                            AddToFloodList(elem.WorldBoardPosiiton.X, elem.WorldBoardPosiiton.Y + 1, r);

                            AddToFloodList(elem.WorldBoardPosiiton.X - 1, elem.WorldBoardPosiiton.Y - 1, r);
                            AddToFloodList(elem.WorldBoardPosiiton.X + 1, elem.WorldBoardPosiiton.Y + 1, r);
                            AddToFloodList(elem.WorldBoardPosiiton.X + 1, elem.WorldBoardPosiiton.Y - 1, r);
                            AddToFloodList(elem.WorldBoardPosiiton.X - 1, elem.WorldBoardPosiiton.Y + 1, r);

                        }
                    }
                }
            }

            return regions;
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

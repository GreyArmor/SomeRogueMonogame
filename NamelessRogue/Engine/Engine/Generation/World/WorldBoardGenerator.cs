using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Engine.Generation.Noise;
using NamelessRogue.Engine.Engine.Generation.World.Meta;
using NamelessRogue.Engine.Engine.Infrastructure;
using NamelessRogue.Engine.Engine.Utility;
using NamelessRogue.shell;
using Color = NamelessRogue.Engine.Engine.Utility.Color;

namespace NamelessRogue.Engine.Engine.Generation.World
{
    public class WorldBoardGenerator
    {
        public static void PopulateWithInitialData(WorldBoard board, NamelessGame game)
        {
            for (int x = 0; x < game.WorldSettings.WorldBoardWidth; x++)
            {
                for (int y = 0; y < game.WorldSettings.WorldBoardHeight; y++)
                {
                    var worldTile = new WorldTile(new Point(x,y));
                    var tile = game.WorldSettings.TerrainGen.GetTile(x, y, 1);
                    worldTile.Terrain = tile.getTerrainType();
                    worldTile.Biome = tile.Biome;
                    board.WorldTiles[x, y] = worldTile;
                }
            }
        }

        public static void PlaceResources(WorldBoard worldBoard, NamelessGame game)
        {
            
        }

        public static void PlaceInitialArtifacts(WorldBoard worldBoard, NamelessGame game)
        {

        }

        public static void DistributeMetaphysics(WorldBoard worldBoard, NamelessGame game)
        {
        }

        public static void PlaceInitialCivilizations(WorldBoard worldBoard, NamelessGame game)
        {
            var worldSettingsContinentTilesPerCivilization = game.WorldSettings.ContinentTilesPerCivilization;

            foreach (var worldBoardContinent in worldBoard.Continents)
            {
                var civNumber = worldBoardContinent.SizeInTiles / worldSettingsContinentTilesPerCivilization;

                var continentTiles = new Queue<WorldTile>();

                foreach (var worldBoardWorldTile in worldBoard.WorldTiles)
                {
                    if (worldBoardWorldTile.Continent == worldBoardContinent && worldBoardWorldTile.Owner == null)
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

                var random = game.WorldSettings.GlobalRandom;

                for (int i = 0; i < civNumber; i++)
                {
                    bool civNameUnique = false;
                    string civName = "";
                    while (!civNameUnique)
                    {
                        civName = game.WorldSettings.NamesGenerator.GetCountryName(random).FirstCharToUpper();
                        civNameUnique = worldBoard.Civilizations.All(x => x.Name != civName);
                    }

                    CultureTemplate culture = game.WorldSettings.CultureTemplates[
                        game.WorldSettings.GlobalRandom.Next(game.WorldSettings.CultureTemplates.Count)];

                    var civilization = new Civilization(civName, new Microsoft.Xna.Framework.Color(
                            new Vector4((float) game.WorldSettings.GlobalRandom.NextDouble(),
                                (float) game.WorldSettings.GlobalRandom.NextDouble(),
                                (float) game.WorldSettings.GlobalRandom.NextDouble(), 1)),
                        culture);
                    worldBoard.Civilizations.Add(civilization);


                    var firstSettlement = new Settlement()
                    {
                        Info = new ObjectInfo()
                        {
                            Name = civilization.CultureTemplate.GetTownName(random)
                        }
                    };

                    civilization.Settlements.Add(firstSettlement);



                    WorldTile tile = null;
                    bool noNeighbooringCivs = false;
                    while (!noNeighbooringCivs)
                    {

                        tile = continentTiles.Dequeue();
                        noNeighbooringCivs = true;
                        for (int x = tile.WorldBoardPosiiton.X - 1; x <= tile.WorldBoardPosiiton.X + 1; x++)
                        {
                            for (int y = tile.WorldBoardPosiiton.Y - 1; y <= tile.WorldBoardPosiiton.Y + 1; y++)
                            {
                                if (worldBoard.WorldTiles[x, y].Owner != null)
                                {
                                    noNeighbooringCivs = false;
                                }
                            }
                        }
                    }


                    tile.Building = firstSettlement;




                    for (int x = tile.WorldBoardPosiiton.X - 1; x <= tile.WorldBoardPosiiton.X + 1; x++)
                    {
                        for (int y = tile.WorldBoardPosiiton.Y - 1; y <= tile.WorldBoardPosiiton.Y + 1; y++)
                        {
                            worldBoard.WorldTiles[x, y].Owner = civilization;
                        }
                    }
                }

               // var names = worldBoard.Civilizations.Select(x => x.Name).Aggregate((s1, s2) => s1 + ", " + s2);
            }

        }

        public static void AnalizeLandmasses(WorldBoard worldBoard, NamelessGame game)
        {
            var s = Stopwatch.StartNew();
            Random rand = game.WorldSettings.GlobalRandom;
            List<Region> regions =  GetRegions(worldBoard, game, rand, (tile => tile.Terrain != TerrainTypes.Water &&
                                                        tile.Continent == null), (tile, region) => tile.Continent = region);

            List<Region> forests = GetRegions(worldBoard, game, rand, (tile => tile.Biome == Biomes.Forest && tile.LandmarkRegion==null), (tile, region) => tile.LandmarkRegion = region);
            List<Region> deserts = GetRegions(worldBoard, game, rand, (tile => tile.Biome == Biomes.Desert && tile.LandmarkRegion == null), (tile, region) => tile.LandmarkRegion = region);
            List<Region> mountains = GetRegions(worldBoard, game, rand, (tile => tile.Biome == Biomes.Mountain && tile.LandmarkRegion == null), (tile, region) => tile.LandmarkRegion = region);
            List<Region> swamps = GetRegions(worldBoard, game, rand, (tile => tile.Biome == Biomes.Swamp && tile.LandmarkRegion == null), (tile, region) => tile.LandmarkRegion = region);
            s.Stop();
            worldBoard.Continents = regions.Where(x => x.SizeInTiles >= 2000).ToList();
            worldBoard.Islands = regions.Where(x => x.SizeInTiles < 2000).ToList();
            worldBoard.Forests = forests.ToList();
            worldBoard.Deserts = deserts.ToList();
            worldBoard.Mountains = mountains.ToList();
            worldBoard.Swamps = swamps.ToList();
        }

        private static List<Region> GetRegions(WorldBoard worldBoard, NamelessGame game, Random rand, Func<WorldTile,bool> searchCriterion, Action<WorldTile,Region> onFoundRegion)
        {
            List<Region> regions = new List<Region>();
            var searchPoint = new Point();

            for (; searchPoint.X < game.WorldSettings.WorldBoardWidth; searchPoint.X++)
            {
                for (searchPoint.Y = 0; searchPoint.Y < game.WorldSettings.WorldBoardHeight; searchPoint.Y++)
                {
                    if (searchCriterion(worldBoard.WorldTiles[searchPoint.X, searchPoint.Y]))
                    {
                        Region r = new Region();
                        r.Color = new Color(rand.NextDouble(), rand.NextDouble(), rand.NextDouble(), 1f);
                        regions.Add(r);

                        var firstNode = worldBoard.WorldTiles[searchPoint.X, searchPoint.Y];

                        var floodList = new Queue<WorldTile>();
                        floodList.Enqueue(firstNode);
                        onFoundRegion(firstNode, r);            
                        r.SizeInTiles++;

                        void AddToFloodList(int x, int y, Region region)
                        {
                            if (searchCriterion(worldBoard.WorldTiles[x, y]))
                            {
                                floodList.Enqueue(worldBoard.WorldTiles[x, y]);
                                onFoundRegion(worldBoard.WorldTiles[x, y],region);
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
}

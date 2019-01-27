using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Markov;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Engine.Factories;
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
            var continentTilesPerResource = game.WorldSettings.ContinentTilesPerResource;

            foreach (var worldBoardContinent in worldBoard.Continents)
            {
                var continentTiles = new Queue<WorldTile>();
                var resNumber = worldBoardContinent.SizeInTiles / continentTilesPerResource;
                foreach (var worldBoardWorldTile in worldBoard.WorldTiles)
                {
                    if (worldBoardWorldTile.Continent == worldBoardContinent && worldBoardWorldTile.Owner == null)
                    {
                        continentTiles.Enqueue(worldBoardWorldTile);
                    }
                }

                //randomize list
                continentTiles = new Queue<WorldTile>(continentTiles.OrderBy(
                    (o) => { return (game.WorldSettings.GlobalRandom.Next() % continentTiles.Count); }));

                var random = game.WorldSettings.GlobalRandom;


                for (int i = 0; i < resNumber; i++)
                {
                    WorldTile tile = null;
                    bool noNeighbooringResources = false;
                    const int squareToCheck = 3;
                    while (!noNeighbooringResources)
                    {

                        tile = continentTiles.Dequeue();
                        noNeighbooringResources = true;
                        for (int x = tile.WorldBoardPosiiton.X - squareToCheck;
                            x <= tile.WorldBoardPosiiton.X + squareToCheck;
                            x++)
                        {
                            for (int y = tile.WorldBoardPosiiton.Y - squareToCheck;
                                y <= tile.WorldBoardPosiiton.Y + squareToCheck;
                                y++)
                            {
                                if (worldBoard.WorldTiles[x, y].Artifact != null)
                                {
                                    noNeighbooringResources = false;
                                }
                            }
                        }
                    }

                    var resource = ResourceLibrary.GetRandomResource(tile.Biome, random);
                    resource.Info.MapPosition = tile.WorldBoardPosiiton;
                    tile.Resource = resource;
                }
            }
        }

        public static void PlaceInitialArtifacts(WorldBoard worldBoard, NamelessGame game)
        {

            var artifactsPerContinentTiles = game.WorldSettings.ContinentTilesPerArtifact;

            foreach (var worldBoardContinent in worldBoard.Continents)
            {
                var continentTiles = new Queue<WorldTile>();
                var artifactNumber = worldBoardContinent.SizeInTiles / artifactsPerContinentTiles;
                foreach (var worldBoardWorldTile in worldBoard.WorldTiles)
                {
                    if (worldBoardWorldTile.Continent == worldBoardContinent && worldBoardWorldTile.Owner == null)
                    {
                        continentTiles.Enqueue(worldBoardWorldTile);
                    }
                }

                //randomize list
                continentTiles = new Queue<WorldTile>(continentTiles.OrderBy(
                    (o) => { return (game.WorldSettings.GlobalRandom.Next() % continentTiles.Count); }));

                var random = game.WorldSettings.GlobalRandom;


                for (int i = 0; i < artifactNumber; i++)
                {



                    WorldTile tile = null;
                    bool noNeighbooringArtifacts = false;
                    const int squareToCheck = 3;
                    while (!noNeighbooringArtifacts)
                    {

                        tile = continentTiles.Dequeue();
                        noNeighbooringArtifacts = true;
                        for (int x = tile.WorldBoardPosiiton.X - squareToCheck;
                            x <= tile.WorldBoardPosiiton.X + squareToCheck;
                            x++)
                        {
                            for (int y = tile.WorldBoardPosiiton.Y - squareToCheck;
                                y <= tile.WorldBoardPosiiton.Y + squareToCheck;
                                y++)
                            {
                                if (worldBoard.WorldTiles[x, y].Artifact != null)
                                {
                                    noNeighbooringArtifacts = false;
                                }
                            }
                        }
                    }

                    //ArtifactLibrary.Artifacts.ToArray();
                    var artifact = ArtifactLibrary.GetRandomArtifact(random);
                    artifact.Info.MapPosition = tile.WorldBoardPosiiton;
                    tile.Artifact = artifact;


                }
            }

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
                    const int squareToCheck = 10;
                    const int radiusToClaim = 7;
                    while (!noNeighbooringCivs)
                    {

                        tile = continentTiles.Dequeue();
                        noNeighbooringCivs = true;
                        for (int x = tile.WorldBoardPosiiton.X - squareToCheck; x <= tile.WorldBoardPosiiton.X + squareToCheck; x++)
                        {
                            for (int y = tile.WorldBoardPosiiton.Y - squareToCheck; y <= tile.WorldBoardPosiiton.Y + squareToCheck; y++)
                            {
                                if (worldBoard.WorldTiles[x, y].Owner != null)
                                {
                                    noNeighbooringCivs = false;
                                }
                            }
                        }
                    }


                   



                    tile.Settlement = firstSettlement;




                    for (int x = tile.WorldBoardPosiiton.X - radiusToClaim; x <= tile.WorldBoardPosiiton.X + radiusToClaim; x++)
                    {
                        for (int y = tile.WorldBoardPosiiton.Y - radiusToClaim; y <= tile.WorldBoardPosiiton.Y + radiusToClaim; y++)
                        {

                            var dX = x - tile.WorldBoardPosiiton.X;
                            var dY = y - tile.WorldBoardPosiiton.Y;

                            if (dX * dX + dY * dY <= (radiusToClaim))
                            {
                                worldBoard.WorldTiles[x, y].Owner = civilization;
                            }

                           
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
                                                        tile.Continent == null), (tile, region) => tile.Continent = region,() => { return new string(continentNamesChain.Chain(game.WorldSettings.GlobalRandom).ToArray()).FirstCharToUpper(); });

            List<Region> forests = GetRegions(worldBoard, game, rand, (tile => tile.Biome == Biomes.Forest && tile.LandmarkRegion==null), (tile, region) => tile.LandmarkRegion = region, () => { return new string(continentNamesChain.Chain(game.WorldSettings.GlobalRandom).ToArray()).FirstCharToUpper() + " forest"; });
            List<Region> deserts = GetRegions(worldBoard, game, rand, (tile => tile.Biome == Biomes.Desert && tile.LandmarkRegion == null), (tile, region) => tile.LandmarkRegion = region, () => { return new string(continentNamesChain.Chain(game.WorldSettings.GlobalRandom).ToArray()).FirstCharToUpper() + " desert"; });
            List<Region> mountains = GetRegions(worldBoard, game, rand, (tile => tile.Biome == Biomes.Mountain && tile.LandmarkRegion == null), (tile, region) => tile.LandmarkRegion = region, () => { return new string(continentNamesChain.Chain(game.WorldSettings.GlobalRandom).ToArray()).FirstCharToUpper() + " mountain"; });
            List<Region> swamps = GetRegions(worldBoard, game, rand, (tile => tile.Biome == Biomes.Swamp && tile.LandmarkRegion == null), (tile, region) => tile.LandmarkRegion = region, () => { return new string(continentNamesChain.Chain(game.WorldSettings.GlobalRandom).ToArray()).FirstCharToUpper() + " swamp"; });
            s.Stop();
            worldBoard.Continents = regions.Where(x => x.SizeInTiles >= 2000).ToList();
            worldBoard.Islands = regions.Where(x => x.SizeInTiles < 2000).ToList();
            worldBoard.Forests = forests.ToList();
            worldBoard.Deserts = deserts.ToList();
            worldBoard.Mountains = mountains.ToList();
            worldBoard.Swamps = swamps.ToList();
        }

        private static List<Region> GetRegions(WorldBoard worldBoard, NamelessGame game, Random rand, Func<WorldTile,bool> searchCriterion, Action<WorldTile,Region> onFoundRegion, Func<string> nameGenerator)
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
                        r.Name = nameGenerator();
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

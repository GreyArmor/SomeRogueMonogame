using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Engine.Generation.Noise;
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
        }

        public static void AnalizeLandmasses(WorldBoard worldBoard, NamelessGame game)
        {
            var s = Stopwatch.StartNew();
            List<WorldTile> unsortedTiles = new List<WorldTile>();

            foreach (var worldBoardWorldTile in worldBoard.WorldTiles)
            {
                unsortedTiles.Add(worldBoardWorldTile);
            }

            List<Region> regions = new List<Region>();
            Random rand = game.WorldSettings.GlobalRandom;
            var searchPoint = new Point();

            for (; searchPoint.X < game.WorldSettings.WorldBoardWidth; searchPoint.X++)
            {

                for (searchPoint.Y = 0; searchPoint.Y < game.WorldSettings.WorldBoardHeight; searchPoint.Y++)
                {
                    if (worldBoard.WorldTiles[searchPoint.X, searchPoint.Y].Terrain != TerrainTypes.Water &&
                        worldBoard.WorldTiles[searchPoint.X, searchPoint.Y].Continent == null)
                    {
                        Region r = new Region();
                        r.Name = "region" + regions.Count;
                        r.Color = new Color(rand.NextDouble(), rand.NextDouble(), rand.NextDouble(), 1f);

                        regions.Add(r);

                        var firstNode = worldBoard.WorldTiles[searchPoint.X, searchPoint.Y];

                        var floodList = new Queue<WorldTile>();
                        floodList.Enqueue(firstNode);
                        firstNode.Continent = r;
                        r.SizeInTiles++;

                        void AddToFloodList(int x, int y, Region region)
                        {
                            if (worldBoard.WorldTiles[x, y].Terrain !=
                                TerrainTypes.Water && worldBoard.WorldTiles[x, y].Continent == null)
                            {
                                floodList.Enqueue(worldBoard.WorldTiles[x, y]);
                                worldBoard.WorldTiles[x, y].Continent = region;
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
                        }
                    }
                }
            }

            s.Stop();
            worldBoard.Continents = regions.Where(x => x.SizeInTiles >= 2000).ToList();
            worldBoard.Islands = regions.Where(x => x.SizeInTiles >= 2000).ToList();
        }
    }
}

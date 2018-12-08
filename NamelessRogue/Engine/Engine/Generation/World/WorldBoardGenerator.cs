using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NamelessRogue.shell;

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
                    var worldTile = new WorldTile();
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
    }
}

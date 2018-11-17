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
        public static void PopulateWithData(WorldBoard board, NamelessGame game)
        {
            for (int x = 0; x < 100; x++)
            {
                for (int y = 0; y < 100; y++)
                {
                    var worldTile = new WorldTile();
                    worldTile.Terrain = game.WorldSettings.TerrainGen.GetTile(x, y, 0.1f).getTerrainType();
                    board.WorldTiles[x, y] = worldTile;
                }
            }
        }
    }
}

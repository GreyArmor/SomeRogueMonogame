using System.Linq;
using AStarNavigator.Providers;
using NamelessRogue.Engine.Abstraction;
using TiledCSPlus;

namespace NamelessRogue.Engine.Factories
{
    public class BuildingDataCache
    {       
        static DiagonalNeighborProviderSelfIncluded diagonalNeighbors = new DiagonalNeighborProviderSelfIncluded();
        static StraightNeighborProviderSelfIncluded straightNeighbors = new StraightNeighborProviderSelfIncluded();
        public bool[,] postProcessingArray;
        public string[,] tilesetPositions;
        public void CalculateCache(int buildingSize, int realSpaceX, int realSpaceY, int floorZ, IWorldProvider worldProvider, TiledLayer mainLayer, TiledTileset tileset)
        {
            postProcessingArray = new bool[buildingSize, buildingSize];
            tilesetPositions = new string[buildingSize, buildingSize];

            for (int loopY = 0; loopY < buildingSize; loopY++)
            {
                for (int loopX = 0; loopX < buildingSize; loopX++)
                {
                    var gameTile = worldProvider.GetTile(realSpaceX + loopX, realSpaceY + loopY, floorZ);
                    var tileId = mainLayer.Data[loopX + (loopY * buildingSize)];
                    if (tileId != 0)
                    {
                        var tile = tileset.Tiles.First(x => x.Id == tileId - 1);
                        var tileObjectType = tile.Properties[0].Value;
                        if (tileObjectType.Contains("wall") || tileObjectType.Contains("door") || tileObjectType.Contains("window") || tileObjectType =="chainlink")
                        {
                            postProcessingArray[loopY, loopX] = true;
                        }
                    }
                }
            }

            for (int loopY = 0; loopY < buildingSize; loopY++)
            {
                for (int loopX = 0; loopX < buildingSize; loopX++)
                {
                    var cellValue = postProcessingArray[loopY, loopX];

                    if (!cellValue)
                    {
                        continue;
                    }

                    var diagonalCells = diagonalNeighbors.GetNeighbors(new AStarNavigator.Tile(loopX, loopY)).ToList();
                    var straightCells = straightNeighbors.GetNeighbors(new AStarNavigator.Tile(loopX, loopY)).ToList();
                    string tilesetPosition = "";
                    foreach (var neighbor in diagonalCells)
                    {
                        //throw away outside of bounds tiles
                        if (neighbor.X < 0 || neighbor.Y < 0 || neighbor.X == buildingSize || neighbor.Y == buildingSize)
                        {
                            tilesetPosition += "0";
                        }
                        else
                        {
                            // also throw away all diagonal tiles
                            if (straightCells.Contains(neighbor) && postProcessingArray[(int)neighbor.Y, (int)neighbor.X])
                            {
                                tilesetPosition += "1";
                            }
                            else
                            {
                                tilesetPosition += "0";
                            }
                        }
                    }
                    tilesetPositions[loopY, loopX] = tilesetPosition;
                }
            }

        }

        //the sane as usual, but we use world data to calculate the tile id
        public void CalculateCacheForRandomizer(int buildingSize, int realSpaceX, int realSpaceY, int floorZ, IWorldProvider worldProvider)
        {
            postProcessingArray = new bool[buildingSize, buildingSize];
            tilesetPositions = new string[buildingSize, buildingSize];

            for (int loopY = 0; loopY < buildingSize; loopY++)
            {
                for (int loopX = 0; loopX < buildingSize; loopX++)
                {
                    var gameTile = worldProvider.GetTile(realSpaceX + loopX, realSpaceY + loopY, floorZ);

                    if (gameTile.AnyEntities())
                    {
                        postProcessingArray[loopY, loopX] = true;
                    }
                }
            }

            for (int loopY = 0; loopY < buildingSize; loopY++)
            {
                for (int loopX = 0; loopX < buildingSize; loopX++)
                {
                    var cellValue = postProcessingArray[loopY, loopX];

                    if (!cellValue)
                    {
                        continue;
                    }

                    var diagonalCells = diagonalNeighbors.GetNeighbors(new AStarNavigator.Tile(loopX, loopY)).ToList();
                    var straightCells = straightNeighbors.GetNeighbors(new AStarNavigator.Tile(loopX, loopY)).ToList();
                    string tilesetPosition = "";
                    foreach (var neighbor in diagonalCells)
                    {
                        //throw away outside of bounds tiles
                        if (neighbor.X < 0 || neighbor.Y < 0 || neighbor.X == buildingSize || neighbor.Y == buildingSize)
                        {
                            tilesetPosition += "0";
                        }
                        else
                        {
                            // also throw away all diagonal tiles
                            if (straightCells.Contains(neighbor) && postProcessingArray[(int)neighbor.Y, (int)neighbor.X])
                            {
                                tilesetPosition += "1";
                            }
                            else
                            {
                                tilesetPosition += "0";
                            }
                        }
                    }
                    tilesetPositions[loopY, loopX] = tilesetPosition;
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RogueSharp.Random;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Generation.Settlement;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.shell;
using NamelessRogue.Engine.Utility;

namespace NamelessRogue.Engine.Factories
{
    public static class SettlementFactory
    {
        public static ConcreteSettlement GenerateSettlement(NamelessGame namelessGame, WorldTile tile, TimelineLayer board,
            IWorldProvider worldProvider)
        {
            var result = new ConcreteSettlement();

            int chunksPerTile = worldProvider.ChunkResolution / namelessGame.WorldSettings.WorldBoardWidth;

            var squareToCheck = 5;

            List<KeyValuePair<Point, Chunk>> allChunksToWorkWith = new List<KeyValuePair<Point, Chunk>>();

            //find chunks to work with
            for (int x = tile.WorldBoardPosiiton.X - squareToCheck; x <= tile.WorldBoardPosiiton.X + squareToCheck; x++)
            {
                for (int y = tile.WorldBoardPosiiton.Y - squareToCheck;
                    y <= tile.WorldBoardPosiiton.Y + squareToCheck;
                    y++)
                {
                    List<KeyValuePair<Point, Chunk>> chunks = new List<KeyValuePair<Point, Chunk>>();

                    int chunkX = x * chunksPerTile;
                    int chunkY = y * chunksPerTile;

                    for (int i = chunkX; i < chunkX + chunksPerTile; i++)
                    {
                        for (int j = chunkY; j < chunkY + chunksPerTile; j++)
                        {
                            var point = new Point(i, j);
                            chunks.Add(new KeyValuePair<Point, Chunk>(point, worldProvider.GetChunks()[point]));
                        }
                    }

                    foreach (var keyValuePair in chunks)
                    {
                        //place them into reality bubble for convenience
                        worldProvider.GetRealityBubbleChunks().Add(keyValuePair.Key, keyValuePair.Value);
                        worldProvider.RealityChunks.Add(keyValuePair.Value);
                        allChunksToWorkWith.Add(keyValuePair);
                    }
                }
            }

            Point minPoint, maxPoint;

            var firstChunk = allChunksToWorkWith.First().Value;

            minPoint = firstChunk.ChunkWorldMapLocationPoint;
            maxPoint = firstChunk.ChunkWorldMapLocationPoint;

            foreach (var keyValuePair in allChunksToWorkWith)
            {
                var currentPoint = keyValuePair.Value.ChunkWorldMapLocationPoint;
                if (currentPoint.X > maxPoint.X || currentPoint.Y > maxPoint.Y)
                {
                    maxPoint = currentPoint;
                }

                if (currentPoint.X < minPoint.X || currentPoint.Y < minPoint.Y)
                {
                    minPoint = currentPoint;
                }
            }

            maxPoint.X += Constants.ChunkSize;
            maxPoint.Y += Constants.ChunkSize;

            var maxVector = maxPoint.ToVector2();
            var minVector = minPoint.ToVector2();

            var center = (minVector + maxVector) / 2;

            var citySize = 100;
            var streetWidth = 10;

            var slots = new CitySlot[citySize / streetWidth, citySize / streetWidth];
            


            GenerateCityBuildingForTest(center, namelessGame, worldProvider);



            result.Center = center.ToPoint();

            foreach (var keyValuePair in allChunksToWorkWith)
            {
                worldProvider.GetRealityBubbleChunks().Remove(keyValuePair.Key);
            }





            return result;
        }


        private static void GenerateCityBuildingForTest(Vector2 center, NamelessGame game, IWorldProvider chunks)
        {

            var citySize = 200;
            //TODO streets
            


            var buildingNumber = (citySize) / 15 * (citySize) / 15;
            var random = new InternalRandom(game.WorldSettings.GlobalRandom.Next());

            var blueprint = BlueprintLibrary.Blueprints.First();

            for (int x = 0; x < citySize; x += 15)
            {
                for (int y = 0; y < citySize; y += 15)
                {
                    BuildingFactory.CreateBuilding((int) (x + center.X - citySize / 2),
                        (int) (y + center.Y - citySize / 2), blueprint, game, chunks, random);
                }
            }
        }

    }


}


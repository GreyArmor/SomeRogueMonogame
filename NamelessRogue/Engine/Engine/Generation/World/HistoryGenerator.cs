 

using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using GeonBit.UI;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Engine.Factories;
using NamelessRogue.Engine.Engine.Generation.World.BoardPieces;
using NamelessRogue.Engine.Engine.Generation.World.Meta;
using NamelessRogue.Engine.Engine.Infrastructure;
using NamelessRogue.shell;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;

namespace NamelessRogue.Engine.Engine.Generation.World
{
    public class HistoryGenerator {

        public class HistoryGenerationSettings
        {
            /// <summary>
            /// Number of turns simulated
            /// </summary>
            public int HowOldIsTheWorld { get; }

        }

        public static List<MapArtifact> Artifacts { get; private set; }
        public static TimeLine BuildTimeline(NamelessGame game, HistoryGenerationSettings settings)
        {

            var timeline = new TimeLine();
            var worldBoard = InitialiseFirstBoard(game,settings);
            timeline.WorldBoardAtEveryAge.Add(worldBoard);
            timeline.CurrentTimelineLayer = worldBoard;
            //for (int i = 1; i < settings.HowOldIsTheWorld; i++)
            //{

            //}
            return timeline;
        }

        //possible to move to configs later
        private static void CreateBasicPieces()
        {
            Artifacts = new List<MapArtifact>();
        }

        private static TimelineLayer InitialiseFirstBoard(NamelessGame game, HistoryGenerationSettings settings)
        {
            var worldBoard = new TimelineLayer(game.WorldSettings.WorldBoardWidth, game.WorldSettings.WorldBoardWidth, 0);
            ChunkData chunkData = new ChunkData(game.WorldSettings);

            worldBoard.Chunks = chunkData;

            WorldBoardGenerator.PopulateWithInitialData(worldBoard, game);
            WorldBoardGenerator.AnalizeLandmasses(worldBoard, game);
            WorldBoardGenerator.PlaceInitialCivilizations(worldBoard, game);
            WorldBoardGenerator.PlaceInitialArtifacts(worldBoard, game);
            WorldBoardGenerator.PlaceResources(worldBoard, game);

            WorldBoardGenerator.DistributeMetaphysics(worldBoard, game);

            WorldTile firsTile = null;
            foreach (var worldBoardWorldTile in worldBoard.WorldTiles)
            {

                if (worldBoardWorldTile.Settlement != null)
                {
                    IChunkProvider worldProvider = chunkData;
                    firsTile = worldBoardWorldTile;
                    var concreteSettlment = SettlementFactory.GenerateSettlement(game, firsTile, worldBoard, worldProvider);

                    firsTile.Settlement.Concrete = concreteSettlment;
                    break;

                }
            }

 

            //var concreteSettlment = SettlementFactory.GenerateSettlement(game, firsTile, worldBoard, worldProvider);

            //firsTile.Settlement.Concrete = concreteSettlment;
          
            return worldBoard;
        }


        private static TimelineLayer ProgressWorldBoard(TimelineLayer previousState, WorldSettings settings)
        {
            var newState = new TimelineLayer(settings.WorldBoardWidth, settings.WorldBoardHeight, previousState.Age + 1);


            
            return newState;
        }

    }
}

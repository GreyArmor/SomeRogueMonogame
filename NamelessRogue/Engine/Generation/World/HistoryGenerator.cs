using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.Generation.World.BoardPieces;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Serialization;
using NamelessRogue.shell;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;

namespace NamelessRogue.Engine.Generation.World
{
    public class HistoryGenerator {
        public static List<MapArtifact> Artifacts { get; private set; }
        public static WorldTemplate BuildWorldTemplate(NamelessGame game)
        {
            var timeline = new WorldTemplate(game.WorldSettings.Seed);
            var worldBoard = InitialiseFirstBoard(game);
            //timeline.WorldBoardAtEveryAge.Add(worldBoard);
            timeline.WorldMap = worldBoard;


            //String appPath = System.IO.Directory.GetCurrentDirectory();
            //Stopwatch s = new Stopwatch(); 
            //s.Start();
            //SaveManager.SaveTimelineLayer(appPath + "\\Layers", worldBoard,
            //    worldBoard.Age.ToString());
            //s.Stop();
            //s = new Stopwatch();
            //s.Start();
            //var timelineLayer = SaveManager.LoadTimelineLayer(appPath + "\\Layers",
            //    worldBoard.Age.ToString());
            //s.Stop();
            //timelineLayer.ToString();
            //for (int i = 1; i < settings.HowOldIsTheWorld; i++)
            //{

            //}
            return timeline;
        }

        private static WorldMap InitialiseFirstBoard(NamelessGame game)
        {
            var worldBoard = new WorldMap(game.WorldSettings.WorldMapResolution);
            ChunkData chunkData = new ChunkData(game.WorldSettings, worldBoard);

            worldBoard.Chunks = chunkData;

            WorldBoardGenerator.PopulateWithInitialData(worldBoard, game);
           // WorldBoardGenerator.AnalizeLandmasses(worldBoard, game);
           // WorldBoardGenerator.PlaceInitialCivilizations(worldBoard, game);
            //WorldBoardGenerator.PlaceInitialArtifacts(worldBoard, game);
            //WorldBoardGenerator.PlaceResources(worldBoard, game);
            //WorldBoardGenerator.DistributeMetaphysics(worldBoard, game);

            //WorldTile firsTile = null;
            //foreach (var worldBoardWorldTile in worldBoard.WorldTiles)
            //{

            //    if (worldBoardWorldTile.Settlement != null)
            //    {
            //        IWorldProvider worldProvider = chunkData;
            //        firsTile = worldBoardWorldTile;
            //        var concreteSettlment = SettlementFactory.GenerateSettlement(game, firsTile, worldBoard, worldProvider);

            //        firsTile.Settlement.Concrete = concreteSettlment;
            //        break;

            //    }
            //}

 

            //var concreteSettlment = SettlementFactory.GenerateSettlement(game, firsTile, worldBoard, worldProvider);

            //firsTile.Settlement.Concrete = concreteSettlment;
          
            return worldBoard;
        }

    }
}

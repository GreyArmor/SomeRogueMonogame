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
        public static WorldTemplate BuildWorldTemplate(NamelessGame game)
        {
            var timeline = new WorldTemplate(game.WorldSettings.Seed);
            var worldBoard = InitialiseFirstBoard(game);
            timeline.WorldMap = worldBoard;

            return timeline;
        }

        private static WorldMap InitialiseFirstBoard(NamelessGame game)
        {
            var worldBoard = new WorldMap(game.WorldSettings.WorldMapResolution);
            ChunkData chunkData = new ChunkData(game.WorldSettings, worldBoard);

            worldBoard.Chunks = chunkData;

            WorldBoardGenerator.PopulateWithInitialData(worldBoard, game);
          
            return worldBoard;
        }

    }
}

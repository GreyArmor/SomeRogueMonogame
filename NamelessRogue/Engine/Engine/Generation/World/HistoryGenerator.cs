 

using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using GeonBit.UI;
using NamelessRogue.Engine.Engine.Generation.World.BoardPieces;
using NamelessRogue.Engine.Engine.Generation.World.Meta;
using NamelessRogue.Engine.Engine.Infrastructure;
using NamelessRogue.shell;
using SharpDX.Direct2D1;

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

     

        public static List<MapResource> FoodResources{ get; set; }
        public static List<MapResource> ManufactoringResources { get; set; }
        public static List<MapResource> ScienceResources { get; set; }
        public static List<MapResource> CultureResources { get; set; }
        public static List<MapResource> ManaResources { get; set; }
        public static List<MapResource> HeaäthResources { get; set; }

        public static List<MapArtifact> Artifacts { get; private set; }
        public static TimeLine BuildTimeline(NamelessGame game, HistoryGenerationSettings settings)
        {

            CreateBasicPieces();

            var timeline = new TimeLine();
            var worldBoard = InitialiseFirstBoard(game,settings);
            timeline.WorldBoardAtEveryAge.Add(worldBoard);
            timeline.CurrentWorldBoard = worldBoard;
            //for (int i = 1; i < settings.HowOldIsTheWorld; i++)
            //{

            //}
            return timeline;
        }

        //possible to move to configs later
        private static void CreateBasicPieces()
        {
            Artifacts = new List<MapArtifact>();

            CreateFoods();


        }

        private static void CreateFoods()
        {
            FoodResources = new List<MapResource>();
            FoodResources.Add(new MapResource()
            {
                Info = new ObjectInfo()
                {
                    Name = "Game",
                    ProductionModifier = new ProductionValue(1, 0, 0, 0, 0, 0)
                },
                AppearsOn = new List<TerrainTypes>() { TerrainTypes.Dirt, TerrainTypes.Grass, },
                Level = 1
            });

            FoodResources.Add(new MapResource()
            {
                Info = new ObjectInfo()
                {
                    Name = "Wheat",
                    ProductionModifier = new ProductionValue(2, 0, 0, 0, 0, 0)
                },
                AppearsOn = new List<TerrainTypes>() { TerrainTypes.Dirt, TerrainTypes.Grass },
                Level = 2
            });


            FoodResources.Add(new MapResource()
            {
                Info = new ObjectInfo()
                {
                    Name = "Salt",
                    ProductionModifier = new ProductionValue(3, 0, 0, 0, 0, 0)
                },
                AppearsOn = new List<TerrainTypes>() { TerrainTypes.Dirt, TerrainTypes.Grass },
                Level = 3
            });
        }

        private static WorldBoard InitialiseFirstBoard(NamelessGame game, HistoryGenerationSettings settings)
        {
            var worldBoard = new WorldBoard(game.WorldSettings.WorldBoardWidth, game.WorldSettings.WorldBoardWidth, 0);
            WorldBoardGenerator.PopulateWithInitialData(worldBoard, game);

            Stack<MapResource> resources = GenerateResourcePoolBasedONTerrain(worldBoard);

          //   WorldBoardGenerator.DistributeMetaphysics(worldBoard, game);
          WorldBoardGenerator.PlaceInitialArtifacts(worldBoard, game);
            WorldBoardGenerator.PlaceResources(worldBoard, game);
            WorldBoardGenerator.PlaceInitialCivilizations(worldBoard, game);
            return worldBoard;
        }

        private static Stack<MapResource> GenerateResourcePoolBasedONTerrain(WorldBoard worldBoard)
        {
            var result = new Stack<MapResource>();
            return result;
        }

        private static WorldBoard ProgressWorldBoard(WorldBoard previousState)
        {
            var newState = new WorldBoard(100,100, previousState.Age + 1);

            return newState;
        }

    }
}

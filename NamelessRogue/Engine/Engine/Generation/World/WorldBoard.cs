 

using System;
using System.Collections.Generic;
using NamelessRogue.Engine.Engine.Components;
using NamelessRogue.Engine.Engine.Components.ChunksAndTiles;

namespace NamelessRogue.Engine.Engine.Generation.World
{
    public class WorldBoard : Component
    {
        public int Age { get; }
        public WorldTile[,] WorldTiles { get; }

        public List<Civilization> Civilizations { get; }
        public List<Region> Continents { set; get; }
        public List<Region> Islands { set; get; }
        public List<Region> Mountains { set; get; }
        public List<Region> Forests { get; set; }
        public List<Region> Deserts { get; set; }
        public List<Region> Swamps { get; set; }
        public ChunkData Chunks { get; set; }

        public WorldBoard(int width, int height, int age)
        {
            WorldTiles = new WorldTile[width, height];
            Age = age;
            Civilizations = new List<Civilization>();
            Continents = new List<Region>();
        }
    }
}

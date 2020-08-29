 

using System;
using System.Collections.Generic;
using NamelessRogue.Engine.Engine.Components;
using NamelessRogue.Engine.Engine.Components.ChunksAndTiles;
using Newtonsoft.Json;

namespace NamelessRogue.Engine.Engine.Generation.World
{
    public class TimelineLayer
    {
        public int Age { get; }
        //[JsonIgnore]
        public WorldTile[,] WorldTiles { get; set; }
        public List<Civilization> Civilizations { get; set; }
        public List<Region> Continents { set; get; }
        public List<Region> Islands { set; get; }
        public List<Region> Mountains { set; get; }
        public List<Region> Forests { get; set; }
        public List<Region> Deserts { get; set; }
        public List<Region> Swamps { get; set; }
        [JsonIgnore]
        public ChunkData Chunks { get; set; }

        public TimelineLayer(int width, int height, int age)
        {
            WorldTiles = new WorldTile[width, height];
            Age = age;
            Civilizations = new List<Civilization>();
            Continents = new List<Region>();
        }
    }
}

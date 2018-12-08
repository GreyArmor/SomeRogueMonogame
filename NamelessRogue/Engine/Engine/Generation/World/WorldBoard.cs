 

using System;
using System.Collections.Generic;
using NamelessRogue.Engine.Engine.Components;

namespace NamelessRogue.Engine.Engine.Generation.World
{
    public class WorldBoard : Component
    {
        public int Age { get; }
        public WorldTile[,] WorldTiles { get; }

        public List<Civilization> Civilizations { get; }

        public WorldBoard(int width, int height, int age)
        {
            WorldTiles = new WorldTile[width, height];
            Age = age;
            Civilizations = new List<Civilization>();
        }
    }
}

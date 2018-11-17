 

using System;
using NamelessRogue.Engine.Engine.Components;

namespace NamelessRogue.Engine.Engine.Generation.World
{
    public class WorldBoard : Component {
       public WorldTile[,] WorldTiles { get; }
        public WorldBoard(int width, int height)
        {
            WorldTiles = new WorldTile[width,height];
        }
    }
}

 

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Engine.Generation.World.Meta;
using NamelessRogue.Engine.Engine.Infrastructure;

namespace NamelessRogue.Engine.Engine.Generation.World.BoardPieces
{
    public class MapResource : BoardPiece
    {
        public int Level { get; set; }
        public List<Biomes> AppearsOn { get; set; } = new List<Biomes>();
    }
}

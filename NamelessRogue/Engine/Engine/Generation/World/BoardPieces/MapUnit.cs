using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Engine.Generation.World.Meta;

namespace NamelessRogue.Engine.Engine.Generation.World.BoardPieces
{
    public class Army : BoardPiece
    {
        public int Supply { get; set; }
        public int Mana { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using NamelessRogue.Engine.Generation.World.Meta;

namespace NamelessRogue.Engine.Generation.World.BoardPieces
{
    public class BoardPiece
    {
        public BoardPiece()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }

        public string Name { get; set; }
        public Vector2 Position { get; set; }
        public char Representation { get; set; }
        public Engine.Utility.Color CharColor { get; set; }
    }
}

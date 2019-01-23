using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NamelessRogue.Engine.Engine.Generation.World.Meta;

namespace NamelessRogue.Engine.Engine.Generation.World.BoardPieces
{
    public class BoardPiece
    {
        public ObjectInfo Info { get; set; } = new ObjectInfo();
        public char Representation { get; set; }
        public Engine.Utility.Color CharColor { get; set; }
    }
}

using Veldrid;
using NamelessRogue.Engine.Generation.World.Meta;

namespace NamelessRogue.Engine.Generation.World.BoardPieces
{
    public class MapArtifact : BoardPiece
    {
        public int TimeOfLife { get; set; }
        public int TimeLeft { get; set; }
    }
}
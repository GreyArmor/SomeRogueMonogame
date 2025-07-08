using Microsoft.Xna.Framework;

namespace NamelessRogue.Engine.Generation.World.BoardPieces
{
    public class MapArtifact : BoardPiece
    {
        public int TimeOfLife { get; set; }
        public int TimeLeft { get; set; }
    }
}
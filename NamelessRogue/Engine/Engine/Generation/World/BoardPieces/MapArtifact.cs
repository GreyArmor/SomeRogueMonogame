using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Engine.Generation.World.Meta;

namespace NamelessRogue.Engine.Engine.Generation.World.BoardPieces
{
    public class MapArtifact : BoardPiece
    {
        public int TimeOfLife { get; set; }
        public int TimeLeft { get; set; }
    }
}
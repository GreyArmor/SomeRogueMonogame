using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Engine.Generation.World.Meta;

namespace NamelessRogue.Engine.Engine.Generation.World.BoardPieces
{
    public class MapUnit : BoardPiece
    {
        public UnitTypes Type { get; }

        public int WeaponLevel { get; set; }
        public int DefenceLevel { get; set; }
    }
}
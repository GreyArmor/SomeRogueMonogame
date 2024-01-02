using System;
using System.Collections.Generic;
using SharpDX;
using NamelessRogue.Engine.Components.WorldBoardComponents.Combat;
using NamelessRogue.Engine.Generation.World.Meta;

namespace NamelessRogue.Engine.Generation.World.BoardPieces
{
    public class Army : BoardPiece
    {
        public int Supply { get; set; }
        public int Mana { get; set; }

        public int MaxNumberOfUnits { get; set; }

        private List<Unit> Units { get; set; } = new List<Unit>();

        public void AddUnit(Unit unit)
        {
            if (Units.Count+1>=MaxNumberOfUnits)
            {
                throw new Exception("Out of capacity to add unit into army");
            }
            else
            {
                Units.Add(unit);
            }

        }

        public void RemoveUnit(Unit unit)
        {
            Units.Remove(unit);
        }

    }
}
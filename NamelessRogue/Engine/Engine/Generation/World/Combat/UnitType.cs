using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Engine.Generation.World.Combat
{

    public enum MovementType
    {
        Ground,
        Flying,
        Naval
    }

    public class UnitType
    {
        private MovementType MovementType { get; set; }


    }
}

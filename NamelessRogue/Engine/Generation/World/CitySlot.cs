using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Utility;

namespace NamelessRogue.Engine.Generation.World
{
    public enum SlotType
    {
        Street,
        Building
    }
    public class CitySlot
    {
		public CitySlot()
		{
		}

		public CitySlot(BoundingBox3D placement)
        {
            Placement = placement;
        }
        public BoundingBox3D Placement { get; set; }
        public bool Occupied { get; set; }
        public IEntity OccupiedBy { get; set; }
    }
}

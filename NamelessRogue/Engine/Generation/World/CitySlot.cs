using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Infrastructure;

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

		public CitySlot(BoundingBox placement)
        {
            Placement = placement;
        }
        public BoundingBox Placement { get; set; }
        public bool Occupied { get; set; }
        public Entity OccupiedBy { get; set; }
    }
}

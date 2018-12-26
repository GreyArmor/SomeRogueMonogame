using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace NamelessRogue.Engine.Engine.Generation.World
{
    public class ConcreteSettlement
    {
        public Point Center { get; set; }
        public List<CitySlot> Slots { get; } = new List<CitySlot>();
    }
}

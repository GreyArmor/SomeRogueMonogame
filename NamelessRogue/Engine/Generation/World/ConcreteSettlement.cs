using NamelessRogue.Engine.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NamelessRogue.Engine.Generation.World
{
    public class ConcreteSettlement
    {
        public Point Center { get; set; }
        public List<CitySlot> Slots { get; } = new List<CitySlot>();
    }
}

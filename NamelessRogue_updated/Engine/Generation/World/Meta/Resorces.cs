using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Generation.World.Meta
{
    public class Resorces
    {
        public int Population { get; set; }
        public int Wealth { get; set; }
        public int Supply { get; set; }
        public int Mana { get; set; }

        public int Influence { get; set; }

        public Resorces(int population, int wealth, int supply, int mana, int influence)
        {
            Population = population;
            Wealth = wealth;
            Supply = supply;
            Mana = mana;
        }
    }
}

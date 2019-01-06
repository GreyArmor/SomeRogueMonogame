using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Engine.Components.Stats
{
    public class Stats : Component
    {
        public Stats()
        {
            Health = new SimpleStat(0, 0, 100);
            Stamina = new SimpleStat(0, 0, 100);
            Strength = new SimpleStat(0, 0, 9999);
            Agility = new SimpleStat(0, 0, 9999);
            Endurance = (new SimpleStat(0, 0, 9999));
            Imagination = (new SimpleStat(0, 0, 9999));
            Willpower = (new SimpleStat(0, 0, 9999));
            Wit = (new SimpleStat(0, 0, 9999));
            Attack = new SimpleStat(0, 0, 9999);
            Defence = new SimpleStat(0, 0, 9999);
            Speed = new SimpleStat(0, 0, 9999);
        }

        public SimpleStat Agility { get; set; }
        public SimpleStat Endurance { get; set; }

        public SimpleStat Health { get; set; }
        public SimpleStat Imagination { get; set; }

        public SimpleStat Stamina { get; set; }
        public SimpleStat Strength { get; set; }
        public SimpleStat Willpower { get; set; }
        public SimpleStat Wit { get; set; }

        public SimpleStat Attack { get; set; }
        public SimpleStat Defence { get; set; }
        public SimpleStat Speed { get; set; }


    }
}

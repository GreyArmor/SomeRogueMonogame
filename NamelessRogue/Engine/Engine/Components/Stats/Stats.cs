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
            Reflexes = new SimpleStat(0, 0, 9999);
            Perception = (new SimpleStat(0, 0, 9999));
            Imagination = (new SimpleStat(0, 0, 9999));
            Willpower = (new SimpleStat(0, 0, 9999));
            Wit = (new SimpleStat(0, 0, 9999));

            Attack = new SimpleStat(0, 0, 9999);
            Defence = new SimpleStat(0, 0, 9999);
            AttackSpeed = new SimpleStat(0, 0, 9999);
            MoveSpeed = new SimpleStat(0, 0, 9999);

        }

   
        public SimpleStat Strength { get; set; }
        public SimpleStat Reflexes { get; set; }
        public SimpleStat Perception { get; set; }
        public SimpleStat Willpower { get; set; }
        public SimpleStat Wit { get; set; }
        public SimpleStat Imagination { get; set; }

        public SimpleStat Health { get; set; }
        public SimpleStat Stamina { get; set; }
        public SimpleStat Attack { get; set; }
        public SimpleStat Defence { get; set; }
        public SimpleStat AttackSpeed { get; set; }
        public SimpleStat MoveSpeed { get; set; }


    }
}

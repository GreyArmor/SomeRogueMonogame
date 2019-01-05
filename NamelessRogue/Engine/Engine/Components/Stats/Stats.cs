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
            Health = new Health(0, 0, 100);
            Stamina = new Stamina(0, 0, 100);
            Strength = new Strength(0, 0, 9999);
            Agility = new Agility(0, 0, 9999);
            Endurance = (new Endurance(0, 0, 9999));
            Imagination = (new Imagination(0, 0, 9999));
            Willpower = (new Willpower(0, 0, 9999));
            Wit = (new Wit(0, 0, 9999));
            Attack = new Attack(0, 0, 9999);
            Defence = new Defence(0, 0, 9999);
            Speed = new Speed(0, 0, 9999);
        }

        public Agility Agility { get; set; }
        public Endurance Endurance { get; set; }

        public Health Health { get; set; }
        public Imagination Imagination { get; set; }

        public Stamina Stamina { get; set; }
        public Strength Strength { get; set; }
        public  Willpower Willpower { get; set; }
        public Wit Wit { get; set; }

        public Attack Attack { get; set; }
        public Defence Defence { get; set; }
        public Speed Speed { get; set; }


    }
}

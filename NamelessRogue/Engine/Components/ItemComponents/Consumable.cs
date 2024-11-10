using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.ItemComponents
{
    public class Consumable : Component
    {
        public int Health { get; internal set; }
        public int Energy { get; internal set; }
        public int Damage { get; internal set; }
        public int Armor { get; internal set; }
        public int Resistance { get; internal set; }
        public int Duration { get; internal set; }

        public bool IsAppliedImmediately {  get; internal set; }

        public bool IsDamageOverTime { get; internal set; }


        public override IComponent Clone()
        {
            var clone = new Consumable();
            clone.Health = Health;
            clone.Energy = Energy;
            clone.Damage = Damage;
            clone.Armor = Armor;
            clone.Resistance = Resistance;
            clone.Duration = Duration;
            clone.IsAppliedImmediately = IsAppliedImmediately;
            clone.IsDamageOverTime = IsDamageOverTime;
            return clone;
        }
    }
}

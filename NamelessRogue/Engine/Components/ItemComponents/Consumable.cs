using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.ItemComponents
{

    public class Buff : Component
    {
        public int Health { get; set; }
        public int Energy { get; set; }
        public int Damage { get; set; }
        public int Armor { get; set; }
        public int Resistance { get; set; }
        public int Duration { get; set; }

        public bool PermanentModifier { get; set; }
        public bool IsAppliedImmediately { get; set; }

        public bool IsDamageOverTime { get;  set; }

        public override IComponent Clone()
        {
            var clone = new Buff();
            clone.Health = Health;
            clone.Energy = Energy;
            clone.Damage = Damage;
            clone.Armor = Armor;
            clone.Resistance = Resistance;
            clone.Duration = Duration;
            clone.PermanentModifier = PermanentModifier;
            clone.IsAppliedImmediately = IsAppliedImmediately;
            clone.IsDamageOverTime = IsDamageOverTime;
            return clone;
        }
    }

    public class Consumable : Component
    { 
        public Consumable(IEnumerable<string> buffIds)
        {
            BuffIds = buffIds.ToList();
        }
    
        public List<string> BuffIds { get; set; } = new List<string>();

        public override IComponent Clone()
        {
            var clone = new Consumable(BuffIds);
            return clone;
        }
    }

    public class OnHitBuffs : Component
    {
        public OnHitBuffs(IEnumerable<string> buffIds)
        {
            BuffIds = buffIds.ToList();
        }

        public List<string> BuffIds { get; set; } = new List<string>();

        public override IComponent Clone()
        {
            var clone = new Consumable(BuffIds);
            return clone;
        }
    }
}

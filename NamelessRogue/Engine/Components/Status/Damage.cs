using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Stats;

namespace NamelessRogue.Engine.Components.Status
{
    public class Damage {

        public Damage() { } 
		public Damage(IEntity source, IEntity target, int damage, DamageType damageType)
        {
            Source = source;
            Target = target;
            DamageValue = damage;
            DamageType = damageType;
        }

		public IEntity Source { get; set; }
        public IEntity Target { get; }
        public int DamageValue { get; set; }

        public DamageType DamageType { get; set; }
    }
}

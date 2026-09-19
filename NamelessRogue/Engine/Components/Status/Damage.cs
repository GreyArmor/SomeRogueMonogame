using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.Engine.Infrastructure;

namespace NamelessRogue.Engine.Components.Status
{
    public class Damage {

        public Damage() { } 
		public Damage(Entity source, Entity target, int damage, DamageType damageType)
        {
            Source = source;
            Target = target;
            DamageValue = damage;
            DamageType = damageType;
        }

		public Entity Source { get; set; }
        public Entity Target { get; }
        public int DamageValue { get; set; }

        public DamageType DamageType { get; set; }
    }
}

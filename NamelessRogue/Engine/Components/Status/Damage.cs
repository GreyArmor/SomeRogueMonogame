using NamelessRogue.Engine.Abstraction;

namespace NamelessRogue.Engine.Components.Status
{
    public class Damage : Component {

        public Damage() { } 
		public Damage(IEntity source = null, int damage = 0)
        {
            this.Source = source;
            this.DamageValue = damage;
        }

		public IEntity Source { get; set; }
		public int DamageValue { get; set; }

		public override IComponent Clone()
        {
            return new Damage(Source, DamageValue);
        }
    }
}

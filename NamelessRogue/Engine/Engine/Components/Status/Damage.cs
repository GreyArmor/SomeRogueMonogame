using NamelessRogue.Engine.Abstraction;

namespace NamelessRogue.Engine.Engine.Components.Status
{
    public class Damage : Component {
        private IEntity source;
        private int damage;

        public Damage(IEntity source, int damage)
        {
            this.source = source;
            this.damage = damage;
        }

        public void setDamage(int damage) {
            this.damage = damage;
        }

        public int getDamage() {
            return damage;
        }

        public void setSource(IEntity source) {
            this.source = source;
        }

        public IEntity getSource() {
            return source;
        }
    }
}

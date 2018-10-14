using NamelessRogue.Engine.Abstraction;

namespace NamelessRogue.Engine.Engine.Components.Interaction
{
    public class DeathCommand : Component {
        private IEntity toKill;

        public DeathCommand(IEntity toKill)
        {
            this.toKill = toKill;
        }

        public void setToKill(IEntity toKill) {
            this.toKill = toKill;
        }

        public IEntity getToKill() {
            return toKill;
        }
    }
}

using NamelessRogue.Engine.Abstraction;
using NamelessRogue_updated.Engine.Abstraction;

namespace NamelessRogue.Engine.Components.Interaction
{
    public class DeathCommand : ICommand
    {
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

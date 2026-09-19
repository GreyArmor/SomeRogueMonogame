using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Infrastructure;

namespace NamelessRogue.Engine.Components.Interaction
{
    public class DeathCommand : ICommand
    {
        private Entity toKill;

        public DeathCommand(Entity toKill)
        {
            this.toKill = toKill;
        }

        public void setToKill(Entity toKill) {
            this.toKill = toKill;
        }

        public Entity getToKill() {
            return toKill;
        }
    }
}

using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Infrastructure;
namespace NamelessRogue.Engine.Components.Interaction
{
    public class AttackCommand : ICommand {
        private Entity source;
        private Entity target;

        public AttackCommand(Entity source, Entity target)
        {
            this.source = source;
            this.target = target;
        }

        public void setSource(Entity source) {
            this.source = source;
        }

        public Entity getSource() {
            return source;
        }

        public void setTarget(Entity target) {
            this.target = target;
        }

        public Entity getTarget() {
            return target;
        }
    }
}

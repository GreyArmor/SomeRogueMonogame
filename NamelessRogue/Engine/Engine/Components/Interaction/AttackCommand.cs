using NamelessRogue.Engine.Abstraction;

namespace NamelessRogue.Engine.Engine.Components.Interaction
{
    public class AttackCommand : Component {
        private IEntity source;
        private IEntity target;

        public AttackCommand(IEntity source, IEntity target)
        {
            this.source = source;
            this.target = target;
        }

        public void setSource(IEntity source) {
            this.source = source;
        }

        public IEntity getSource() {
            return source;
        }

        public void setTarget(IEntity target) {
            this.target = target;
        }

        public IEntity getTarget() {
            return target;
        }
    }
}

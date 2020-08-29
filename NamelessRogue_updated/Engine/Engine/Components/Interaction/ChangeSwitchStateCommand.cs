 

namespace NamelessRogue.Engine.Engine.Components.Interaction
{
    public class ChangeSwitchStateCommand : Component {

        private SimpleSwitch target;
        private bool active;

        public ChangeSwitchStateCommand(SimpleSwitch target, bool active) {
            this.target = target;
            this.active = active;
        }

        public override IComponent Clone()
        {
            return new ChangeSwitchStateCommand(target,active);
        }

        public SimpleSwitch getTarget() {
            return target;
        }

        public bool isActive() {
            return active;
        }
    }
}

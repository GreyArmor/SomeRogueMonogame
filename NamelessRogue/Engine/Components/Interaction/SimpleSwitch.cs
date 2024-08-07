 

namespace NamelessRogue.Engine.Components.Interaction
{
    public class SimpleSwitch : Component {

        public SimpleSwitch(bool switchActiv)
        {
            this.switchActive = switchActiv;
        }

		public SimpleSwitch()
		{
		}

		private bool switchActive;

        public void setSwitchActive(bool switchActive) {
            this.switchActive = switchActive;
        }

        public bool isSwitchActive() {
            return switchActive;
        }

        public override IComponent Clone()
        {
           return new SimpleSwitch(switchActive);
        }
    }
}

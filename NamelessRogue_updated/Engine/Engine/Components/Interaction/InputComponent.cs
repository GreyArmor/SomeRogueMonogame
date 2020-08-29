using System.Collections.Generic;
using NamelessRogue.Engine.Engine.Input;

namespace NamelessRogue.Engine.Engine.Components.Interaction
{
    public class InputComponent : Component {
	
        public InputComponent()
        {
            Intents = new List<Intent>();
        }
        public List<Intent> Intents;

        public override IComponent Clone()
        {
           return new InputComponent(){ Intents = this.Intents};
        }
    }
}

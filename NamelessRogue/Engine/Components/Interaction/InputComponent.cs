using System.Collections.Generic;
using NamelessRogue.Engine.Input;

namespace NamelessRogue.Engine.Components.Interaction
{
    public class InputComponent : Component {
	
        public InputComponent()
        {
            Intents = new List<Intent>();
        }
        public List<Intent> Intents;

        public bool IsDelayed { get; internal set; }

        public override IComponent Clone()
        {
           return new InputComponent(){ Intents = this.Intents};
        }
    }
}

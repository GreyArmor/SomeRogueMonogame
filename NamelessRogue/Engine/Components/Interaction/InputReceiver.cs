 

namespace NamelessRogue.Engine.Components.Interaction
{
    public class InputReceiver : Component
    {
        public override IComponent Clone()
        {
            return new InputReceiver();
        }
    }
}

 

namespace NamelessRogue.Engine.Engine.Components.Interaction
{
    public class Movable : Component
    {
        public override IComponent Clone()
        {
            return new Movable();
        }
    }
}

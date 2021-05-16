 

namespace NamelessRogue.Engine.Components.Interaction
{
    public class Player : Component
    {
        public override IComponent Clone()
        {
            return new Player();
        }
    }
}

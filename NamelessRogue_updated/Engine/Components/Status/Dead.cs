 

namespace NamelessRogue.Engine.Components.Status
{
    public class Dead : Component
    {
        public override IComponent Clone()
        {
           return new Dead();
        }
    }
}

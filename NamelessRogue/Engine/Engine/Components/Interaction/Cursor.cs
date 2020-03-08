 

namespace NamelessRogue.Engine.Engine.Components.Interaction
{
    public class Cursor : Component
    {
        public override IComponent Clone()
        {
           return new Cursor();
        }
    }
}

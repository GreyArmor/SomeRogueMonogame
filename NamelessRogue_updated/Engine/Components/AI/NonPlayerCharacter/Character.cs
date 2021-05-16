 

namespace NamelessRogue.Engine.Components.AI.NonPlayerCharacter
{
    public class Character : Component
    {
        public override IComponent Clone()
        {
            return new Character();
        }
    }
}

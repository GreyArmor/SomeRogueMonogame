 

namespace NamelessRogue.Engine.Components.AI.NonPlayerCharacter
{
    public class AIControlled : Component
    {
        public override IComponent Clone()
        {
            return  new AIControlled();
        }
    }
}

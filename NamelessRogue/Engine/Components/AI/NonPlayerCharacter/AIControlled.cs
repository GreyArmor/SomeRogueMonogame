 

namespace NamelessRogue.Engine.Components.AI.NonPlayerCharacter
{

    public enum AIType : int
    {
        Idle,
        Pedestrian,
        Guard,
        Shopkeeper,
        AggressiveAttacker,
    }
    public enum Affinity
    {
        Neutral, 
        Hostile,
        Friendly
    }
    public class AIControlled : Component
    {
        public Affinity Affinity { get; set; }
        public override IComponent Clone()
        {
            return  new AIControlled();
        }
    }
}

using System.Collections.Generic;

namespace NamelessRogue.Engine.Components.AI.NonPlayerCharacter
{
    public class Character : Component
    {
        public Character(string factionId)
        {
            FactionId = factionId;
        }

        public Character(string factionId, Dictionary<string, int> factionRelations)
        {
            FactionId = factionId;
            FactionRelations = factionRelations;
        }
        public string FactionId { get; set; }
        public Dictionary<string, int> FactionRelations { get; set; } = new Dictionary<string, int>();
        public override IComponent Clone()
        {
            return new Character(FactionId);
        }
    }
}

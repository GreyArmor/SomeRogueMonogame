using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Interaction
{

    public enum Category
    {
        Generic, Improvisation, Hacking, Tech, StreetSmarts, 
    }

    public class AbilityParameters : Component
    {
        public TargetMode TargetMode { get; set; }

        public ActivationMode ActivationMode { get; set;}

        public int AreaOfEffect { get; set; } = 0;

        public bool IsActive { get; set; }

        public int Range { get; set; } = 0;

        public int CooldownTurns { get; set; } = 0;

        public int CooldownTurnsRemaining { get; set; } = 0;

        public int EnergyCost { get; set; } = 0;

        public int ActionPointsCost { get; set; } = 0;

        public List<AbilityAction> AbilityActions { get; set; } = new List<AbilityAction>();

        public AbilityParameters() { }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Interaction
{
    public enum TargetMode
    {
        None, Self, Targeted, 
    }

    public enum ActivationMode
    {
        Passive, Toggle, Activatable,
    }

    public class AbilityParameters : Component
    {
        public TargetMode TargetMode { get; set; }

        public ActivationMode ActivationMode { get; set;}

        public int AreaOfEffect { get; set; } = 0;

        public bool IsActive { get; set; }

        public AbilityParameters() { }
    }
}

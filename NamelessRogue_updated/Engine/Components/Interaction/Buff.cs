using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Interaction
{

    public enum ModifierType { 
        None,

    }

    public class Buff
    {
        public int DurationInTurns { get; set; }
        public List<ModifierType> Modifiers { get; set; } = new List<ModifierType>();
    }
}

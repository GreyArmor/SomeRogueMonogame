using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Interaction
{
    public class Buff
    {
        public int DurationInTurns { get; set; }
        public List<Modifier> Modifiers { get; set; } = new List<Modifier>();
    }
}

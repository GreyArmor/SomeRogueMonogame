using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Engine.Components.Interaction
{
    public class Buffs : Component {

        public List<Buff> Children { get; set; } = new List<Buff>();

        public override IComponent Clone()
        {
            return new Buffs();
        }
    }
}

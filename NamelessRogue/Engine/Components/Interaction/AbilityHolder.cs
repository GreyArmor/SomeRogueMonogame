using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Interaction
{
    public class AbilityHolder : Component
    {
        private List<Entity> abilities;
        public AbilityHolder()
        {
            abilities = new List<Entity>();
        }
        public List<Entity> Abilities { get { return abilities; } set { abilities = value; } }

        public override IComponent Clone()
        {
            throw new System.NotImplementedException();
        }
    }
}

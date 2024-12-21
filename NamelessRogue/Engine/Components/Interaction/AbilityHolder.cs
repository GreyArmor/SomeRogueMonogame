using NamelessRogue.Engine.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Interaction
{
    public class AbilityHolder : Component
    {
        private List<IEntity> abilities;
        public AbilityHolder()
        {
            abilities = new List<IEntity>();
        }
        public List<IEntity> Abilities { get { return abilities; } set { abilities = value; } }

        public override IComponent Clone()
        {
            throw new System.NotImplementedException();
        }
    }
}

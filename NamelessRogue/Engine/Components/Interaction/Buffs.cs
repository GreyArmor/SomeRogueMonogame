using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Interaction
{


    public class TimeModifier : Component
    {
        public int TurnsToLast { get;set; } 
    }
    public class InstantModifier : Component
    {}

    public class ModifierComponent : Component
    {}

    public class ModifiersCollection : Component {

        public ModifiersCollection(Entity accumulator)
        {
            this.Accumulator = accumulator;
        }
        public Entity Accumulator { get; set; }
        public List<IEntity> ModifierEntities { get; set; } = new List<IEntity>();

        public override IComponent Clone()
        {
            return new ModifiersCollection(Accumulator) { ModifierEntities = ModifierEntities };
        }
    }
}

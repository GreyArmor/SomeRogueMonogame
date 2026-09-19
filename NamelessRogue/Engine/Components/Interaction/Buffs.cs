using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Interaction
{


    public class TimedModifier : Component
    {
        public int TurnsToLast { get;set; } 
    }

    public class ModifierComponent : Component
    {}

    public class ModifiersCollection : Component {

        public ModifiersCollection(Entity accumulator)
        {
            this.Accumulator = accumulator;
        }
        public Entity Accumulator { get; set; }
        public List<Entity> ModifierEntities { get; set; } = new List<Entity>();

        public override IComponent Clone()
        {
            return new ModifiersCollection(Accumulator) { ModifierEntities = ModifierEntities };
        }
    }
}

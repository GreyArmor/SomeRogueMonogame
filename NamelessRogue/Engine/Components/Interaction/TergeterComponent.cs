using NamelessRogue.Engine.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NamelessRogue.Engine.Infrastructure;

namespace NamelessRogue.Engine.Components.Interaction
{
    public class TergeterComponent : Component
    {
        public int TabulationIndex { get; set; } = -1;
        public List<Entity> Targets { get; set; } = new List<Entity>();

        public int CurrentTargetingRange { get; set; } = 0;

        internal void RemoveEntity(Entity entity)
        {
            if(Targets.Contains(entity))
            {
                var currentEntity = Targets[TabulationIndex];
                var entityTabulationIndex = Targets.IndexOf(entity);
                if (entityTabulationIndex > TabulationIndex)
                {                   
                    TabulationIndex--;
                }

                Targets.Remove(entity);
            }
        }
    }
}

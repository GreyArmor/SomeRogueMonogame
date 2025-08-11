using NamelessRogue.Engine.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Interaction
{
    public class TergeterComponent : Component
    {
        public int TabulationIndex { get; set; } = -1;
        public List<IEntity> Targets { get; set; } = new List<IEntity>();

        public int CurrentTargetingRange { get; set; } = 0;

        internal void RemoveEntity(IEntity entity)
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

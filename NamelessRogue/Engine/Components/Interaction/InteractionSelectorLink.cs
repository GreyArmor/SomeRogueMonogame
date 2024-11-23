using MonoGame.Extended.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NamelessRogue.shell;
using NamelessRogue.Engine.Infrastructure;
using Entity = NamelessRogue.Engine.Infrastructure.Entity;
using NamelessRogue.Engine.Abstraction;

namespace NamelessRogue.Engine.Components.Interaction
{
    public class InteractionSelectorLink : Component
    {
        public InteractionSelectorLink(IEntity linkedEntity)
        {
            LinkedEntity = linkedEntity;
        }

        public IEntity LinkedEntity { get; }
    }
}

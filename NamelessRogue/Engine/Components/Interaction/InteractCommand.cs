
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Interaction
{
    public class InteractCommand : ICommand
    {
        public InteractCommand(Entity interactableEntity) {
            InteractableEntity = interactableEntity;
        }

        public Entity InteractableEntity { get; }
    }
}

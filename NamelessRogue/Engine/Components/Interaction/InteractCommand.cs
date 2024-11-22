using MonoGame.Extended.ECS;
using NamelessRogue.Engine.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Interaction
{
    public class InteractCommand : ICommand
    {
        public InteractCommand(IEntity interactableEntity) {
            InteractableEntity = interactableEntity;
        }

        public IEntity InteractableEntity { get; }
    }
}

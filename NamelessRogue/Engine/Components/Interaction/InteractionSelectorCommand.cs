using NamelessRogue.Engine.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NamelessRogue.Engine.Infrastructure;

namespace NamelessRogue.Engine.Components.Interaction
{
    public class InteractionSelectorCommand : ICommand
    {
        public List<Entity> InteractableEntities { get; } 
        public InteractionSelectorCommand(IEnumerable<Entity> interactableEntities)
        {
            InteractableEntities = new List<Entity>(interactableEntities);
        }
    }

    public class TerminateInteractionSelectorCommand : ICommand
    {
    }
}

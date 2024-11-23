using NamelessRogue.Engine.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Interaction
{
    public class InteractionSelectorCommand : ICommand
    {
        public List<IEntity> InteractableEntities { get; } 
        public InteractionSelectorCommand(IEnumerable<IEntity> interactableEntities)
        {
            InteractableEntities = new List<IEntity>(interactableEntities);
        }
    }

    public class TerminateInteractionSelectorCommand : ICommand
    {
    }
}

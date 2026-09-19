using NamelessRogue.Engine.Abstraction;
using System.Collections.Generic;
using NamelessRogue.Engine.Infrastructure;

namespace NamelessRogue.Engine.Components.Interaction
{
    public class StartItemPickUpDialogCommand : ICommand
    {
        public StartItemPickUpDialogCommand(IEnumerable<Entity> entitiesToPickUp)
        {
            EntitiesToPickUp = entitiesToPickUp;
        }

        public IEnumerable<Entity> EntitiesToPickUp { get; }
    }

}

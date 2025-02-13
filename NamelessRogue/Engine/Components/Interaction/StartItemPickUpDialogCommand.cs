using NamelessRogue.Engine.Abstraction;
using System.Collections.Generic;

namespace NamelessRogue.Engine.Components.Interaction
{
    public class StartItemPickUpDialogCommand : ICommand
    {
        public StartItemPickUpDialogCommand(IEnumerable<IEntity> entitiesToPickUp)
        {
            EntitiesToPickUp = entitiesToPickUp;
        }

        public IEnumerable<IEntity> EntitiesToPickUp { get; }
    }

}

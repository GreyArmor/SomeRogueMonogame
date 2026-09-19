using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Infrastructure;

namespace NamelessRogue.Engine.Components.Interaction
{
    public class StartTradeCommand : ICommand
    {
        public Entity entityToTradeWith;

        public StartTradeCommand(Entity entityToTradeWith)
        {
            this.entityToTradeWith = entityToTradeWith;
        }
    }
}

using NamelessRogue.Engine.Abstraction;

namespace NamelessRogue.Engine.Components.Interaction
{
    public class StartTradeCommand : ICommand
    {
        public IEntity entityToTradeWith;

        public StartTradeCommand(IEntity entityToTradeWith)
        {
            this.entityToTradeWith = entityToTradeWith;
        }
    }

}

using NamelessRogue.Engine.Abstraction;
using System.Collections.Generic;

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

    public class EndTradeCommand : ICommand
    {
        public EndTradeCommand(){}
    }

    public class TradeTransationCommand : ICommand
    {
        public IEntity entityToTradeWith;
        public readonly List<IEntity> itemsToGiveToPlayer;
        public readonly List<IEntity> itemsToGiveToNpc;
        public readonly int cashToTransfer;

        public TradeTransationCommand(IEntity entityToTradeWith, List<IEntity> itemsToGiveToPlayer, List<IEntity> itemsToGiveToNpc, int cashToTransfer)
        {
            this.entityToTradeWith = entityToTradeWith;
            this.itemsToGiveToPlayer = itemsToGiveToPlayer;
            this.itemsToGiveToNpc = itemsToGiveToNpc;
            this.cashToTransfer = cashToTransfer;
        }
    }

}

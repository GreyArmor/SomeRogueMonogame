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


        public TradeTransationCommand(IEntity rightEntity, IEntity leftEntity,  List<IEntity> itemsToGiveToLeftentity, List<IEntity> itemsToGiveToRight, int cashToTransferFromrightToLeft)
        {
            RightEntity = rightEntity;
            LeftEntity = leftEntity;
            ItemsToGiveToLeftEntity = itemsToGiveToLeftentity;
            ItemsToGiveToRightEntity = itemsToGiveToRight;
            CashToTransferFromRightToLeft = cashToTransferFromrightToLeft;
        }

        public IEntity RightEntity { get; }
        public IEntity LeftEntity { get; }
        public List<IEntity> ItemsToGiveToLeftEntity { get; }
        public List<IEntity> ItemsToGiveToRightEntity { get; }
        public int CashToTransferFromRightToLeft { get; }
    }

}

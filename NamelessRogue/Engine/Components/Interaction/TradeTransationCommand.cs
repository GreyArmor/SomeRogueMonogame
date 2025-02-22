using NamelessRogue.Engine.Abstraction;
using System.Collections.Generic;

namespace NamelessRogue.Engine.Components.Interaction
{
    public class TradeTransationCommand : ICommand
    {


        public TradeTransationCommand(IEntity rightEntity, IEntity leftEntity,  List<IEntity> itemsToGiveToLeftentity, List<IEntity> itemsToGiveToRight, int cashToTransferFromLeftToRight)
        {
            RightEntity = rightEntity;
            LeftEntity = leftEntity;
            ItemsToGiveToLeftEntity = itemsToGiveToLeftentity;
            ItemsToGiveToRightEntity = itemsToGiveToRight;
            CashToTransferFromLeftToRight = cashToTransferFromLeftToRight;
        }

        public IEntity RightEntity { get; }
        public IEntity LeftEntity { get; }
        public List<IEntity> ItemsToGiveToLeftEntity { get; }
        public List<IEntity> ItemsToGiveToRightEntity { get; }
        public int CashToTransferFromLeftToRight { get; }
    }

}

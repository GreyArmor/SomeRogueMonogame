using NamelessRogue.Engine.Abstraction;
using System.Collections.Generic;
using NamelessRogue.Engine.Infrastructure;

namespace NamelessRogue.Engine.Components.Interaction
{
    public class TradeTransationCommand : ICommand
    {
        public TradeTransationCommand(Entity rightEntity, Entity leftEntity,  List<Entity> itemsToGiveToLeftentity, List<Entity> itemsToGiveToRight, int cashToTransferFromLeftToRight)
        {
            RightEntity = rightEntity;
            LeftEntity = leftEntity;
            ItemsToGiveToLeftEntity = itemsToGiveToLeftentity;
            ItemsToGiveToRightEntity = itemsToGiveToRight;
            CashToTransferFromLeftToRight = cashToTransferFromLeftToRight;
        }

        public Entity RightEntity { get; }
        public Entity LeftEntity { get; }
        public List<Entity> ItemsToGiveToLeftEntity { get; }
        public List<Entity> ItemsToGiveToRightEntity { get; }
        public int CashToTransferFromLeftToRight { get; }
    }

}

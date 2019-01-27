using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Engine.Components.Interaction;
using NamelessRogue.Engine.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Engine.Components.Physical;
using NamelessRogue.Engine.Engine.Generation.World;
using NamelessRogue.Engine.Engine.Input;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Systems
{
    public class InventorySystem : ISystem
    {

        public void Update(long gameTime, NamelessGame namelessGame)
        {
            IEntity worldEntity = namelessGame.GetEntityByComponentClass<TimeLine>();
            IChunkProvider worldProvider = null;
            if (worldEntity != null)
            {
                worldProvider = worldEntity.GetComponentOfType<TimeLine>().CurrentWorldBoard.Chunks;
                foreach (IEntity entity in namelessGame.GetEntities())
                {
                    DropItemCommand dropCommand = entity.GetComponentOfType<DropItemCommand>();
                    if (dropCommand != null)
                    {
                        var tile = worldProvider.GetTile(dropCommand.WhereToDrop.X, dropCommand.WhereToDrop.Y);

                        foreach (var dropCommandItem in dropCommand.Items)
                        {
                            tile.getEntitiesOnTile().Add(dropCommandItem);
                            dropCommand.Holder.GetItems().Remove(dropCommandItem);
                        }

                        entity.RemoveComponentOfType<DropItemCommand>();
                    }

                    PickUpItemCommand pickupCommand = entity.GetComponentOfType<PickUpItemCommand>();
                    if (pickupCommand != null)
                    {

                        foreach (var pickupCommandItem in pickupCommand.Items)
                        {
                            var tile = worldProvider.GetTile(pickupCommand.WhereToPickUp.X,
                                pickupCommand.WhereToPickUp.Y);
                            tile.getEntitiesOnTile().Remove(pickupCommandItem);


                            pickupCommand.Holder.GetItems().Add(pickupCommandItem);
                        }

                        entity.RemoveComponentOfType<PickUpItemCommand>();
                    }


                }
            }
        }
    }
}
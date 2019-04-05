using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.Interaction;
using NamelessRogue.Engine.Engine.Components.Rendering;
using NamelessRogue.Engine.Engine.Generation.World;
using NamelessRogue.Engine.Engine.Infrastructure;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Systems.Inventory
{
    public class InventorySystem : ISystem
    {

        public void Update(long gameTime, NamelessGame namelessGame)
        {
            IEntity worldEntity = namelessGame.GetEntityByComponentClass<TimeLine>();
            IChunkProvider worldProvider = null;
            if (worldEntity != null)
            {
                worldProvider = worldEntity.GetComponentOfType<TimeLine>().CurrentTimelineLayer.Chunks;
                foreach (IEntity entity in namelessGame.GetEntities())
                {
                    DropItemCommand dropCommand = entity.GetComponentOfType<DropItemCommand>();
                    if (dropCommand != null)
                    {
                        var tile = worldProvider.GetTile(dropCommand.WhereToDrop.X, dropCommand.WhereToDrop.Y);

                        foreach (var dropCommandItem in dropCommand.Items)
                        {
                            tile.getEntitiesOnTile().Add((Entity) dropCommandItem);
                            dropCommand.Holder.GetItems().Remove(dropCommandItem);
                            dropCommandItem.GetComponentOfType<Drawable>().setVisible(true);
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
                            tile.getEntitiesOnTile().Remove((Entity) pickupCommandItem);


                            pickupCommand.Holder.GetItems().Add(pickupCommandItem);

                            pickupCommandItem.GetComponentOfType<Drawable>().setVisible(false);

                        }

                        entity.RemoveComponentOfType<PickUpItemCommand>();
                    }
                }
            }
        }
    }
}
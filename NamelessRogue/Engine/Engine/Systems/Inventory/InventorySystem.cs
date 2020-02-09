using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.Interaction;
using NamelessRogue.Engine.Engine.Components.Physical;
using NamelessRogue.Engine.Engine.Components.Rendering;
using NamelessRogue.Engine.Engine.Factories;
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
            IWorldProvider worldProvider = null;
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
                            var position = dropCommandItem.GetComponentOfType<Position>();
                            position.p = new Point(dropCommand.WhereToDrop.X, dropCommand.WhereToDrop.Y);
                        }

                        entity.RemoveComponentOfType<DropItemCommand>();
                        UiFactory.InventoryScreen?.FillItems(namelessGame);
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
                            UiFactory.InventoryScreen?.FillItems(namelessGame);
                            UiFactory.PickUpItemsScreen?.FillItems(namelessGame);
                        }

                        entity.RemoveComponentOfType<PickUpItemCommand>();
                    }
                }
            }
        }
    }
}
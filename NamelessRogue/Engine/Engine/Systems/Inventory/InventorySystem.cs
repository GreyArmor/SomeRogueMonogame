using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.Interaction;
using NamelessRogue.Engine.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Engine.Components.Physical;
using NamelessRogue.Engine.Engine.Components.Rendering;
using NamelessRogue.Engine.Engine.Factories;
using NamelessRogue.Engine.Engine.Generation.World;
using NamelessRogue.Engine.Engine.Infrastructure;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Systems.Inventory
{
    public class InventorySystem : BaseSystem
    {
        public InventorySystem()
        {
            Signature = new HashSet<Type>();
            Signature.Add(typeof(DropItemCommand));
            Signature.Add(typeof(PickUpItemCommand));
        }

        public override bool IsEntityMatchesSignature(IEntity entity)
        {
            var entityComponentTypes = new HashSet<Type>(entity.GetAllComponents().Select(x => x.GetType()));

            foreach (var type in Signature)
            {
                if (entityComponentTypes.Contains(type))
                {
                    return true;
                }
            }
            return false;
        }


        public override HashSet<Type> Signature { get; }

        public override void Update(long gameTime, NamelessGame namelessGame)
        {
            IEntity worldEntity = namelessGame.TimelineEntity;
            IWorldProvider worldProvider = null;
            if (worldEntity != null)
            {
                worldProvider = worldEntity.GetComponentOfType<TimeLine>().CurrentTimelineLayer.Chunks;
                foreach (IEntity entity in RegisteredEntities.ToList())
                {
                    DropItemCommand dropCommand = entity.GetComponentOfType<DropItemCommand>();
                    if (dropCommand != null)
                    {
                        var tile = worldProvider.GetTile(dropCommand.WhereToDrop.X, dropCommand.WhereToDrop.Y);

                        foreach (var dropCommandItem in dropCommand.Items)
                        {
                            tile.AddEntity((Entity)dropCommandItem);
                            dropCommand.Holder.GetItems().Remove(dropCommandItem);
                            dropCommandItem.GetComponentOfType<Drawable>().setVisible(true);
                            var position = dropCommandItem.GetComponentOfType<Position>();
                            position.p = new Point(dropCommand.WhereToDrop.X, dropCommand.WhereToDrop.Y);
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
                            tile.RemoveEntity((Entity) pickupCommandItem);
                            pickupCommandItem.GetComponentOfType<Drawable>().setVisible(false);
                            var ammo = pickupCommandItem.GetComponentOfType<Ammo>();
                            if (ammo != null)
                            {
                                var itemsEntities = pickupCommand.Holder.GetItems();
                                var itemsWithAmmo = itemsEntities.Select(x=>x).Where(i => i.GetComponentOfType<Ammo>() != null);
                                var sameTypeItem = itemsWithAmmo.FirstOrDefault(x => x.GetComponentOfType<Ammo>().Type.Name == ammo.Type.Name);
                                if (sameTypeItem != null)
                                {
                                    sameTypeItem.GetComponentOfType<Item>().Amount +=
                                        pickupCommandItem.GetComponentOfType<Item>().Amount;
                                    namelessGame.RemoveEntity(pickupCommandItem);
                                }
                                else
                                {
                                    pickupCommand.Holder.GetItems().Add(pickupCommandItem);
                                }
                            }
                            else
                            {
                                pickupCommand.Holder.GetItems().Add(pickupCommandItem);
                            }
                        }

                        entity.RemoveComponentOfType<PickUpItemCommand>();
                    }
                }
            }
        }
    }
}
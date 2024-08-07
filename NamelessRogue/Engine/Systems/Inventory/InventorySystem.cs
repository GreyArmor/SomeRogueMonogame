using System;
using System.Collections.Generic;
using System.Linq;

using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Utility;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Systems.Inventory
{
    public class InventorySystem : BaseSystem
    {
        public InventorySystem()
        {
            Signature = new HashSet<Type>();
        }

        public override HashSet<Type> Signature { get; }

        public override void Update(GameTime gameTime, NamelessGame game)
        {
            IEntity worldEntity = game.TimelineEntity;
            IWorldProvider worldProvider = null;
            if (worldEntity != null)
            {
                worldProvider = worldEntity.GetComponentOfType<TimeLine>().CurrentTimelineLayer.Chunks;
                while (game.Commander.DequeueCommand(out DropItemCommand dropCommand))
                {
                    var tile = worldProvider.GetTile(dropCommand.WhereToDrop.X, dropCommand.WhereToDrop.Y);

                    foreach (var dropCommandItem in dropCommand.Items)
                    {
                        tile.AddEntity((Entity)dropCommandItem);
                        dropCommand.Holder.Items.Remove(dropCommandItem);
                        dropCommandItem.GetComponentOfType<Drawable>().Visible = true;
                        var position = dropCommandItem.GetComponentOfType<Position>();
                        position.Point = new Point(dropCommand.WhereToDrop.X, dropCommand.WhereToDrop.Y);
                    }
                }
                while (game.Commander.DequeueCommand(out PickUpItemCommand pickupCommand))
                {
                    if (pickupCommand != null)
                    {
                        foreach (var pickupCommandItem in pickupCommand.Items)
                        {
                            var tile = worldProvider.GetTile(pickupCommand.WhereToPickUp.X,
                                pickupCommand.WhereToPickUp.Y);
                            tile.RemoveEntity((Entity) pickupCommandItem);
                            pickupCommandItem.GetComponentOfType<Drawable>().Visible = false;
                            var ammo = pickupCommandItem.GetComponentOfType<Ammo>();
                            if (ammo != null)
                            {
                                var itemsEntities = pickupCommand.Holder.Items;
                                var itemsWithAmmo = itemsEntities.Select(x=>x).Where(i => i.GetComponentOfType<Ammo>() != null);
                                var sameTypeItem = itemsWithAmmo.FirstOrDefault(x => x.GetComponentOfType<Ammo>().Type.Name == ammo.Type.Name);
                                if (sameTypeItem != null)
                                {
                                    sameTypeItem.GetComponentOfType<Item>().Amount +=
                                        pickupCommandItem.GetComponentOfType<Item>().Amount;
                                    game.RemoveEntity(pickupCommandItem);
                                }
                                else
                                {
                                    pickupCommand.Holder.Items.Add(pickupCommandItem);
                                }
                            }
                            else
                            {
                                pickupCommand.Holder.Items.Add(pickupCommandItem);
                            }
                        }
                    }
                }
            }
        }
    }
}
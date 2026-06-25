using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Components.UI;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Systems.Ingame;
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

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            IEntity worldEntity = namelessGame.WorldTemplateEntity;
            IWorldProvider worldProvider = null;
            if (worldEntity != null)
            {
                worldProvider = worldEntity.GetComponentOfType<WorldTemplate>().WorldMap.Chunks;
                while (namelessGame.Commander.DequeueCommand(out DropItemCommand dropCommand))
                {
                    var tile = worldProvider.GetTile(dropCommand.WhereToDrop.X, dropCommand.WhereToDrop.Y, 0);

                    foreach (var dropCommandItem in dropCommand.Items)
                    {
                        tile.AddEntity((Entity)dropCommandItem);
                        dropCommand.Holder.Items.Remove(dropCommandItem);
                        dropCommandItem.GetComponentOfType<Drawable>().Visible = true;
                        var position = dropCommandItem.GetComponentOfType<Position>();
                        position.Point = new Utility.Vector3Int(dropCommand.WhereToDrop.X, dropCommand.WhereToDrop.Y, 0);
                    }
                }

                while (namelessGame.Commander.DequeueCommand(out PickUpItemCommand pickupCommand))
                {
                    if (pickupCommand != null)
                    {
                        if(pickupCommand.Items.Count()>1 && pickupCommand.CallMultipleChoiceDialog)
                        {                    
                            var chatCommand = new StartItemPickUpDialogCommand(pickupCommand.Items);
                            namelessGame.Commander.EnqueueCommand(chatCommand);
                            namelessGame.ContextToSwitch = ContextFactory.GetPickupItemsDialogContext(namelessGame);
                        }
                        else
                        {
                            foreach (var pickupCommandItem in pickupCommand.Items)
                            {
                                var itemPosition = pickupCommandItem.GetComponentOfType<Position>().Point;

                                var tile = worldProvider.GetTile(itemPosition.X,
                                    itemPosition.Y, itemPosition.Z);
                                tile.RemoveEntity((Entity)pickupCommandItem);
                                pickupCommandItem.GetComponentOfType<Drawable>().Visible = false;
                                var ammo = pickupCommandItem.GetComponentOfType<Ammo>();
                                if (ammo != null)
                                {
                                    var itemsEntities = pickupCommand.Holder.Items;
                                    var itemsWithAmmo = itemsEntities.Select(x => x).Where(i => i.GetComponentOfType<Ammo>() != null);
                                    //var sameTypeItem = itemsWithAmmo.FirstOrDefault(x => x.GetComponentOfType<Ammo>().Type.Name == ammo.Type.Name);
                                    //if (sameTypeItem != null)
                                    //{
                                    //    sameTypeItem.GetComponentOfType<Item>().Amount +=
                                    //        pickupCommandItem.GetComponentOfType<Item>().Amount;
                                    //    namelessGame.RemoveEntity(pickupCommandItem);
                                    //}
                                    //else
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
}
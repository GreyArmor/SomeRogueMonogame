using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Environment;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Systems.Ingame;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Systems
{
    internal class InteractSystem : BaseSystem
    {
        public override HashSet<Type> Signature { get; } = new HashSet<Type>() { typeof(InteractionSelectorLink) };

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {

            while (namelessGame.Commander.DequeueCommand(out TerminateInteractionSelectorCommand command))
            {
                var selectorEntities = RegisteredEntities.ToList();
                foreach (var entity in selectorEntities)
                {
                    namelessGame.RemoveEntity(entity);
                }
            }

            while (namelessGame.Commander.DequeueCommand(out InteractionSelectorCommand command))
            {
                foreach (var entity in command.InteractableEntities)
                {
                    var selectorEntity = new Entity();
                    selectorEntity.AddComponent(new InteractionSelectorLink(entity));
                    Drawable dr = new Drawable("Cursor", new Engine.Utility.Color(0.9, 0.9, 0.9));
                    dr.Visible = true;
                    selectorEntity.AddComponent(dr);

                    var entityPositionClone = (Position)entity.GetComponentOfType<Position>().Clone();
                    selectorEntity.AddComponent(entityPositionClone);

                    namelessGame.WorldProvider.AddEntityToNewLocation(selectorEntity, entityPositionClone.X, entityPositionClone.Y, entityPositionClone.Z);

                }
            }

            while (namelessGame.Commander.DequeueCommand(out InteractCommand command))
            {
                //we need to remove all selector squares after we made a choice
                var selectorEntities = RegisteredEntities.ToList();
                foreach (var entity in selectorEntities)
                {
                    namelessGame.RemoveEntity(entity);
                }         

                var interactionEntity = command.InteractableEntity;
                var interactionEntityPosition = command.InteractableEntity.GetComponentOfType<Position>();
                var door = interactionEntity.GetComponentOfType<Door>();
                var item = interactionEntity.GetComponentOfType<Item>();
                if (door != null)
                {
                    SimpleSwitch simpleSwitch = interactionEntity.GetComponentOfType<SimpleSwitch>();
                    if (simpleSwitch != null == simpleSwitch.isSwitchActive())
                    {
                        interactionEntity.GetComponentOfType<Drawable>()
                            .ObjectID = "openDoor";
                        interactionEntity.RemoveComponentOfType<BlocksVision>();
                        interactionEntity.RemoveComponentOfType<OccupiesTile>();

                        namelessGame.Commander.EnqueueCommand(
                            new ChangeSwitchStateCommand(simpleSwitch, false));
                        var ap = namelessGame.PlayerEntity.GetComponentOfType<ActionPoints>();
                        ap.Points -= Constants.ActionsMovementCost;
                    }
                    else if (simpleSwitch != null == !simpleSwitch.isSwitchActive())
                    {
                        interactionEntity.GetComponentOfType<Drawable>()
                            .ObjectID = "door";
                        interactionEntity.AddComponent(new BlocksVision());
                        interactionEntity.AddComponent(new OccupiesTile());

                        namelessGame.Commander.EnqueueCommand(
                            new ChangeSwitchStateCommand(simpleSwitch, true));
                        var ap = namelessGame.PlayerEntity.GetComponentOfType<ActionPoints>();
                        ap.Points -= Constants.ActionsMovementCost;
                    }
                    namelessGame.Commander.EnqueueCommand(new PlaySoundCommand("DoorOpen", false, 0.1f));
                    namelessGame.Commander.EnqueueCommand(new UpdateVisualChunkCommand(new Utility.Vector3Int(interactionEntityPosition.X/Constants.ChunkSize, interactionEntityPosition.Y/Constants.ChunkSize, interactionEntityPosition.Z)));


                }


                if(item!=null)
                {
                    var tile = namelessGame.WorldProvider.GetTile(interactionEntityPosition.X, interactionEntityPosition.Y, interactionEntityPosition.Z);
                    var tileEntities = tile.GetEntities();
                    var tileItems = new List<IEntity>();
                    foreach (var tileEntity in tileEntities)
                    {
                        var interactionItem = tileEntity.GetComponentOfType<Item>();
                        if (interactionItem != null)
                        {
                            tileItems.Add(tileEntity);
                        }
                    }

                    namelessGame.Commander.EnqueueCommand(new PickUpItemCommand(tileItems, namelessGame.PlayerEntity.GetComponentOfType<ItemsHolder>(), true));
                }
            }
        }
    }
}

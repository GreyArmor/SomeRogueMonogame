using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Components.Environment;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Infrastructure;
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

                var door = interactionEntity.GetComponentOfType<Door>();
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
                }
            }
        }
    }
}

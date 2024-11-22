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
        public override HashSet<Type> Signature { get; } = new HashSet<Type>();

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            while (namelessGame.Commander.DequeueCommand(out InteractCommand command))
            {
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
                }
            }
        }
    }
}

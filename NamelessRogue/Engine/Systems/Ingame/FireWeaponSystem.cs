using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Components.AI.NonPlayerCharacter;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Systems.Ingame
{
    internal class FireWeaponSystem : BaseSystem
    {
        public override HashSet<Type> Signature { get; } = new HashSet<Type>();

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            while (namelessGame.Commander.DequeueCommand(out FireWeaponCommand command))
            {
                var sourceEntity = command.Source;
                var sourcePosition = sourceEntity.GetComponentOfType<Position>();
                var cursorPosition = command.Target;
                var tile = namelessGame.WorldProvider.GetTile(cursorPosition.X, cursorPosition.Y, cursorPosition.Z);
                if (tile.AnyEntities())
                {
                    var tileEntity = tile.GetEntities().FirstOrDefault();
                    if (tileEntity != null && tileEntity.Id != sourceEntity.Id)
                    {
                        var character = tile.GetEntities().FirstOrDefault(x => x.GetComponentOfType<Character>() != null);
                        if (character != null)
                        {
                            var combatCommand = new AttackCommand(sourceEntity, character);
                            namelessGame.Commander.EnqueueCommand(combatCommand);

                            AttachToTargetCommand snapToTarget = new AttachToTargetCommand(tileEntity);
                            namelessGame.Commander.EnqueueCommand(snapToTarget);
                        }
                    }
                }
                var createProjectileCommand = new CreateProjectileCommand(sourcePosition.Point, cursorPosition, DamageType.Ballistic);
                namelessGame.Commander.EnqueueCommand(createProjectileCommand);

                var ap = sourceEntity.GetComponentOfType<ActionPoints>();
                ap.Points -= Constants.ActionsMovementCost;
            }
        }
    }
}

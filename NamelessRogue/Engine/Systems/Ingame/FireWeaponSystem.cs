using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Components.AI.NonPlayerCharacter;
using NamelessRogue.Engine.Components.Environment;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Utility;
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
                var projectileTarget = cursorPosition;
                var firstEntity = TargetingHelper.GetObjectsAlongPath(namelessGame.WorldProvider, sourcePosition.Point, cursorPosition, true).FirstOrDefault();

                var firstEntityPosition = sourceEntity.GetComponentOfType<Position>();

                var tile = namelessGame.WorldProvider.GetTile(cursorPosition.X, cursorPosition.Y, cursorPosition.Z);
                if (tile.AnyEntities())
                {
                    foreach (var tileEntity in tile.GetEntities())
                    {
                        var character = tileEntity.GetComponentOfType<Character>();
                        if(character!=null)
                        {
                            AttachToTargetCommand snapToTarget = new AttachToTargetCommand(firstEntity);
                            namelessGame.Commander.EnqueueCommand(snapToTarget);
                            break;
                        }
                    }
                }

                if (firstEntity != null)
                {
                    var character = firstEntity.GetComponentOfType<Character>();
                    var furniture = firstEntity.GetComponentOfType<Furniture>();
                    if (character != null && character.Id != sourceEntity.Id)
                    {
                        var combatCommand = new AttackCommand(sourceEntity, firstEntity);
                        namelessGame.Commander.EnqueueCommand(combatCommand);           
                    }
                    //put furniture damage here
                    if (furniture != null)
                    {
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

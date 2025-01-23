using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.AI.Pathfinder;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NamelessRogue.Engine.Systems.Ingame
{
    internal class AbilitySystem : BaseSystem
    {
        public override HashSet<Type> Signature { get; } = new HashSet<Type>();

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            while (namelessGame.Commander.DequeueCommand(out ActivateTargetedAbilityCommand command))
            {
                var abilityParameters = command.Ability.GetComponentOfType<AbilityParameters>();
                var abilityBuffs = command.Ability.GetComponentOfType<AssociatedBuffs>();

                abilityParameters.CooldownTurnsRemaining = abilityParameters.CooldownTurns;

                var cursorEntity = namelessGame.CursorEntity;
                var cursorPos = cursorEntity.GetComponentOfType<Position>();

                if (abilityParameters.TargetMode == TargetMode.Targeted || abilityParameters.TargetMode == TargetMode.TargetFriends || abilityParameters.TargetMode == TargetMode.TargetEnemies)
                {
                    foreach (var abilityAction in abilityParameters.AbilityActions)
                    {
                        ProcessAbilityAction(namelessGame, command, cursorPos, abilityAction);
                    }
                    if (abilityBuffs != null)
                    {
                        IEntity target = GetTargetOnSite(namelessGame, cursorPos);
                        if (target != null)
                        {
                            var modifiers = target.GetComponentOfType<ModifiersCollection>();
                            if (modifiers != null)
                            {
                                foreach (string id in abilityBuffs.BuffIds)
                                {
                                    var buff = BuffLibrary.CreateBuffFromData(namelessGame, BuffLibrary.DataById[id]);
                                    target.GetComponentOfType<ModifiersCollection>().ModifierEntities.Add(buff);
                                }
                            }
                        }
                    }
                }
            }

            while (namelessGame.Commander.DequeueCommand(out ActivateSelfAbilityCommand command))
            {
                var abilityParameters = command.Ability.GetComponentOfType<AbilityParameters>();
                var abilityBuffs = command.Ability.GetComponentOfType<AssociatedBuffs>();
                abilityParameters.CooldownTurnsRemaining = abilityParameters.CooldownTurns;

                if (abilityParameters.TargetMode == TargetMode.Self)
                {

                    var player = namelessGame.PlayerEntity;
                    var playerPos = player.GetComponentOfType<Position>();

                    foreach (var abilityAction in abilityParameters.AbilityActions)
                    {
                        ProcessAbilityAction(namelessGame, command, playerPos, abilityAction);
                    }

                    if (abilityBuffs != null)
                    {
                        IEntity target = command.Source;
                        foreach (string id in abilityBuffs.BuffIds)
                        {
                            var buff = BuffLibrary.CreateBuffFromData(namelessGame, BuffLibrary.DataById[id]);
                            target.GetComponentOfType<ModifiersCollection>().ModifierEntities.Add(buff);
                        }
                    }
                }
            }
        }

        private static void ProcessAbilityAction(NamelessGame namelessGame, IActivatedAbilityCommand command, Position targetPosition, AbilityAction abilityAction)
        {
            switch (abilityAction)
            {
                case AbilityAction.JumpToTarget:
                    AbilityLogicLibrary.Jump(command.Source, targetPosition.Point);
                    break;
                case AbilityAction.JumpBesidesTarget:

                    var sourcePosition = command.Source.GetComponentOfType<Position>();

                    var sourceV2 = sourcePosition.Point.ToPoint().ToVector2();

                    var neighbors = AllNeighborProviderFlowfield.GetNeighbors(targetPosition.Point.ToPoint());

                    var closestNeighbor = neighbors.OrderBy(neighbor => (neighbor.ToVector2() - sourceV2).LengthSquared()).First();

                    AbilityLogicLibrary.Jump(command.Source, new Utility.Vector3Int(closestNeighbor.X, closestNeighbor.Y, targetPosition.Z));

                    break;
                case AbilityAction.AttackTargetMelee:
                case AbilityAction.AttackTargetRanged:
                    IEntity entityThatOccupiedTile = null;
                    entityThatOccupiedTile = GetTargetOnSite(namelessGame, targetPosition);
                    if (entityThatOccupiedTile != null)
                    {
                        namelessGame.Commander.EnqueueCommand(new AttackCommand(command.Source, entityThatOccupiedTile));
                    }
                    break;
                case AbilityAction.StartFire:
                    break;
            }
        }

        private static IEntity GetTargetOnSite(NamelessGame namelessGame, Position cursorPos)
        {
            IEntity entityThatOccupiedTile = null;
            Tile tile = namelessGame.WorldProvider.GetTile(cursorPos.Point.X, cursorPos.Point.Y, cursorPos.Point.Z);
            foreach (IEntity tileEntity in tile.GetEntities())
            {
                OccupiesTile occupiesTile = tileEntity.GetComponentOfType<OccupiesTile>();

                if (occupiesTile != null)
                {
                    entityThatOccupiedTile = tileEntity;
                    break;
                }
            }

            return entityThatOccupiedTile;
        }
    }
}

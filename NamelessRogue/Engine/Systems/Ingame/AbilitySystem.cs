using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.AI.Pathfinder;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

                if(abilityParameters.TargetMode == TargetMode.Targeted || abilityParameters.TargetMode == TargetMode.TargetFriends || abilityParameters.TargetMode == TargetMode.TargetEnemies)
                {
                    var cursorEntity = namelessGame.CursorEntity;
                    var cursorPos = cursorEntity.GetComponentOfType<Position>();

                    foreach (var abilityAction in abilityParameters.AbilityActions)
                    {
                        switch (abilityAction)
                        {
                            case AbilityAction.JumpToTarget:
                                AbilityLogicLibrary.Jump(command.Source, cursorPos.Point);
                                break;
                            case AbilityAction.JumpBesidesTarget:

                                var sourcePosition = command.Source.GetComponentOfType<Position>();

                                var sourceV2 = sourcePosition.Point.ToPoint().ToVector2();

                                var neighbors = AllNeighborProviderFlowfield.GetNeighbors(cursorPos.Point.ToPoint());

                                var closestNeighbor = neighbors.OrderBy(neighbor => (neighbor.ToVector2()- sourceV2).LengthSquared()).First();

                                AbilityLogicLibrary.Jump(command.Source, new Utility.Vector3Int(closestNeighbor.X, closestNeighbor.Y, cursorPos.Z));

                                break;
                            case AbilityAction.AttackTargetMelee:
                            case AbilityAction.AttackTargetRanged:
                                IEntity entityThatOccupiedTile = null;
                                Tile tile = namelessGame.WorldProvider.GetTile(cursorPos.Point.X, cursorPos.Point.Y, cursorPos.Point.Z);
                                foreach (IEntity tileEntity in tile.GetEntities())
                                {
                                    OccupiesTile occupiesTile =
                                        tileEntity.GetComponentOfType<OccupiesTile>();

                                    if (occupiesTile != null)
                                    {
                                        entityThatOccupiedTile = tileEntity;
                                        break;
                                    }
                                }
                                if (entityThatOccupiedTile!=null)
                                {
                                    namelessGame.Commander.EnqueueCommand(new AttackCommand(command.Source, entityThatOccupiedTile));
                                }                           
                                break;
                            case AbilityAction.StartFire:
                                break;
                        }
                    }


                   
                }
            }


            while (namelessGame.Commander.DequeueCommand(out ActivateSelfAbilityCommand command))
            {
                var abilityParameters = command.Ability.GetComponentOfType<AbilityParameters>();

                if (abilityParameters.TargetMode == TargetMode.Targeted)
                {
                    var player = namelessGame.PlayerEntity;
                    var playerPos = player.GetComponentOfType<Position>();
                                      
                }
            }
        }
    }
}

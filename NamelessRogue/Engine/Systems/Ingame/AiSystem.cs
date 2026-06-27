using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.AI.NonPlayerCharacter;
using NamelessRogue.Engine.Components.AI.Pathfinder;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Components.Environment;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.Engine.Components.Status;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Utility;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NamelessRogue.Engine.Systems.Ingame
{
    public class AiSystem : BaseSystem
    {

        public AiSystem()
        {
            Signature = new HashSet<Type>();
            Signature.Add(typeof(AIControlled));
            Signature.Add(typeof(ActionPoints));
        }


        public override HashSet<Type> Signature { get; }

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            var playerEntity = namelessGame.PlayerEntity;
            //if (namelessGame.TurnUpdated)
            //{
            //    foreach (IEntity entity in this.RegisteredEntities)
            //    {
            //        var npcAP = entity.GetComponentOfType<ActionPoints>();
            //        npcAP.Points = 100;
            //    }
            //}

            IEntity worldEntity = namelessGame.WorldTemplateEntity;          
            IWorldProvider worldProvider = null;
            WorldTemplate worldTemplate = null;
            if (worldEntity != null)
            {
                worldTemplate = worldEntity.GetComponentOfType<WorldTemplate>();
                worldProvider = worldTemplate.WorldMap.Chunks;               
            }

            if (worldProvider != null)
            {
                foreach (IEntity entity in this.RegisteredEntities)
                {
                    AIControlled ac = entity.GetComponentOfType<AIControlled>();
                    Dead dead = entity.GetComponentOfType<Dead>();

                    var stats = entity.GetComponentOfType<CharacterStats>();

                    var actionPoints = entity.GetComponentOfType<ActionPoints>();
                    if (dead == null && actionPoints.Points >= 0)
                    {
                        CharacterStats npcStats = entity.GetComponentOfType<CharacterStats>();

                        FollowPlayerAi basicAi = entity.GetComponentOfType<FollowPlayerAi>();
                        HostileTurretAI hostileTurretAI = entity.GetComponentOfType<HostileTurretAI>();
                        FollowShootPlayerAi followShootPlayerAi = entity.GetComponentOfType<FollowShootPlayerAi>();
                        OscillatorMovementAI oscillatorMovementAI = entity.GetComponentOfType<OscillatorMovementAI>();
                        PedestrianMovementAi pedestrianMovementAI = entity.GetComponentOfType<PedestrianMovementAi>();
                        Position playerPosition = namelessGame.PlayerEntity
                            .GetComponentOfType<Position>();


                        if (oscillatorMovementAI != null)
                        {
                            Point toPoint = oscillatorMovementAI.To.ToPoint();
                            Point fromPoint = oscillatorMovementAI.From.ToPoint();
                            var entityPos = entity.GetComponentOfType<Position>().Point;
                            var flowMoveComponent = entity.GetComponentOfType<FlowMoveComponent>();
                            if (flowMoveComponent.FinishedMoving)
                            {
                                entity.GetComponentOfType<ActionPoints>().Points = -200;
                                var pathId = oscillatorMovementAI.MovesToTarget ?
                                    namelessGame.FlowFieldController.CalculateTo(toPoint, fromPoint) :
                                    namelessGame.FlowFieldController.CalculateTo(fromPoint, toPoint);
                                if (pathId > -1)
                                {
                                    flowMoveComponent.To = oscillatorMovementAI.MovesToTarget ? toPoint : fromPoint;
                                    flowMoveComponent.PathId = pathId;
                                    flowMoveComponent.FinishedMoving = false;                                  
                                }
                            }
                            if (!flowMoveComponent.FinishedMoving)
                            {
                                var nextPoint = namelessGame.FlowFieldController.GetNextPoint(flowMoveComponent.PathId, entityPos.ToPoint());
                                if (flowMoveComponent.To == nextPoint)
                                {
                                    flowMoveComponent.FinishedMoving = true;
                                    oscillatorMovementAI.MovesToTarget = !oscillatorMovementAI.MovesToTarget;
                                    //continue;
                                }
                                namelessGame.WorldProvider.MoveEntity(entity,
                                  nextPoint.X, nextPoint.Y, 0);
                                entity.GetComponentOfType<ActionPoints>().Points = -200;
                            }
                        }
                        else
                        if (pedestrianMovementAI != null && namelessGame.MacroNavigator.Locations.Any())
                        {

                         

                            var entityPos = entity.GetComponentOfType<Position>().Point;
                            var chunkPos = new Vector3Int(entityPos.X / Constants.ChunkSize, entityPos.Y / Constants.ChunkSize, 0);
                            
                        
                            var flowMoveComponent = entity.GetComponentOfType<FlowMoveComponent>();
                            if (flowMoveComponent.FinishedMoving)
                            {
                                entity.GetComponentOfType<ActionPoints>().Points = -200;
                                if (flowMoveComponent.CurrentMacroNode != null)
                                {
                                    var waypoints = flowMoveComponent.CurrentMacroNode.Neighbors;
                                    var closestWaypoint = waypoints[Random.Shared.Next(waypoints.Count)];
                                    var pathId = closestWaypoint.FlowFieldId;
                                    if (pathId > -1)
                                    {
                                        flowMoveComponent.To = closestWaypoint.RealityPosition.ToPoint();
                                        flowMoveComponent.PathId = pathId;
                                        flowMoveComponent.FinishedMoving = false;
                                        flowMoveComponent.CurrentMacroNode = closestWaypoint;
                                    }
                                }
                                else
                                {
                                    var closestLocation = namelessGame.MacroNavigator.Locations.Where(l => l.BoundingBox.Contains(new Vector3(entityPos.X, entityPos.Y, 0)) != ContainmentType.Disjoint).
                                        FirstOrDefault(x=>x.Nodes.Any());

                                    if (closestLocation == null || closestLocation.Nodes.Count == 0)
                                    {
                                        entity.RemoveComponentOfType<AIControlled>();
                                        continue;
                                    }
                                    var waypoints = closestLocation.Nodes;                                  
                                    var closestWaypoint = waypoints.Where(x => x.RealityPosition != entityPos).OrderBy(w => (w.RealityPosition - entityPos).Length()).FirstOrDefault();
                                    var pathId = closestWaypoint.FlowFieldId;
                                    if (pathId > -1)
                                    {
                                        flowMoveComponent.To = closestWaypoint.RealityPosition.ToPoint();
                                        flowMoveComponent.PathId = pathId;
                                        flowMoveComponent.FinishedMoving = false;
                                        flowMoveComponent.CurrentMacroNode = closestWaypoint;
                                    }
                                }

                               // namelessGame.FlowFieldController.CurrentPathModels[flowMoveComponent.PathId].FullPath.ClearDebug();
                              //  namelessGame.FlowFieldController.CurrentPathModels[flowMoveComponent.PathId].FullPath.DrawDebug();

                            }
                            else
                            if (!flowMoveComponent.FinishedMoving)
                            {
 
                                try
                                {
                                    var nextPoint = namelessGame.FlowFieldController.GetNextPoint(flowMoveComponent.PathId, entityPos.ToPoint());
                                    if (flowMoveComponent.To == nextPoint)
                                    {
                                        flowMoveComponent.FinishedMoving = true;
                                        namelessGame.FlowFieldController.CurrentPathModels[flowMoveComponent.PathId].FullPath.ClearDebug();
                                        //continue;
                                    }
                                    namelessGame.WorldProvider.MoveEntitySwapCharacters(entity,
                                      nextPoint.X, nextPoint.Y, 0);
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine($"Error moving entity {entity.Id}: {ex.Message}");
                                    Debug.WriteLine($"entityPos {entityPos}");
                                    Debug.WriteLine($"flowMoveComponent.To {flowMoveComponent.To}");
                                    Debug.WriteLine($"flowMoveComponent.CurrentMacroNode.FlowFieldId {flowMoveComponent.CurrentMacroNode.FlowFieldId}");
                                    Debug.WriteLine($"flowMoveComponent.CurrentMacroNode.RealityPosition {flowMoveComponent.CurrentMacroNode.RealityPosition}");
                                    flowMoveComponent.FinishedMoving = true;
                                    entity.RemoveComponentOfType<AIControlled>();
                                }
                                entity.GetComponentOfType<ActionPoints>().Points = -200;
                            }
                        }
                        //else
                        //if (basicAi != null)
                        //{
                        //    switch (basicAi.State)
                        //    {
                        //        case BasicAiStates.Idle:
                        //        case BasicAiStates.Moving:
                        //            var pPos = playerPosition.Point;
                        //            MoveTo(entity, namelessGame, new Point(pPos.X, pPos.Y), true, basicAi);
                        //            var route = basicAi.Route;
                        //            if (route.Count == 0)
                        //            {
                        //                basicAi.State = (BasicAiStates.Idle);
                        //            }
                        //            break;
                        //        default:
                        //            break;

                        //    }
                        //}
                        //else if (hostileTurretAI != null)
                        //{
                        //    switch (hostileTurretAI.State)
                        //    {
                        //        case HostileTurretState.Idle:
                        //            {
                        //                var pPos = playerPosition.Point;
                        //                var entityPos = entity.GetComponentOfType<Position>().Point;
                        //                var distance = (pPos - entityPos).Length();

                        //                var visionRange = 6;
                        //                if (distance <= visionRange)
                        //                {
                        //                    hostileTurretAI.Target = playerEntity;
                        //                    hostileTurretAI.State = HostileTurretState.Attacking;
                        //                    goto case HostileTurretState.Attacking;
                        //                }
                        //            }
                        //            break;
                        //        case HostileTurretState.Attacking:
                        //            {
                        //                var targetPos = hostileTurretAI.Target.GetComponentOfType<Position>().Point;
                        //                var entityPos = entity.GetComponentOfType<Position>().Point;
                        //                var visible = TargetingHelper.IsTargetVisible(worldProvider, targetPos, entityPos, npcStats);
                        //                if (visible)
                        //                {
                        //                    FireWeaponCommand command = new FireWeaponCommand(entity, targetPos);
                        //                    namelessGame.Commander.EnqueueCommand(command);

                        //                    entity.GetComponentOfType<ActionPoints>().Points = -100;
                        //                }

                        //            }
                        //            break;
                        //    }
                        //}

                        //else if (followShootPlayerAi != null)
                        //{
                        //    switch (followShootPlayerAi.State)
                        //    {
                        //        case ShooterAiStates.Idle:
                        //            {
                        //                var pPos = playerPosition.Point;
                        //                var entityPos = entity.GetComponentOfType<Position>().Point;
                        //                var visible = TargetingHelper.IsTargetVisible(worldProvider, pPos, entityPos, npcStats);
                        //                if (visible)
                        //                {
                        //                    followShootPlayerAi.Target = playerEntity;
                        //                    followShootPlayerAi.State = ShooterAiStates.Aiming;
                        //                    namelessGame.Commander.EnqueueCommand(new PlayCharacterAnimationForNumberOfLoopsCommand(entity, AnimationType.TakeAim, 1));
                        //                    namelessGame.Commander.EnqueueCommand(new LockIdleAnimationCommand(entity, AnimationType.Aiming));
                        //                    goto case ShooterAiStates.Aiming;
                        //                }
                        //            }
                        //            break;
                        //        case ShooterAiStates.Aiming:
                        //            {
                        //                var targetPos = followShootPlayerAi.Target.GetComponentOfType<Position>().Point;
                        //                var entityPos = entity.GetComponentOfType<Position>().Point;
                        //                var visible = TargetingHelper.IsTargetVisible(worldProvider, targetPos, entityPos, npcStats);
                        //                var distance = (targetPos - entityPos).Length();

                        //                if (!visible)
                        //                {

                        //                    MoveTo(entity, namelessGame, new Point(targetPos.X, targetPos.Y), true, followShootPlayerAi);
                        //                    var route = followShootPlayerAi.Route;
                        //                    if (route.Count == 0)
                        //                    {
                        //                        followShootPlayerAi.State = ShooterAiStates.Idle;
                        //                    }

                        //                    //followShootPlayerAi.State = ShooterAiStates.Idle;
                        //                    //followShootPlayerAi.ShootingTarget = targetPos;
                        //                    namelessGame.Commander.EnqueueCommand(new LockIdleAnimationCommand(entity, AnimationType.Aiming));
                        //                }
                        //                else
                        //                {
                        //                    var weaponRange = npcStats.WeaponStats[0].Range;
                        //                    if (distance <= weaponRange)
                        //                    {
                        //                        followShootPlayerAi.State = ShooterAiStates.Shooting;
                        //                        followShootPlayerAi.ShootingTarget = targetPos;
                        //                    }
                        //                    else
                        //                    {
                        //                        MoveTo(entity, namelessGame, new Point(targetPos.X, targetPos.Y), true, followShootPlayerAi);
                        //                        var route = followShootPlayerAi.Route;
                        //                        if (route.Count == 0)
                        //                        {
                        //                            followShootPlayerAi.State = ShooterAiStates.Idle;
                        //                        }
                        //                    }
                        //                }

                        //                entity.GetComponentOfType<ActionPoints>().Points = -200;
                        //            }
                        //            break;
                        //        case ShooterAiStates.Shooting:
                        //            {
                        //                FireWeaponCommand command = new FireWeaponCommand(entity, followShootPlayerAi.ShootingTarget);
                        //                namelessGame.Commander.EnqueueCommand(command);

                        //                entity.GetComponentOfType<ActionPoints>().Points = -100;
                        //                followShootPlayerAi.State = ShooterAiStates.Aiming;
                        //            }
                        //            break;
                        //    }
                        //}
                    }
                }
            }
        }

       


        public void MoveTo(IEntity movableEntity, NamelessGame namelessGame, Point destination, bool moveBesides, IRounteContainingAI aiComponent)
        {
            IEntity worldEntity = namelessGame.WorldTemplateEntity;
            IWorldProvider worldProvider = null;
            if (worldEntity != null)
            {
                worldProvider = worldEntity.GetComponentOfType<WorldTemplate>().WorldMap.Chunks;
            }

            Position position = movableEntity.GetComponentOfType<Position>();
            var route = aiComponent.Route;

            if (!route.Any())
            {
                AStarPathfinderSimple pathfinder = new AStarPathfinderSimple();
                List<Point> path = pathfinder.FindPath(position.Point.ToPoint(),
                    new Point(destination.X, destination.Y), worldProvider,
                    namelessGame);
                if (moveBesides)
                {
                    aiComponent.Route = new Queue<Point>(path.Take(path.Count - 1));
                }
                else
                {
                    aiComponent.Route = new Queue<Point>(path);
                }

                aiComponent.DestinationPoint = destination;
            }

            route = aiComponent.Route;
            if (route.Any())
            {
                Point nextPosition = route.Dequeue();
                Tile tileToMoveTo = worldProvider.GetTile(nextPosition.X, nextPosition.Y, 0);
                if (!tileToMoveTo.IsPassable() || destination != aiComponent.DestinationPoint)
                {
                    AStarPathfinderSimple pathfinder = new AStarPathfinderSimple();
                    List<Point> path = pathfinder.FindPath(position.Point.ToPoint(),
                        destination, worldProvider, namelessGame);
                    path = path.Skip(1)
                        .ToList(); // we dont need the first point in the path because its the point we are standing on currently
                    if (path.Any())
                    {

                        aiComponent.Route = new Queue<Point>(path);

                        if (moveBesides)
                        {
                            aiComponent.Route = new Queue<Point>(path.Take(path.Count - 1));
                        }
                        else
                        {
                            aiComponent.Route = new Queue<Point>(path);
                        }
                    }
                    else
                    {
                        aiComponent.Route = new Queue<Point>();
                        nextPosition = position.Point.ToPoint();
                    }

                    aiComponent.DestinationPoint = destination;
                }

                worldProvider.MoveEntity(movableEntity,
                   nextPosition.X, nextPosition.Y,0);

                var ap = movableEntity.GetComponentOfType<ActionPoints>();
                ap.Points -= 200;
            }

            if (route.Count == 0)
            {
                var ap = movableEntity.GetComponentOfType<ActionPoints>();
                ap.Points -= 200;
            }
        }
    }
}
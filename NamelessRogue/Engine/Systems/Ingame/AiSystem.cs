using Microsoft.CodeAnalysis.Text;
using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.AI.NonPlayerCharacter;
using NamelessRogue.Engine.Components.AI.Pathfinder;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Components.Environment;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.Engine.Components.Status;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Utility;
using NamelessRogue.shell;
using SharpDX.Direct2D1.Effects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Entity = NamelessRogue.Engine.Infrastructure.Entity;
using Tile = NamelessRogue.Engine.Components.ChunksAndTiles.Tile;

namespace NamelessRogue.Engine.Systems.Ingame
{
    public class AiSystem : BaseSystem
    {
        private bool first = true;

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
                ////debug code;
                if (first)
                {
                    first = false;
                    CharacterFactory.CharacterDataById.TryGetValue("ba29fb6c-dd89-49a5-a320-a35258f270ed", out var characterData);
                    var location = namelessGame.MacroNavigator.Locations.First(x => x.InternalNodes.Any() && x.Type != LocationType.CrossingVertical && x.Type != LocationType.CrossingHorizontal && x.InternalNodes.Any(x => x.IsCrossingNode()));

                    var waypoint = location.InternalNodes.First(x => x.IsCrossingNode());
                    var character = CharacterFactory.CreateCharacterFromData(namelessGame, new Vector3Int(waypoint.RealityPosition.X, waypoint.RealityPosition.Y, 0), characterData);
                    var gameTile = worldProvider.GetTile(waypoint.RealityPosition.X, waypoint.RealityPosition.Y, 0);
                    namelessGame.AddEntity(character);
                    gameTile.AddEntity(character);

                    namelessGame.WorldProvider.MoveEntity(namelessGame.PlayerEntity,
                                 waypoint.RealityPosition.X - 1, waypoint.RealityPosition.Y, 0);
                }

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
                                    namelessGame.PathfindingController.CalculateTo(toPoint, fromPoint) :
                                    namelessGame.PathfindingController.CalculateTo(fromPoint, toPoint);
                                if (pathId > -1)
                                {
                                    flowMoveComponent.To = oscillatorMovementAI.MovesToTarget ? toPoint : fromPoint;
                                    flowMoveComponent.PathChain = new List<int>() { pathId };
                                    flowMoveComponent.CurrentPathIndex = 0;
                                    flowMoveComponent.FinishedMoving = false;                                  
                                }
                            }
                            if (!flowMoveComponent.FinishedMoving)
                            {
                                var hasNext = namelessGame.PathfindingController.GetNextPoint(flowMoveComponent.PathChain[flowMoveComponent.CurrentPathIndex], entityPos.ToPoint(), out Point? nextPoint);
                                if (flowMoveComponent.To == nextPoint)
                                {
                                    flowMoveComponent.FinishedMoving = true;
                                    oscillatorMovementAI.MovesToTarget = !oscillatorMovementAI.MovesToTarget;
                                    //continue;
                                }
                                if (hasNext && nextPoint.HasValue)
                                {
                                    namelessGame.WorldProvider.MoveEntity(entity,
                                      nextPoint.Value.X, nextPoint.Value.Y, 0);
                                    entity.GetComponentOfType<ActionPoints>().Points = -200;
                                }
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
                                MacroNode closestWaypoint;
                                if (flowMoveComponent.CurrentMacroNode != null)
                                {
                                    var waypoints = flowMoveComponent.CurrentMacroNode.NeighborConnectionPaths;
                                    closestWaypoint = waypoints.ToList()[Random.Shared.Next(waypoints.Count)].Node;
                                }
                                else
                                {
                                    var closestLocation = namelessGame.MacroNavigator.Locations.Where(l => l.BoundingBox.IsPointInside(entityPos.X, entityPos.Y)).
                                        FirstOrDefault(x => x.InternalNodes.Any());

                                    if (closestLocation == null || closestLocation.InternalNodes.Count == 0)
                                    {
                                        entity.RemoveComponentOfType<AIControlled>();
                                        continue;
                                    }
                                    var waypoints = closestLocation.InternalNodes;
                                    closestWaypoint = waypoints.ToList()[Random.Shared.Next(waypoints.Count)];
                                }

                                if (closestWaypoint.IsCrossingNode())
                                {
                                    var closestWaypointRealityPosition = closestWaypoint.RealityPosition;
                                    var nextCrossingConnection = closestWaypoint.GetCrossingConnections().FirstOrDefault();
                                    var pathChain = new List<int>() { closestWaypoint.LocationPathId, nextCrossingConnection.PathId };
                                 //   pathChain.Reverse();
                                    flowMoveComponent.To = closestWaypoint.RealityPosition.ToPoint();
                                    flowMoveComponent.PathChain = pathChain;
                                    flowMoveComponent.CurrentPathIndex = 0;
                                    flowMoveComponent.FinishedMoving = false;
                                    flowMoveComponent.CurrentMacroNode = closestWaypoint;

                                    foreach(var pathid in pathChain)
                                    {
                                       // namelessGame.PathfindingController.CurrentPathModels[pathid].ClearDebug();
                                       //  namelessGame.PathfindingController.CurrentPathModels[pathid].DrawDebug();
                                    }

                                }
                                else
                                {
                                    var pathId = closestWaypoint.LocationPathId;
                                    if (pathId > -1)
                                    {
                                        flowMoveComponent.To = closestWaypoint.RealityPosition.ToPoint();
                                        flowMoveComponent.PathChain = new List<int>() { pathId };
                                        flowMoveComponent.CurrentPathIndex = 0;
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
                                if(flowMoveComponent.TurnsToWait > 0)
                                {
                                    flowMoveComponent.TurnsToWait--;
                                    entity.GetComponentOfType<ActionPoints>().Points = -200;
                                    continue;
                                }
                                try
                                {
                                    var hasNext = namelessGame.PathfindingController.GetNextPoint(flowMoveComponent.PathId, entityPos.ToPoint(), out Point? nextPoint);
                                    if (hasNext && nextPoint.HasValue)
                                    {
                                        if (flowMoveComponent.To == nextPoint.Value)
                                        {
                                            flowMoveComponent.CurrentPathIndex++;
                                            if (flowMoveComponent.PathId==-1)
                                            {
                                                flowMoveComponent.FinishedMoving = true;
                                                flowMoveComponent.CurrentMacroNode = null;
                                            }
                                            else
                                            {
                                                flowMoveComponent.To = namelessGame.PathfindingController.GetPathEndPoint(flowMoveComponent.PathChain[flowMoveComponent.CurrentPathIndex]);
                                            }
                                            //namelessGame.FlowFieldController.CurrentPathModels[flowMoveComponent.PathId].FullPath.ClearDebug();
                                            //continue;
                                        }
                                        //MoveEntitySwapCharacters(entity, worldProvider, namelessGame,
                                        //  nextPoint.Value.X, nextPoint.Value.Y, 0);
                                        namelessGame.WorldProvider.MoveEntityIgnoreCharacters(entity,
                                        nextPoint.Value.X, nextPoint.Value.Y, 0);

                                    }
                                    else
                                    {                                        
                                        flowMoveComponent.FinishedMoving = true;
                                        flowMoveComponent.CurrentMacroNode = null;
                                        //namelessGame.FlowFieldController.CurrentPathModels[flowMoveComponent.PathId].FullPath.ClearDebug();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine($"Error moving entity {entity.Id}: {ex.Message}");
                                    Debug.WriteLine($"entityPos {entityPos}");
                                    Debug.WriteLine($"flowMoveComponent.To {flowMoveComponent.To}");
                                    Debug.WriteLine($"flowMoveComponent.CurrentMacroNode.FlowFieldId {flowMoveComponent.CurrentMacroNode.LocationPathId}");
                                    Debug.WriteLine($"flowMoveComponent.CurrentMacroNode.RealityPosition {flowMoveComponent.CurrentMacroNode.RealityPosition}");
                                    flowMoveComponent.FinishedMoving = true;
                                    flowMoveComponent.CurrentMacroNode = null;
                                    // entity.RemoveComponentOfType<AIControlled>();
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

        public bool MoveEntitySwapCharacters(IEntity entity, IWorldProvider worldProvider, NamelessGame game, int x, int y, int z)
        {
            Position position = entity.GetComponentOfType<Position>();
            var flowMoveComponent = entity.GetComponentOfType<FlowMoveComponent>();
            if (position != null)
            {          

                Tile oldTile = worldProvider.GetTile(position.Point.X, position.Point.Y, position.Point.Z);
                Tile newTile = worldProvider.GetTile(x, y, z);

                if (newTile.IsPassable())
                {
                    oldTile.RemoveEntity((Entity)entity);
                    newTile.AddEntity((Entity)entity);
                    position.Point = new Vector3Int(x, y, z);
                    return true;
                }
                else if (newTile.IsPassableIgnoringCharacters())
                {
                    var newTileEntities = newTile.GetEntities();

                    foreach (var otherEntity in newTileEntities)
                    {
                        var isCharacter = otherEntity.GetComponentOfType<Character>() != null;
                        if (isCharacter)
                        {
                            var otherFlowMoveComponent = otherEntity.GetComponentOfType<FlowMoveComponent>();

                            if(otherFlowMoveComponent == null)
                            {
                                return false;
                            }
                            if (otherFlowMoveComponent.TurnsToWait == 0)
                            {
                                flowMoveComponent.TurnsToWait++; 
                                return false;
                            }
                            else
                            {
                                newTile.RemoveEntity((Entity)otherEntity);
                                oldTile.AddEntity((Entity)otherEntity);
                                var otherPosition = otherEntity.GetComponentOfType<Position>();
                                if (otherPosition != null)
                                {
                                    otherPosition.Point = new Vector3Int(position.Point.X, position.Point.Y, position.Point.Z);
                                }
                                oldTile.RemoveEntity((Entity)entity);
                                newTile.AddEntity((Entity)entity);
                                position.Point = new Vector3Int(x, y, z);
                                return true;
                            }
                        }
                    }
                    return false;
                }
            }

            return false;
        }
    }
}
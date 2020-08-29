using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.AI.NonPlayerCharacter;
using NamelessRogue.Engine.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Engine.Components.Interaction;
using NamelessRogue.Engine.Engine.Components.Physical;
using NamelessRogue.Engine.Engine.Components.Status;
using NamelessRogue.Engine.Engine.Generation.World;
using NamelessRogue.Engine.Engine.Infrastructure;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Systems.Ingame
{
    public class AiSystem : BaseSystem
    {

        public AiSystem()
        {
            Signature = new HashSet<Type>();
            Signature.Add(typeof(AIControlled));
            Signature.Add(typeof(ActionPoints));
            Signature.Add(typeof(BasicAi));
        }


        public override HashSet<Type> Signature { get; }

        public override void Update(long gameTime, NamelessGame namelessGame)
        {

            var playerEntity = namelessGame.PlayerEntity;
            var ap = playerEntity.GetComponentOfType<ActionPoints>();
            if (ap.Points >= 100)
            {
                return;
            }

            IEntity worldEntity = namelessGame.TimelineEntity;
            IWorldProvider worldProvider = null;
            if (worldEntity != null)
            {
                worldProvider = worldEntity.GetComponentOfType<TimeLine>().CurrentTimelineLayer.Chunks;
            }


            if (worldProvider != null)
            {
                foreach (IEntity entity in this.RegisteredEntities)
                {
                    AIControlled ac = entity.GetComponentOfType<AIControlled>();
                    Dead dead = entity.GetComponentOfType<Dead>();
                    var actionPoints = entity.GetComponentOfType<ActionPoints>();
                    if (dead == null && actionPoints.Points >= 100)
                    {
                        BasicAi basicAi = entity.GetComponentOfType<BasicAi>();
                        Position playerPosition = namelessGame.PlayerEntity
                            .GetComponentOfType<Position>();

                        switch (basicAi.State)
                        {
                            case BasicAiStates.Idle:
                            case BasicAiStates.Moving:
                                var pPos = playerPosition.p.ToVector2();
                                MoveTo(entity, namelessGame, playerPosition.p, true);
                                var route = basicAi.Route;
                                if (route.Count == 0)
                                {
                                    basicAi.State = (BasicAiStates.Idle);
                                }

                                break;
                            case BasicAiStates.Attacking:
                                break;
                            default:
                                break;

                        }
                    }
                }
            }
        }



        public void MoveTo(IEntity movableEntity, NamelessGame namelessGame, Point destination, bool moveBesides)
        {
            IEntity worldEntity = namelessGame.TimelineEntity;
            IWorldProvider worldProvider = null;
            if (worldEntity != null)
            {
                worldProvider = worldEntity.GetComponentOfType<TimeLine>().CurrentTimelineLayer.Chunks;
            }

            Position position = movableEntity.GetComponentOfType<Position>();
            BasicAi basicAi = movableEntity.GetComponentOfType<BasicAi>();
            var route = basicAi.Route;

            if (!route.Any())
            {
                AStarPathfinderSimple pathfinder = new AStarPathfinderSimple();
                List<Point> path = pathfinder.FindPath(position.p,
                    new Point(destination.X, destination.Y), worldProvider,
                    namelessGame);
                if (moveBesides)
                {
                    basicAi.Route = new Queue<Point>(path.Take(path.Count - 1));
                }
                else
                {
                    basicAi.Route = new Queue<Point>(path);
                }

                basicAi.DestinationPoint = destination;
            }

            route = basicAi.Route;
            if (route.Any())
            {
                Point nextPosition = route.Dequeue();
                Tile tileToMoveTo = worldProvider.GetTile(nextPosition.X, nextPosition.Y);
                if (!tileToMoveTo.IsPassable() || destination != basicAi.DestinationPoint)
                {
                    AStarPathfinderSimple pathfinder = new AStarPathfinderSimple();
                    List<Point> path = pathfinder.FindPath(position.p,
                        destination, worldProvider, namelessGame);
                    path = path.Skip(1)
                        .ToList(); // we dont need the first point in the path because its the point we are standing on currently
                    if (path.Any())
                    {

                        basicAi.Route = new Queue<Point>(path);

                        if (moveBesides)
                        {
                            basicAi.Route = new Queue<Point>(path.Take(path.Count - 1));
                        }
                        else
                        {
                            basicAi.Route = new Queue<Point>(path);
                        }
                    }
                    else
                    {
                        basicAi.Route = new Queue<Point>();
                        nextPosition = position.p;
                    }

                    basicAi.DestinationPoint = destination;
                }

                worldProvider.MoveEntity(movableEntity,
                    new Point(nextPosition.X, nextPosition.Y));

                var ap = movableEntity.GetComponentOfType<ActionPoints>();
                ap.Points -= Constants.ActionsMovementCost;
            }

            if (route.Count == 0)
            {
                var ap = movableEntity.GetComponentOfType<ActionPoints>();
                ap.Points = 0;
            }
        }
    }
}
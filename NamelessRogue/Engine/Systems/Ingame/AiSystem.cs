using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SharpDX;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.AI.NonPlayerCharacter;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Status;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.shell;
using NamelessRogue.Engine.Utility;

namespace NamelessRogue.Engine.Systems.Ingame
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

        public override void Update(GameTime gameTime, NamelessGame game)
        {

            var playerEntity = game.PlayerEntity;
            var ap = playerEntity.GetComponentOfType<ActionPoints>();
            if (ap.Points >= 100)
            {
                return;
            }

            IEntity worldEntity = game.TimelineEntity;
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
                        Position playerPosition = game.PlayerEntity
                            .GetComponentOfType<Position>();

                        switch (basicAi.State)
                        {
                            case BasicAiStates.Idle:
                            case BasicAiStates.Moving:
                                var pPos = playerPosition.Point.ToVector2();
                                MoveTo(entity, game, playerPosition.Point, true);
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



        public void MoveTo(IEntity movableEntity, NamelessGame game, Point destination, bool moveBesides)
        {
            IEntity worldEntity = game.TimelineEntity;
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
                List<Point> path = pathfinder.FindPath(position.Point,
                    new Point(destination.X, destination.Y), worldProvider,
                    game);
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
                    List<Point> path = pathfinder.FindPath(position.Point,
                        destination, worldProvider, game);
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
                        nextPosition = position.Point;
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
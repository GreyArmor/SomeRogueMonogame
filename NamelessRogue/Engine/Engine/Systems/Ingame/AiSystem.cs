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
    public class AiSystem : ISystem
    {


        public void Update(long gameTime, NamelessGame namelessGame)
        {

            var playerEntity = namelessGame.GetEntitiesByComponentClass<Player>().First();
            var ap = playerEntity.GetComponentOfType<ActionPoints>();
            if (ap.Points >= 100)
            {
                return;
            }

            IEntity worldEntity = namelessGame.GetEntityByComponentClass<TimeLine>();
            IWorldProvider worldProvider = null;
            if (worldEntity != null)
            {
                worldProvider = worldEntity.GetComponentOfType<TimeLine>().CurrentTimelineLayer.Chunks;
            }


            if (worldProvider != null)
            {
                foreach (IEntity entity in namelessGame.GetEntities())
                {
                    AIControlled ac = entity.GetComponentOfType<AIControlled>();
                    Dead dead = entity.GetComponentOfType<Dead>();
                    var actionPoints = entity.GetComponentOfType<ActionPoints>();
                    if (ac != null && dead==null && actionPoints.Points>=100)
                    {
                        BasicAi basicAi = entity.GetComponentOfType<BasicAi>();

                        if (basicAi != null)

                        {
                            switch (basicAi.State)
                            {
                                case BasicAiStates.Idle:
                                case BasicAiStates.Moving:
                                    //if (basicAi.State == BasicAiStates.Idle)
                                    //{

                                        Position playerPosition = namelessGame.GetEntityByComponentClass<Player>()
                                            .GetComponentOfType<Position>();
                                    //    Position position = entity.GetComponentOfType<Position>();
                                    //    //  namelessGame.WriteLineToConsole("Starting movement state idle position = " + position.p.toString());
                                    //    if (position != null)
                                    //    {
                                    //        AStarPathfinderSimple pathfinder = new AStarPathfinderSimple();


                                    //        List<Point> path = pathfinder.FindPath(position.p,
                                    //            new Point(playerPosition.p.X, playerPosition.p.Y), worldProvider,
                                    //            namelessGame);
                                    //        if (path != null)
                                    //        {
                                    //            basicAi.Route = new Queue<Point>(path.Take(path.Count - 1));
                                    //            basicAi.State = (BasicAiStates.Moving);
                                    //        }
                                    //        else
                                    //        {
                                    //            basicAi.State = (BasicAiStates.Idle);
                                    //        }
                                    //    }
                                    //}
                                    MoveTo(entity, namelessGame, playerPosition.p,true);
                                    break;
                                default:
                                    break;

                            }
                        }
                    }
                }
            }
        }


        public void MoveTo(IEntity movableEntity, NamelessGame namelessGame, Point destination, bool moveBesides)
        {
            IEntity worldEntity = namelessGame.GetEntityByComponentClass<TimeLine>();
            IWorldProvider worldProvider = null;
            if (worldEntity != null)
            {
                worldProvider = worldEntity.GetComponentOfType<TimeLine>().CurrentTimelineLayer.Chunks;
            }

            Position position = movableEntity.GetComponentOfType<Position>();
            BasicAi basicAi = movableEntity.GetComponentOfType<BasicAi>();
            var route = basicAi.Route;

            if(!route.Any())
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
                    path = path.Skip(1).ToList(); // we dont need the first point in the path because its the point we are standing on currently
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
                basicAi.State = (BasicAiStates.Idle);
                var ap = movableEntity.GetComponentOfType<ActionPoints>();
                ap.Points = 0;
            }
        }
    }
}
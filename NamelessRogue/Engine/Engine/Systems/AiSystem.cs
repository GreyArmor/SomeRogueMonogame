using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.AI.NonPlayerCharacter;
using NamelessRogue.Engine.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Engine.Components.Interaction;
using NamelessRogue.Engine.Engine.Components.Physical;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Systems
{
    public class AiSystem : ISystem
    {
        private long previousGametimeForMove = 0;


        public void Update(long gameTime, NamelessGame namelessGame)
        {
            if (gameTime - previousGametimeForMove > 150)
            {
                previousGametimeForMove = gameTime;
                IEntity worldEntity = namelessGame.GetEntityByComponentClass<ChunkData>();
                IChunkProvider worldProvider = null;
                if (worldEntity != null)
                {
                    worldProvider = worldEntity.GetComponentOfType<ChunkData>();
                }

                if (worldProvider != null)
                {
                    foreach (IEntity entity in namelessGame.GetEntities())
                    {
                        AIControlled ac = entity.GetComponentOfType<AIControlled>();
                        if (ac != null)
                        {
                            BasicAi basicAi = entity.GetComponentOfType<BasicAi>();

                            if (basicAi != null)
                            {
                                switch (basicAi.getState())
                                {
                                    case BasicAiStates.Idle:
                                    {

                                        Position playerPosition = namelessGame.GetEntityByComponentClass<Player>()
                                            .GetComponentOfType<Position>();
                                        Position position = entity.GetComponentOfType<Position>();
                                        //  namelessGame.WriteLineToConsole("Starting movement state idle position = " + position.p.toString());
                                        if (position != null)
                                        {
                                            AStarPathfinderSimple pathfinder = new AStarPathfinderSimple();


                                            List<Point> path = pathfinder.FindPath(position.p,
                                                new Point(playerPosition.p.Y, playerPosition.p.X), worldProvider, true);
                                            if (path == null)
                                            {
                                                path = pathfinder.FindPath(position.p,
                                                    new Point(playerPosition.p.Y, playerPosition.p.X), worldProvider,
                                                    true);
                                            }

                                            if (path != null)
                                            {
                                                basicAi.setRoute(path);
                                                basicAi.setState(BasicAiStates.Moving);
                                            }
                                            else
                                            {
                                                basicAi.setState(BasicAiStates.Idle);
                                            }
                                        }
                                    }
                                        break;
                                    case BasicAiStates.Moving:
                                    {
                                        Position position = entity.GetComponentOfType<Position>();
                                        //       namelessGame.WriteLineToConsole("moving position = " + position.p.toString());

                                        List<Point> route = basicAi.getRoute();
                                        if (route.Count > 0 && position != null)
                                        {
                                            Point nextPosition = route[0];
                                            Tile tileToMoveTo = worldProvider.getTile(nextPosition.Y, nextPosition.X);

                                            if (!tileToMoveTo.GetPassable(namelessGame))
                                            {
                                                AStarPathfinderSimple pathfinder = new AStarPathfinderSimple();
                                                List<Point> path = pathfinder.FindPath(position.p,
                                                    route[(route.Count-1)], worldProvider, true);
                                                if (path == null)
                                                {
                                                    path = pathfinder.FindPath(position.p,
                                                        route[route.Count], worldProvider, true);
                                                }

                                                if (path != null)
                                                {
                                                    basicAi.setRoute(path);
                                                }
                                                else
                                                {
                                                    route.Clear();
                                                }
                                            }

                                            // namelessGame.WriteLineToConsole("moving to nextPosition  = " + nextPosition.toString());
                                            MoveToCommand mc = new MoveToCommand(nextPosition.Y, nextPosition.X,
                                                entity);
                                            entity.AddComponent(mc);
                                            if (route.Count > 0)
                                            {
                                                route.RemoveAt(0);
                                            }
                                        }

                                        if (route.Count == 0)
                                        {
                                            basicAi.setState(BasicAiStates.Idle);
                                        }

                                       
                                    }
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
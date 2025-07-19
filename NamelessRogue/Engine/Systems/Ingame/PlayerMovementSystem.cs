using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.AI.NonPlayerCharacter;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Components.Environment;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Input;
using NamelessRogue.Engine.Utility;
using NamelessRogue.shell;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Systems.Ingame
{
    internal class PlayerMovementSystem : BaseSystem
    {
        public override HashSet<Type> Signature { get; } = new HashSet<Type>();

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            while (namelessGame.Commander.DequeueCommand(out PlayerMovementCommand command))
            {
                //probably spaghetti code, but when we interact with more than one object, we create interaction selector items, and when
                //we move into them we choose to interact with that object, when we do literally anything else we should terminate the selection process;
                bool terminateInteractSelector = true;

                var entityToMove = namelessGame.PlayerEntity;
                var position = entityToMove.GetComponentOfType<Position>();
                var point = new Vector3Int(position.Point.X, position.Point.Y, position.Point.Z);
                bool changingZlevel = false;

                GetNewPosition(command.Directions, ref point, ref changingZlevel);

                if (point.Z < 0 || point.Z > Constants.ChunkHeight)
                {
                    continue;
                }

                IEntity worldEntity = namelessGame.TimelineEntity;
                IWorldProvider worldProvider = null;
                if (worldEntity != null)
                {
                    worldProvider = worldEntity.GetComponentOfType<WorldTemplate>().WorldMap.Chunks;
                }

                if (changingZlevel)
                {
                    Tile playerTile = worldProvider.GetTile(position.X, position.Y, position.Z);
                    StairsComponent stairs = null;
                    foreach (IEntity tileEntity in playerTile.GetEntities())
                    {
                        stairs = tileEntity.GetComponentOfType<StairsComponent>();
                        if (stairs != null)
                        {
                            break;
                        }
                    }
                    if (stairs == null)
                    {
                        continue;
                    }
                }

                Tile tileToMoveTo = worldProvider.GetTile(point.X, point.Y, point.Z);
                if (tileToMoveTo == null)
                {
                    continue;
                }

                InteractionSelectorLink interactionSelectorLink = null;
                foreach (IEntity tileEntity in tileToMoveTo.GetEntities())
                {
                    OccupiesTile occupiesTile =
                        tileEntity.GetComponentOfType<OccupiesTile>();

                    interactionSelectorLink = tileEntity.GetComponentOfType<InteractionSelectorLink>();

                    if (interactionSelectorLink != null)
                    {
                        terminateInteractSelector = false;
                        break;
                    }
                }

                if (interactionSelectorLink != null)
                {
                    var interactCommand = new InteractCommand(interactionSelectorLink.LinkedEntity);
                    namelessGame.Commander.EnqueueCommand(interactCommand);
                    continue;
                }

                IEntity entityThatOccupiedTile = null;

                foreach (IEntity tileEntity in tileToMoveTo.GetEntities())
                {
                    OccupiesTile occupiesTile =
                        tileEntity.GetComponentOfType<OccupiesTile>();

                    if (occupiesTile != null)
                    {
                        entityThatOccupiedTile = tileEntity;
                        break;
                    }
                }

                if (entityThatOccupiedTile != null)
                {
                    Door door = entityThatOccupiedTile.GetComponentOfType<Door>();
                    Character characterComponent =
                        entityThatOccupiedTile.GetComponentOfType<Character>();
                    if (door != null)
                    {
                        SimpleSwitch simpleSwitch =
                            entityThatOccupiedTile.GetComponentOfType<SimpleSwitch>();
                        if (simpleSwitch != null == simpleSwitch.isSwitchActive())
                        {
                            entityThatOccupiedTile.GetComponentOfType<Drawable>()
                                .ObjectID = "openDoor";
                            entityThatOccupiedTile.RemoveComponentOfType<BlocksVision>();
                            entityThatOccupiedTile.RemoveComponentOfType<OccupiesTile>();

                            namelessGame.Commander.EnqueueCommand(
                                new ChangeSwitchStateCommand(simpleSwitch, false));
                            var ap = entityToMove.GetComponentOfType<ActionPoints>();
                            ap.Points -= Constants.ActionsMovementCost;
                            //   playerEntity.RemoveComponentOfType<HasTurn>();
                            namelessGame.Commander.EnqueueCommand(new PlaySoundCommand("DoorOpen", false, 0.1f));
                            namelessGame.Commander.EnqueueCommand(new UpdateVisualChunkCommand(new Vector3Int(point.X / Constants.ChunkSize, point.Y / Constants.ChunkSize, point.Z)));


                        }
                        else
                        {
                            worldProvider.MoveEntity(entityToMove,
                                new Vector3Int(point.X, point.Y, position.Z));
                            var ap = entityToMove.GetComponentOfType<ActionPoints>();
                            ap.Points -= Constants.ActionsMovementCost;

                        }
                    }

                    if (characterComponent != null)
                    {
                        //TODO: if hostile
                        namelessGame.Commander.EnqueueCommand(new AttackCommand(entityToMove,
                            entityThatOccupiedTile));


                        //TODO: do something else if friendly: chat, trade, etc

                    }
                }
                else
                {
                    worldProvider.MoveEntity(entityToMove,
                        new Vector3Int(point.X, point.Y, point.Z));
                    var ap = entityToMove.GetComponentOfType<ActionPoints>();
                    ap.Points -= Constants.ActionsMovementCost;
                }

                if (terminateInteractSelector)
                {
                    var interactCommand = new TerminateInteractionSelectorCommand();
                    namelessGame.Commander.EnqueueCommand(interactCommand);
                }
            }

            while (namelessGame.Commander.DequeueCommand(out CursorMovementCommand command))
            {
                var entityToMove = namelessGame.CursorEntity;
                var playerPosition = namelessGame.PlayerEntity.GetComponentOfType<Position>();
                var position = entityToMove.GetComponentOfType<Position>();
                var point = new Vector3Int(position.Point.X, position.Point.Y, position.Point.Z);
                bool changingZlevel = false;
                GetNewPosition(command.Directions, ref point, ref changingZlevel);
                var targeter = namelessGame.TargeterEntity.GetComponentOfType<TergeterComponent>();
                var distance = (playerPosition.Point - point).Length();

                if (point.Z < 0 || point.Z > Constants.ChunkHeight)
                {
                    continue;
                }

                position.Point = point;

                namelessGame.Commander.EnqueueCommand(new DetachFromToTargetCommand());

            }
        }

        private static void GetNewPosition(IEnumerable<MovementDirection> directions, ref Vector3Int point, ref bool changingZlevel)
        {
            foreach (var direction in directions)
            {
                switch (direction)
                {
                    case MovementDirection.Left:
                        point.X--;
                        break;
                    case MovementDirection.Right:
                        point.X++;
                        break;
                    case MovementDirection.Top:
                        point.Y--;
                        break;
                    case MovementDirection.Bottom:
                        point.Y++;
                        break;
                    case MovementDirection.Down:
                        changingZlevel = true;
                        point.Z--;
                        break;
                    case MovementDirection.Up:
                        changingZlevel = true;
                        point.Z++;
                        break;
                }
            }
        }
    }
}

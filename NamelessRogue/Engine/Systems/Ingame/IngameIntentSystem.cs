using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.AI.NonPlayerCharacter;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Components.Environment;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Components.UI;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Input;
using NamelessRogue.Engine.Serialization;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Systems.Ingame
{
    public class IngameIntentSystem : BaseSystem
    {

        public IngameIntentSystem()
        {
            Signature = new HashSet<Type>();
            Signature.Add(typeof(InputComponent));
        }

        public override HashSet<Type> Signature { get; }

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            if (!namelessGame.IsActive) { return; }
            foreach (IEntity entity in RegisteredEntities)
            {
                InputComponent inputComponent = entity.GetComponentOfType<InputComponent>();
                if (inputComponent != null)
                {
                    var playerEntity = namelessGame.PlayerEntity;
                    foreach (Intent intent in inputComponent.Intents)
                    {

                        switch (intent.Intention)
                        {

                            //case IntentEnum.MoveUp:
                            //case IntentEnum.MoveDown:
                            //case IntentEnum.MoveLeft:
                            //case IntentEnum.MoveRight:
                            //case IntentEnum.MoveTopLeft:
                            //case IntentEnum.MoveTopRight:
                            //case IntentEnum.MoveBottomLeft:
                            //case IntentEnum.MoveBottomRight:
								//MoveCamera3dCommand cameraMove = new MoveCamera3dCommand();
        //                        if (intent.Intention == IntentEnum.MoveUp || intent.Intention == IntentEnum.MoveTopLeft || intent.Intention == IntentEnum.MoveTopRight)
        //                        {
        //                            cameraMove.MovesToMake.Add(MoveType.Forward);
								//}
								//if(intent.Intention == IntentEnum.MoveDown || intent.Intention == IntentEnum.MoveBottomLeft || intent.Intention == IntentEnum.MoveBottomRight)

								//{
								//	cameraMove.MovesToMake.Add(MoveType.Backward);
								//}
								//if(intent.Intention == IntentEnum.MoveLeft || intent.Intention == IntentEnum.MoveTopLeft || intent.Intention == IntentEnum.MoveBottomLeft)

								//{
								//	cameraMove.MovesToMake.Add(MoveType.Left);
								//}
								//if(intent.Intention == IntentEnum.MoveRight || intent.Intention == IntentEnum.MoveTopRight || intent.Intention == IntentEnum.MoveBottomRight)

								//{
								//	cameraMove.MovesToMake.Add(MoveType.Right);
								//}
								//namelessGame.Commander.EnqueueCommand(cameraMove);
								//break;

                            case IntentEnum.MoveUp:
                            case IntentEnum.MoveDown:
                            case IntentEnum.MoveLeft:
                            case IntentEnum.MoveRight:
                            case IntentEnum.MoveTopLeft:
                            case IntentEnum.MoveTopRight:
                            case IntentEnum.MoveBottomLeft:
                            case IntentEnum.MoveBottomRight:
                            {

                                Position position = playerEntity.GetComponentOfType<Position>();
                                var actionPoints = playerEntity.GetComponentOfType<ActionPoints>();
                                if (position != null && actionPoints.Points >= 100)
                                {

                                    int newX =
                                        intent.Intention == IntentEnum.MoveLeft || intent.Intention == IntentEnum.MoveBottomLeft ||
                                        intent.Intention == IntentEnum.MoveTopLeft ? position.Point.X - 1 :
                                        intent.Intention == IntentEnum.MoveRight || intent.Intention == IntentEnum.MoveBottomRight ||
                                        intent.Intention == IntentEnum.MoveTopRight ? position.Point.X + 1 :
                                        position.Point.X;
                                    int newY =
                                        intent.Intention == IntentEnum.MoveDown || intent.Intention == IntentEnum.MoveBottomLeft ||
                                        intent.Intention == IntentEnum.MoveBottomRight ? position.Point.Y + 1 :
                                        intent.Intention == IntentEnum.MoveUp || intent.Intention == IntentEnum.MoveTopLeft ||
                                        intent.Intention == IntentEnum.MoveTopRight ? position.Point.Y - 1 :
                                        position.Point.Y;

                                    IEntity worldEntity = namelessGame.TimelineEntity;
                                    IWorldProvider worldProvider = null;
                                    if (worldEntity != null)
                                    {
                                        worldProvider = worldEntity.GetComponentOfType<TimeLine>().CurrentTimelineLayer.Chunks;
                                    }

                                    Tile tileToMoveTo = worldProvider.GetTile(newX, newY);


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
                                                    .ObjectID = "OpenDoor";
                                                entityThatOccupiedTile.RemoveComponentOfType<BlocksVision>();
                                                entityThatOccupiedTile.RemoveComponentOfType<OccupiesTile>();

                                                namelessGame.Commander.EnqueueCommand(
                                                    new ChangeSwitchStateCommand(simpleSwitch, false));
                                                var ap = playerEntity.GetComponentOfType<ActionPoints>();
                                                ap.Points -= Constants.ActionsMovementCost;
                                                //   playerEntity.RemoveComponentOfType<HasTurn>();

                                            }
                                            else
                                            {

                                                worldProvider.MoveEntity(playerEntity,
                                                    new Point(newX, newY));
                                                var ap = playerEntity.GetComponentOfType<ActionPoints>();
                                                ap.Points -= Constants.ActionsMovementCost;

                                            }
                                        }

                                        if (characterComponent != null)
                                        {
                                            //TODO: if hostile
                                            namelessGame.Commander.EnqueueCommand(new AttackCommand(playerEntity,
                                                entityThatOccupiedTile));

                                            var ap = playerEntity.GetComponentOfType<ActionPoints>();
                                            ap.Points -= Constants.ActionsAttackCost;
                                            // playerEntity.RemoveComponentOfType<HasTurn>();

                                            //TODO: do something else if friendly: chat, trade, etc

                                        }
                                    }
                                    else
                                    {
                                        worldProvider.MoveEntity(playerEntity,
                                            new Point(newX, newY));
                                        var ap = playerEntity.GetComponentOfType<ActionPoints>();
                                        ap.Points -= Constants.ActionsMovementCost;
                                    }
                                }
                            }

                            break;
                        case IntentEnum.LookAtMode:
                                //InputReceiver receiver = new InputReceiver();
                                //Player player = entity.GetComponentOfType<Player>();
                                //cursor = entity.GetComponentOfType<Cursor>();
                                //entity.RemoveComponentOfType<InputReceiver>();
                                //if (player != null)
                                //{
                                //    IEntity cursorEntity = namelessGame.CursorEntity;
                                //    cursorEntity.AddComponent(receiver);
                                //    Drawable cursorDrawable = cursorEntity.GetComponentOfType<Drawable>();
                                //    cursorDrawable.setVisible(true);
                                //    Position cursorPosition = cursorEntity.GetComponentOfType<Position>();
                                //    Position playerPosition = entity.GetComponentOfType<Position>();
                                //    cursorPosition.p.X = (playerPosition.p.X);
                                //    cursorPosition.p.Y = (playerPosition.p.Y);

                                //}
                                //else if (cursor != null)
                                //{
                                //    IEntity playerEntity = namelessGame.PlayerEntity;
                                //    playerEntity.AddComponent(receiver);
                                //    Drawable cursorDrawable = entity.GetComponentOfType<Drawable>();
                                //    cursorDrawable.setVisible(false);

                                //}

                                break;
                            case IntentEnum.PickUpItem:
                            {                                
                                var actionPoints = playerEntity.GetComponentOfType<ActionPoints>();

                                if (actionPoints.Points >= 100)
                                {
                                    IEntity worldEntity = namelessGame.TimelineEntity;
                                    IWorldProvider worldProvider = null;
                                    if (worldEntity != null)
                                    {
                                        worldProvider = worldEntity.GetComponentOfType<TimeLine>().CurrentTimelineLayer
                                            .Chunks;
                                    }


                                    var position = playerEntity.GetComponentOfType<Position>();
                                    var itemHolder = playerEntity.GetComponentOfType<ItemsHolder>();
                                    var tile = worldProvider.GetTile(position.Point.X, position.Point.Y);

                                    List<IEntity> itemsToPickUp = new List<IEntity>();
                                    foreach (var entityOnTIle in tile.GetEntities())
                                    {
                                        var itemComponent = entityOnTIle.GetComponentOfType<Item>();
                                        if (itemComponent != null)
                                        {
                                            itemsToPickUp.Add(entityOnTIle);
                                        }
                                    }

                                    if (itemsToPickUp.Any())
                                    {

                                            /*
                                            if (itemsToPickUp.Count > 1)
                                            {
                                                namelessGame.ContextToSwitch =
                                                    ContextFactory.GetPickUpItemContext(namelessGame);
                                                UIController.PickUpItemsScreen.FillItems(namelessGame);
                                                if (UIController.PickUpItemsScreen.ItemsTable.Items.Any())
                                                {
                                                    UIController.PickUpItemsScreen.ItemsTable.SelectedIndex = 0;
                                                }
                                            }
                                            else
                                            {
                                                StringBuilder builder = new StringBuilder();
                                                var itemsCommand = new PickUpItemCommand(itemsToPickUp, itemHolder,
                                                    position.Point);
                                                namelessGame.Commander.EnqueueCommand(itemsCommand);

                                                foreach (var entity1 in itemsToPickUp)
                                                {
                                                    var desc = entity1.GetComponentOfType<Description>();
                                                    if (desc != null)
                                                    {
                                                        builder.Append($"Picked up: {desc.Name} \n");
                                                    }
                                                }

                                                var logCommand = new HudLogMessageCommand();
                                                logCommand.LogMessage += builder.ToString();
                                                namelessGame.Commander.EnqueueCommand(logCommand);

                                            }

                                        var ap = playerEntity.GetComponentOfType<ActionPoints>();
                                        ap.Points -= Constants.ActionsPickUpCost;
                                        //playerEntity.RemoveComponentOfType<HasTurn>();
                                            */
                                    }

                                }

                                break;
                            }
                            case IntentEnum.SkipTurn:
                            {
                                var actionPoints = playerEntity.GetComponentOfType<ActionPoints>();

                                    if (actionPoints.Points >= 100)
                                    {
                                        var ap = entity.GetComponentOfType<ActionPoints>();
                                        ap.Points -= Constants.ActionsMovementCost;
                                        var logCommand = new HudLogMessageCommand();
                                        logCommand.LogMessage += "Waiting";
                                        namelessGame.Commander.EnqueueCommand(logCommand);

                                        //   playerEntity.RemoveComponentOfType<HasTurn>();
                                    }
                            }
                                break;
                            case IntentEnum.Quicksave:
                                namelessGame.ScheduleSave();
                                break;
                            case IntentEnum.Quickload:
                                namelessGame.ScheduleLoad();
                                break;
                            case IntentEnum.ZoomIn:
                                var zoomCommand = new ZoomCommand(false);
                                namelessGame.Commander.EnqueueCommand(zoomCommand);
                                break;
                            case IntentEnum.ZoomOut:
                                var zoomOutCommand = new ZoomCommand();
                                namelessGame.Commander.EnqueueCommand(zoomOutCommand);
                                break;
                            case IntentEnum.MouseChanged:

								MouseState mouseState = Mouse.GetState();

								var player = namelessGame.PlayerEntity;
								var selectionData = player.GetComponentOfType<SelectionData>();

                                if (selectionData.SelectionState == SelectionState.None && mouseState.LeftButton == ButtonState.Pressed)
                                {
                                    namelessGame.Commander.EnqueueCommand(new SelectionCommand(SelectionState.Start, mouseState.Position));
                                }
                                else if ((selectionData.SelectionState == SelectionState.Start || selectionData.SelectionState == SelectionState.Drag) && mouseState.LeftButton == ButtonState.Pressed)
                                {
                                    namelessGame.Commander.EnqueueCommand(new SelectionCommand(SelectionState.Drag, mouseState.Position));
                                }
                                else if (selectionData.SelectionState == SelectionState.Drag && mouseState.LeftButton == ButtonState.Released)
                                {
									namelessGame.Commander.EnqueueCommand(new SelectionCommand(SelectionState.End, mouseState.Position));
								}
								break;
                            default:
                                break;
                        }
                    }

                    inputComponent.Intents.Clear();
                }
            }
        }
    }
}

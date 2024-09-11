using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Timers;
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
using NamelessRogue.Engine.Utility;
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
                            case IntentEnum.MoveUp:
                            case IntentEnum.MoveDown:
                            case IntentEnum.MoveLeft:
                            case IntentEnum.MoveRight:
                            case IntentEnum.MoveTopLeft:
                            case IntentEnum.MoveTopRight:
                            case IntentEnum.MoveBottomLeft:
                            case IntentEnum.MoveBottomRight:
                            case IntentEnum.MoveAscent:
                            case IntentEnum.MoveDescent:
                                {
                                    //TODO: REFACTORING move to another dedicated system
                                    var playerReceiver = playerEntity.GetComponentOfType<InputReceiver>();
                                    var cursorReceiver = namelessGame.CursorEntity.GetComponentOfType<InputReceiver>();
                                    Position position = null;
                                    if (playerReceiver != null)
                                    {
                                        position = playerEntity.GetComponentOfType<Position>();
                                    }
                                    else if(cursorReceiver != null)
                                    {
                                        position = namelessGame.CursorEntity.GetComponentOfType<Position>();
                                    }
                                   
                                    int newX =
                                              intent.Intention == IntentEnum.MoveLeft || intent.Intention == IntentEnum.MoveBottomLeft ||
                                              intent.Intention == IntentEnum.MoveTopLeft ? position.X - 1 :
                                              intent.Intention == IntentEnum.MoveRight || intent.Intention == IntentEnum.MoveBottomRight ||
                                              intent.Intention == IntentEnum.MoveTopRight ? position.X + 1 :
                                              position.X;
                                    int newY =
                                        intent.Intention == IntentEnum.MoveDown || intent.Intention == IntentEnum.MoveBottomLeft ||
                                        intent.Intention == IntentEnum.MoveBottomRight ? position.Y + 1 :
                                        intent.Intention == IntentEnum.MoveUp || intent.Intention == IntentEnum.MoveTopLeft ||
                                        intent.Intention == IntentEnum.MoveTopRight ? position.Y - 1 :
                                        position.Y;

                                    int newZ = intent.Intention == IntentEnum.MoveAscent ? position.Z + 1 :
                                               intent.Intention == IntentEnum.MoveDescent ? position.Z - 1 : position.Z;

                                    bool changingZlevel = intent.Intention == IntentEnum.MoveAscent || intent.Intention == IntentEnum.MoveDescent;

                                 
                                 
                                    if (playerReceiver != null)
                                    {
                                      
                                        var actionPoints = playerEntity.GetComponentOfType<ActionPoints>();
                                        if (actionPoints.Points >= 100)
                                        {
                                            if (newZ < 0 || newZ > Constants.ChunkHeight)
                                            {
                                                continue;
                                            }

                                            IEntity worldEntity = namelessGame.TimelineEntity;
                                            IWorldProvider worldProvider = null;
                                            if (worldEntity != null)
                                            {
                                                worldProvider = worldEntity.GetComponentOfType<TimeLine>().CurrentTimelineLayer.Chunks;
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


                                            Tile tileToMoveTo = worldProvider.GetTile(newX, newY, newZ);
                                            if (tileToMoveTo == null)
                                            {
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
                                                        var ap = playerEntity.GetComponentOfType<ActionPoints>();
                                                        ap.Points -= Constants.ActionsMovementCost;
                                                        //   playerEntity.RemoveComponentOfType<HasTurn>();

                                                    }
                                                    else
                                                    {

                                                        worldProvider.MoveEntity(playerEntity,
                                                            new Vector3Int(newX, newY, position.Z));
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
                                                    new Vector3Int(newX, newY, newZ));
                                                var ap = playerEntity.GetComponentOfType<ActionPoints>();
                                                ap.Points -= Constants.ActionsMovementCost;
                                            }
                                        }
                                    }
                                    else if (cursorReceiver != null)
                                    {
                                        position.Point = new Vector3Int(newX, newY, newZ);
                                    }
                                }
                            break;
                            case IntentEnum.LookAtMode:
                                {
                                    InputReceiver receiver = new InputReceiver();

                                    IEntity cursorEntity = namelessGame.CursorEntity;                              

                                    var playerReceiver = playerEntity.GetComponentOfType<InputReceiver>();
                                    var cursorReceiver = namelessGame.CursorEntity.GetComponentOfType<InputReceiver>();
                                    playerEntity.RemoveComponentOfType<InputReceiver>();

                                    if (playerReceiver != null)
                                    {
                                        cursorEntity.AddComponent(receiver);
                                        Drawable cursorDrawable = cursorEntity.GetComponentOfType<Drawable>();
                                        cursorDrawable.Visible = true;
                                        Position cursorPosition = cursorEntity.GetComponentOfType<Position>();
                                        Position playerPosition = entity.GetComponentOfType<Position>();
                                        cursorPosition.Point = playerPosition.Point;
                                        namelessGame.FollowedByCameraEntity = cursorEntity;
                                        playerEntity.RemoveComponent(playerReceiver);

                                    }
                                    else if (cursorReceiver != null)
                                    {
                                        playerEntity.AddComponent(receiver);
                                        Drawable cursorDrawable = cursorEntity.GetComponentOfType<Drawable>();
                                        cursorDrawable.Visible = false;
                                        cursorEntity.RemoveComponent(cursorReceiver);
                                        namelessGame.FollowedByCameraEntity = playerEntity;
                                    }
                                }

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
                                    var tile = worldProvider.GetTile(position.X, position.Y, position.Z);

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

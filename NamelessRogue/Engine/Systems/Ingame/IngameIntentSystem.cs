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
using NamelessRogue.Engine.Components.Stats;
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

        public List<IntentEnum> SingleKeyPressIntents { get; set; } = new List<IntentEnum>();

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            if (!namelessGame.IsActive) { return; }
            var playerEntity = namelessGame.PlayerEntity;
            InputComponent inputComponent = playerEntity.GetComponentOfType<InputComponent>();
            if (inputComponent != null && !inputComponent.IsDelayed)
            {
                foreach (Intent intent in inputComponent.Intents)
                {
                    if (SingleKeyPressIntents.Contains(intent.Intention))
                    {
                        continue;
                    }

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
                                var playerReceiver = playerEntity.GetComponentOfType<InputReceiver>();
                                var cursorReceiver = namelessGame.CursorEntity.GetComponentOfType<InputReceiver>();

                                IEntity entityToMove = null;

                                Position position = null;
                                bool playerMovement = true;

                                if (playerReceiver != null)
                                {
                                    position = playerEntity.GetComponentOfType<Position>();
                                    entityToMove = playerEntity;
                                }
                                else if (cursorReceiver != null)
                                {
                                    position = namelessGame.CursorEntity.GetComponentOfType<Position>();
                                    entityToMove = namelessGame.CursorEntity;
                                    playerMovement = false;
                                }

                                MovementDirection newX =
                                    intent.Intention == IntentEnum.MoveLeft || intent.Intention == IntentEnum.MoveBottomLeft ||
                                    intent.Intention == IntentEnum.MoveTopLeft ? MovementDirection.Left :
                                    intent.Intention == IntentEnum.MoveRight || intent.Intention == IntentEnum.MoveBottomRight ||
                                    intent.Intention == IntentEnum.MoveTopRight ? MovementDirection.Right :
                                    MovementDirection.None;
                                MovementDirection newY =
                                    intent.Intention == IntentEnum.MoveDown || intent.Intention == IntentEnum.MoveBottomLeft ||
                                    intent.Intention == IntentEnum.MoveBottomRight ? MovementDirection.Bottom :
                                    intent.Intention == IntentEnum.MoveUp || intent.Intention == IntentEnum.MoveTopLeft ||
                                    intent.Intention == IntentEnum.MoveTopRight ? MovementDirection.Top :
                                   MovementDirection.None;

                                MovementDirection newZ = intent.Intention == IntentEnum.MoveAscent ? MovementDirection.Up :
                                           intent.Intention == IntentEnum.MoveDescent ? MovementDirection.Down : MovementDirection.None;

                                var directions = new List<MovementDirection>() { newX, newY, newZ };
                                directions.RemoveAll(x => x == MovementDirection.None);
                                if(playerMovement)
                                {
                                    var movementCommand = new PlayerMovementCommand(directions);
                                    namelessGame.Commander.EnqueueCommand(movementCommand);
                                }
                                else
                                {
                                    var movementCommand = new CursorMovementCommand(directions);
                                    namelessGame.Commander.EnqueueCommand(movementCommand);
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
                                    Position playerPosition = playerEntity.GetComponentOfType<Position>();
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
                                    var ap = playerEntity.GetComponentOfType<ActionPoints>();
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
                            break;
                        case IntentEnum.Fire:
                            {
                                if (TargetingSystem.State == TargetingState.NotTargeting)
                                {
                                    var starTargetingCommand = new StartTargetingCommand();
                                    namelessGame.Commander.EnqueueCommand(starTargetingCommand);
                                }
                                else
                                {
                                    FireWeaponCommand command = new FireWeaponCommand();
                                    namelessGame.Commander.EnqueueCommand(command);

                                    SingleKeyPressIntents.Add(IntentEnum.Fire);

                                }
                            }
                            break;
                        case IntentEnum.Escape:
                            {
                                if (TargetingSystem.State == TargetingState.Targeting)
                                {
                                    var endTargetingCommand = new EndTargetingCommand();
                                    namelessGame.Commander.EnqueueCommand(endTargetingCommand);
                                }
                            }
                            break;
                        case IntentEnum.SwitchTarget:
                            {
                                if (TargetingSystem.State == TargetingState.Targeting)
                                {
                                    var tabCommand = new TabTargetingCommand();
                                    namelessGame.Commander.EnqueueCommand(tabCommand);
                                }
                            }
                            break;
                        case IntentEnum.Interact:
                            {
                                List<Entity> interactableEntities = new List<Entity>();
                                var playerPosition = playerEntity.GetComponentOfType<Position>();
                                for (var x = playerPosition.X - 1; x <= playerPosition.X + 1; x++)
                                {
                                    for (var y = playerPosition.Y - 1; y <= playerPosition.Y + 1; y++)
                                    {
                                        var tile = namelessGame.WorldProvider.GetTile(x, y, playerPosition.Z);
                                        var tileEntities = tile.GetEntities();
                                        foreach (var tileEntity in tileEntities)
                                        {
                                            var interactable = tileEntity.GetComponentOfType<Interactable>();
                                            if (interactable != null)
                                            {
                                                interactableEntities.Add(tileEntity);
                                            }
                                        }
                                        if (interactableEntities.Count > 1)
                                        {
                                            var interactSelectorCommand = new InteractionSelectorCommand(interactableEntities);
                                            namelessGame.Commander.EnqueueCommand(interactSelectorCommand);
                                        }
                                    }
                                }

                                if (interactableEntities.Count == 1)
                                {
                                    var interactCommand = new InteractCommand(interactableEntities.First());
                                    namelessGame.Commander.EnqueueCommand(interactCommand);
                                }
                            }
                            break;
                        default:
                            break;
                    }                 
                }

                foreach (var intent in SingleKeyPressIntents.ToList())
                {
                    if (!inputComponent.Intents.Any(x => x.Intention == intent))
                    {
                        SingleKeyPressIntents.Remove(intent);
                    }
                }
                inputComponent.Intents.Clear();
            }
        }
    }
}

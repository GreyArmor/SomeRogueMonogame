using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NamelessRogue.Engine.Abstraction;
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
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Systems.Ingame
{
    public class PlayerMovementSubProcessor : IngameIntentSystemSubProcessor
    {
        public void Process(NamelessGame namelessGame, IngameIntentSystem system, Intent intent)
        {
            var playerEntity = namelessGame.PlayerEntity;
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

                        IEntity entityToMove = null;

                        Position position = null;

                        if (playerReceiver != null)
                        {
                            position = playerEntity.GetComponentOfType<Position>();
                            entityToMove = playerEntity;
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

                        var movementCommand = new PlayerMovementCommand(directions);
                        namelessGame.Commander.EnqueueCommand(movementCommand);
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
                case IntentEnum.QuickBarPress:

                    var parsed = int.TryParse(intent.PressedChar.ToString(), out int abilityIndex);
                    if (parsed)
                    {
                        var abilityBinder = playerEntity.GetComponentOfType<AbilityBinder>();
                        var hasAbilityBound = abilityBinder.AbilityBindings.TryGetValue(abilityIndex, out var ability);

                        if (hasAbilityBound)
                        {
                            var abilityParams = ability.GetComponentOfType<AbilityParameters>();
                            var stats = namelessGame.PlayerEntity.GetComponentOfType<CharacterStats>();
                           
                            var desc = ability.GetComponentOfType<Description>();
                            if (abilityParams.CooldownTurnsRemaining>0)
                            {                              
                                var logMessage = new HudLogMessageCommand($@"Ability {desc.Name} is not ready!");
                                namelessGame.Commander.EnqueueCommand(logMessage);
                                break;
                            }

                            if (stats.Energy.Value < abilityParams.EnergyCost)
                            {
                                var logMessage = new HudLogMessageCommand($@"Not enough energy!");
                                namelessGame.Commander.EnqueueCommand(logMessage);
                                break;
                            }

                            switch (abilityParams.ActivationMode)
                            {
                                case ActivationMode.Passive:
                                    break;
                                case ActivationMode.Toggle:
                                    abilityParams.IsActive = !abilityParams.IsActive;
                                    break;
                                case ActivationMode.Activatable:
                                    {
                                        switch (abilityParams.TargetMode)
                                        {
                                            case TargetMode.None:
                                                break;
                                            case TargetMode.Self:
                                                var activateSelfAbilityCommand = new ActivateSelfAbilityCommand(playerEntity, ability);
                                                namelessGame.Commander.EnqueueCommand(activateSelfAbilityCommand);
                                                break;
                                            case TargetMode.Targeted: 
                                            case TargetMode.TargetEnemies: 
                                            case TargetMode.TargetFriends:
                                                if (TargetingSystem.State == TargetingState.NotTargeting)
                                                {
                                                    var targetingMode = TargetingMode.None;
                                                    switch (abilityParams.TargetMode)
                                                    {
                                                        case TargetMode.Targeted:
                                                            targetingMode = TargetingMode.None;
                                                            break;
                                                        case TargetMode.TargetEnemies:
                                                            targetingMode = TargetingMode.Enemies;
                                                            break;
                                                        case TargetMode.TargetFriends:
                                                            targetingMode = TargetingMode.Friends;
                                                            break;
                                                    }
                                                    var starTargetingCommand = new StartTargetingCommand(targetingMode, abilityParams.Range);
                                                    namelessGame.Commander.EnqueueCommand(starTargetingCommand);

                                                    var switchModeCommand = new IngameIntentSystemModeSwitchCommand(IngameIntentSystemMode.QuickBarAiming, abilityIndex);
                                                    namelessGame.Commander.EnqueueCommand(switchModeCommand);

                                                }
                                                break;
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                    break;
                case IntentEnum.Fire:
                    {
                        if (TargetingSystem.State == TargetingState.NotTargeting)
                        {

                            var stats = playerEntity.GetComponentOfType<ModifiersCollection>().Accumulator.GetComponentOfType<CharacterStats>();
                            var range = 0;
                            if(stats.WeaponStats.Any())
                            {
                                range = stats.WeaponStats.Min(x=>x.Range);
                            }
                            var starTargetingCommand = new StartTargetingCommand(TargetingMode.Enemies, range);
                            namelessGame.Commander.EnqueueCommand(starTargetingCommand);

                            var switchModeCommand = new IngameIntentSystemModeSwitchCommand(IngameIntentSystemMode.FireWeapon);
                            namelessGame.Commander.EnqueueCommand(switchModeCommand);
                        }
                    }
                    break;
                case IntentEnum.Escape:
                    {
                    }
                    break;
                case IntentEnum.SwitchTarget:
                    {
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
                                        break;
                                    }
                                }

                            }
                        }
                        if (interactableEntities.Count > 1)
                        {
                            var interactSelectorCommand = new InteractionSelectorCommand(interactableEntities);
                            namelessGame.Commander.EnqueueCommand(interactSelectorCommand);
                        }

                        if (interactableEntities.Count == 1)
                        {
                            var interactCommand = new InteractCommand(interactableEntities.First());
                            namelessGame.Commander.EnqueueCommand(interactCommand);
                        }
                    }
                    break;
                case IntentEnum.Chat:
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
                                    var interactable = tileEntity.GetComponentOfType<DialogComponent>();
                                    if (interactable != null)
                                    {
                                        interactableEntities.Add(tileEntity);
                                        break;
                                    }
                                }

                            }
                        }

                        if (interactableEntities.Count > 0)
                        {
                            var chatCommand = new StartDialogCommand(interactableEntities.First());
                            namelessGame.Commander.EnqueueCommand(chatCommand);
                            namelessGame.ContextToSwitch = ContextFactory.GetDialogContext(namelessGame);
                        }

                 

                    }
                    break;
                default:
                    break;
            }
        }
    }
}

using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Input;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Systems.Ingame
{
    public class AbilityTargetingSubProcessor : IngameIntentSystemSubProcessor
    {
        public int AbilityIndex { get; set; } = 0;
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
                        var cursorReceiver = namelessGame.CursorEntity.GetComponentOfType<InputReceiver>();

                        IEntity entityToMove = null;

                        Position position = null;
                        if (cursorReceiver != null)
                        {
                            position = namelessGame.CursorEntity.GetComponentOfType<Position>();
                            entityToMove = namelessGame.CursorEntity;
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

                        var movementCommand = new CursorMovementCommand(directions);
                        namelessGame.Commander.EnqueueCommand(movementCommand);

                    }
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
                    system.SingleKeyPressIntents.Add(IntentEnum.QuickBarPress);
                    var parsed = int.TryParse(intent.PressedChar.ToString(), out int abilityIndex);
                    if (parsed && abilityIndex == this.AbilityIndex)
                    {
                        //fall through explicitly, apparently this is how its done in c#;
                        goto case IntentEnum.Fire;
                    }
                    break;                  
                case IntentEnum.Fire:
                    {
           

                        Position cursorPosition = namelessGame.CursorEntity.GetComponentOfType<Position>();
                        Position playerPosition = namelessGame.PlayerEntity.GetComponentOfType<Position>();
                        var targeter = namelessGame.TargeterEntity.GetComponentOfType<TergeterComponent>();

                        var distance = (cursorPosition.Point - playerPosition.Point).Length();

                        if (targeter.CurrentTargetingRange >= distance)
                        {
                            var abilityBinder = playerEntity.GetComponentOfType<AbilityBinder>();
                            var ability = abilityBinder.AbilityBindings[AbilityIndex];
                            var activateAbility = new ActivateTargetedAbilityCommand(playerEntity, ability);
                            namelessGame.Commander.EnqueueCommand(activateAbility);

                            if (TargetingSystem.State == TargetingState.Targeting)
                            {
                                var endTargetingCommand = new EndTargetingCommand();
                                namelessGame.Commander.EnqueueCommand(endTargetingCommand);
                            }

                            var switchModeCommand = new IngameIntentSystemModeSwitchCommand(IngameIntentSystemMode.PlayerMovement);
                            namelessGame.Commander.EnqueueCommand(switchModeCommand);

                            system.SingleKeyPressIntents.Add(IntentEnum.Fire);
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

                        var switchModeCommand = new IngameIntentSystemModeSwitchCommand(IngameIntentSystemMode.PlayerMovement);
                        namelessGame.Commander.EnqueueCommand(switchModeCommand);

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
                default:
                    break;
            }
        }
    }
}

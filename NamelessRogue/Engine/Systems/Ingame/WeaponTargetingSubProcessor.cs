using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.Engine.Input;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Systems.Ingame
{
    public class WeaponTargetingSubProcessor : IngameIntentSystemSubProcessor
    {
        public void Process(NamelessGame namelessGame, IngameIntentSystem system, Intent intent)
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
                case IntentEnum.Fire:
                    {

                        var stats = namelessGame.PlayerEntity.GetComponentOfType<ModifiersCollection>().Accumulator.GetComponentOfType<CharacterStats>();
                        var range = 0;
                        if (stats.WeaponStats.Any())
                        {
                            range = stats.WeaponStats.Min(x => x.Range);
                        }

                        Position cursorPosition = namelessGame.CursorEntity.GetComponentOfType<Position>();
                        Position playerPosition = namelessGame.PlayerEntity.GetComponentOfType<Position>();
                        var targeter = namelessGame.TargeterEntity.GetComponentOfType<TergeterComponent>();

                        var distance = (cursorPosition.Point - playerPosition.Point).Length();
                        if (targeter.CurrentTargetingRange >= distance)
                        {
                            FireWeaponCommand command = new FireWeaponCommand();
                            namelessGame.Commander.EnqueueCommand(command);
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

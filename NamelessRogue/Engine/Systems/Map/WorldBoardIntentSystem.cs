﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Input;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Systems.Map
{
    public class WorldBoardIntentSystem : BaseSystem
    {
        public WorldBoardIntentSystem()
        {
            Signature = new HashSet<Type>();
            Signature.Add(typeof(InputComponent));
        }
        public override HashSet<Type> Signature { get; }

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            foreach (IEntity entity in RegisteredEntities)
            {
                InputComponent inputComponent = entity.GetComponentOfType<InputComponent>();
                if (inputComponent != null)
                {
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
                            {
                                var cursorEntity = namelessGame.CursorEntity;
                                Position position = cursorEntity.GetComponentOfType<Position>();
                                if (position != null)
                                {

                                    int newX =
                                        intent.Intention == IntentEnum.MoveLeft || intent.Intention == IntentEnum.MoveBottomLeft ||
                                        intent.Intention == IntentEnum.MoveTopLeft ? position.X - 1 :
                                        intent.Intention == IntentEnum.MoveRight || intent.Intention == IntentEnum.MoveBottomRight ||
                                        intent.Intention == IntentEnum.MoveTopRight ? position.X + 1 :
                                        position.X;
                                    int newY =
                                        intent.Intention == IntentEnum.MoveDown || intent.Intention == IntentEnum.MoveBottomLeft ||
                                        intent.Intention == IntentEnum.MoveBottomRight ? position.Y - 1 :
                                        intent.Intention == IntentEnum.MoveUp || intent.Intention == IntentEnum.MoveTopLeft ||
                                        intent.Intention == IntentEnum.MoveTopRight ? position.Y + 1 :
                                        position.Y;

                                    position.Point = new Utility.Vector3Int(newX, newY, position.Z);
                                }
                                break;
                            }
                            case IntentEnum.ZoomOut:
                                {
                                    var zoomCommand = new ZoomCommand(false);
                                    namelessGame.Commander.EnqueueCommand(zoomCommand);
                                    break;
                                }
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

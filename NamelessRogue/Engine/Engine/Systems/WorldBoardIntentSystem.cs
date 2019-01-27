﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.AI.NonPlayerCharacter;
using NamelessRogue.Engine.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Engine.Components.Environment;
using NamelessRogue.Engine.Engine.Components.Interaction;
using NamelessRogue.Engine.Engine.Components.Physical;
using NamelessRogue.Engine.Engine.Components.Rendering;
using NamelessRogue.Engine.Engine.Input;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Systems
{
    public class WorldBoardIntentSystem : ISystem
    {
        public void Update(long gameTime, NamelessGame namelessGame)
        {
            foreach (IEntity entity in namelessGame.GetEntities())
            {
                InputComponent inputComponent = entity.GetComponentOfType<InputComponent>();
                if (inputComponent != null)
                {
                    foreach (Intent intent in inputComponent.Intents)
                    {

                        switch (intent)
                        {

                            case Intent.MoveUp:
                            case Intent.MoveDown:
                            case Intent.MoveLeft:
                            case Intent.MoveRight:
                            case Intent.MoveTopLeft:
                            case Intent.MoveTopRight:
                            case Intent.MoveBottomLeft:
                            case Intent.MoveBottomRight:
                            {
                                var cursorEntity = namelessGame.GetEntitiesByComponentClass<Cursor>().First();
                                Position position = cursorEntity.GetComponentOfType<Position>();
                                if (position != null)
                                {

                                    int newX =
                                        intent == Intent.MoveLeft || intent == Intent.MoveBottomLeft ||
                                        intent == Intent.MoveTopLeft ? position.p.X - 1 :
                                        intent == Intent.MoveRight || intent == Intent.MoveBottomRight ||
                                        intent == Intent.MoveTopRight ? position.p.X + 1 :
                                        position.p.X;
                                    int newY =
                                        intent == Intent.MoveDown || intent == Intent.MoveBottomLeft ||
                                        intent == Intent.MoveBottomRight ? position.p.Y - 1 :
                                        intent == Intent.MoveUp || intent == Intent.MoveTopLeft ||
                                        intent == Intent.MoveTopRight ? position.p.Y + 1 :
                                        position.p.Y;


                                    cursorEntity.AddComponent(new MoveToCommand(newX, newY, cursorEntity));



                                }

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

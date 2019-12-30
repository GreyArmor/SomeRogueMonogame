using System.Linq;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.Interaction;
using NamelessRogue.Engine.Engine.Components.Physical;
using NamelessRogue.Engine.Engine.Input;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Systems.Map
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

                                    position.p = new Point(newX, newY);
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

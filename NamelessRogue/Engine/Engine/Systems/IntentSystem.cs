using Microsoft.Xna.Framework;
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
    public class IntentSystem : ISystem
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
                        Cursor cursor;
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
                                Position position = entity.GetComponentOfType<Position>();
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

                                    IEntity worldEntity = namelessGame.GetEntityByComponentClass<ChunkData>();
                                    IChunkProvider worldProvider = null;
                                    if (worldEntity != null)
                                    {
                                        worldProvider = worldEntity.GetComponentOfType<ChunkData>();
                                    }

                                    Tile tileToMoveTo = worldProvider.getTile(newX, newY);
                                    cursor = entity.GetComponentOfType<Cursor>();

                                    //if cursor is moving then it passes through everything
                                    if (cursor != null)
                                    {
                                        var command = entity.GetComponentOfType<MoveToCommand>();
                                        if (command != null)
                                        {
                                            entity.RemoveComponentOfType<MoveToCommand>();
                                        }

                                        entity.AddComponent(new MoveToCommand(newX, newY, entity));
                                    }
                                    else
                                    {

                                        IEntity entityThatOccupiedTile = null;
                                        foreach (IEntity tileEntity in tileToMoveTo.getEntitiesOnTile())
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
                                                        .setRepresentation('o');
                                                    entityThatOccupiedTile.AddComponent(
                                                        new ChangeSwitchStateCommand(simpleSwitch, false));
                                                    tileToMoveTo.setPassable(true);
                                                    // namelessGame.WriteLineToConsole("Opened the door!");
                                                }
                                                else
                                                {
                                                    var command = entity.GetComponentOfType<MoveToCommand>();
                                                    if (command != null)
                                                    {
                                                        entity.RemoveComponentOfType<MoveToCommand>();
                                                    }

                                                    entity.AddComponent(new MoveToCommand(newX, newY, entity));
                                                }
                                            }

                                            if (characterComponent != null)
                                            {
                                                //TODO: if hostile
                                                entity.AddComponent(new AttackCommand(entity, entityThatOccupiedTile));
                                                //TODO: do something else if friendly: chat, trade, etc

                                            }
                                        }
                                        else
                                        {
                                            var command = entity.GetComponentOfType<MoveToCommand>();
                                            if (command != null)
                                            {
                                                entity.RemoveComponentOfType<MoveToCommand>();
                                            }

                                            entity.AddComponent(new MoveToCommand(newX, newY, entity));
                                        }
                                    }
                                }
                            }

                                break;
                            case Intent.LookAtMode:
                                InputReceiver receiver = new InputReceiver();
                                Player player = entity.GetComponentOfType<Player>();
                                cursor = entity.GetComponentOfType<Cursor>();
                                entity.RemoveComponentOfType<InputReceiver>();
                                if (player != null)
                                {
                                    IEntity cursorEntity = namelessGame.GetEntityByComponentClass<Cursor>();
                                    cursorEntity.AddComponent(receiver);
                                    Drawable cursorDrawable = cursorEntity.GetComponentOfType<Drawable>();
                                    cursorDrawable.setVisible(true);
                                    Position cursorPosition = cursorEntity.GetComponentOfType<Position>();
                                    Position playerPosition = entity.GetComponentOfType<Position>();
                                    cursorPosition.p.X = (playerPosition.p.X);
                                    cursorPosition.p.Y = (playerPosition.p.Y);

                                }
                                else if (cursor != null)
                                {
                                    IEntity playerEntity = namelessGame.GetEntityByComponentClass<Player>();
                                    playerEntity.AddComponent(receiver);
                                    Drawable cursorDrawable = entity.GetComponentOfType<Drawable>();
                                    cursorDrawable.setVisible(false);

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

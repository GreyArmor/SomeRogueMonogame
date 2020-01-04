using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.AI.NonPlayerCharacter;
using NamelessRogue.Engine.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Engine.Components.Environment;
using NamelessRogue.Engine.Engine.Components.Interaction;
using NamelessRogue.Engine.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Engine.Components.Physical;
using NamelessRogue.Engine.Engine.Components.Rendering;
using NamelessRogue.Engine.Engine.Components.UI;
using NamelessRogue.Engine.Engine.Generation.World;
using NamelessRogue.Engine.Engine.Infrastructure;
using NamelessRogue.Engine.Engine.Input;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Systems.Ingame
{
    public class IngameIntentSystem : ISystem
    {




        public void Update(long gameTime, NamelessGame namelessGame)
        {
            foreach (IEntity entity in namelessGame.GetEntities())
            {
                InputComponent inputComponent = entity.GetComponentOfType<InputComponent>();
                if (inputComponent != null)
                {
                    var playerEntity = namelessGame.GetEntitiesByComponentClass<Player>().First();
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
                                Position position = playerEntity.GetComponentOfType<Position>();
                                var actionPoints = playerEntity.GetComponentOfType<ActionPoints>();
                                if (position != null && actionPoints.Points>=100)
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

                                    IEntity worldEntity = namelessGame.GetEntityByComponentClass<TimeLine>();
                                    IWorldProvider worldProvider = null;
                                    if (worldEntity != null)
                                    {
                                        worldProvider = worldEntity.GetComponentOfType<TimeLine>().CurrentTimelineLayer.Chunks;
                                    }

                                        Tile tileToMoveTo = worldProvider.GetTile(newX, newY);


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
                                                entityThatOccupiedTile.RemoveComponentOfType<BlocksVision>();
                                                entityThatOccupiedTile.RemoveComponentOfType<OccupiesTile>();
                                                entityThatOccupiedTile.AddComponent(
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
                                            playerEntity.AddComponent(new AttackCommand(playerEntity,
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
                            case Intent.LookAtMode:
                                //InputReceiver receiver = new InputReceiver();
                                //Player player = entity.GetComponentOfType<Player>();
                                //cursor = entity.GetComponentOfType<Cursor>();
                                //entity.RemoveComponentOfType<InputReceiver>();
                                //if (player != null)
                                //{
                                //    IEntity cursorEntity = namelessGame.GetEntityByComponentClass<Cursor>();
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
                                //    IEntity playerEntity = namelessGame.GetEntityByComponentClass<Player>();
                                //    playerEntity.AddComponent(receiver);
                                //    Drawable cursorDrawable = entity.GetComponentOfType<Drawable>();
                                //    cursorDrawable.setVisible(false);

                                //}

                                break;
                            case Intent.PickUpItem:
                            {
                                var actionPoints = playerEntity.GetComponentOfType<ActionPoints>();

                                if (actionPoints.Points >= 100)
                                {
                                    IEntity worldEntity = namelessGame.GetEntityByComponentClass<TimeLine>();
                                    IWorldProvider worldProvider = null;
                                    if (worldEntity != null)
                                    {
                                        worldProvider = worldEntity.GetComponentOfType<TimeLine>().CurrentTimelineLayer
                                            .Chunks;
                                    }


                                    var position = playerEntity.GetComponentOfType<Position>();
                                    var itemHolder = playerEntity.GetComponentOfType<ItemsHolder>();
                                    var tile = worldProvider.GetTile(position.p.X, position.p.Y);

                                    List<IEntity> itemsToPickUp = new List<IEntity>();
                                    foreach (var entityOnTIle in tile.getEntitiesOnTile())
                                    {
                                        var itemComponent = entityOnTIle.GetComponentOfType<Item>();
                                        if (itemComponent != null)
                                        {
                                            itemsToPickUp.Add(entityOnTIle);
                                        }
                                    }

                                    if (itemsToPickUp.Any())
                                    {
                                        StringBuilder builder = new StringBuilder();
                                        var itemsCommand = new PickUpItemCommand(itemsToPickUp, itemHolder, position.p);
                                        playerEntity.AddComponent(itemsCommand);

                                        foreach (var entity1 in itemsToPickUp)
                                        {
                                            var desc = entity1.GetComponentOfType<Description>();
                                            if (desc != null)
                                            {
                                                builder.Append($"Picked up: {desc.Name} ");
                                            }
                                        }

                                        var logCommand = playerEntity.GetComponentOfType<HudLogMessageCommand>();
                                        if (logCommand == null)
                                        {
                                            logCommand = new HudLogMessageCommand();
                                            playerEntity.AddComponent(logCommand);
                                        }

                                        logCommand.LogMessage += builder.ToString();

                                        var ap = playerEntity.GetComponentOfType<ActionPoints>();
                                        ap.Points -= Constants.ActionsPickUpCost;
                                        //playerEntity.RemoveComponentOfType<HasTurn>();
                                    }

                                }

                                break;
                            }
                            case Intent.SkipTurn:
                            {
                                var actionPoints = playerEntity.GetComponentOfType<ActionPoints>();

                                if (actionPoints.Points >= 100)
                                {
                                    var ap = entity.GetComponentOfType<ActionPoints>();
                                    ap.Points -= Constants.ActionsMovementCost;

                                        var logCommand = playerEntity.GetComponentOfType<HudLogMessageCommand>();
                                    if (logCommand == null)
                                    {
                                        logCommand = new HudLogMessageCommand();
                                        playerEntity.AddComponent(logCommand);
                                    }

                                    logCommand.LogMessage += "Waiting";

                                    //   playerEntity.RemoveComponentOfType<HasTurn>();
                                }
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
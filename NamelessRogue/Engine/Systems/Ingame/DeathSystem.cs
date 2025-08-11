using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Components.Status;
using NamelessRogue.Engine.Components.UI;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;

namespace NamelessRogue.Engine.Systems.Ingame
{
    public class DeathSystem : BaseSystem
    {
        public DeathSystem()
        {
            Signature = new HashSet<Type>();
        }

        public override HashSet<Type> Signature { get; }

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            while (namelessGame.Commander.DequeueCommand(out DeathCommand command))
            {

                IEntity entityToKill = command.getToKill();

                if (entityToKill == namelessGame.PlayerEntity)
                {
                    continue;
                }

                var dead = entityToKill.GetComponentOfType<Dead>();
                if (dead == null)
                {
                    entityToKill.AddComponent(new Dead());
                    var removeFromTargetingCommand = new RemoveFromTargetingAndSwitchTargetCommand(entityToKill);
                    namelessGame.Commander.EnqueueCommand(removeFromTargetingCommand);

                }
                else
                {
                    continue;
                }

                var deathanimationCommand = new PlayCharacterAnimationForNumberOfLoopsCommand(entityToKill, AnimationType.Death, 6);
                namelessGame.Commander.EnqueueCommand(deathanimationCommand);

                IEntity worldEntity = namelessGame.TimelineEntity;
                IWorldProvider worldProvider = null;
                if (worldEntity != null)
                {
                    worldProvider = worldEntity.GetComponentOfType<WorldTemplate>().WorldMap.Chunks;
                }

                Position position = entityToKill.GetComponentOfType<Position>();
                OccupiesTile occupiesTile = entityToKill.GetComponentOfType<OccupiesTile>();
                if (occupiesTile != null && position != null)
                {
                    Tile tile = worldProvider.GetTile(position.Point.X, position.Point.Y, position.Point.Z);
                    tile.RemoveEntity((Entity) entityToKill);
                }

                entityToKill.RemoveComponentOfType<OccupiesTile>();

                var droppedItems = entityToKill.GetComponentOfType<DroppedItemsComponent>();
                if (droppedItems != null)
                {
                    foreach (var droppedItem in droppedItems.DroppedItems)
                    {
                        var isDropping = Random.Shared.Next(100) <= droppedItem.Probability;

                        if(!isDropping)
                        {
                            continue;
                        }
                        Tile tile = worldProvider.GetTile(position.Point.X, position.Point.Y, position.Point.Z);
                        bool itemExists = ItemLibrary.ItemDataById.TryGetValue(droppedItem.ItemId, out var item);
                        if (itemExists)
                        {
                            var itemEntity = ItemLibrary.CreateItemFromData(namelessGame, item);
                            itemEntity.AddComponent(new Position() { Point = position.Point});
                            itemEntity.AddComponent(new Interactable());
                            tile.AddEntity(itemEntity);
                        }
                    }
                }

                var modifiers = entityToKill.GetComponentOfType<ModifiersCollection>();
                if (modifiers != null)
                {
                    modifiers.ModifierEntities.Clear();
                    entityToKill.RemoveComponent(modifiers);
                    namelessGame.RemoveEntity(modifiers.Accumulator);
                }

                Description d = entityToKill.GetComponentOfType<Description>();

                if (d != null)
                {
                    var logCommand = new HudLogMessageCommand();
                    namelessGame.Commander.EnqueueCommand(logCommand);
                    logCommand.LogMessage += d.Name + " is dead!";
                }

                Drawable drawable = entityToKill.GetComponentOfType<Drawable>();
                if(drawable!=null)
                {
                    drawable.CastsShadow = false;
                }

                namelessGame.Commander.EnqueueCommand(new LockIdleAnimationCommand(entityToKill, AnimationType.Dead));
            
            }

        }
	}
}
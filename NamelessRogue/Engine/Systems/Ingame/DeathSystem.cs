
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Components.Status;
using NamelessRogue.Engine.Components.UI;
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

        public override void Update(GameTime gameTime, NamelessGame game)
        {
            while (game.Commander.DequeueCommand(out DeathCommand command))
            {

                IEntity entityToKill = command.getToKill();
                entityToKill.AddComponent(new Dead());

                Drawable drawable = entityToKill.GetComponentOfType<Drawable>();
                if (drawable != null)
                {
                    drawable.Representation = '%';
                }

                IEntity worldEntity = game.TimelineEntity;
                IWorldProvider worldProvider = null;
                if (worldEntity != null)
                {
                    worldProvider = worldEntity.GetComponentOfType<TimeLine>().CurrentTimelineLayer.Chunks;
                }

                Position position = entityToKill.GetComponentOfType<Position>();
                OccupiesTile occupiesTile = entityToKill.GetComponentOfType<OccupiesTile>();
                if (occupiesTile != null && position != null)
                {
                    Tile tile = worldProvider.GetTile(position.Point.Y, position.Point.X);
                    tile.RemoveEntity((Entity) entityToKill);
                }

                entityToKill.RemoveComponentOfType<OccupiesTile>();

                Description d = entityToKill.GetComponentOfType<Description>();

                if (d != null)
                {
                    // game.WriteLineToConsole(d.Name + " is dead!");
                }
            }

        }
	}
}
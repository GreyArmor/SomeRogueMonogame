using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Engine.Components.Interaction;
using NamelessRogue.Engine.Engine.Components.Physical;
using NamelessRogue.Engine.Engine.Infrastructure;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Systems
{
    public class MovementSystem : ISystem
    {

        public void Update(long gameTime, NamelessGame namelessGame)
        {
            foreach (IEntity entity in namelessGame.GetEntities()) {

                MoveToCommand moveCommand = entity.GetComponentOfType<MoveToCommand>();
                if (moveCommand != null)
                {
                    Position position = moveCommand.getEntityToMove().GetComponentOfType<Position>();
                    if (position != null)
                    {

                        IEntity worldEntity = namelessGame.GetEntityByComponentClass<ChunkData>();
                        IChunkProvider worldProvider = null;
                        if (worldEntity != null)
                        {
                            worldProvider = worldEntity.GetComponentOfType<ChunkData>();
                        }

                        Tile oldTile = worldProvider.GetTile(position.p.X, position.p.Y);
                        Tile newTile = worldProvider.GetTile(moveCommand.p.X, moveCommand.p.Y);

                        oldTile.getEntitiesOnTile().Remove((Entity) entity);
                        newTile.getEntitiesOnTile().Add((Entity) entity);


                        position.p.X = (moveCommand.p.X);
                        position.p.Y = (moveCommand.p.Y);

                        entity.RemoveComponentOfType<MoveToCommand>();
                        //namelessGame.WriteLineToConsole("Moved to x = " + String.valueOf(position.p.Y));


                    }
                }
            }
        }
    }
}


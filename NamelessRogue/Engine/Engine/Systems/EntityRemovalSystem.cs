using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.EngineInfrastructure;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Systems
{
    public class EntityRemovalSystem : ISystem
    {

        public void Update(long gameTime, NamelessGame namelessGame)
        {
            foreach (IEntity entity in namelessGame.GetEntities())
            {
                ToRemove toRemove = entity.GetComponentOfType<ToRemove>();
                if (toRemove != null)
                {
                    namelessGame.RemoveEntity(entity);
                }
            }
        }
    }
}

using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.Interaction;
using NamelessRogue.Engine.Engine.Components.Stats;
using NamelessRogue.Engine.Engine.Components.Status;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Systems
{
    public class DamageHandlingSystem : ISystem
    {

        public void Update(long gameTime, NamelessGame namelessGame)
        {
            foreach (IEntity entity in namelessGame.GetEntities()) {
                Damage damage = entity.GetComponentOfType<Damage>();
                if (damage != null)
                {
                    SimpleStat health = entity.GetComponentOfType<Stats>().Health;
                    if (health != null)
                    {
                        health.Value -= damage.getDamage();
                        if (health.Value <= health.MinValue)
                        {
                            entity.AddComponent(new DeathCommand(entity));
                        }
                    }

                    entity.RemoveComponentOfType<Damage>();
                }
            }
        }
    }
}

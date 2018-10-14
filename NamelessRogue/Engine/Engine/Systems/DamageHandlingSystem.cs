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
                    Health health = entity.GetComponentOfType<Health>();
                    if (health != null)
                    {
                        health.setValue(health.getValue() - damage.getDamage());
                        if (health.getValue() < health.getMinValue())
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

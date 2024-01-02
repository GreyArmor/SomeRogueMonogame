using SharpDX;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.Engine.Components.Status;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using NamelessRogue.Engine.Infrastructure;

namespace NamelessRogue.Engine.Systems.Ingame
{
    public class DamageHandlingSystem : BaseSystem
    {
        public DamageHandlingSystem()
        {
            Signature = new HashSet<Type>();
            Signature.Add(typeof(Damage));
            Signature.Add(typeof(Stats));
        }

        public override HashSet<Type> Signature { get; }

        public override void Update(GameTime gameTime, NamelessGame game)
        {
            foreach (IEntity entity in RegisteredEntities)
            {
                Damage damage = entity.GetComponentOfType<Damage>();
                SimpleStat health = entity.GetComponentOfType<Stats>().Health;
                health.Value -= damage.DamageValue;
                if (health.Value <= health.MinValue)
                {
                    game.Commander.EnqueueCommand(new DeathCommand(entity));
                }

                entity.RemoveComponentOfType<Damage>();
            }
        }
	}
}

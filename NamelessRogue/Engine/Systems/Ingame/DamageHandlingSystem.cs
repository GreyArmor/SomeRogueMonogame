using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.Engine.Components.Status;
using NamelessRogue.Engine.Components.UI;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;

namespace NamelessRogue.Engine.Systems.Ingame
{

    public class DealDamageCommand : ICommand
    {
        public DealDamageCommand(Damage damage)
        {
            Damage = damage;
        }

        public Damage Damage { get; }
    }

        public class DamageHandlingSystem : BaseSystem
    {
        public DamageHandlingSystem()
        {
            Signature = new HashSet<Type>();
        }

        public override HashSet<Type> Signature { get; }

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            while (namelessGame.Commander.DequeueCommand(out DealDamageCommand command))
            {
                Damage damage = command.Damage;
                SimpleStat health = damage.Target.GetComponentOfType<CharacterStats>().Health;
                health.Value -= damage.DamageValue;

                var sourceDesc = damage.Source.GetComponentOfType<Description>();
                var targetDesc = damage.Target.GetComponentOfType<Description>();
                if (sourceDesc != null && targetDesc != null)
                {
                    namelessGame.Commander.EnqueueCommand(new HudLogMessageCommand($@"{sourceDesc.Name} deals {damage.DamageValue} damage to {targetDesc.Name}"));
                }
                if (health.Value <= health.MinValue)
                {
                    namelessGame.Commander.EnqueueCommand(new DeathCommand(damage.Target));
                }
            }
        }
	}
}

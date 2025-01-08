using System;
using System.Collections.Generic;
using RogueSharp.Random;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.Engine.Components.UI;
using NamelessRogue.Engine.Utility;
using NamelessRogue.shell;
using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using System.Linq;
using NamelessRogue.Engine.Infrastructure;

namespace NamelessRogue.Engine.Systems.Ingame
{
    public class CombatSystem : BaseSystem
    {
        public CombatSystem()
        {
            Signature = new HashSet<Type>();
        }
        public override HashSet<Type> Signature { get; }

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            while (namelessGame.Commander.DequeueCommand(out AttackCommand ac))
            {
                var random = new InternalRandom();

                var source = ac.getSource();
             
                var sourceStats = GetAccumulatedStats(source);
                var targetStats = GetAccumulatedStats(ac.getTarget());

                var weaponMin = 0;
                var weaponMax = 0; 

                if(sourceStats.WeaponStats.Any())
                {
                    weaponMin = sourceStats.WeaponStats.Select(x => x.MinimumDamage).Aggregate((a, b) => a + b);
                    weaponMax = sourceStats.WeaponStats.Select(x => x.MaximumDamage).Aggregate((a, b) => a + b);
                }

                var armor = 0;

                if (targetStats.Armor.Any())
                {
                    armor = targetStats.Armor.Select(x => x.Value.Value).Aggregate((a, b) => a + b);
                }

                int rawDamage = Random.Shared.Next(weaponMin, weaponMax);
                int damage = rawDamage - armor;
                if (damage < 0)
                {
                    damage = 0;
                }
                DamageHelper.ApplyDamage(ac.getTarget(), ac.getSource(), damage);

                Description targetDescription = ac.getTarget().GetComponentOfType<Description>();
                Description sourceDescription = ac.getSource().GetComponentOfType<Description>();
                if (targetDescription != null && sourceDescription != null)
                {
                    var logCommand = new HudLogMessageCommand();
                    namelessGame.Commander.EnqueueCommand(logCommand);

                    logCommand.LogMessage += (sourceDescription.Name + " deals " + (damage) + " damage to " + targetDescription.Name + $@" (Raw {rawDamage} - Armor {armor})");
                    //namelessGame.WriteLineToConsole;
                }

                var ap = source.GetComponentOfType<ActionPoints>();
                ap.Points -= Constants.ActionsAttackCost;

                var playAttackAnimationCommand = new PlayCharacterAnimationCommand(source, AnimationType.Attack);
                namelessGame.Commander.EnqueueCommand(playAttackAnimationCommand);
            }
        }

        private CharacterStats GetAccumulatedStats(IEntity entity)
        {
            var stats = entity.GetComponentOfType<CharacterStats>();
            var modifiers = entity.GetComponentOfType<ModifiersCollection>();
            var accumulatorEntity = modifiers.Accumulator;
            var accumulatedStats = accumulatorEntity.GetComponentOfType<CharacterStats>();
            return accumulatedStats;
        }
    }
}

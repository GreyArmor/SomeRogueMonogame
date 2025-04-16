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
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.Components.Status;

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
                var target = ac.getTarget();
                var sourceStats = GetAccumulatedStats(source);
                var targetStats = GetAccumulatedStats(ac.getTarget());
                var onHiBuffIds = GetOnHitBuffIds(source, namelessGame);

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

                var damageCommand = new DealDamageCommand(new Damage(source, target, damage, DamageType.Ballistic));
                namelessGame.Commander.EnqueueCommand(damageCommand);

                Description targetDescription = ac.getTarget().GetComponentOfType<Description>();
                Description sourceDescription = ac.getSource().GetComponentOfType<Description>();
                if (targetDescription != null && sourceDescription != null)
                {
                  //  var logCommand = new HudLogMessageCommand();
                 //   namelessGame.Commander.EnqueueCommand(logCommand);

                 //   logCommand.LogMessage += (sourceDescription.Name + " deals " + (damage) + " damage to " + targetDescription.Name + $@" (Raw {rawDamage} - Armor {armor})");
                    //namelessGame.WriteLineToConsole;
                }

                if (onHiBuffIds != null)
                {
                    foreach (string id in onHiBuffIds)
                    {
                        if (id != "")
                        {
                            var buff = BuffLibrary.CreateBuffFromData(namelessGame, BuffLibrary.DataById[id]);
                            target.GetComponentOfType<ModifiersCollection>().ModifierEntities.Add(buff);
                        }
                    }
                }

                var ap = source.GetComponentOfType<ActionPoints>();
                ap.Points -= Constants.ActionsAttackCost;

                var playAttackAnimationCommand = new PlayCharacterAnimationForATimeCommand(source, AnimationType.Attack, 100);
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

        private List<string> GetOnHitBuffIds(IEntity entity, NamelessGame game)
        {

            var equipment = entity.GetComponentOfType<EquipmentSlots>();
            if (equipment == null) {
                return null;
            }

            var ids = new List<string>();

            var weapons = new List<EquipmentSlot>();
            weapons.Add(equipment.Slots.FirstOrDefault(x => x.Item1 == Slot.LefHand)?.Item2);
            weapons.Add(equipment.Slots.FirstOrDefault(x => x.Item1 == Slot.RightHand)?.Item2);
            foreach (var weapon in weapons)
            {
                if (weapon.Equipment != null)
                {
                    var weaponEntity = game.GetEntity(weapon.Equipment.ParentEntityId);
                    var onHitBuffs =weaponEntity.GetComponentOfType<AssociatedBuffs>();
                    if(onHitBuffs != null)
                    {
                        ids.AddRange(onHitBuffs.BuffIds);
                    }
                }
            }

            return ids;
        }
    }
}

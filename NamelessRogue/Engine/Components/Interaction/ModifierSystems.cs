using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles.Modifiers;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.Engine.Systems;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Interaction
{
    internal class ModifierSystem : BaseSystem
    {
        public override HashSet<Type> Signature { get; } = new HashSet<Type> { typeof(ModifiersCollection) };

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            foreach (var entity in RegisteredEntities)
            {
                var modifiers = entity.GetComponentOfType<ModifiersCollection>();
                var accumulatorEntity = modifiers.Accumulator;

                var accumulatedStats = accumulatorEntity.GetComponentOfType<CharacterStats>();

                accumulatedStats.ResetValue();

                var entitstats = entity.GetComponentOfType<CharacterStats>();

                if (entitstats != null)
                {
                    if (namelessGame.TurnUpdated)
                    {
                        foreach (var modifier in modifiers.ModifierEntities)
                        {
                            var consumable = modifier.GetComponentOfType<Consumable>();
                            var timeConstrains = modifier.GetComponentOfType<TimedModifier>();
                            if (timeConstrains != null)
                            {
                                if (consumable.IsDamageOverTime)
                                {
                                    entitstats.Health.Value += consumable.Health;
                                    entitstats.Energy.Value += consumable.Energy;
                                }
                                timeConstrains.TurnsToLast--;
                            }            
                        }
                    }
                    accumulatedStats.Add(entitstats);
                }


                List<IEntity> modifiersToRemove = new List<IEntity>();
                foreach (var modifier in modifiers.ModifierEntities)
                {
                    var timeConstrains = modifier.GetComponentOfType<TimedModifier>();
                    //skip and remove modifiers that expired
                    if (timeConstrains != null && timeConstrains.TurnsToLast == 0)
                    {
                        modifiersToRemove.Add(modifier);
                        continue;
                    }

                    var modifierAS = modifier.GetComponentOfType<ArmorStats>();
                    var modifierRS = modifier.GetComponentOfType<ResistanceStat>();
                    var modifierWS = modifier.GetComponentOfType<WeaponStats>();
                    var modifierStats = modifier.GetComponentOfType<CharacterStats>();

                    if (modifierAS != null)
                    {
                        accumulatedStats.Armor.Add(modifierAS);
                    }
                    if (modifierRS != null)
                    {
                        accumulatedStats.Resistances.Add(modifierRS);
                    }
                    if (modifierWS != null)
                    {
                        accumulatedStats.WeaponStats.Add(modifierWS);
                    }
                    if (modifierStats != null)
                    {
                        accumulatedStats.Add(modifierStats);
                    }
                }

                foreach (var modifier in modifiersToRemove)
                {
                    modifiers.ModifierEntities.Remove(modifier);
                }

            }
        }
    }
}

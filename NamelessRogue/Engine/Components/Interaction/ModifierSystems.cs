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

                var entitStats = entity.GetComponentOfType<CharacterStats>();

               
                List<IEntity> modifiersToRemove = new List<IEntity>();
                foreach (var modifier in modifiers.ModifierEntities)
                {
                    var timeConstrains = modifier.GetComponentOfType<TimedModifier>();
                    var buff = modifier.GetComponentOfType<Buff>();


                    if (timeConstrains != null && timeConstrains.TurnsToLast == 0)
                    {
                        modifiersToRemove.Add(modifier);
                        continue;
                    }

                    if (buff.IsDamageOverTime && !namelessGame.TurnUpdated)
                    {
                        continue;
                    }

                    if (namelessGame.TurnUpdated)
                    {
                        if (timeConstrains != null)
                        {
                            timeConstrains.TurnsToLast--;
                        }
                    }

                    var modifierAS = modifier.GetComponentOfType<ArmorStats>();
                    var modifierRS = modifier.GetComponentOfType<ResistanceStat>();
                    var modifierWS = modifier.GetComponentOfType<WeaponStats>();
                    var modifierStats = modifier.GetComponentOfType<CharacterStats>();


                    if (buff.PermanentModifier)
                    {
                        if (modifierAS != null)
                        {
                            entitStats.Armor.Add(modifierAS);
                        }
                        if (modifierRS != null)
                        {
                            entitStats.Resistances.Add(modifierRS);
                        }
                        if (modifierWS != null)
                        {
                            entitStats.WeaponStats.Add(modifierWS);
                        }
                        if (modifierStats != null)
                        {
                            entitStats.Add(modifierStats);
                        }
                    }
                    else
                    {
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

                    if ((buff != null && buff.IsAppliedImmediately))
                    {
                        modifiersToRemove.Add(modifier);
                    }

                }

                if (entitStats != null)
                {                   
                    accumulatedStats.Add(entitStats);
                }

                foreach (var modifier in modifiersToRemove)
                {
                    modifiers.ModifierEntities.Remove(modifier);
                }

            }
        }
    }
}

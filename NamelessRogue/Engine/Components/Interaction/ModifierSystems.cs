using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles.Modifiers;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.Engine.Components.Status;
using NamelessRogue.Engine.Components.UI;
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
                var entityDesc = entity.GetComponentOfType<Description>();
                var modifiers = entity.GetComponentOfType<ModifiersCollection>();
                var accumulatorEntity = modifiers.Accumulator;

                var accumulatedStats = accumulatorEntity.GetComponentOfType<CharacterStats>();

                accumulatedStats.ResetValue();

                var entitStats = entity.GetComponentOfType<CharacterStats>();

               
                List<IEntity> modifiersToRemove = new List<IEntity>();
                foreach (var modifier in modifiers.ModifierEntities)
                {
                    var modifierDescription = modifier.GetComponentOfType<Description>();
                    var timeConstrains = modifier.GetComponentOfType<TimedModifier>();
                    var buff = modifier.GetComponentOfType<Buff>();


                    if (timeConstrains != null && timeConstrains.TurnsToLast == 0)
                    {
                        modifiersToRemove.Add(modifier);
                        continue;
                    }

                    if (buff != null && buff.IsDamageOverTime && !namelessGame.TurnUpdated)
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


                    if (buff != null && buff.PermanentModifier)
                    {
                        AccumulateStats(namelessGame, entityDesc, modifierDescription, entitStats, modifierAS, modifierRS, modifierWS, modifierStats);
                    }
                    else
                    {
                        AccumulateStats(namelessGame, entityDesc, modifierDescription, accumulatedStats, modifierAS, modifierRS, modifierWS, modifierStats);
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

        private static void AccumulateStats(NamelessGame namelessGame, Description entityDescription, Description modifierDescription, CharacterStats entitStats, ArmorStats modifierAS, ResistanceStat modifierRS, WeaponStats modifierWS, CharacterStats modifierStats)
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
                var logCommand = new HudLogMessageCommand();
                namelessGame.Commander.EnqueueCommand(logCommand);
                if (modifierStats.Health.Value<0)
                {
                    logCommand.LogMessage += modifierDescription.Name + " deals " + (modifierStats.Health.Value) + " damage to " + entityDescription.Name;
                }
                else if (modifierStats.Health.Value>0)
                {
                    logCommand.LogMessage += modifierDescription.Name + " restores " + (modifierStats.Health.Value) + " health to " + entityDescription.Name;
                }

                if (modifierStats.Energy.Value < 0)
                {
                    logCommand.LogMessage += modifierDescription.Name + " drains " + (modifierStats.Energy.Value) + " energy from " + entityDescription.Name;
                }
                else if (modifierStats.Energy.Value > 0)
                {
                    logCommand.LogMessage += modifierDescription.Name + " restores " + (modifierStats.Energy.Value) + " energy  to " + entityDescription.Name;
                }

                entitStats.Add(modifierStats);
            }
        }
    }
}

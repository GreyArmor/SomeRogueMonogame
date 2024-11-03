using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles.Modifiers;
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

                var stats = accumulatorEntity.GetComponentOfType<CharacterStats>();

                stats.ResetValue();

                var entitstats = entity.GetComponentOfType<CharacterStats>();

                if (entitstats != null)
                {
                    stats.Add(entitstats);
                }

                foreach (var modifier in modifiers.ModifierEntities)
                {
                    var modifierAS = modifier.GetComponentOfType<ArmorStats>();
                    var modifierRS = modifier.GetComponentOfType<ResistanceStat>();
                    var modifierWS = modifier.GetComponentOfType<WeaponStats>();
                    var modifierStats = modifier.GetComponentOfType<CharacterStats>();

                    if (modifierAS != null)
                    {
                        stats.Armor.Add(modifierAS);
                    }
                    if (modifierRS != null)
                    {
                        stats.Resistances.Add(modifierRS);
                    }
                    if (modifierWS != null)
                    {
                        stats.WeaponStats.Add(modifierWS);
                    }
                    if (modifierStats != null)
                    {
                        stats.Add(modifierStats);
                    }
                }
            }
        }
    }
}

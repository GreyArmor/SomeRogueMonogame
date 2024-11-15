using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity = NamelessRogue.Engine.Infrastructure.Entity;
namespace NamelessRogue.Engine.Systems.Ingame
{
    internal class ConsumableSystem : BaseSystem
    {
        public override HashSet<Type> Signature { get; } = new HashSet<Type>();

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            while (namelessGame.Commander.DequeueCommand(out ConsumeCommand command))
            {
                var player = namelessGame.PlayerEntity;
                var item = command.ItemEntity;

                var consumableComponent = item.GetComponentOfType<Consumable>();
                var playerStats = player.GetComponentOfType<CharacterStats>();

                var drawable = item.GetComponentOfType<Drawable>();

                var inventory = player.GetComponentOfType<ItemsHolder>();
                inventory.Items.Remove(item);

              

                if (consumableComponent.IsAppliedImmediately)
                {                    
                    playerStats.Health.Value += consumableComponent.Health;
                    playerStats.Energy.Value += consumableComponent.Energy;
                }
                else
                {
                    var buffEntity = new Entity();
                    var consumableClone = consumableComponent.Clone();
                    buffEntity.AddComponent((Drawable)drawable.Clone());
                    buffEntity.AddComponent(new ModifierComponent());
                    buffEntity.AddComponent(new TimedModifier() { TurnsToLast = consumableComponent.Duration });
                    buffEntity.AddComponent(consumableClone);

                    if (consumableComponent.Armor != 0)
                    {
                        buffEntity.AddComponent(new ArmorStats() { DamageType = DamageType.Physical, Value = new SimpleStat(consumableComponent.Armor, -999, 999) });
                    }
                    if(consumableComponent.Resistance != 0)
                    {
                        buffEntity.AddComponent(new ResistanceStat() { DamageType = DamageType.Physical, Value = new SimpleStat(consumableComponent.Resistance, -999, 999) });
                    }
                    if (consumableComponent.Damage != 0)
                    {
                        buffEntity.AddComponent(new WeaponStats() { DamageType = DamageType.Physical, MinimumDamage = consumableComponent.Damage, MaximumDamage = consumableComponent.Damage});
                    }

                    player.GetComponentOfType<ModifiersCollection>().ModifierEntities.Add(buffEntity);

                }
            }
        }
    }
}

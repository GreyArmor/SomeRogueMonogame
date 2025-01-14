using Microsoft.Xna.Framework;
using MonoGame.Extended.ECS;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.Generation.Editor;
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

                var iconComponent = item.GetComponentOfType<UiIconComponent>();

                var inventory = player.GetComponentOfType<ItemsHolder>();
                inventory.Items.Remove(item);

                foreach (var buffId in consumableComponent.BuffIds)
                {
                    var buffTemplateData = BuffLibrary.DataById[buffId];
                    var buffEntity = BuffLibrary.CreateBuffFromData(namelessGame, BuffLibrary.DataById[buffId]);
                    player.GetComponentOfType<ModifiersCollection>().ModifierEntities.Add(buffEntity); 
                }
            }
        }
    }
}

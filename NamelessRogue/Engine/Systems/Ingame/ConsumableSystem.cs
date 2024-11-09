using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

                playerStats.Health.Value += consumableComponent.Heath;
                playerStats.Energy.Value += consumableComponent.Energy;

                var inventory = player.GetComponentOfType<ItemsHolder>();
                inventory.Items.Remove(item);

            }
        }
    }
}

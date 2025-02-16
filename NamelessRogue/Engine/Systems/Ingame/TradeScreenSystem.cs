using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.UI;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Systems.Ingame
{
    public class TradeScreenSystem : BaseSystem
    {
        public override HashSet<Type> Signature { get; } = new HashSet<Type>();

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            while (namelessGame.Commander.DequeueCommand(out StartTradeCommand command))
            {
                UIContainer.Instance.TradeScreen.FillTables(command.entityToTradeWith, namelessGame.PlayerEntity);
            }

            while (namelessGame.Commander.DequeueCommand(out TradeTransationCommand command))
            {
                var leftEntity = command.LeftEntity;
                var rightEntity = command.RightEntity;

                var leftItemsHolder = leftEntity.GetComponentOfType<ItemsHolder>();
                var rightItemsHolder = rightEntity.GetComponentOfType<ItemsHolder>();

                foreach (var item in command.ItemsToGiveToLeftEntity)
                {
                    rightItemsHolder.Items.Remove(item);
                    leftItemsHolder.Items.Add(item);
                }

                foreach (var item in command.ItemsToGiveToRightEntity)
                {
                    rightItemsHolder.Items.Add(item);
                    leftItemsHolder.Items.Remove(item);
                }

                if (command.CashToTransferFromRightToLeft != 0)
                {
                    var leftEntityStats = leftEntity.GetComponentOfType<CharacterStats>();
                    var rightEntityStats = rightEntity.GetComponentOfType<CharacterStats>();

                    leftEntityStats.Money += command.CashToTransferFromRightToLeft;
                    rightEntityStats.Money -= command.CashToTransferFromRightToLeft;
                }

                UIContainer.Instance.TradeScreen.FillTables(rightEntity, leftEntity);
            }

            while (namelessGame.Commander.DequeueCommand(out EndTradeCommand command))
            {
                namelessGame.ContextToSwitch = ContextFactory.GetIngameContext(namelessGame);
            }
        }
    }
}

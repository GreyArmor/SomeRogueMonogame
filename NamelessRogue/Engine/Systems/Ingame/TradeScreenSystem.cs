using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
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

        public IEntity NPCTotradeWith { get; set; } = null;

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            while (namelessGame.Commander.DequeueCommand(out StartTradeCommand command))
            {
                UIContainer.Instance.TradeScreen.FillTables(command.entityToTradeWith, namelessGame.PlayerEntity);
            }

            while (namelessGame.Commander.DequeueCommand(out TradeTransationCommand command))
            {

            }

            while (namelessGame.Commander.DequeueCommand(out EndTradeCommand command))
            {
                NPCTotradeWith = null;
            }
        }
    }
}

using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.Engine.Components.UI;
using NamelessRogue.Engine.Systems;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.UI
{
    internal class DialogScreenSystem : BaseSystem
    {
        public override HashSet<Type> Signature { get; } = new HashSet<Type>();

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            while (namelessGame.Commander.DequeueCommand(out UpdateDialogScreenCommand command)) {
                var description = command.CurrentDialogEntity.GetComponentOfType<Description>();                
                UIContainer.Instance.DialogScreen.NpcName = description.Name;
                UIContainer.Instance.DialogScreen.CurrentDialogData = command.CurrentDialogData;                 
            }
        }
    }
}

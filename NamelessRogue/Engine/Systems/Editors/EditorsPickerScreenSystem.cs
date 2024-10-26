using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.UI;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Systems.Editors
{
    internal class EditorsPickerScreenSystem : BaseSystem
    {
        public override HashSet<Type> Signature { get; } = new HashSet<Type>();

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            switch (UIContainer.Instance.EditorsPickerScreen.EditorsPickerScreenActions)
            {
                case EditorsPickerScreenActions.ItemEditor:
                    break;
                case EditorsPickerScreenActions.CharacterEditor:
                    break;
                case EditorsPickerScreenActions.Back:
                    namelessGame.ContextToSwitch = ContextFactory.GetMainMenuContext(namelessGame);
                    break;
                default:
                    break;
            }

            UIContainer.Instance.EditorsPickerScreen.EditorsPickerScreenActions = EditorsPickerScreenActions.None;
        }
    }
}

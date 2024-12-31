using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.UI;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Systems.Editors
{
    internal class EditorCharacterScreenSystem : BaseSystem
    {
        public override HashSet<Type> Signature { get; } = new HashSet<Type>();

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            switch (UIContainer.Instance.EditorCharacterScreen.EditorCharacterScreenActions)
            {
                case EditorCharacterScreenActions.Back:
                    namelessGame.ContextToSwitch = ContextFactory.GetEditorsPickerContext(namelessGame);
                    break;
                default:
                    break;
            }

            UIContainer.Instance.EditorCharacterScreen.EditorCharacterScreenActions = EditorCharacterScreenActions.None;
        }
    }
}

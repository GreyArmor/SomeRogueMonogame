using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.UI;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;

namespace NamelessRogue.Engine.Systems.Editors
{
    internal class EditorLocationScreenSystem : BaseSystem
    {
        public override HashSet<Type> Signature { get; } = new HashSet<Type>();

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            switch (UIContainer.Instance.EditorLocationScreen.EditorBuffScreenActions)
            {
                case EditorBuffScreenActions.Back:
                    namelessGame.ContextToSwitch = ContextFactory.GetEditorsPickerContext(namelessGame);
                    break;
                default:
                    break;
            }

            UIContainer.Instance.EditorLocationScreen.EditorBuffScreenActions = EditorBuffScreenActions.None;
        }
    }
}


using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.UI;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Systems.MainMenu
{
    public class EditorDialogScreenSystem : BaseSystem
    {
        public override HashSet<Type> Signature { get; } = new HashSet<Type>();

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            switch (UIContainer.Instance.EditorDialogScreen.Action)
            {
                case EditorDialogScreenAction.Exit:
                    namelessGame.ContextToSwitch = ContextFactory.GetEditorsPickerContext(namelessGame);
                    break;
                default:
                    break;
            }

            UIContainer.Instance.EditorDialogScreen.Action = EditorDialogScreenAction.None;
        }
    }

    public class EditorQuestScreenSystem : BaseSystem
    {
        public override HashSet<Type> Signature { get; } = new HashSet<Type>();

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
        }
    }


}

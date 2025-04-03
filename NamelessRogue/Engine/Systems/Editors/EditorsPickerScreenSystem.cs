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
                case EditorsPickerScreenActions.LocationEditor:
                    namelessGame.ContextToSwitch = ContextFactory.GetEditorLocationContext(namelessGame);
                    break;
                case EditorsPickerScreenActions.ItemEditor:
                    namelessGame.ContextToSwitch = ContextFactory.GetEditorItemContext(namelessGame);
                    break;
                case EditorsPickerScreenActions.QuestEditor:
                    namelessGame.ContextToSwitch = ContextFactory.GetEEditorQuestContext(namelessGame);
                    break;
                case EditorsPickerScreenActions.AbilityEditor:
                    namelessGame.ContextToSwitch = ContextFactory.GetEditorAbilityContext(namelessGame);
                    break;
                case EditorsPickerScreenActions.CharacterEditor:
                    namelessGame.ContextToSwitch = ContextFactory.GetEditorCharacterContext(namelessGame);
                    break;
                case EditorsPickerScreenActions.Back:
                    namelessGame.ContextToSwitch = ContextFactory.GetMainMenuContext(namelessGame);
                    break;
                case EditorsPickerScreenActions.None:
                    break;
                case EditorsPickerScreenActions.BuffEditor:
                    namelessGame.ContextToSwitch = ContextFactory.GetEditorBuffContext(namelessGame);
                    break;
                    case EditorsPickerScreenActions.Dialog:
                    namelessGame.ContextToSwitch = ContextFactory.GetEditorDialogContext(namelessGame);
                    break;
                default:
                    break;
            }

            UIContainer.Instance.EditorsPickerScreen.EditorsPickerScreenActions = EditorsPickerScreenActions.None;
        }
    }
}

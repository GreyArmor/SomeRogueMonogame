using ImGuiNET;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.UI
{
    public enum EditorsPickerScreenActions
    {
        None,
        ItemEditor,
        CharacterEditor,
        Back,
        AbilityEditor,
        BuffEditor,
        Dialog,
        QuestEditor,
        LocationEditor,
    }
    public class EditorsPickerScreen : BaseScreen
    {

        Vector2 buttonSize = new Vector2(200, 50);
        public EditorsPickerScreenActions EditorsPickerScreenActions { get; set; } = EditorsPickerScreenActions.None;
        public EditorsPickerScreen(NamelessGame game) : base(game)
        {
        }

        public override void DrawLayout()
        {          
            ImGui.Begin("", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar);
            {
                ImGui.SetCursorPos((uiSize / 2) - (buttonSize/2));
                ImGui.BeginChild("EditorPicker");

                if (ButtonWithSound("Location editor", buttonSize))
                {
                    EditorsPickerScreenActions = EditorsPickerScreenActions.LocationEditor;
                }

                if (ButtonWithSound("Quest editor", buttonSize))
                {
                    EditorsPickerScreenActions = EditorsPickerScreenActions.QuestEditor;
                }

                if (ButtonWithSound("Character editor", buttonSize))
                {
                    EditorsPickerScreenActions = EditorsPickerScreenActions.CharacterEditor;
                }
                if (ButtonWithSound("Item editor", buttonSize))
                {
                    EditorsPickerScreenActions = EditorsPickerScreenActions.ItemEditor;
                }

                if (ButtonWithSound("Ability editor", buttonSize))
                {
                    EditorsPickerScreenActions = EditorsPickerScreenActions.AbilityEditor;
                }

                if (ButtonWithSound("Buff editor", buttonSize))
                {
                    EditorsPickerScreenActions = EditorsPickerScreenActions.BuffEditor;
                }

                if (ButtonWithSound("Dialog editor", buttonSize))
                {
                    EditorsPickerScreenActions = EditorsPickerScreenActions.Dialog;
                }



                if (ButtonWithSound("Back", buttonSize))
                {
                    EditorsPickerScreenActions = EditorsPickerScreenActions.Back;
                }
                ImGui.EndChild();
            }
            ImGui.End();

        }
    }
}

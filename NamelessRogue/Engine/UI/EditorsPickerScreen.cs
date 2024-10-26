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
                if (ButtonWithSound("Item editor", buttonSize))
                {
                    EditorsPickerScreenActions = EditorsPickerScreenActions.ItemEditor;
                }
                if (ButtonWithSound("Character editor", buttonSize))
                {
                    EditorsPickerScreenActions = EditorsPickerScreenActions.CharacterEditor;
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

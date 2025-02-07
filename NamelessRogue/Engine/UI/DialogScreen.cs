using Autofac.Features.Indexed;
using ImGuiNET;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Generation.Editor;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.UI
{
    public class DialogScreen : BaseScreen
    {

        public string NpcName { get; set; } = "";
        public int CurrentSelectedOptionIndex;
        public DialogData CurrentDialogData { get; set; } = null;
        public DialogScreen(NamelessGame game) : base(game)
        {
        }

        public override void DrawLayout()
        {
            ImGui.SetNextWindowPos(new Vector2(uiSize.X * 1 / 3, uiSize.Y * 1 / 3));
            ImGui.SetNextWindowSize(new Vector2(uiSize.X * 1 / 4, uiSize.Y * 1 / 4));
            ImGui.Begin("", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.Modal);
            {
                if (CurrentDialogData != null)
                {
                    ValidateIndex();

                    ImGui.Text(NpcName);
                    ImGui.Text(CurrentDialogData.Response);

                    string[] optionsList = CurrentDialogData.Options.Select((option, index) => { return $@"[{index}] " + option.OptionText; }).ToArray();
                    bool clicked = ImGui.ListBox("##options", ref CurrentSelectedOptionIndex, optionsList, optionsList.Length);
                    if (clicked)
                    {
                        PickDialogOptionCommand command = new PickDialogOptionCommand(CurrentDialogData.Options[CurrentSelectedOptionIndex]);
                        game.Commander.EnqueueCommand(command);
                    }

                }
                ImGui.End();
            }
        }

        private void ValidateIndex()
        {
            if (CurrentSelectedOptionIndex < 0)
            {
                CurrentSelectedOptionIndex = 0;
            }
            if (CurrentSelectedOptionIndex >= CurrentDialogData.Options.Length)
            {
                CurrentSelectedOptionIndex = CurrentDialogData.Options.Length - 1;
            }
        }
    }
}

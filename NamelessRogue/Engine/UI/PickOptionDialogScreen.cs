using ImGuiNET;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.shell;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace NamelessRogue.Engine.UI
{

    public class DialogPickOption
    {
        public string Text { get; set; }
        public object OptionData {  get; set; }
    }

    public class PickOptionDialogScreen : BaseScreen
    {
        List<DialogPickOption> options = new List<DialogPickOption>(); 
        public List<DialogPickOption> Options { get => options; set => options = value; }
        public int CurrentSelectedOptionIndex;
        public PickOptionDialogScreen(NamelessGame game) : base(game)
        {
        }

        public override void DrawLayout()
        {
            ImGui.SetNextWindowPos(new Vector2(uiSize.X * 1 / 3, uiSize.Y * 1 / 3));
            var windowSize = new Vector2(uiSize.X * 1 / 4, uiSize.Y * 1 / 4);
            ImGui.SetNextWindowSize(windowSize);
            ImGui.Begin("", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.Modal);
            {
                if (options != null && options.Any())
                {
                    ValidateIndex();

                    string[] optionsList = options.Select((option, index) => { return $@"[{index}] " + option.Text; }).ToArray();
                    ImGui.SetNextItemWidth(windowSize.X);
                    bool clicked = ImGui.ListBox("##options", ref CurrentSelectedOptionIndex, optionsList, optionsList.Length);
                    if (clicked)
                    {
                        PickOptionDialogOptionCommand command = new PickOptionDialogOptionCommand(options[CurrentSelectedOptionIndex]);
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
            if (CurrentSelectedOptionIndex >= options.Count)
            {
                CurrentSelectedOptionIndex = options.Count - 1;
            }
        }
    }
}

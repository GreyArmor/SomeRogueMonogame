using ImGuiNET;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.shell;
using System.Numerics;

namespace NamelessRogue.Engine.UI
{
    public class NewGameGenerationProgressScreen : BaseScreen
    {
        public float ProgressFraction { get; set; } = 0f;
        Vector2 ProgressBarSize { get; set; }
        public NewGameGenerationProgressScreen(NamelessGame game) : base(game)
        {
            ProgressBarSize = new Vector2(-1, 50);
        }

        public override void DrawLayout()
        {
            ImGui.SetNextWindowPos(new System.Numerics.Vector2());
            ImGui.Begin("", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar);

            ImGui.PushFont(ImGUI_FontLibrary.AnonymousPro_Regular24);
            ImGui.SetWindowSize(uiSize);

            ImGui.SetNextWindowPos(new Vector2(0, uiSize.Y - 50 - (buttonSize.Y * 2)));
            // ImGui.SetNextWindowPos(new Vector2(0, uiSize.Y - 50 - buttonSize.Y));
            ImGui.BeginChild("##buttons", new Vector2(0, buttonSize.Y + 2));
            {
                ImGui.ProgressBar(ProgressFraction, ProgressBarSize, "Generating world...");

                //if (ButtonWithSound("Cancel", buttonSize, true))
                //{
                //    game.ContextToSwitch = ContextFactory.GetWorldGenContext(game);
                //}
            }
            ImGui.EndChild();



            ImGui.PopFont();
            ImGui.End();
        }
    }
}

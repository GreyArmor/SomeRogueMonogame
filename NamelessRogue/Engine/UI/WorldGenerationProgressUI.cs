using ImGuiNET;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using static NamelessRogue.Engine.Generation.World.WorldBoardGenerator;

namespace NamelessRogue.Engine.UI
{
    public class WorldGenerationProgressUI : BaseScreen
    {
        public float ProgressFraction { get; set; } = 0f;
        Vector2 ProgressBarSize { get; set; }

        WorldGenerationParameters worldGenerationParameters;
        public WorldGenerationProgressUI(NamelessGame game) : base(game)
        {
            ProgressBarSize = new Vector2(game.GetActualWidth(), 50);
        }

        public override void DrawLayout()
        {
            ImGui.SetNextWindowPos(new System.Numerics.Vector2());
            ImGui.Begin("", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar);

            ImGui.PushFont(ImGUI_FontLibrary.AnonymousPro_Regular24);
            ImGui.SetWindowSize(uiSize);

            ImGui.ProgressBar(ProgressFraction, ProgressBarSize, "Generating world...");
            if (ButtonWithSound("Cancel", buttonSize, worldGenerationParameters.seed.Any()))
            {
                game.ContextToSwitch = ContextFactory.GetWorldGenerationProgressContext(game);
            }

            ImGui.PopFont();
            ImGui.End();
        }
    }
}

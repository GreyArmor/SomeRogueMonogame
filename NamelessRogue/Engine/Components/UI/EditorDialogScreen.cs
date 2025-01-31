using NamelessRogue.Engine.UI;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace NamelessRogue.Engine.UI
{
    public class EditorDialogScreen : BaseScreen
    {
        public EditorDialogScreen(NamelessGame game) : base(game)
        {
        }

        public override void DrawLayout()
        {
            ImGui.Begin("", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar);
            ImGui.BeginChild("menu", UiSize, false);
            {

                //"label" is also an ID on InputText, placing ## in front of it makes it invisible while retaining the ID
                ImGui.Text("Seed");
                ImGui.InputText("##seedinput", ref _seed, 30);
                ImGui.Spacing();
                ImGui.Text("Width");
                ImGui.InputInt("##widthinput", ref worldWidth, 1, 10);
                ImGui.Spacing();
                ImGui.Text("Height");
                ImGui.InputInt("##Heightinput", ref worldHeight, 1, 10);

                if (worldWidth < 100) worldWidth = 100;
                if (worldWidth > 1000) worldWidth = 1000;

                if (worldHeight < 100) worldHeight = 100;
                if (worldHeight > 1000) worldHeight = 1000;

                if (ButtonWithSound("Generate", buttonSize, _seed.Any())) { Action = WorldGenAction.Generate; }
                if (ButtonWithSound("Exit", buttonSize)) { Action = WorldGenAction.Exit; }
            }

            //ImGui.EndChild();
            //}
            ImGui.PopFont();
            ImGui.End();
            ImGui.End();
        }
    }
}

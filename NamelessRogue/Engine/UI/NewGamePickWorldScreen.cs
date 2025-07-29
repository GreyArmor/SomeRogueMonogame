using ImGuiNET;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.shell;
using System;
using System.IO;
using System.Linq;
using System.Numerics;

namespace NamelessRogue.Engine.UI
{
    public class NewGamePickWorldScreen : BaseScreen
    {
        private string[] currentFiles;
        int currentFile = 0;

        Random random;
        public NewGamePickWorldScreen(NamelessGame game) : base(game)
        {
            buttonSize = new System.Numerics.Vector2(uiSize.X / 4, 50);
        }

        public override void DrawLayout()
        {
            ImGui.SetNextWindowPos(new System.Numerics.Vector2());
            ImGui.Begin("", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar);

            var menuSizeX = (uiSize.X / 2);
            var centeredPositonX = (uiSize.X / 2) - (menuSizeX / 2);
            ImGui.PushFont(ImGUI_FontLibrary.AnonymousPro_Regular24);
            ImGui.SetWindowSize(uiSize);
            ImGui.SetNextWindowPos(new Vector2(centeredPositonX, 0));

            currentFiles = Directory.EnumerateFiles("Worlds").ToArray();
            ImGui.BeginChild("##currentItems", new Vector2(menuSizeX, uiSize.Y - 50), false);
            {
                ImGui.BeginChild("##listChild");
                {
                    ImGui.Text("Pick world:");

                    if (currentFiles != null && currentFiles.Any())
                    {
                        ImGui.SetNextItemWidth(menuSizeX);
                        var clicked = ImGui.ListBox("##currentItemsByType", ref currentFile, currentFiles, currentFiles.Length, currentFiles.Length);
                        if(clicked)
                        {
                            currentFiles.ToString();
                        }
                    }
                }
                ImGui.EndChild();
                ImGui.SetNextWindowPos(new Vector2(centeredPositonX, uiSize.Y - 50 - buttonSize.Y));
               // ImGui.SetNextWindowPos(new Vector2(0, uiSize.Y - 50 - buttonSize.Y));
                ImGui.BeginChild("##buttons" , new Vector2(menuSizeX, buttonSize.Y));
                {
                    if (ButtonWithSound("Start", buttonSize)) { game.Commander.EnqueueCommand(new StartNewGameInAWorldCommand(currentFiles[currentFile])); }
                    ImGui.SameLine();
                    if (ButtonWithSound("Back", buttonSize)) { game.Commander.EnqueueCommand(new ExitContextCommand()); }
                }
                ImGui.EndChild();

            }
            ImGui.EndChild();


            //}
            ImGui.PopFont();
            ImGui.End();
        }
    }
}

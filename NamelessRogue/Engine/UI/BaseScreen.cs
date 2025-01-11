using ImGuiNET;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Sounds;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace NamelessRogue.Engine.UI
{

    public abstract class BaseScreen : IBaseGuiScreen
	{
		protected NamelessGame game;
		protected System.Numerics.Vector2 uiSize;
		public BaseScreen(NamelessGame game)
		{
			this.game = game;
			uiSize = new System.Numerics.Vector2(game.GetActualWidth(), game.GetActualHeight());
		}
		public abstract void DrawLayout();

		public bool ButtonWithSound(String text, System.Numerics.Vector2 size, bool enabled = true)
		{
			bool clicked = false;
			if (enabled)
			{
				clicked = ImGui.Button(text, size);
			}
			else
			{
				ImGui.BeginDisabled();
				ImGui.PushStyleVar(ImGuiStyleVar.Alpha, ImGui.GetStyle().Alpha * 0.5f);
				ImGui.Button(text, size);
				ImGui.PopStyleVar();
				ImGui.EndDisabled();
			}
			if (clicked) { game.Commander.EnqueueCommand(new PlaySoundCommand("ButtonClick", false, 0.5f)); }
			return clicked;
		}

        protected static uint ColorToUInt(Color color)
        {
            return (uint)((color.A << 24) | (color.R << 16) | (color.G << 8) | color.B);
        }

        protected void _fillTreeRecursive(string path, IEnumerable<string> fileExtensions, ref string selectedFile)
        {
            ImGui.SetNextItemOpen(true);
            List<string> topDirectoryFiles = new List<string>();
            List<string> subdirectoryFiles = new List<string>();
            foreach (string extension in fileExtensions)
            {
                topDirectoryFiles.AddRange(Directory.GetFiles(path, extension, SearchOption.TopDirectoryOnly));
            }

            foreach (string extension in fileExtensions)
            {
                subdirectoryFiles.AddRange(Directory.GetFiles(path, extension, SearchOption.AllDirectories));
            }
            if (topDirectoryFiles.Any() || subdirectoryFiles.Any())
            {
                ImGui.PushID(path.GetHashCode());
                if (ImGui.TreeNode(Path.GetFileName(path)))
                {
                    ImGui.PopID();

                    subdirectoryFiles = subdirectoryFiles.Except(topDirectoryFiles).ToList();

                    foreach (var file in topDirectoryFiles)
                    {
                        ImGui.PushID(path.GetHashCode() + file.GetHashCode());

                        var flags = file == selectedFile ? ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.Selected : ImGuiTreeNodeFlags.Leaf;
                        ImGui.TreeNodeEx(Path.GetFileName(file), flags);
                        if (ImGui.IsItemClicked() && !ImGui.IsItemToggledOpen())
                        {
                            selectedFile = file;
                        }
                        ImGui.TreePop();
                        ImGui.PopID();
                    }

                    if (subdirectoryFiles.Any())
                    {
                        var subdirectories = Directory.GetDirectories(path);
                        foreach (var subdirectory in subdirectories)
                        {
                            _fillTreeRecursive(subdirectory, fileExtensions, ref selectedFile);
                        }
                    }
                    ImGui.TreePop();
                }
            }
        }


    }
}

using ImGuiNET;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Generation.Editor;
using NamelessRogue.Engine.Sounds;
using NamelessRogue.Engine.Systems.Ingame;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Xml.Serialization;
using Color = System.Drawing.Color;
using Vector2 = System.Numerics.Vector2;

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


        public Vector2 UiSize { get => uiSize; set => uiSize = value; }

     
        protected void DrawCooldownCircle(Vector2 position, int radius, float percentageLeft)
        {
            const int pointsCount = 30;
            var angle = -90f;
            List<Vector2> polygon = new List<Vector2>();
            polygon.Add(position);
            var percentageOfCircleLeft = percentageLeft * pointsCount + 1;

            for (int i = 0; i < percentageOfCircleLeft; i++)
            {
                var x = MathF.Cos(MathHelper.ToRadians(angle));
                var y = MathF.Sin(MathHelper.ToRadians(angle));
                angle -= 360f / pointsCount;
                polygon.Add(new Vector2(position.X + (x * radius), position.Y + (y * radius)));
            }

            polygon.Add(position);
            var polygonArray = polygon.ToArray();
            ImGui.GetForegroundDrawList().AddConvexPolyFilled(ref polygonArray[0], polygon.Count(), ColorToUInt(Color.FromArgb(96, 0, 0, 0)));
        }
    }

    internal class OptionsPopupChooseOptionCommand : ICommand
    {
        public OptionsPopupChooseOptionCommand(int optionsPopupCurrentItem)
        {
            OptionsPopupCurrentItem = optionsPopupCurrentItem;
        }

        public int OptionsPopupCurrentItem { get; }
    }
}

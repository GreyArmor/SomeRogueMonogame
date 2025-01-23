using ImGuiNET;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
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

        int optionsPopupCurrentItem = -1;
        string[] optionsPopupItems = null;
        bool openOptionPopup = false;
        Vector2 optionsPopupPosition = Vector2.Zero;

        public int CurrentOptionsItem
        {
            get { return optionsPopupCurrentItem; }
            set
            {
                optionsPopupCurrentItem = value;
                if (optionsPopupCurrentItem < 0)
                {
                    optionsPopupCurrentItem = 0;
                }
                else if(optionsPopupCurrentItem >= optionsPopupItems.Length)
                {
                    optionsPopupCurrentItem = optionsPopupItems.Length - 1;
                }
            }
        }

        public Vector2 UiSize { get => uiSize; set => uiSize = value; }

        public void OpenOptionsPopUp(IEnumerable<string> options, Vector2 position)
        {
            openOptionPopup = true;
            optionsPopupItems = options.ToArray();
            optionsPopupCurrentItem = 0;
            optionsPopupPosition = position;
        }

        public void CloseOptionsPopUp()
        {
            openOptionPopup = false;
            optionsPopupCurrentItem = -1;
            optionsPopupItems = null;
            optionsPopupPosition = Vector2.Zero;
        }

        public void DrawOptionsPopup()
        {
            if (openOptionPopup)
            {
                ImGui.OpenPopup("##OptionsPopup");

                var popupWidth = 0;
                foreach (var option in optionsPopupItems)
                {
                    var textsize = ImGui.CalcTextSize(option);
                    if (popupWidth < textsize.X)
                    {
                        popupWidth = (int)textsize.X;
                    }
                }
                ImGui.SetNextWindowPos(new Vector2(-popupWidth/2, 0) + uiSize / 2);           

                //ImGui.SetNextWindowSize(new Vector2(100 + popupWidth, 100 + height * optionsPopupItems.Count()));
                bool drop_open = true;
                if (ImGui.BeginPopupModal("##OptionsPopup", ref drop_open, ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.AlwaysAutoResize))
                {
                    var size = ImGui.GetItemRectSize();
                    bool clicked = ImGui.ListBox("##listboOptions", ref optionsPopupCurrentItem, optionsPopupItems, optionsPopupItems.Length);
                    if (clicked)
                    {
                        game.Commander.EnqueueCommand(new OptionsPopupChooseOptionCommand(optionsPopupCurrentItem));
                        openOptionPopup = false;
                        ImGui.CloseCurrentPopup();
                    }
                }
                ImGui.EndPopup();
            }
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

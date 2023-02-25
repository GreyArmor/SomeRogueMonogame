using ImGuiNET;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NamelessRogue.Engine.UI
{
	public enum WorldGenAction
	{
		None,
		Generate,
		Exit
	}
	public class WorldGenerationUI : BaseScreen
	{
		public bool LocalMapDisplay { get; set; } = false;
		public WorldGenAction Action { get; set; } = WorldGenAction.None;

		public string Description { get; internal set; } = "";

		System.Numerics.Vector2 menuPosition;
		System.Numerics.Vector2 buttonSpacing = new System.Numerics.Vector2(0, 5);
		System.Numerics.Vector2 buttonSize;
		System.Numerics.Vector2 shiftVector;
		System.Numerics.Vector2 sidebarSize;

		string _seed = "";
		int worldWidth = 1000;
		int worldHeight = 1000;
		public WorldGenerationUI(NamelessGame game) : base(game)
		{
			buttonSize = new System.Numerics.Vector2(game.Settings.HudWidth - 10, 50);
			shiftVector = new System.Numerics.Vector2(0, buttonSpacing.Y + buttonSize.Y);
			sidebarSize = new System.Numerics.Vector2(uiSize.X / 2, uiSize.Y);
			Random r = new Random();
			_seed = r.Next().ToString();
		}

		public override void DrawLayout()
		{
			menuPosition = new System.Numerics.Vector2(uiSize.X /2 , 0);
			ImGui.SetNextWindowPos(new System.Numerics.Vector2());
			ImGui.Begin("", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar);
			ImGui.PushFont(ImGUI_FontLibrary.AnonymousPro_Regular24);
			ImGui.SetWindowSize(uiSize);

			ImGui.SetCursorPos(menuPosition);
			{
				ImGui.BeginChild("menu", sidebarSize, false);
				{
					ImGui.Text("Seed");
					ImGui.InputText("", ref _seed, 30);
					ImGui.Text("Width ");
					ImGui.SameLine();
					ImGui.InputInt("", ref worldWidth, 1, 10);
					ImGui.Text("Height");
					ImGui.SameLine();					
					ImGui.InputInt("", ref worldHeight, 1, 10);

					if (worldWidth < 100) worldWidth = 100;
					if (worldWidth > 1000) worldWidth = 1000;

					if (worldHeight < 100) worldHeight = 100;
					if (worldHeight > 1000) worldHeight = 1000;

					if (ButtonWithSound("Generate", buttonSize, _seed.Any())) { Action = WorldGenAction.Generate; }
					if (ButtonWithSound("Exit", buttonSize)) { Action = WorldGenAction.Exit; }
				}
			
				ImGui.EndChild();
			}
			ImGui.PopFont();
			ImGui.End();
		}
	}
}

using ImGuiNET;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Text;

namespace NamelessRogue.Engine.UI
{
	public enum MapAction
	{
		None,
		ArtifactMode,
		TerrainMode,
		PoliticalMode,
		RegionsMode,
		Exit
	}

	public enum MapMode
	{
		ArtifactMode,
		TerrainMode,
		PoliticalMode,
		RegionsMode,
	}

	public class MapScreen : BaseScreen
	{
		public bool LocalMapDisplay { get; set; } = false;
		public MapAction Action { get; set; } = MapAction.None;
		public MapMode Mode { get; set; } = MapMode.TerrainMode;

		public string Description { get; internal set; } = "";

		System.Numerics.Vector2 menuPosition;
		System.Numerics.Vector2 buttonSpacing = new System.Numerics.Vector2(0, 5);
		System.Numerics.Vector2 buttonSize;
		System.Numerics.Vector2 shiftVector;
		System.Numerics.Vector2 sidebarSize;
		int buttonCount = 6;
		public MapScreen(NamelessGame game) : base(game)
		{
			buttonSize = new System.Numerics.Vector2(game.Settings.HudWidth - 10, 50);
			shiftVector = new System.Numerics.Vector2(0, buttonSpacing.Y + buttonSize.Y);
			sidebarSize = new System.Numerics.Vector2(game.Settings.HudWidth, shiftVector.Y + buttonSize.Y * buttonCount);
		}

		public override void DrawLayout()
		{
			menuPosition = new System.Numerics.Vector2(uiSize.X - game.Settings.HudWidth + sidebarSize.X / 2 - buttonSize.X / 2, (uiSize.Y / 2) - (sidebarSize.Y / 2));
			ImGui.SetNextWindowPos(new System.Numerics.Vector2());
			ImGui.Begin("", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar);
			ImGui.PushFont(ImGUI_FontLibrary.AnonymousPro_Regular24);
			ImGui.SetWindowSize(uiSize);

			ImGui.SetCursorPos(menuPosition);
			{
				ImGui.BeginChild("menu", sidebarSize);
				{

					if (ImGui.Button("Artifacts", buttonSize)) { Action = MapAction.ArtifactMode; Mode = MapMode.ArtifactMode; };

					ImGui.SetCursorPos(shiftVector);
					if (ImGui.Button("Political", buttonSize)) { Action = MapAction.PoliticalMode; Mode = MapMode.PoliticalMode; };

					ImGui.SetCursorPos(shiftVector * 2);
					if (ImGui.Button("Terrain", buttonSize)) { Action = MapAction.TerrainMode; Mode = MapMode.TerrainMode; };

					ImGui.SetCursorPos(shiftVector * 3);
					if (ImGui.Button("Regions", buttonSize)) { Action = MapAction.RegionsMode; Mode = MapMode.RegionsMode; };

					ImGui.SetCursorPos(shiftVector * 4);
					if (ImGui.RadioButton("LocalMap", LocalMapDisplay)) { LocalMapDisplay = !LocalMapDisplay; };

					ImGui.SetCursorPos(shiftVector * 5);
					if (ImGui.Button("Exit", buttonSize)) { Action = MapAction.Exit; }

				}
				var labelPosition = menuPosition;
				labelPosition.Y = 0;
				ImGui.SetCursorPos(labelPosition);
				ImGui.Text(Description);
				ImGui.PopFont();
				ImGui.EndChild();
			}
			
			ImGui.End();

		}
	}
}

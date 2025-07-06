using ImGuiNET;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Systems.Map;
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
			var zoomComponent = game.WorldMapCameraEntity.GetComponentOfType<WorldMapCameraComponent>();
            menuPosition = new System.Numerics.Vector2(uiSize.X - game.Settings.HudWidth, 0);
            ImGui.Begin("", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoMove);

            ImGui.PushFont(ImGUI_FontLibrary.AnonymousPro_Regular24);
			ImGui.SetWindowSize(uiSize);

			ImGui.SetCursorPos(menuPosition);
			{
				ImGui.BeginChild("menu", sidebarSize);
				{
					ImGui.Text("Zoom");
					ImGui.SliderInt("##zoomslider", ref zoomComponent.zoomIndex, 0, 6);
                    zoomComponent.Zoom = Constants.WorldMapZoomValues[zoomComponent.zoomIndex];
 					if (ButtonWithSound("Exit", buttonSize)) { Action = MapAction.Exit; }

				}
				ImGui.PopFont();
				ImGui.EndChild();
			}
			
			ImGui.End();

		}
	}
}

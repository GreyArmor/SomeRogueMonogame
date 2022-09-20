using ImGuiNET;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Text;

namespace NamelessRogue.Engine.UI
{
	public class MainMenuScreen : BaseScreen
	{	
		System.Numerics.Vector2 uiPosition;
		System.Numerics.Vector2 buttonSpacing = new System.Numerics.Vector2(0, 10);
		System.Numerics.Vector2 buttonSize  = new System.Numerics.Vector2(200, 40);
		System.Numerics.Vector2 shiftVector;
		public MainMenuScreen(NamelessGame game) : base(game) {
			shiftVector = new System.Numerics.Vector2(0, buttonSpacing.Y + buttonSize.Y);
		}

		public override void DrawLayout()
		{
			uiPosition = new System.Numerics.Vector2(uiSize.X - 500, uiSize.Y / 2);
			
			ImGui.Begin("", ImGuiWindowFlags.NoBackground|ImGuiWindowFlags.NoTitleBar|ImGuiWindowFlags.NoResize|ImGuiWindowFlags.NoMove|ImGuiWindowFlags.NoScrollbar);
			ImGui.SetWindowSize(uiSize);
			ImGui.SetCursorPos(uiPosition);
			ImGui.Button("New game", buttonSize);
			ImGui.SetCursorPos(uiPosition + shiftVector);
			ImGui.Button("Load game", buttonSize);
			ImGui.SetCursorPos(uiPosition + (shiftVector*2));
			ImGui.Button("World generation", buttonSize);
			ImGui.SetCursorPos(uiPosition + (shiftVector * 3));
			ImGui.Button("Exit", buttonSize);
			ImGui.End();

		}
	}
}

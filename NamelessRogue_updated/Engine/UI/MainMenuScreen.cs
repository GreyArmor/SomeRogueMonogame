using ImGuiNET;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Text;

namespace NamelessRogue.Engine.UI
{
	public enum MainMenuAction
	{
		None,
		GenerateNewTimeline,
		NewGame,
		Options,
		LoadGame,
		Exit
	}

	public class MainMenuScreen : BaseScreen
	{

		public MainMenuAction Action { get; set; } = MainMenuAction.None;

		System.Numerics.Vector2 menuPosition;
		System.Numerics.Vector2 buttonSpacing = new System.Numerics.Vector2(10, 0);
		System.Numerics.Vector2 buttonSize;
		System.Numerics.Vector2 shiftVector;
		System.Numerics.Vector2 menuSize;
		int buttonCount = 4;
		public MainMenuScreen(NamelessGame game) : base(game) {
			buttonSize = new System.Numerics.Vector2((uiSize.X / buttonCount) - buttonSpacing.X, 50);
			shiftVector = new System.Numerics.Vector2(buttonSpacing.X + buttonSize.X, 0);
			menuSize = new System.Numerics.Vector2(uiSize.X, uiSize.Y * 0.1f);
		}

		public override void DrawLayout()
		{
			menuPosition = new System.Numerics.Vector2((uiSize.X - ((buttonSize.X + buttonSpacing.X) * buttonCount))/2, uiSize.Y * 0.9f);
			ImGui.SetNextWindowPos(new System.Numerics.Vector2());
			ImGui.Begin("", ImGuiWindowFlags.NoBackground|ImGuiWindowFlags.NoTitleBar|ImGuiWindowFlags.NoResize|ImGuiWindowFlags.NoScrollbar);

			ImGui.SetWindowSize(uiSize);
		
			ImGui.SetCursorPos(menuPosition);
			{
				ImGui.BeginChild("menu", menuSize);
				{
					ImGui.PushFont(ImGUI_FontLibrary.AnonymousPro_Regular24);
					if (ButtonWithSound("New game", buttonSize)) { Action = MainMenuAction.NewGame; };

					ImGui.SetCursorPos(shiftVector);
					if (ButtonWithSound("Load game", buttonSize)) { Action = MainMenuAction.LoadGame; }

					ImGui.SetCursorPos(shiftVector * 2);
					if (ButtonWithSound("World generation", buttonSize)) { Action = MainMenuAction.GenerateNewTimeline; }

					ImGui.SetCursorPos(shiftVector * 3);
					if (ButtonWithSound("Exit", buttonSize)) { Action = MainMenuAction.Exit; }
					ImGui.PopFont();
				}
				ImGui.EndChild();
			}
			ImGui.End();

		}
	}
}

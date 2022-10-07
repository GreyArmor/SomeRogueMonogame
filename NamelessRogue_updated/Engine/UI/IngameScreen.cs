using ImGuiNET;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Text;

namespace NamelessRogue.Engine.UI
{
	public enum HudAction
	{
		None,
		OpenWorldMap,
		OpenInventory,
		Options,
		LoadGame,
		Exit
	}

	public class IngameScreen : BaseScreen
	{

		public HudAction Action { get; set; } = HudAction.None;

		System.Numerics.Vector2 menuPosition;
		System.Numerics.Vector2 buttonSpacing = new System.Numerics.Vector2(0, 5);
		System.Numerics.Vector2 buttonSize;
		System.Numerics.Vector2 shiftVector;
		System.Numerics.Vector2 sidebarSize;
		int buttonCount = 2;
		public IngameScreen(NamelessGame game) : base(game)
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

			ImGui.SetWindowSize(uiSize);

			ImGui.SetCursorPos(menuPosition);
			{
				ImGui.BeginChild("menu", sidebarSize);
				{
					ImGui.PushFont(ImGUI_FontLibrary.AnonymousPro_Regular24);
					if (ImGui.Button("Open map", buttonSize)) { Action = HudAction.OpenWorldMap; };

					ImGui.SetCursorPos(shiftVector);
					if (ImGui.Button("Open inventory", buttonSize)) { Action = HudAction.OpenInventory; }
					ImGui.PopFont();
				}
				ImGui.EndChild();
			}
			ImGui.End();

		}
	}
}

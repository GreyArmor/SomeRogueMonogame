using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ECS;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Sounds;
using NamelessRogue.Engine.Systems.Ingame;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

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
		public static string FPS = "0";


		public HudAction Action { get; set; } = HudAction.None;

		System.Numerics.Vector2 menuPosition;
		System.Numerics.Vector2 buttonSpacing = new System.Numerics.Vector2(0, 5);
		System.Numerics.Vector2 buttonSize;
		System.Numerics.Vector2 shiftVector;
		System.Numerics.Vector2 sidebarSize;
        private Texture2D texture;
        int buttonCount = 4;
		public IngameScreen(NamelessGame game) : base(game)
		{
			buttonSize = new System.Numerics.Vector2(game.Settings.HudWidth - 10, 50);
			shiftVector = new System.Numerics.Vector2(0, buttonSpacing.Y + buttonSize.Y);
			sidebarSize = new System.Numerics.Vector2(game.Settings.HudWidth, shiftVector.Y + buttonSize.Y * buttonCount);
            texture = game.Content.Load<Texture2D>("DfFont");

        }
		public static int rowIndexEnd = 33;
		public static int verticesPerRow = 36;
        public static int substractionCoef = 1;

        public override void DrawLayout()
		{
			menuPosition = new System.Numerics.Vector2(uiSize.X - game.Settings.HudWidth, 0);
			ImGui.SetNextWindowPos(new System.Numerics.Vector2());
			ImGui.Begin("", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar);

			ImGui.SetWindowSize(uiSize);

            Player player = game.PlayerEntity.GetComponentOfType<Player>();
            var modifiers = game.PlayerEntity.GetComponentOfType<ModifiersCollection>();
			var stats = modifiers.Accumulator.GetComponentOfType<CharacterStats>();

            var healthText = stats.Health.Value.ToString() + "//" + (stats.Health.MaxValue.ToString());
            var energyText = stats.Energy.Value.ToString() + "//" + (stats.Energy.MaxValue.ToString());

            var healthTextSize = ImGui.CalcTextSize(healthText);
            var energyTextSize = ImGui.CalcTextSize(energyText);

            ImGui.SetCursorPos((HudElementsRenderingSystem.HealthPosition - new Microsoft.Xna.Framework.Vector2(0, healthTextSize.Y)).ToNumerics());
            ImGui.Text(healthText);

            ImGui.SetCursorPos((HudElementsRenderingSystem.EnergyPosition - new Microsoft.Xna.Framework.Vector2(0, energyTextSize.Y)).ToNumerics());
            ImGui.Text(energyText);


            ImGui.SetCursorPos(menuPosition);
			{
				ImGui.BeginChild("sidebar");
				{

					ImGui.Text("Turn: " + game.CurrentGame.Turn);
                    ImGui.Text("HP: " + stats.Health.Value);
					ImGui.SameLine();
					ImGui.Text("EP: " + stats.Energy.Value);
					ImGui.Text("Speed: " + stats.Speed.Value);
					ImGui.Text("Vision range: " + stats.VisionRange.Value);
					ImGui.Text("Weight: " + stats.Weight.Value);
					ImGui.Text("Armor:");
					var armors = stats.GetArmorByTypes();
					foreach (var armor in armors)
					{
						ImGui.Text("     ");
						ImGui.SameLine();
						ImGui.Text(armor.Key + ": " + armor.Value);
					}

                    ImGui.Text("Resistances:");
                    var resistances = stats.GetArmorByTypes();
                    foreach (var res in resistances)
                    {
                        ImGui.Text("     ");
                        ImGui.SameLine();
                        ImGui.Text(res.Key + ": " + res.Value);
                    }

                    ImGui.Text("Damage:");
                    var weapons = stats.GetWeaponsByTypes();
					foreach (var weapon in weapons)
					{
                        ImGui.Text("     ");
                        ImGui.SameLine();
                        ImGui.Text(weapon.Key + ": " + weapon.Value.Item1 + " - " + weapon.Value.Item2);
                    }	

                    ImGui.BeginChild("menu", sidebarSize);
					{
						ImGui.PushFont(ImGUI_FontLibrary.AnonymousPro_Regular24);
						if (ButtonWithSound("Open map", buttonSize)) { Action = HudAction.OpenWorldMap; };

						ImGui.SetCursorPos(shiftVector);
						if (ButtonWithSound("Open inventory", buttonSize)) { Action = HudAction.OpenInventory; }

						ImGui.PopFont();

					}
					ImGui.EndChild();
				}
                ImGui.EndChild();
            }
			ImGui.End();

		}
	}
}

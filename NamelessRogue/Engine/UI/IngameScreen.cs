using ImGuiNET;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ECS;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.Engine.Components.UI;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Sounds;
using NamelessRogue.Engine.Systems.Ingame;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        OpenAbilities,
        Exit,
  
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
        string log = "";
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

        int iconSize = 32;
        public override void DrawLayout()
        {
            menuPosition = new System.Numerics.Vector2(uiSize.X - game.Settings.HudWidth, 0);
            ImGui.SetNextWindowPos(new System.Numerics.Vector2());
            ImGui.Begin("", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar);


            DrawOptionsPopup();

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
                    DrawBuffs();
                    ImGui.Text("FPS " + FPS);
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

                    ImGui.BeginChild("menu", sidebarSize, false, ImGuiWindowFlags.NoScrollbar);
                    {
                        ImGui.PushFont(ImGUI_FontLibrary.AnonymousPro_Regular24);
                        if (ButtonWithSound("Open map", buttonSize)) { Action = HudAction.OpenWorldMap; };

                        ImGui.SetCursorPos(shiftVector);
                        if (ButtonWithSound("Open inventory", buttonSize)) { Action = HudAction.OpenInventory; }

                        ImGui.SetCursorPos(shiftVector*2);
                        if (ButtonWithSound("Abilities", buttonSize)) { Action = HudAction.OpenAbilities; }


                        ImGui.PopFont();


                    }
                    ImGui.EndChild();
                }
                var pos = ImGui.GetCursorPos();
                var logWindowSize = new System.Numerics.Vector2(sidebarSize.X, uiSize.Y - pos.Y - 50);
                ImGui.BeginChild("logWindow", logWindowSize, true);
                {
                    var splitStr = log.Split('\n');
                    foreach (var str in splitStr)
                    {
                        ImGui.TextUnformatted(str);
                    }
                    if (ImGui.GetScrollY() >= ImGui.GetScrollMaxY())
                    {
                        ImGui.SetScrollHereY(1.0f);
                    }
                }
                ImGui.EndChild();
               
            }
            ImGui.EndChild();
            DrawAbilityBar();

            ImGui.End();

        }

        private void DrawBuffs()
        {
            ImGui.SetCursorPos(new System.Numerics.Vector2(0, 0));
            {
                var modifiers = game.PlayerEntity.GetComponentOfType<ModifiersCollection>();

                bool noBuffs = true;

                foreach (var modifier in modifiers.ModifierEntities)
                {
                    var icon = modifier.GetComponentOfType<UiIconComponent>();
                    var equipment = modifier.GetComponentOfType<Equipment>();
                    var timed = modifier.GetComponentOfType<TimedModifier>();
                    if (icon != null && equipment == null)
                    {
                        noBuffs = false;

                        ImGui.BeginChild(modifier.GetHashCode().ToString(), new System.Numerics.Vector2(iconSize + 1));
                        ImGui.Image(ImGuiImageLibrary.Textures["cellDeselected"], new System.Numerics.Vector2(iconSize, iconSize));
                        ImGui.SetCursorPos(new System.Numerics.Vector2(0));
                        ImGui.Image(ImGuiImageLibrary.Textures[icon.IconId], new System.Numerics.Vector2(iconSize, iconSize));
                        if (timed != null)
                        {
                            ImGui.PushFont(ImGUI_FontLibrary.AnonymousPro_Regular24);

                            string durationText = timed.TurnsToLast.ToString();
                            var textSize = ImGui.CalcTextSize(durationText);
                            //crude outline	
                            ImGui.PushStyleColor(ImGuiCol.Text, new System.Numerics.Vector4(0, 0, 0, 1));
                            ImGui.SetCursorPos(new System.Numerics.Vector2(iconSize - 1, iconSize) - textSize);
                            ImGui.Text(durationText);
                            ImGui.SetCursorPos(new System.Numerics.Vector2(iconSize + 1, iconSize) - textSize);
                            ImGui.Text(durationText);
                            ImGui.SetCursorPos(new System.Numerics.Vector2(iconSize, iconSize - 1) - textSize);
                            ImGui.Text(durationText);
                            ImGui.SetCursorPos(new System.Numerics.Vector2(iconSize, iconSize + 1) - textSize);
                            ImGui.Text(durationText);
                            ImGui.PopStyleColor();
                            //
                            ImGui.SetCursorPos(new System.Numerics.Vector2(iconSize, iconSize) - textSize);
                            ImGui.Text(durationText);

                            ImGui.PopFont();
                        }
                        ImGui.EndChild();
                        ImGui.SameLine();
                    }
                }

                if (noBuffs)
                {
                    ImGui.SetCursorPosY(iconSize);
                }

                ImGui.NewLine();
            }
        }

        private void DrawAbilityBar()
        {

            var abilityBinder = game.PlayerEntity.GetComponentOfType<AbilityBinder>();
            
            ImGui.SetCursorPos(new System.Numerics.Vector2(iconSize, uiSize.Y - (iconSize*2)));
            {
                for (int i = 0; i < 10; i++)
                {
                    var idValue = i + 1;
                    var isAbilityBound = abilityBinder.AbilityBindings.TryGetValue(idValue, out var binding);

                    ImGui.BeginChild(i + "abilityBind", new System.Numerics.Vector2(iconSize + 1));
                    ImGui.Image(ImGuiImageLibrary.Textures["cellDeselected"], new System.Numerics.Vector2(iconSize, iconSize));
                    if (isAbilityBound)
                    {
                        var icon = binding.GetComponentOfType<UiIconComponent>();
                        if (icon != null)
                        {
                            ImGui.SetCursorPos(new System.Numerics.Vector2(0));
                            ImGui.Image(ImGuiImageLibrary.Textures[icon.IconId], new System.Numerics.Vector2(iconSize, iconSize));
                            if (ImGui.IsItemHovered())
                            {
                                var description = binding.GetComponentOfType<Description>();
                                if (description != null)
                                {
                                    ImGui.SetTooltip(description.Text);
                                }
                            }

                        }
                    }
                    ImGui.PushFont(ImGUI_FontLibrary.AnonymousPro_Regular16);
                   
                    idValue = idValue > 9 ? 0 : idValue;
                    string idText = idValue.ToString();
                    var textSize = ImGui.CalcTextSize(idText);
                    //crude outline	
                    ImGui.PushStyleColor(ImGuiCol.Text, new System.Numerics.Vector4(0, 0, 0, 1));
                    ImGui.SetCursorPos(new System.Numerics.Vector2(iconSize - 1, iconSize) - textSize);
                    ImGui.Text(idText);
                    ImGui.SetCursorPos(new System.Numerics.Vector2(iconSize + 1, iconSize) - textSize);
                    ImGui.Text(idText);
                    ImGui.SetCursorPos(new System.Numerics.Vector2(iconSize, iconSize - 1) - textSize);
                    ImGui.Text(idText);
                    ImGui.SetCursorPos(new System.Numerics.Vector2(iconSize, iconSize + 1) - textSize);
                    ImGui.Text(idText);
                    ImGui.PopStyleColor();
                    //
                    ImGui.SetCursorPos(new System.Numerics.Vector2(iconSize, iconSize) - textSize);
                    ImGui.Text(idText);

                    ImGui.PopFont();

                    ImGui.EndChild();
                    ImGui.SameLine();
                }
            }
            ImGui.NewLine();
        }
    


        internal void LogMessage(string logMessage)
        {
			log += logMessage + "\n";
        }
    }
}

using ImGuiNET;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.Engine.Components.UI;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using Point = System.Drawing.Point;
using Vector2 = System.Numerics.Vector2;

namespace NamelessRogue.Engine.UI
{
    public enum AbilityScreenAction
    {
        None,
    }

    public class AbilityScreen : BaseScreen
    {
        public AbilityScreenAction Action { get; set; } = AbilityScreenAction.None;
        public readonly int CountOfFilters = 7;

        Vector2 halfsize;
        Vector2 quartersize;
        int iconSize = 32;
        int iconSizeWithMargin = 34;
        int rightSideWidth;

        public AbilityScreen(NamelessGame game) : base(game)
        {
            uiSize = new System.Numerics.Vector2(game.GetActualWidth(), game.GetActualHeight());
            halfsize = uiSize / 2;
            quartersize = halfsize / 2;
            rightSideWidth = (int)(((halfsize.X / iconSizeWithMargin) - 2) * iconSizeWithMargin);

        }
        public override void DrawLayout()
        {
            ImGui.SetNextWindowPos(new System.Numerics.Vector2());
            ImGui.Begin("", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar);

            ImGui.SetWindowSize(uiSize);

            var abilityBinder = game.PlayerEntity.GetComponentOfType<AbilityBinder>();
            var abilityHolder = game.PlayerEntity.GetComponentOfType<AbilityHolder>();

            if (abilityBinder != null)
            {
                ImGui.SetCursorPos(new System.Numerics.Vector2(10, 10));
                {
                    ImGui.BeginChild("AbilityCollection");
                    for (int i = 0; i < 10; i++)
                    {
                        foreach (var ability in abilityHolder.Abilities)
                        {
                            var abilityDesc = ability.GetComponentOfType<Description>();
                            var icon = ability.GetComponentOfType<UiIconComponent>();

                            var cursorPosition = ImGui.GetCursorPos();
                            ImGui.Image(ImGuiImageLibrary.Textures["cellDeselected"], new System.Numerics.Vector2(iconSize, iconSize));
                            ImGui.SetCursorPos(cursorPosition);
                            ImGui.Image(ImGuiImageLibrary.Textures[icon.IconId], new System.Numerics.Vector2(iconSize, iconSize));
                            ImGui.Text(abilityDesc.Name);
                            ImGui.Text("    " + abilityDesc.Text);
                            ImGui.Separator();
                        }
                    }
                  
                    ImGui.EndChild();
                }
            }


            DrawAbilityBar();

            ImGui.End();
        }

        private void DrawAbilityBar()
        {

            var abilityBinder = game.PlayerEntity.GetComponentOfType<AbilityBinder>();

            ImGui.SetCursorPos(new System.Numerics.Vector2(iconSize, uiSize.Y - (iconSize * 2)));
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

    }
}

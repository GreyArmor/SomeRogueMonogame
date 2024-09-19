using ImGuiNET;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Components.UI;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Input;
using NamelessRogue.Engine.Systems.Inventory;
using NamelessRogue.shell;
using RogueSharp;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Web;
using System.Windows.Forms;
using Point = System.Drawing.Point;
using Vector2 = System.Numerics.Vector2;

namespace NamelessRogue.Engine.UI
{
    public enum InventoryScreeAction
    {
        None,
    }


    public class GridCell
    {
        public Guid ItemId { get; set; }
        public Point GridPosition { get; set; }
    }


    public class InventoryGridModel
    {
        public GridCell[,] Cells { get; set; }
        public int Width { get; }
        public int Height { get; }

        public InventoryGridModel(int width, int height)
        {
            Cells = new GridCell[width, height];
            Width = width;
            Height = height;
        }

        public void Fill(ItemsHolder holder, List<ItemType> filters)
        {
            var itemQueue = new Queue<IEntity>(holder.Items.Where(entity => filters.Contains(entity.GetComponentOfType<Item>().Type)));
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (!itemQueue.Any())
                    {
                        break;
                    }
                    var item = itemQueue.Dequeue();
                    var itemComponent = item.GetComponentOfType<Item>();
                        Cells[x, y] = new GridCell();
                        Cells[x, y].GridPosition = new Point(x, y);
                        Cells[x, y].ItemId = item.Id;
                }
            }
        }

        public void Clear()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Cells[x, y] = null;
                }
            }
        }
    }


    public class FilterFlags
    {
        public bool All {  get; set; }
        public bool Weapons { get; set; }
        public bool Armor { get; set; }
        public bool Consumables { get; set; }
        public bool Food { get; set; }
        public bool Ammo { get; set; }

        public bool Misc { get; set; }
    }

    public class InventoryScreen : BaseScreen
    {
        FilterFlags flags = new FilterFlags() { All = true };
        public InventoryGridModel InventoryGridModel { get; set; }
        public MainMenuAction Action { get; set; } = MainMenuAction.None;
        Vector2 halfsize;
        int iconSize = 32;
        int iconSizeWithMargin = 34;

        int topMenuHeight = 40;
        int topMenuButtonWidth;

        int rightSideWidth;
        List<ItemType> filters = new List<ItemType>();        
        public Point SelectedCell { get; set; } = new Point();
        public InventoryScreen(NamelessGame game) : base(game)
        {
            uiSize = new System.Numerics.Vector2(game.GetActualWidth(), game.GetActualHeight());
            halfsize = uiSize / 2;

            rightSideWidth = (int)(((halfsize.X / iconSizeWithMargin) - 2) * iconSizeWithMargin);

            InventoryGridModel = new InventoryGridModel((int)(halfsize.X/ iconSizeWithMargin), (int)(halfsize.Y / iconSizeWithMargin));
            topMenuButtonWidth = (int)(rightSideWidth / 7);
        }

        public override void DrawLayout()
        {
            filters.Clear();
            ImGui.SetNextWindowPos(new System.Numerics.Vector2());
            ImGui.Begin("", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar);

            ImGui.SetWindowSize(uiSize);
            bool _addSelectableSameLine(string text, bool selected, params ItemType[] filter)
            {
                var clicked = ImGui.Selectable(text, selected, ImGuiSelectableFlags.None, new System.Numerics.Vector2(topMenuButtonWidth, topMenuHeight));
                ImGui.SameLine();
                if (clicked)
                {
                    selected = !selected;
                }

                if(selected)
                {
                    filters.AddRange(filter);
                }
                return selected;
                
            }

            ImGui.SetCursorPos(new System.Numerics.Vector2(halfsize.X, 0));
            {
                flags.All = _addSelectableSameLine("All", flags.All, Enum.GetValues(typeof(ItemType)).Cast<ItemType>().ToArray());
                flags.Weapons = _addSelectableSameLine("Weapons", flags.Weapons, ItemType.Weapon);
                flags.Armor = _addSelectableSameLine("Armor", flags.Armor, ItemType.Armor);
                flags.Consumables = _addSelectableSameLine("Consumables", flags.Consumables, ItemType.Consumable);
                flags.Food = _addSelectableSameLine("Food", flags.Food, ItemType.Food);
                flags.Ammo = _addSelectableSameLine("Ammo", flags.Ammo, ItemType.Ammo);
                flags.Misc = _addSelectableSameLine("Misc", flags.Misc, ItemType.Misc);

                if(filters.Any())
                {
                    Clear();
                    Fill(filters);
                }
                ImGui.SetCursorPos(new System.Numerics.Vector2(halfsize.X, topMenuHeight));
                ImGui.BeginChild("inventoryGrid", new Vector2(halfsize.X, uiSize.Y));
                {
                    for (int y = 0; y < InventoryGridModel.Height; y++)
                    {
                        for (int x = 0; x < InventoryGridModel.Width; x++)
                        {
                            var cell = InventoryGridModel.Cells[x, y];
                            string itemId = "";

                            if (cell != null)
                            {
                                var drawable = game.GetEntity(cell.ItemId).GetComponentOfType<Drawable>();
                                itemId = drawable.ObjectID;
                            }
                            ImGui.SetCursorPos(new System.Numerics.Vector2(34 * x, (iconSizeWithMargin * y)));
                            if (x == SelectedCell.X && y == SelectedCell.Y)
                            {
                                if ((34 * y) > uiSize.Y)
                                {
                                    ImGui.SetScrollY((34 * y) - uiSize.Y);
                                }                            
                                ImGui.Image(ImGuiImageLibrary.Textures["cellSelected"], new Vector2(iconSize, iconSize));
                            }
                            else
                            {
                                ImGui.Image(ImGuiImageLibrary.Textures["cellDeselected"], new Vector2(iconSize, iconSize));
                            }
                            ImGui.SetCursorPos(new System.Numerics.Vector2(34 * x, (iconSizeWithMargin * y)));
                            if (itemId! != "")
                            {
                                ImGui.Image(ImGuiImageLibrary.Textures[itemId], new Vector2(iconSize, iconSize));
                            }
                        }
                    }
                    if (SelectedCell.X >= 0 && SelectedCell.Y >= 0 && SelectedCell.X < InventoryGridModel.Width && SelectedCell.Y < InventoryGridModel.Height)
                    {
                        ImGui.SetCursorPos(new System.Numerics.Vector2(0, (iconSizeWithMargin * InventoryGridModel.Height)));
                        ImGui.BeginChild("inventoryBorder", new Vector2(halfsize.X, halfsize.Y), true);
                        var selectedCell = InventoryGridModel.Cells[SelectedCell.X, SelectedCell.Y];
                        if (selectedCell != null)
                        {
                            ImGui.PushFont(ImGUI_FontLibrary.AnonymousPro_Regular24);
                            string itemDescription = "";

                            var selectedItem = game.GetEntity(selectedCell.ItemId);
                            var desccomponent = selectedItem.GetComponentOfType<Description>();
                            var itemComponent = selectedItem.GetComponentOfType<Item>();
                            var itemWeaponStats = selectedItem.GetComponentOfType<WeaponStats>();

                            itemDescription += $@"{desccomponent.Name} \n";
                            itemDescription += $@"{desccomponent.Text} \n";
                            itemDescription += $@"Manufacturer: {itemComponent.Author} \n";
                            // itemDescription += $@"Manufacturer: {itemComponent.} \n";

                            if (itemWeaponStats != null)
                            {
                                itemDescription += $@"Attack type: {itemWeaponStats.AttackType.ToString()} \n";
                                itemDescription += $@"Ammo type:   {itemWeaponStats.AmmoType.ToString()}   \n";
                                itemDescription += $@"Damage: {itemWeaponStats.MinimumDamage.ToString()} - {itemWeaponStats.MaximumDamage.ToString()} \n";
                                itemDescription += $@"Range: {itemWeaponStats.Range.ToString()} \n";
                                itemDescription += $@"Max ammo: {itemWeaponStats.AmmoInClip.ToString()} \n";
                                itemDescription += $@"Current ammo: {itemWeaponStats.CurrentAmmo.ToString()} \n";
                            }
                           // ImGui.SetCursorPos(new System.Numerics.Vector2(0, (iconSizeWithMargin * InventoryGridModel.Height)));
                            // ImGui.LogText(itemDescription);
                            var splitSting = itemDescription.Split("\\n");
                            for (int i = 0; i < splitSting.Count(); i++)
                            {
                                ImGui.TextWrapped(splitSting[i]);
                            }
                            ImGui.PopFont();
                        }
                        ImGui.EndChild();
                    }


                }
                ImGui.EndChild();
            }
            ImGui.End();
        }

        public void Fill(List<ItemType> filters)
        {
            var player = game.PlayerEntity;
            var holder = player.GetComponentOfType<ItemsHolder>();
            InventoryGridModel.Fill(holder, filters);
        }

        public void FillAll()
        {
            Fill(Enum.GetValues(typeof(ItemType)).Cast<ItemType>().ToList());
        }

        public void Clear()
        {
            InventoryGridModel.Clear(); 
        }
    }
}

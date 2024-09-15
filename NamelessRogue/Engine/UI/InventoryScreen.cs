using ImGuiNET;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Input;
using NamelessRogue.Engine.Systems.Inventory;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Web;
using System.Windows.Forms;

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

    public class InventoryScreen : BaseScreen
    {
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

            rightSideWidth = (int)(((halfsize.X / iconSize) - 2) * iconSizeWithMargin);

            InventoryGridModel = new InventoryGridModel((int)(halfsize.X/iconSize) - 2, (int)(halfsize.Y / iconSize) - 2);
            topMenuButtonWidth = (int)(rightSideWidth / 7);
        }

        public override void DrawLayout()
        {
            filters.Clear();
            ImGui.SetNextWindowPos(new System.Numerics.Vector2());
            ImGui.Begin("", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar);

            ImGui.SetWindowSize(uiSize);


            void _addButtonSameLine(string text, params ItemType[] filter)
            {
                if (ButtonWithSound(text, new Vector2(topMenuButtonWidth, topMenuHeight)))
                {
                    filters.AddRange(filter);
                }
                ImGui.SameLine();
            }

            ImGui.SetCursorPos(new System.Numerics.Vector2(halfsize.X, 0));
            {
                _addButtonSameLine("All", Enum.GetValues(typeof(ItemType)).Cast<ItemType>().ToArray());
                _addButtonSameLine("Weapons", ItemType.Weapon);
                _addButtonSameLine("Armor", ItemType.Armor);
                _addButtonSameLine("Consumables", ItemType.Consumable);
                _addButtonSameLine("Food", ItemType.Food);
                _addButtonSameLine("Ammo", ItemType.Ammo);
                _addButtonSameLine("Misc", ItemType.Misc);
                _addButtonSameLine("Weapons", ItemType.Weapon);

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

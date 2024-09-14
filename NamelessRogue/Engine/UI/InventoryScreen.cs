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

        public void Fill(ItemsHolder holder)
        {
            var itemQueue = new Queue<IEntity>(holder.Items.ToList());
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (!itemQueue.Any())
                    {
                        break;
                    }
                    var item = itemQueue.Dequeue();
                    Cells[x, y] = new GridCell();
                    Cells[x, y].GridPosition = new Point(x, y);
                    Cells[x, y].ItemId = item.Id;
                }
            }
        }
    }

    public class InventoryScreen : BaseScreen
    {
        public InventoryGridModel InventoryGridModel { get; set; }
        public MainMenuAction Action { get; set; } = MainMenuAction.None;
        Vector2 halfsize;

        public Point SelectedCell { get; set; } = new Point();
        public InventoryScreen(NamelessGame game) : base(game)
        {
            uiSize = new System.Numerics.Vector2(game.GetActualWidth(), game.GetActualHeight());
            halfsize = uiSize / 2;
            InventoryGridModel = new InventoryGridModel(20, 50);
        }

        public override void DrawLayout()
        {
            ImGui.SetNextWindowPos(new System.Numerics.Vector2());
            ImGui.Begin("", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar);

            ImGui.SetWindowSize(uiSize);

            ImGui.SetCursorPos(new System.Numerics.Vector2(halfsize.X, 0));
            {
                ImGui.BeginChild("inventoryGrid");
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
                            ImGui.SetCursorPos(new System.Numerics.Vector2(34 * x, 34 * y));
                            if (x == SelectedCell.X && y == SelectedCell.Y)
                            {
                                ImGui.Image(ImGuiImageLibrary.Textures["cellSelected"], new Vector2(32, 32));
                            }
                            else
                            {                                
                                ImGui.Image(ImGuiImageLibrary.Textures["cellDeselected"], new Vector2(32, 32));
                            }
                            ImGui.SetCursorPos(new System.Numerics.Vector2(34 * x, 34 * y));
                            if (itemId! != "")
                            {
                                ImGui.Image(ImGuiImageLibrary.Textures[itemId], new Vector2(32, 32));
                            }

                        }
                    }
                }
                ImGui.EndChild();
            }
            ImGui.End();
        }

        public void Fill()
        {
            var player = game.PlayerEntity;
            var holder = player.GetComponentOfType<ItemsHolder>();
            InventoryGridModel.Fill(holder);
        }

    }
}

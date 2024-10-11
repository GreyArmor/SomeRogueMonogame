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
using static System.Net.Mime.MediaTypeNames;
using Point = System.Drawing.Point;
using Vector2 = System.Numerics.Vector2;

namespace NamelessRogue.Engine.UI
{
    public enum InventoryScreeAction
    {
        None,
    }

    public enum InventoryScreenCursorMode
    {
        Items, ItemsFilter, Equipment,
    }

    public class GridCell
    {
        public Guid ItemId { get; set; }
        public Point GridPosition { get; set; }
    }

    public class EquipmentSlot
    {
        public Guid ItemId { get; set; }
        public Point GridPosition { get; set; }
        public Slot Slot { get; set; }
    }

    public class InventoryGridModel
    {
        public Point SelectedCell { get; set; } = new Point();
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

    public class InventoryEquipmentVisualModel
    {
        public Dictionary<Point, Slot> CursorPositionsDict = new Dictionary<Point, Slot>();
        public Dictionary<Slot, Vector2> IconPositionsDict = new Dictionary<Slot, Vector2>();
        public Point SelectedCell { get; set; } = new Point();
        public InventoryEquipmentVisualModel(Vector2 uiSize, Vector2 halfsize, Vector2 quartersize, int equipmentSize)
        {

            void _addSlot(Slot slot, Point point, Vector2 position)
            {
                CursorPositionsDict.Add(point, slot);
                IconPositionsDict.Add(slot, position);
            }

            _addSlot(Slot.Head, new Point(1, 0),        new Vector2(quartersize.X, 0));
            _addSlot(Slot.Face, new Point(1, 1),        new Vector2(quartersize.X, quartersize.Y / 2));
            _addSlot(Slot.Torso, new Point(1, 2),       new Vector2(quartersize.X, halfsize.Y - (quartersize.Y / 2)));
            _addSlot(Slot.Legs, new Point(1, 3),        new Vector2(quartersize.X, halfsize.Y + (quartersize.Y / 2)));
            _addSlot(Slot.Feet, new Point(1, 4),        new Vector2(quartersize.X, uiSize.Y - equipmentSize - (32)));
  
            _addSlot(Slot.Hands, new Point(2, 1),       new Vector2(halfsize.X - (equipmentSize * 2), quartersize.Y));
            _addSlot(Slot.Back, new Point(0, 1),        new Vector2(equipmentSize, quartersize.Y));

            _addSlot(Slot.LefHand, new Point(0, 2), new Vector2(equipmentSize, halfsize.Y));
            _addSlot(Slot.RightHand, new Point(2, 2), new Vector2(halfsize.X - (equipmentSize * 2), halfsize.Y));
        }

    }

    public class InventoryFiltersVisualModel
    {
        public Point SelectedCell { get; set; } = new Point();
    }
    public class FilterFlags
    {
        public bool[] FilterArray = new bool[7];

        public bool All { get { return FilterArray[0]; } set { FilterArray[0] = value; } }
        public bool Weapons { get { return FilterArray[1]; } set { FilterArray[1] = value; } }
        public bool Armor { get { return FilterArray[2]; } set { FilterArray[2] = value; } }
        public bool Consumables { get { return FilterArray[3]; } set { FilterArray[3] = value; } }
        public bool Food { get { return FilterArray[4]; } set { FilterArray[4] = value; } }
        public bool Ammo { get { return FilterArray[5]; } set { FilterArray[5] = value; } }

        public bool Misc { get { return FilterArray[6]; } set { FilterArray[6] = value; } }
    }

    public class InventoryScreen : BaseScreen
    {
        FilterFlags flags = new FilterFlags() { All = true };

        public FilterFlags Flags { get { return flags; } set { flags = value; } }
        public InventoryGridModel GridModel { get; set; }

        public InventoryFiltersVisualModel FiltersVisualModel { get; set; }

        public InventoryEquipmentVisualModel EquipmentVisualModel { get; set; } 
        public MainMenuAction Action { get; set; } = MainMenuAction.None;
        public InventoryScreenCursorMode CursorMode { get; set; } = InventoryScreenCursorMode.Items;

        public readonly int CountOfFilters = 7;

        Vector2 halfsize;
        Vector2 quartersize;
        int iconSize = 32;
        int iconSizeWithMargin = 34;

        int topMenuHeight = 40;
        int topMenuButtonWidth;

        int equipmentSize = 64;

        int rightSideWidth;
        List<ItemType> filters = new List<ItemType>();        

        public InventoryScreen(NamelessGame game) : base(game)
        {
            uiSize = new System.Numerics.Vector2(game.GetActualWidth(), game.GetActualHeight());
            halfsize = uiSize / 2;
            quartersize = halfsize / 2;
            rightSideWidth = (int)(((halfsize.X / iconSizeWithMargin) - 2) * iconSizeWithMargin);

         
            topMenuButtonWidth = (int)(rightSideWidth / 7);

            GridModel = new InventoryGridModel((int)(halfsize.X / iconSizeWithMargin), (int)(halfsize.Y / iconSizeWithMargin));
            EquipmentVisualModel = new InventoryEquipmentVisualModel(uiSize, halfsize, quartersize, equipmentSize);
            FiltersVisualModel = new InventoryFiltersVisualModel();
        }
        bool _addSelectableSameLine(int index, string text, bool selected, params ItemType[] filter)
        {
            bool drawBorder = CursorMode == InventoryScreenCursorMode.ItemsFilter && FiltersVisualModel.SelectedCell.X == index;

            if (drawBorder)
            {
                var cursorPos = ImGui.GetCursorPos();
                ImGui.Image(ImGuiImageLibrary.Textures["selectionColor"], new Vector2(topMenuButtonWidth, topMenuHeight));
                ImGui.SetCursorPos(cursorPos);
            }
            var clicked = ImGui.Selectable(text, selected, ImGuiSelectableFlags.None, new System.Numerics.Vector2(topMenuButtonWidth, topMenuHeight));
            ImGui.SameLine();

            if (clicked)
            {
                selected = !selected;
            }
            if (selected)
            {
                filters.AddRange(filter);
            }
            return selected;
        }

        void _addEquipmentCell(Vector2 position, string text)
        {
            ImGui.SetCursorPos(new System.Numerics.Vector2(quartersize.X, 0));
            ImGui.Image(ImGuiImageLibrary.Textures["cellDeselected"], new Vector2(equipmentSize, equipmentSize));
        }

        public override void DrawLayout()
        {
            filters.Clear();
            ImGui.SetNextWindowPos(new System.Numerics.Vector2());
            ImGui.Begin("", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar);

            ImGui.SetWindowSize(uiSize);   

            ImGui.SetCursorPos(new System.Numerics.Vector2(halfsize.X, 0));
            {
                flags.All = _addSelectableSameLine(0, "All", flags.All, Enum.GetValues(typeof(ItemType)).Cast<ItemType>().ToArray());
                flags.Weapons = _addSelectableSameLine(1, "Weapons", flags.Weapons, ItemType.Weapon);
                flags.Armor = _addSelectableSameLine(2, "Armor", flags.Armor, ItemType.Armor);
                flags.Consumables = _addSelectableSameLine(3, "Consumables", flags.Consumables, ItemType.Consumable);
                flags.Food = _addSelectableSameLine(4, "Food", flags.Food, ItemType.Food);
                flags.Ammo = _addSelectableSameLine(5, "Ammo", flags.Ammo, ItemType.Ammo);
                flags.Misc = _addSelectableSameLine(6, "Misc", flags.Misc, ItemType.Misc);

                Clear();
                FillInventory(filters);

                ImGui.SetCursorPos(new System.Numerics.Vector2(halfsize.X, topMenuHeight));
                ImGui.BeginChild("inventoryGrid", new Vector2(halfsize.X, uiSize.Y));
                {
                    for (int y = 0; y <GridModel.Height; y++)
                    {
                        for (int x = 0; x < GridModel.Width; x++)
                        {
                            var cell = GridModel.Cells[x, y];
                            string itemId = "";

                            if (cell != null)
                            {
                                var drawable = game.GetEntity(cell.ItemId).GetComponentOfType<Drawable>();
                                itemId = drawable.ObjectID;
                            }
                            ImGui.SetCursorPos(new System.Numerics.Vector2(34 * x, (iconSizeWithMargin * y)));
                            if (this.CursorMode == InventoryScreenCursorMode.Items && x == GridModel.SelectedCell.X && y == GridModel.SelectedCell.Y )
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
                    ImGui.SetCursorPos(new System.Numerics.Vector2(0, (iconSizeWithMargin * GridModel.Height)));
                    ImGui.BeginChild("inventoryBorder", new Vector2(halfsize.X, halfsize.Y), true);
                    if (GridModel.SelectedCell.X >= 0 && GridModel.SelectedCell.Y >= 0 && GridModel.SelectedCell.X < GridModel.Width && GridModel.SelectedCell.Y < GridModel.Height)
                    {
                        if (this.CursorMode == InventoryScreenCursorMode.Items)
                        {
                            var selectedCell = GridModel.Cells[GridModel.SelectedCell.X, GridModel.SelectedCell.Y];
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
                        }                       
                    }
                    ImGui.EndChild();
                }
                ImGui.EndChild();
            }


            
            ImGui.SetCursorPos(new System.Numerics.Vector2(halfsize.X, 0));
            {
                ImGui.Begin("", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar);

                var hasValue = EquipmentVisualModel.CursorPositionsDict.TryGetValue(EquipmentVisualModel.SelectedCell, out var currentSlot);

                foreach (var slot in Enum.GetValues(typeof(Slot)).Cast<Slot>())
                {
                    var pos = EquipmentVisualModel.IconPositionsDict[slot];
                    ImGui.SetCursorPos(pos);
                    ImGui.Text(slot.ToString());
                    ImGui.SetCursorPos(new Vector2(pos.X, pos.Y+(ImGui.GetFontSize()*2)));
                    if (hasValue && currentSlot == slot && CursorMode == InventoryScreenCursorMode.Equipment)
                    {
                        ImGui.Image(ImGuiImageLibrary.Textures["cellSelected"], new Vector2(equipmentSize, equipmentSize));
                    }
                    else
                    {
                        ImGui.Image(ImGuiImageLibrary.Textures["cellDeselected"], new Vector2(equipmentSize, equipmentSize));
                    }
                }

                ////head
                //ImGui.SetCursorPos(new System.Numerics.Vector2(quartersize.X, 0));
                //ImGui.Image(ImGuiImageLibrary.Textures["cellDeselected"], new Vector2(equipmentSize, equipmentSize));

                ImGui.EndChild();
            }

            ImGui.End();
        }

        public void FillInventory(List<ItemType> filters)
        {
            var player = game.PlayerEntity;
            var holder = player.GetComponentOfType<ItemsHolder>();
            GridModel.Fill(holder, filters);
        }

        public void FillInventoryWithAll()
        {
            FillInventory(Enum.GetValues(typeof(ItemType)).Cast<ItemType>().ToList());
        }

        public void Clear()
        {
            GridModel.Clear(); 
        }
    }
}

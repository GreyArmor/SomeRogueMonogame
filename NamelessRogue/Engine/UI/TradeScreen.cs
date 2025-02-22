using ImGuiNET;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.Engine.Components.UI;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Systems.Ingame;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.UI
{
    public enum TradeCursorMode
    {
        RightTable, LeftTable, RightTableFilters, LeftTableFilters
    }
    public class TradeScreen : BaseScreen
    {
        const int backgroundSelectorHeight = 30;
        TradeScreenTableModel rightTable = null;
        TradeScreenTableModel leftTable = null;
        TradeScreenTableModel currentTable = null;
       
        TradeCursorMode cursorMode = TradeCursorMode.LeftTable;
        private IEntity rightTableEntity;
        private IEntity leftTableEntity;

        public TradeCursorMode CursorMode { get => cursorMode; private set => cursorMode = value; }
        public IEntity RightTableEntity { get => rightTableEntity; set => rightTableEntity = value; }
        public IEntity LeftTableEntity { get => leftTableEntity; set => leftTableEntity = value; }       

        public TradeScreen(NamelessGame game) : base(game)
        {
            rightTable = new TradeScreenTableModel();
            leftTable = new TradeScreenTableModel();
            var allItemTypes = Enum.GetValues(typeof(ItemType)).Cast<ItemType>().ToArray();
            rightTable.Filters.AddRange(allItemTypes);
            rightTable.Flags.All = true;
            leftTable.Filters.AddRange(allItemTypes);
            leftTable.Flags.All = true;
        }


        bool _addSelectableSameLine(int index, string text, bool selected, bool filterSelection, TradeScreenTableModel tableModel, ref bool filterClicked, params ItemType[] filter)
        {
            bool drawBorder = filterSelection && tableModel.SelectedFilterIndex == index;
            var textSize = ImGui.CalcTextSize(text);
            if (drawBorder)
            {           
                var cursorPos = ImGui.GetCursorPos();
                ImGui.Image(ImGuiImageLibrary.Textures["selectionColor"], new Vector2(textSize.X, 20));
                ImGui.SetCursorPos(cursorPos);
            }
            var clicked = ImGui.Selectable(text, selected, ImGuiSelectableFlags.None, new System.Numerics.Vector2(textSize.X, 20));
            ImGui.SameLine();
            if (clicked)
            {
                filterClicked = true;
                selected = !selected;
            }
            if (selected)
            {
                tableModel.Filters.AddRange(filter);
            }

            return selected;
        }

        public override void DrawLayout()
        {
            if (rightTable != null && leftTable != null)
            {
                ImGui.Begin("", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoMove);
                {
                    bool showLeftTabletSelector = CursorMode == TradeCursorMode.LeftTable;
                    bool showRightTabletSelector = CursorMode == TradeCursorMode.RightTable;
                    bool showLeftTabletFilterSelector = CursorMode == TradeCursorMode.LeftTableFilters;
                    bool showRightTabletFilterSelector = CursorMode == TradeCursorMode.RightTableFilters;

                    var tableOffsetY = new Vector2(0, 40);
                    var tableOffsetX = new Vector2(20, 0);
                    var middleGapOffsetX = new Vector2(10, 0);
                    var leftTablePosition = new Vector2(0, 20) + tableOffsetX;
                    var rightTablePosition = new Vector2(uiSize.X / 2, 20) + tableOffsetX;

                    var leftTableSize =  new Vector2(uiSize.X / 2 - 20, uiSize.Y / 2) - tableOffsetX;
                    var rightTableSize = new Vector2(uiSize.X / 2 - 20, uiSize.Y / 2) - tableOffsetX;
                    var middlePosition = new Vector2(uiSize.X / 2, 20);

                    var tabledescriptionSize = new Vector2(uiSize.X / 2 - 20, uiSize.Y/3) - tableOffsetX;
                    int total = GetTotal();
                    var totalText = total + "$"; 
                    
                    ImGui.PushFont(ImGUI_FontLibrary.AnonymousPro_Regular24);
                    var totalTextSize = ImGui.CalcTextSize(totalText);
                    ImGui.SetCursorPos(middlePosition - new Vector2(totalTextSize.X/2, 0)); 

                    ImGui.Text(totalText);
                    ImGui.PopFont();
                    DrawTable(leftTable, leftTablePosition + tableOffsetY, leftTableSize, showLeftTabletSelector, showLeftTabletFilterSelector);
                    DrawTable(rightTable, rightTablePosition + tableOffsetY, rightTableSize, showRightTabletSelector, showRightTabletFilterSelector);
                    DrawSelectedItemDescriptionTable(leftTable, leftTablePosition + new Vector2(0, leftTableSize.Y) + tableOffsetY, tabledescriptionSize);
                    DrawSelectedItemDescriptionTable(rightTable, rightTablePosition + new Vector2(0, rightTableSize.Y) + tableOffsetY, tabledescriptionSize);
                    //   DrawSelectedItemDescriptionTable(rightTable, rightTablePosition + rightTableSize, rightTableSize);

                    var leftTableMoney = GetLeftTableMoney();
                    var rightTableMoney = GetRightTableMoney();

                    var leftTableMoneyStr = leftTableMoney + "$";
                    var rightTableMoneyStr = rightTableMoney + "$";

                    var positionYAfterTables = ImGui.GetCursorPosY();

                    ImGui.SetCursorPos(new Vector2(10, positionYAfterTables) + tableOffsetX);
                    ImGui.PushFont(ImGUI_FontLibrary.AnonymousPro_Regular24);
                    ImGui.Text(leftTableMoneyStr);
                    ImGui.PopFont();

                    ImGui.SetCursorPos(new Vector2(middlePosition.X - (buttonSize.X / 2), positionYAfterTables));
                    bool unableToTrade = IsUnableToTrade(total, leftTableMoney, rightTableMoney);
                    var tradePressed = ButtonWithSound("[T]rade", buttonSize, !unableToTrade);
                    if (tradePressed)
                    {
                        CreateTrade(total);
                    }
                    ImGui.PushFont(ImGUI_FontLibrary.AnonymousPro_Regular24);

                    var rightTableTextSize = ImGui.CalcTextSize(rightTableMoneyStr);
                    ImGui.SetCursorPos(new Vector2(UiSize.X - (rightTableTextSize.X*2), positionYAfterTables) + tableOffsetX);
                  
                    ImGui.Text(rightTableMoneyStr);
                    ImGui.PopFont();

                    ImGui.SetCursorPos(new Vector2(UiSize.X - (buttonSize.X) - tableOffsetX.X, 5));
                    var escape = ButtonWithSound("[Esc]ape", buttonSize, true);
                    if (escape)
                    {
                        var endTrade = new EndTradeCommand();
                        game.Commander.EnqueueCommand(endTrade);
                    }

                }
                ImGui.End();
            }
        }

        public int GetRightTableMoney()
        {
            return RightTableEntity.GetComponentOfType<CharacterStats>().Money;
        }

        public int GetLeftTableMoney()
        {
            return LeftTableEntity.GetComponentOfType<CharacterStats>().Money;
        }

        public int GetTotal()
        {
            var leftTotal = leftTable.SumOfSelectedObjects();
            var rightTotal = rightTable.SumOfSelectedObjects();
            var total = rightTotal - leftTotal;
            return total;
        }

        public void CreateTrade(int total)
        {
            List<IEntity> leftSelectedentities = leftTable.Items.Where(x => x.selectedForTrade).Select(item => item.entityReference).ToList();
            List<IEntity> rightSelectedentities = rightTable.Items.Where(x => x.selectedForTrade).Select(item => item.entityReference).ToList();

            var tradeCommand = new TradeTransationCommand(RightTableEntity, LeftTableEntity, rightSelectedentities, leftSelectedentities, total);
            game.Commander.EnqueueCommand(tradeCommand);
        }

        public bool IsUnableToTrade(int total, int leftTableMoney, int rightTableMoney)
        {
            return rightTableMoney < -total || leftTableMoney < total;
        }

        private void DrawTable(TradeScreenTableModel table, Vector2 position, Vector2 size, bool showSelector, bool showFilterSelector)
        {
            var tableId = table.GetHashCode().ToString();
            int numberOfCharactersInLine = (int)((size.X-32) / ImGui.CalcTextSize(".").X) - 5 ;
            ImGui.SetCursorPos(position);
           
            ImGui.BeginChild(tableId, size, false, ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove);
            {
                table.Filters.Clear();
                bool filterClicked = false;
                table.Flags.All = _addSelectableSameLine(0, "All", table.Flags.All, showFilterSelector, table, ref filterClicked, Enum.GetValues(typeof(ItemType)).Cast<ItemType>().ToArray());
                table.Flags.Weapons = _addSelectableSameLine(1, "Weapons", table.Flags.Weapons, showFilterSelector, table, ref filterClicked, ItemType.Weapon);
                table.Flags.Armor = _addSelectableSameLine(2, "Armor", table.Flags.Armor, showFilterSelector, table, ref filterClicked, ItemType.Armor);
                table.Flags.Consumables = _addSelectableSameLine(3, "Consumables", table.Flags.Consumables, showFilterSelector, table, ref filterClicked, ItemType.Consumable);
                table.Flags.Food = _addSelectableSameLine(4, "Food", table.Flags.Food, showFilterSelector, table, ref filterClicked, ItemType.Supplies);
                table.Flags.Ammo = _addSelectableSameLine(5, "Ammo", table.Flags.Ammo, showFilterSelector, table, ref filterClicked, ItemType.Ammo);
                table.Flags.Misc = _addSelectableSameLine(6, "Misc", table.Flags.Misc, showFilterSelector, table, ref filterClicked, ItemType.Misc);

                if(filterClicked)
                {
                    table.Fill(table.TableEntity, table.Filters);
                }

                ImGui.NewLine();

                var posYoffset = new Vector2(0, ImGui.GetCursorPosY());

                ImGui.BeginChild(tableId, size - posYoffset, true, ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove);
                {
                    for (int i = 0; i < table.Items.Count; i++)
                    {
                        TradeScreenItem tableItem = table.Items[i];
                        ImGui.BeginChild("itemChild" + i + tableId, new Vector2(size.X, 49), false, ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove);
                        var textSize = ImGui.CalcTextSize(table.ItemsNames[i]);
                        int numberOfCharacters = 0;
                        bool itemClicked = false;
                        if (tableItem.selectedForTrade)
                        {
                            ImGui.Text(">");
                            ImGui.SameLine();
                            numberOfCharacters++;
                        }
                        var prevCursorPos = ImGui.GetCursorPos();
                        ImGui.Image(ImGuiImageLibrary.Textures[table.ItemIconsIds[i]], new Vector2(32, 32));

                        ImGui.SameLine();

                        numberOfCharacters += (table.ItemsNames[i] + table.ItemsPrices[i] + "$").Length;
                        int numberOfDots = numberOfCharactersInLine - numberOfCharacters;

                        string spaces = new string(' ', numberOfDots);

                        ImGui.Text(table.ItemsNames[i] + spaces + table.ItemsPrices[i] + "$");

                        var itemSize = ImGui.GetItemRectSize();
                        var itemPos = ImGui.GetItemRectMin();
                        var style = ImGui.GetStyle();
                        if (showSelector && i == table.SelectedItem)
                        {
                            ImGui.Separator();

                        }
                        ImGui.EndChild();

                        itemClicked = !itemClicked ? ImGui.IsItemClicked() : itemClicked;
                        if (itemClicked)
                        {
                            tableItem.selectedForTrade = !tableItem.selectedForTrade;
                            table.CurrentChanged = true;
                            table.SelectedItem = i;
                        }

                        if (table.CurrentChanged && i == table.SelectedItem)
                        {
                            var scrollY = ImGui.GetScrollY();
                            var pos = ImGui.GetCursorPosY();

                            if (pos >= scrollY + size.Y)
                            {
                                ImGui.SetScrollHereY(1.0f);
                            }
                            else if ((pos - 48) <= scrollY)
                            {
                                ImGui.SetScrollY(pos - 55);
                            }
                            table.CurrentChanged = true;
                        }
                    }                 
                }
                ImGui.EndChild();
            }
            ImGui.EndChild();
        }

        private void DrawSelectedItemDescriptionTable(TradeScreenTableModel table, Vector2 position, Vector2 size)
        {
            var tableId = table.GetHashCode().ToString();
            int numberOfCharactersInLine = (int)((size.X - 32) / ImGui.CalcTextSize(".").X) - 5;
            ImGui.SetCursorPos(position);
            ImGui.BeginChild("itemDesc" + tableId, size, true, ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove);
            if (table.Items.Count > 0)
            {
                TradeScreenItem tableItem = table.Items[table.SelectedItem];
                ImGui.Image(ImGuiImageLibrary.Textures[table.ItemIconsIds[table.SelectedItem]], new Vector2(32, 32));
                ImGui.SameLine();
                ImGui.Text(table.ItemsNames[table.SelectedItem]);
                ImGui.Text(table.ItemsDescriptions[table.SelectedItem]);
            }
            ImGui.EndChild();


        }

        public void FillTables(IEntity leftTableEntity, IEntity rightTableEntity)
        {           
            rightTable.Fill(rightTableEntity, rightTable.Filters);
            leftTable.Fill(leftTableEntity, leftTable.Filters);

            this.RightTableEntity = rightTableEntity;
            this.LeftTableEntity = leftTableEntity;
            currentTable = leftTable;
        }

        public void ReleaseTables()
        {
            rightTable = null;
            leftTable = null;
            currentTable = null;
        }

        internal void MoveCursorUp()
        {
            if (currentTable.SelectedItem>0)
            {
                currentTable.SelectedItem--;
                currentTable.CurrentChanged = true;
            }
            else
            {
                switch (cursorMode)
                {
                    case TradeCursorMode.LeftTable:
                        this.SwitchMode(TradeCursorMode.LeftTableFilters);
                        break;
                    case TradeCursorMode.RightTable:
                        this.SwitchMode(TradeCursorMode.RightTableFilters);
                        break;
                }
            }
        }

        internal void MoveCursorDown()
        {
            if (currentTable.SelectedItem < currentTable.Items.Count - 1)
            {
                currentTable.SelectedItem++;
                currentTable.CurrentChanged = true;
            }
            else
            {
                switch (cursorMode)
                {
                    case TradeCursorMode.LeftTable:
                        this.SwitchMode(TradeCursorMode.LeftTableFilters);
                        break;
                    case TradeCursorMode.RightTable:
                        this.SwitchMode(TradeCursorMode.RightTableFilters);
                        break;
                }
            }
        }

        public void SwitchMode(TradeCursorMode mode)
        {
            if (mode == TradeCursorMode.RightTable || mode == TradeCursorMode.RightTableFilters)
            {
                currentTable = rightTable;
            }

            if (mode == TradeCursorMode.LeftTable || mode == TradeCursorMode.LeftTableFilters)
            {
                currentTable = leftTable;
            }

            CursorMode = mode;
        }

        internal void SelectDeselectCurrentItem()
        {
            switch (cursorMode)
            {
                case TradeCursorMode.RightTable:
                case TradeCursorMode.LeftTable:
                    if (currentTable != null && currentTable.SelectedItem >= 0)
                    {
                        currentTable.Items[currentTable.SelectedItem].selectedForTrade = !currentTable.Items[currentTable.SelectedItem].selectedForTrade;
                    }
                    break;
                case TradeCursorMode.RightTableFilters:
                case TradeCursorMode.LeftTableFilters:
                    if (currentTable != null && currentTable.SelectedFilterIndex >= 0)
                    {
                        currentTable.Flags.FilterArray[currentTable.SelectedFilterIndex] = !currentTable.Flags.FilterArray[currentTable.SelectedFilterIndex];

                        var selected = currentTable.Flags.FilterArray[currentTable.SelectedFilterIndex];

                        if (selected)
                        {
                            currentTable.Filters.AddRange(currentTable.Flags.Filters[currentTable.SelectedFilterIndex]);
                        }
                        else
                        {
                            foreach(var type in currentTable.Flags.Filters[currentTable.SelectedFilterIndex])
                            {
                                currentTable.Filters.Remove(type);
                            }
                        }

                        currentTable.Fill(currentTable.TableEntity, currentTable.Filters);
                    }
                    break;
            }            
        }

        internal void MoveFilterCursorLeft()
        {
            if (currentTable.SelectedFilterIndex > 0)
            {
                currentTable.SelectedFilterIndex--;
            }
            else
            {
                switch (cursorMode)
                {
                    case TradeCursorMode.RightTableFilters:
                        this.SwitchMode(TradeCursorMode.LeftTableFilters);
                        currentTable.SelectedFilterIndex = currentTable.Flags.FilterArray.Length - 1;
                        break;
                    case TradeCursorMode.LeftTableFilters:
                        this.SwitchMode(TradeCursorMode.RightTableFilters);
                        currentTable.SelectedFilterIndex = currentTable.Flags.FilterArray.Length - 1;
                        break;
                }
            }
        }

        internal void MoveFilterCursorRight()
        {
            if (currentTable.SelectedFilterIndex < currentTable.Flags.FilterArray.Length-1)
            {
                currentTable.SelectedFilterIndex++;
            }
            else
            {
                switch (cursorMode)
                {
                    case TradeCursorMode.RightTableFilters:
                        this.SwitchMode(TradeCursorMode.LeftTableFilters);
                        currentTable.SelectedFilterIndex = 0;
                        break;
                    case TradeCursorMode.LeftTableFilters:
                        this.SwitchMode(TradeCursorMode.RightTableFilters);
                        currentTable.SelectedFilterIndex = 0;
                        break;
                }
            }
        }

        internal void MoveFilterCursorDown()
        {
            switch (cursorMode)
            {
                case TradeCursorMode.RightTableFilters:
                    this.SwitchMode(TradeCursorMode.RightTable);
                    break;
                case TradeCursorMode.LeftTableFilters:
                    this.SwitchMode(TradeCursorMode.LeftTable);
                    break;
            }
        }
        internal void MoveFilterCursorUp()
        {
            switch (cursorMode)
            {
                case TradeCursorMode.RightTableFilters:
                    this.SwitchMode(TradeCursorMode.RightTable);
                    break;
                case TradeCursorMode.LeftTableFilters:
                    this.SwitchMode(TradeCursorMode.LeftTable);
                    break;
            }
        }
    }


    public class TradeScreenItem
    {
        public IEntity entityReference;
        public bool selectedForTrade;
    }

    public class TradeScreenTableModel
    {
        public FilterFlags Flags { get; set; } = new FilterFlags() { All = true };
        public string FilterString { get; set; } = "";
        public TradeScreenTableModel() { }

        public bool CurrentChanged { get; set; }
        public List<TradeScreenItem> Items { get; set; } = new List<TradeScreenItem>();
        public List<string> ItemsNames = new List<string>();
        public List<string> ItemsDescriptions = new List<string>();
        public List<int> ItemsPrices = new List<int>();
        public List<string> ItemIconsIds = new List<string>();
        public int Cash { get; set; }
        public int SelectedFilterIndex { get; set; } = 0;
        public List<ItemType> Filters { get; set; } = new List<ItemType>();

        public int SelectedItem;
        public IEntity TableEntity { get; set; }

        public void Fill(IEntity itemsHolderEntity, List<ItemType> filters)
        {
            SelectedItem = 0;
            TableEntity = itemsHolderEntity;
            Items.Clear();
            ItemsNames.Clear();
            ItemsDescriptions.Clear();
            ItemsPrices.Clear();
            ItemIconsIds.Clear();
            var characterStats = itemsHolderEntity.GetComponentOfType<CharacterStats>();
            var itemsHolder = itemsHolderEntity.GetComponentOfType<ItemsHolder>();
                       
            foreach (var item in itemsHolder.Items)
            {

                var itemComponent = item.GetComponentOfType<Item>();
                if(!filters.Contains(itemComponent.Type))
                {
                    continue;
                }
                Items.Add(new TradeScreenItem { entityReference = item, selectedForTrade = false });
                var description = item.GetComponentOfType<Description>();
              
                var icon = item.GetComponentOfType<UiIconComponent>();
                ItemsNames.Add(description.Name);
                ItemsDescriptions.Add(description.Text);
                ItemsPrices.Add(itemComponent.Price);
                ItemIconsIds.Add(icon.IconId);
            }

            Cash = 0;
            if (characterStats != null)
            {
                Cash = characterStats.Money;
            }
        }

        public int SumOfSelectedObjects()
        {
            int total = 0;
            for (int i = 0; i < Items.Count; i++)
            {
                TradeScreenItem item = Items[i];
                if(item.selectedForTrade)
                {
                    total += ItemsPrices[i];
                }
            }
            return total;
        }
    }
}

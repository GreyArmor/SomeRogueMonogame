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
        RightTable, LeftTable, BottomMenu
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

        public TradeCursorMode CursorMode { get => cursorMode; set => cursorMode = value; }
        public IEntity RightTableEntity { get => rightTableEntity; set => rightTableEntity = value; }
        public IEntity LeftTableEntity { get => leftTableEntity; set => leftTableEntity = value; }       

        public TradeScreen(NamelessGame game) : base(game)
        {
        }


        public override void DrawLayout()
        {
            if (rightTable != null && leftTable != null)
            {
                ImGui.Begin("", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoMove);
                {
                    bool showLeftTabletSelector = CursorMode == TradeCursorMode.LeftTable;
                    bool showRightTabletSelector = CursorMode == TradeCursorMode.RightTable;

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
                    DrawTable(leftTable, leftTablePosition + tableOffsetY, leftTableSize, showLeftTabletSelector);
                    DrawTable(rightTable, rightTablePosition + tableOffsetY, rightTableSize, showRightTabletSelector);
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

        private void DrawTable(TradeScreenTableModel table, Vector2 position, Vector2 size, bool showSelector)
        {
            var tableId = table.GetHashCode().ToString();
            int numberOfCharactersInLine = (int)((size.X-32) / ImGui.CalcTextSize(".").X) - 5 ;
            ImGui.SetCursorPos(position);
            ImGui.BeginChild(tableId, size, true, ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove);
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

         
            ImGui.EndChild();
        }

        private void DrawSelectedItemDescriptionTable(TradeScreenTableModel table, Vector2 position, Vector2 size)
        {
            if (table.Items.Count > 0)
            {
                var tableId = table.GetHashCode().ToString();
                int numberOfCharactersInLine = (int)((size.X - 32) / ImGui.CalcTextSize(".").X) - 5;
                ImGui.SetCursorPos(position);
                ImGui.BeginChild("itemDesc" + tableId, size, true, ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove);
                TradeScreenItem tableItem = table.Items[table.SelectedItem];
                ImGui.Image(ImGuiImageLibrary.Textures[table.ItemIconsIds[table.SelectedItem]], new Vector2(32, 32));
                ImGui.SameLine();
                ImGui.Text(table.ItemsNames[table.SelectedItem]);
                ImGui.Text(table.ItemsDescriptions[table.SelectedItem]);


                ImGui.EndChild();
            }
        
         
        }

        public void FillTables(IEntity leftTableEntity, IEntity rightTableEntity)
        {
            rightTable = new TradeScreenTableModel();
            leftTable = new TradeScreenTableModel();

            rightTable.Fill(rightTableEntity);
            leftTable.Fill(leftTableEntity);

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
            if (currentTable != null && currentTable.SelectedItem>0)
            {
                currentTable.SelectedItem--;
                currentTable.CurrentChanged = true;
            }
        }

        internal void MoveCursorDown()
        {
            if (currentTable != null && currentTable.SelectedItem < currentTable.Items.Count - 1)
            {
                currentTable.SelectedItem++;
                currentTable.CurrentChanged = true;
            }
        }

        public void SwitchMode(TradeCursorMode mode)
        {
            if (mode == TradeCursorMode.RightTable)
            {
                currentTable = rightTable;
            }

            if (mode == TradeCursorMode.LeftTable)
            {
                currentTable = leftTable;
            }

            CursorMode = mode;
        }

        internal void SelectDeselectCurrentItem()
        {
            if (currentTable != null && currentTable.SelectedItem >= 0)
            {
                currentTable.Items[currentTable.SelectedItem].selectedForTrade = !currentTable.Items[currentTable.SelectedItem].selectedForTrade;
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
        public TradeScreenTableModel() { }

        public bool CurrentChanged { get; set; }
        public List<TradeScreenItem> Items { get; set; } = new List<TradeScreenItem>();
        public List<string> ItemsNames = new List<string>();
        public List<string> ItemsDescriptions = new List<string>();
        public List<int> ItemsPrices = new List<int>();
        public List<string> ItemIconsIds = new List<string>();
        public int Cash { get; set; }
        public int SelectedItem;

        public void Fill(IEntity itemsHolderEntity)
        {
            Items.Clear();
            ItemsNames.Clear();
            ItemsDescriptions.Clear();
            ItemsPrices.Clear();
            ItemIconsIds.Clear();
            var characterStats = itemsHolderEntity.GetComponentOfType<CharacterStats>();
            var itemsHolder = itemsHolderEntity.GetComponentOfType<ItemsHolder>();

            foreach (var item in itemsHolder.Items)
            {
                Items.Add(new TradeScreenItem { entityReference = item, selectedForTrade = false });
                var description = item.GetComponentOfType<Description>();
                var itemComponent = item.GetComponentOfType<Item>();
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

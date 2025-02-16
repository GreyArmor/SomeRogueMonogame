using ImGuiNET;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.Engine.Components.UI;
using NamelessRogue.Engine.Systems.Ingame;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
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
                    DrawTable(rightTable, new Vector2(20,20), new Vector2(uiSize.X/2 - 20, uiSize.Y -100), showRightTabletSelector);
                    DrawTable(leftTable, new Vector2(uiSize.X / 2 + 20, 20), new Vector2(uiSize.X / 2 - 40, uiSize.Y - 100), showLeftTabletSelector);         
                    ImGui.SetCursorPos(new Vector2(uiSize.X/2, ImGui.GetCursorPosY()));
                    var tradePressed = ButtonWithSound("Trade", buttonSize, true);

                    if (tradePressed)
                    {
                        List<IEntity> leftSelectedentities = leftTable.Items.Where(x=>x.selectedForTrade).Select(item=>item.entityReference).ToList();
                        List<IEntity> rightSelectedentities = rightTable.Items.Where(x => x.selectedForTrade).Select(item => item.entityReference).ToList();

                        var tradeCommand = new TradeTransationCommand(rightTableEntity, leftTableEntity, rightSelectedentities, leftSelectedentities, 100);
                        game.Commander.EnqueueCommand(tradeCommand);
                    }

                }
                ImGui.End();
            }
        }

        private void DrawTable(TradeScreenTableModel table, Vector2 position, Vector2 size, bool showSelector)
        {
            var tableId = table.GetHashCode().ToString();
            int numberOfCharactersInLine = (int)(size.X / ImGui.CalcTextSize(".").X) - 3 ;
            ImGui.SetCursorPos(position);
            ImGui.BeginChild(tableId, size, true, ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoScrollbar);
            for (int i = 0; i < table.Items.Count; i++)
            {
                TradeScreenItem tableItem = table.Items[i];
                ImGui.PushItemWidth(size.X);
                var textSize = ImGui.CalcTextSize(table.ItemsNames[i]);
                int numberOfCharacters = 0;
                bool itemClicked = false;
                if (tableItem.selectedForTrade)
                {
                    ImGui.Text(">");
                    itemClicked = !itemClicked ? ImGui.IsItemClicked() : itemClicked;
                    ImGui.SameLine();
                    numberOfCharacters++;
                }
                numberOfCharacters += (table.ItemsNames[i] + table.ItemsPrices[i] + "$").Length;
                int numberOfDots = numberOfCharactersInLine - numberOfCharacters;

                string spaces = new string(' ', numberOfDots);

                ImGui.Text(table.ItemsNames[i] + spaces + table.ItemsPrices[i] + "$");
                itemClicked = !itemClicked ? ImGui.IsItemClicked() : itemClicked;
                if (itemClicked)
                {
                    tableItem.selectedForTrade = !tableItem.selectedForTrade;
                }
                var itemSize = ImGui.GetItemRectSize();
                var itemPos = ImGui.GetItemRectMin();
                var style = ImGui.GetStyle();
                if (showSelector && i == table.SelectedItem)
                {
                    ImGui.Separator();
                }
            }
            ImGui.EndChild();
        }

        public void FillTables(IEntity rightTableEntity, IEntity leftTableEntity)
        {
            rightTable = new TradeScreenTableModel();
            leftTable = new TradeScreenTableModel();

            rightTable.Fill(rightTableEntity);
            leftTable.Fill(leftTableEntity);

            this.rightTableEntity = rightTableEntity;
            this.leftTableEntity = leftTableEntity;
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
            }
        }

        internal void MoveCursorDown()
        {
            if (currentTable != null && currentTable.SelectedItem < currentTable.Items.Count)
            {
                currentTable.SelectedItem++;
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
            if (currentTable != null && currentTable.SelectedItem > 0)
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

        public List<TradeScreenItem> Items { get; set; } = new List<TradeScreenItem>();
        public List<string> ItemsNames = new List<string>();
        public List<string> ItemsDescriptions = new List<string>();
        public List<int> ItemsPrices = new List<int>();
        public int Cash { get; set; }
        public int SelectedItem;

        public void Fill(IEntity itemsHolderEntity)
        {
            Items.Clear();
            ItemsNames.Clear();
            ItemsDescriptions.Clear();
            ItemsPrices.Clear();
            var characterStats = itemsHolderEntity.GetComponentOfType<CharacterStats>();
            var itemsHolder = itemsHolderEntity.GetComponentOfType<ItemsHolder>();

            foreach (var item in itemsHolder.Items)
            {
                Items.Add(new TradeScreenItem { entityReference = item, selectedForTrade = false });
                var description = item.GetComponentOfType<Description>();
                var itemComponent = item.GetComponentOfType<Item>();
                ItemsNames.Add(description.Name);
                ItemsDescriptions.Add(description.Text);
                ItemsPrices.Add(itemComponent.Price);
            }

            Cash = 0;
            if (characterStats != null)
            {
                Cash = characterStats.Money;
            }
        }
    }
}

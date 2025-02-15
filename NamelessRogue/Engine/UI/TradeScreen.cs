using ImGuiNET;
using NamelessRogue.Engine.Abstraction;
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
    public class TradeScreen : BaseScreen
    {
        const int backgroundSelectorHeight = 30;
        TradeScreenTableModel rightTable = null;
        TradeScreenTableModel leftTable = null;
        public TradeScreen(NamelessGame game) : base(game)
        {
        }


        public override void DrawLayout()
        {
            if (rightTable != null && leftTable != null)
            {
                ImGui.Begin("", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoMove);
                {
                    DrawTable(rightTable, new Vector2(20,20), new Vector2(uiSize.X/2 - 20, uiSize.Y -100));
                    DrawTable(leftTable, new Vector2(uiSize.X / 2 + 20, 20), new Vector2(uiSize.X / 2 - 40, uiSize.Y - 100));

                }
                ImGui.End();
            }
        }

        private void DrawTable(TradeScreenTableModel table, Vector2 position, Vector2 size)
        {
            var tableId = table.GetHashCode().ToString();
            ImGui.SetCursorPos(position);
            ImGui.BeginChild(tableId, size, true, ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoScrollbar);
            for (int i = 0; i < table.Items.Count; i++)
            {
                TradeScreenItem tableItem = table.Items[i];
                ImGui.PushItemWidth(size.X);
                var textSize = ImGui.CalcTextSize(table.ItemsNames[i]);
                if (tableItem.selectedForTrade)
                {
                    ImGui.Text("> " +table.ItemsNames[i]);
                }
                else
                {
                    ImGui.Text(table.ItemsNames[i]);
                }
                var itemSize = ImGui.GetItemRectSize();
                var itemPos = ImGui.GetItemRectMin();
                var style = ImGui.GetStyle();
                if (i == table.SelectedItem)
                {
                    ImGui.Separator();
                    var lineHeight = textSize.Y + (style.FramePadding.Y);
                    var shiftY = lineHeight * table.SelectedItem;
                 //   ImGui.GetBackgroundDrawList().AddRectFilled(position + new Vector2(0, shiftY), position + new Vector2(size.X, shiftY + lineHeight), ColorToUInt(System.Drawing.Color.FromArgb(255, 255, 255, 255)));
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
        }

        public void ReleaseTables()
        {
            rightTable = null;
            leftTable = null;
        }

        internal void MoveCursorUp()
        {
            if (leftTable != null && leftTable.SelectedItem>0)
            {
                leftTable.SelectedItem--;
            }
        }

        internal void MoveCursorDown()
        {
            if (leftTable != null && leftTable.SelectedItem < leftTable.Items.Count)
            {
                leftTable.SelectedItem++;
            }
        }

        internal void SelectDeselectCurrentItem()
        {
            leftTable.Items[leftTable.SelectedItem].selectedForTrade = !leftTable.Items[leftTable.SelectedItem].selectedForTrade;
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
        public int Cash { get; set; }
        public int SelectedItem;

        public void Fill(IEntity itemsHolderEntity)
        {
            Items.Clear();
            ItemsNames.Clear();
            ItemsDescriptions.Clear();

            var characterStats = itemsHolderEntity.GetComponentOfType<CharacterStats>();
            var itemsHolder = itemsHolderEntity.GetComponentOfType<ItemsHolder>();

            foreach (var item in itemsHolder.Items)
            {
                Items.Add(new TradeScreenItem { entityReference = item, selectedForTrade = false });
                var description = item.GetComponentOfType<Description>();
                ItemsNames.Add(description.Name);
                ItemsDescriptions.Add(description.Text);
            }

            Cash = 0;
            if (characterStats != null)
            {
                Cash = characterStats.Money;
            }
        }
    }
}

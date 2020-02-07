using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.Interaction;
using NamelessRogue.Engine.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Engine.Components.UI;
using NamelessRogue.Engine.Engine.UiScreens.UI;
using NamelessRogue.Engine.Engine.Utility;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.UiScreens
{
    public enum InventoryScreenAction
    {
        ReturnToGame
    }
    public class InventoryScreen : BaseGuiScreen
    {
        private NamelessGame game;

        public ImageTextButton ReturnToGame { get; set; }
        public Table EquipmentBox { get; set; }
        public Table ItemBox { get; set; }
        public List<InventoryScreenAction> Actions { get; set; } = new List<InventoryScreenAction>();

        public InventoryScreen(NamelessGame game)
        {
            this.game = game;

            Panel = new Panel()
            {
                Width = game.GetActualWidth(),
                Height = game.GetActualCharacterHeight(),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top
            };
            ReturnToGame = new ImageTextButton()
            {
                GridRow = 1,
                GridColumn = 1,
                ContentHorizontalAlignment = HorizontalAlignment.Center,
                ContentVerticalAlignment = VerticalAlignment.Center,
                Text = "Back",
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Right,
                Width = 200,
                Height = 50
            };
            ReturnToGame.Click += OnClickReturnToGame;

            var grid = new Grid() { VerticalAlignment = VerticalAlignment.Stretch, ColumnSpacing = 3, RowSpacing = 2 };
            grid.RowsProportions.Add(new Proportion(ProportionType.Fill));
            grid.RowsProportions.Add(new Proportion(ProportionType.Pixels, 50));

            EquipmentBox = new Table() { GridColumn = 0, GridRow = 0, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
            ItemBox = new Table() { GridColumn = 1, GridRow = 0, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };


            FillItems(game);


            grid.Widgets.Add(EquipmentBox);
            grid.Widgets.Add(ItemBox);
            grid.Widgets.Add(ReturnToGame);
            Panel.Widgets.Add(grid);

            Desktop.Widgets.Add(Panel);

        }

        public void FillItems(NamelessGame game)
        {
            EquipmentBox.Items.Clear();
            ItemBox.Items.Clear();

            var playerEntity = game.GetEntityByComponentClass<Player>();

            var itemsHolder = playerEntity.GetComponentOfType<ItemsHolder>();
            var equipment = playerEntity.GetComponentOfType<EquipmentSlots>();

            var headerItem = new TableItem(4);
            headerItem.Cells[0].Widgets.Add(new Label() { Text = "Hotkey", });
            headerItem.Cells[1].Widgets.Add(new Label() { Text = "Name", });
            headerItem.Cells[2].Widgets.Add(new Label() { Text = "Weight", });
            headerItem.Cells[3].Widgets.Add(new Label() { Text = "Type", });
            ItemBox.Items.Add(headerItem);

            List<IEntity> list = itemsHolder.GetItems();
            char hotkey = Char.MinValue;
            for (int i = 0; i < list.Count; i++)
            {
                IEntity entity = list[i];
                
                if (i == 0)
                {
                    hotkey = HotkeyHelper.alphabet.First();
                }
                else
                {
                    hotkey = HotkeyHelper.GetNextKey(hotkey);
                }

                Description desc = entity.GetComponentOfType<Description>();
                Item item = entity.GetComponentOfType<Item>();

                var tableItem = new TableItem(4);
                tableItem.Tag = entity;
                tableItem.Hotkey = hotkey;
                tableItem.Cells[0].Widgets.Add(new Label() { Text = hotkey.ToString(), HorizontalAlignment = HorizontalAlignment.Center});
                tableItem.Cells[1].Widgets.Add(new Label() { Text = desc.Name, });
                tableItem.Cells[2].Widgets.Add(new Label() { Text = item.Weight.ToString(), });
                tableItem.Cells[3].Widgets.Add(new Label() { Text = item.Type.ToString(), });
                ItemBox.Items.Add(tableItem);
            }


            var headerEquipmentItem = new TableItem(3);
            headerEquipmentItem.Cells[0].Widgets.Add(new Label() { Text = "Slot", });
            headerEquipmentItem.Cells[1].Widgets.Add(new Label() { Text = "Name", });
            EquipmentBox.Items.Add(headerEquipmentItem);

            foreach (var equipmentSlot in equipment.Slots)
            {
                Description desc = equipmentSlot.Value.Equipment?.Parent.GetComponentOfType<Description>();
                var text = desc != null ? desc.Name : "Nothing";

                var tableItem = new TableItem(3);
                tableItem.Cells[0].Widgets.Add(new Label() { Text = equipmentSlot.Key.ToString(), });
                tableItem.Cells[1].Widgets.Add(new Label() { Text = text });
                EquipmentBox.Items.Add(tableItem);
            }

            if (ItemBox.Items.Any())
            {
                ItemBox.SelectedIndex = 0;
            }
        }

        private void OnClickReturnToGame(object sender, EventArgs e)
        {
            Actions.Add(InventoryScreenAction.ReturnToGame);
        }
    }
}

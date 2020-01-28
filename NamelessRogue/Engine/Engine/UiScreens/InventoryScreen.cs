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
        public ScrollableListBox EquipmentBox { get; set; }
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

            EquipmentBox = new ScrollableListBox() { GridColumn = 0, GridRow = 0, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
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

            var headerItem = new TableItem(3);
            headerItem.Cells[0].Widgets.Add(new Label() { Text = "Name", });
            headerItem.Cells[1].Widgets.Add(new Label() { Text = "Weight", });
            headerItem.Cells[2].Widgets.Add(new Label() { Text = "Type", });
            ItemBox.Items.Add(headerItem);

            foreach (var entity in itemsHolder.GetItems())
            {
                Description desc = entity.GetComponentOfType<Description>();
                var tableItem = new TableItem(3);
                tableItem.Cells[0].Widgets.Add(new Label() { Text = desc.Name, });
                tableItem.Cells[1].Widgets.Add(new Label() { Text = 1.ToString(), });
                //TODO
                tableItem.Cells[2].Widgets.Add(new Label() { Text = "Weapon", });
                ItemBox.Items.Add(tableItem);
            }

            foreach (var equipmentSlot in equipment.Slots)
            {
                Description desc = equipmentSlot.Value.Equipment?.Parent.GetComponentOfType<Description>();
                var text = desc != null ? desc.Name : "Nothing";
                EquipmentBox.Items.Add(new ListItem($"{equipmentSlot.Key.ToString()}: {text}", Color.White, equipmentSlot));
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

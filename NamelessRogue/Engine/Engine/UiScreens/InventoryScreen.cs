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
using NamelessRogue.Engine.Engine.Components.Physical;
using NamelessRogue.Engine.Engine.Components.UI;
using NamelessRogue.Engine.Engine.Factories;
using NamelessRogue.Engine.Engine.UiScreens.UI;
using NamelessRogue.Engine.Engine.Utility;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.UiScreens
{
    public enum InventoryScreenAction
    {
        ReturnToGame
    }

    public enum ItemDialogActions
    {
        Drop,
        Equip,
        Cancel

    }

    public class InventoryScreen : BaseGuiScreen
    {
        private NamelessGame game;

        public ImageTextButton ReturnToGame { get; set; }
        public Table EquipmentBox { get; set; }
        public Table ItemBox { get; set; }
        public List<InventoryScreenAction> Actions { get; set; } = new List<InventoryScreenAction>();
        private ChoiceDialog itemChoiceDialog;

        public ChoiceDialog ItemChoiceDialog
        {
            get => itemChoiceDialog;
            set => itemChoiceDialog = value;
        }

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

            var grid = new Grid() {VerticalAlignment = VerticalAlignment.Stretch, ColumnSpacing = 3, RowSpacing = 2};
            grid.RowsProportions.Add(new Proportion(ProportionType.Fill));
            grid.RowsProportions.Add(new Proportion(ProportionType.Pixels, 50));

            EquipmentBox = new Table()
            {
                GridColumn = 0, GridRow = 0, HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            ItemBox = new Table()
            {
                GridColumn = 1, GridRow = 0, HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };


            FillItems(game);


            grid.Widgets.Add(EquipmentBox);
            grid.Widgets.Add(ItemBox);
            grid.Widgets.Add(ReturnToGame);
            Panel.Widgets.Add(grid);

            Desktop.Widgets.Add(Panel);

            SelectedTable = ItemBox;

            itemChoiceDialog = new ChoiceDialog(
                new ChoiceOption() {Id = ItemDialogActions.Equip, Text = "Equip"},
                new ChoiceOption() {Id = ItemDialogActions.Drop, Text = "Drop"},
                new ChoiceOption() {Id = ItemDialogActions.Cancel, Text = "Cancel"}
            );

            ItemBox.OnItemClick += () =>
            {
   
                int selectedIndex = ItemBox.SelectedIndex.Value;
                if (selectedIndex > 0)
                {
                    selectedItem = ItemBox.SelectedItem;
                    OpenItemBoxDialog();
                }
            };

            itemChoiceDialog.OptionsTable.OnItemClick += () =>
            {

                var playerEntity = game.GetEntitiesByComponentClass<Player>().First();
                var equipmentEntity = (IEntity)selectedItem.Tag;
                var itemsHolder = playerEntity.GetComponentOfType<ItemsHolder>();
                var equipment = playerEntity.GetComponentOfType<EquipmentSlots>();

                var equipmentItem = equipmentEntity.GetComponentOfType<Equipment>();
        


                var chosenItem = (ChoiceOption)itemChoiceDialog.OptionsTable.SelectedItem.Tag;
                ItemDialogActions itemDialogActions = (ItemDialogActions)chosenItem.Id;
                switch (itemDialogActions)
                {
                    case ItemDialogActions.Drop:
                        var position = playerEntity.GetComponentOfType<Position>();
                        var command = new DropItemCommand(new List<IEntity>(){ equipmentItem.Parent },itemsHolder, position.p);
                        playerEntity.AddComponent(command);
                        ItemBox.Items.Remove(selectedItem);
                        break;  
                    case ItemDialogActions.Equip:
                        equipment.Equip(equipmentItem);
                        ItemBox.Items.Remove(selectedItem);
                        break;
                    case ItemDialogActions.Cancel:
                        break;
                    default:
                        break;
                }
                CloseItemBoxDialog();
             
            };
        }

        private TableItem selectedItem;


        public void SelectTable(Table table)
        {
            SelectedTable.SelectedIndex = null;
            SelectedTable = table;
            SelectedTable.SelectedIndex = 0;
        }

        public void SwitchSelectedTable()
        {
            if (SelectedTable == ItemBox)
            {
                SelectTable(EquipmentBox);
            }
            else if (SelectedTable == EquipmentBox)
            {
                SelectTable(ItemBox);
            }
        }

        public bool ItemBoxDialogOpened { get; private set; }
        public Table SelectedTable { get => selectedTable; set => selectedTable = value; }

        public void OpenItemBoxDialog()
        {
            ItemBoxDialogOpened = true;
            itemChoiceDialog.OptionsTable.SelectedIndex = 0;
            itemChoiceDialog.ShowModal(new Point(game.GetActualWidth() / 2, game.GetActualHeight() / 2));
            SelectTable(itemChoiceDialog.OptionsTable);
        }

        public void CloseItemBoxDialog()
        {
            SelectTable(ItemBox);
            itemChoiceDialog.Close();
        }

        private Table selectedTable;
        public void ScrollSelectedTableDown()
        {
            var prevIndex = SelectedTable.SelectedIndex.Value;
            SelectedTable.OnKeyDown(Keys.Down); /* += 1;*/

            int nextIndex = SelectedTable.SelectedIndex.Value;

            bool move = false;
            if (SelectedTable.SelectedIndex == prevIndex)
            {
                nextIndex = 0;
                move = true;
            }

            if (SelectedTable.Items.Any())
            {
                SelectedTable.SelectedIndex = nextIndex;
                if (move)
                {
                    SelectedTable.OnKeyDown(Keys.Down);
                    SelectedTable.OnKeyDown(Keys.Up);
                }
            }
        }

        public void ScrollSelectedTableUp()
        {
            var prevIndex = SelectedTable.SelectedIndex.Value;
            SelectedTable.OnKeyDown(Keys.Up); /* -= 1;*/
            int nextIndex = SelectedTable.SelectedIndex.Value;
            bool move = false;
            if (SelectedTable.SelectedIndex == prevIndex)
            {
                nextIndex = SelectedTable.Items.Count - 1;
                move = true;
            }

            if (SelectedTable.Items.Any())
            {
                SelectedTable.SelectedIndex = nextIndex;
                if (move)
                {
                    SelectedTable.OnKeyDown(Keys.Up);
                    SelectedTable.OnKeyDown(Keys.Down);

                }
            }

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

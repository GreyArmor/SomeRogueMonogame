using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
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

    public enum EquipmentDialogActions
    {
        Drop,
        Unequip,
        Cancel
    }


    public class InventoryScreen : TableScreen {
        private NamelessGame game;

        public ImageTextButton ReturnToGame { get; set; }
        public Table EquipmentBox { get; set; }
        public Table ItemBox { get; set; }
        public List<InventoryScreenAction> Actions { get; set; } = new List<InventoryScreenAction>();

        public ChoiceDialog ItemChoiceDialog { get; set; }

        public ChoiceDialog EquipmentChoiceDialog { get; set; }

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

            ItemChoiceDialog = new ChoiceDialog(
                new ChoiceOption() { Id = ItemDialogActions.Equip, Text = "Equip" },
                new ChoiceOption() { Id = ItemDialogActions.Drop, Text = "Drop" },
                new ChoiceOption() { Id = ItemDialogActions.Cancel, Text = "Cancel" }
            );

            EquipmentChoiceDialog = new ChoiceDialog(
                new ChoiceOption() { Id = EquipmentDialogActions.Unequip, Text = "Unequip" },
                new ChoiceOption() { Id = EquipmentDialogActions.Drop, Text = "Drop" },
                new ChoiceOption() { Id = EquipmentDialogActions.Cancel, Text = "Cancel" }
            );

            ItemBox.OnItemClick += (TableItem selectedItem) =>
            {
   
                int selectedIndex = ItemBox.Items.IndexOf(selectedItem);
                if (selectedIndex > 0)
                {
                    this.selectedItem = selectedItem;

                    var equipmentEntity = (IEntity)selectedItem.Tag;
                    var equipmentItem = equipmentEntity.GetComponentOfType<Equipment>();
                    FillEquipmentChoiceDialog(equipmentItem);
                    OpenDialog(ItemChoiceDialog);
                }
            };

            EquipmentBox.OnItemClick += (TableItem selectedItem) =>
            {

                int selectedIndex = EquipmentBox.Items.IndexOf(selectedItem);
                if (selectedIndex > 0)
                {
                    this.selectedItem = selectedItem;
                    OpenDialog(EquipmentChoiceDialog);
                }
            };

            EquipmentChoiceDialog.OptionsTable.OnItemClick += (TableItem selectedItemOptions) =>
            {

                if (selectedItem.Tag == null)
                {
                    CloseDialog(EquipmentChoiceDialog);
                    return;
                }

                var playerEntity = game.GetEntitiesByComponentClass<Player>().First();
                var slot = (EquipmentSlots.Slot)selectedItem.Tag;
                var itemsHolder = playerEntity.GetComponentOfType<ItemsHolder>();
                var equipment = playerEntity.GetComponentOfType<EquipmentSlots>();

                var equipmentItem = equipment.Slots.First(x=>x.Item1==slot).Item2.Equipment;

                if (equipmentItem == null)
                {
                    CloseDialog(EquipmentChoiceDialog);
                    return;
                }
        
                var chosenItem = (ChoiceOption)selectedItemOptions.Tag;
                var  dialogActions = (EquipmentDialogActions)chosenItem.Id;
                switch (dialogActions)
                {
                    case EquipmentDialogActions.Drop:
                        equipment.TakeOff(slot);
                        var position = playerEntity.GetComponentOfType<Position>();
                        var command = new DropItemCommand(new List<IEntity>(){ equipmentItem.Parent },itemsHolder, position.p);
                        playerEntity.AddComponent(command);
                        break;  
                    case EquipmentDialogActions.Unequip:
                        equipment.TakeOff(slot);
                        break;
                    case EquipmentDialogActions.Cancel:
                        break;
                    default:
                        break;
                }
                CloseDialog(EquipmentChoiceDialog);
             
            };

            ItemChoiceDialog.Closed += (sender, args) => { SelectTable(ItemBox); dialogOpened = false; };

            ItemChoiceDialog.OptionsTable.OnItemClick += (TableItem selectedItemOptions) =>
            {

                var playerEntity = game.GetEntitiesByComponentClass<Player>().First();
                var equipmentEntity = (IEntity)selectedItem.Tag;
                var itemsHolder = playerEntity.GetComponentOfType<ItemsHolder>();
                var equipment = playerEntity.GetComponentOfType<EquipmentSlots>();

                var equipmentItem = equipmentEntity.GetComponentOfType<Equipment>();

             

                var chosenItem = (ChoiceOption)selectedItemOptions.Tag;
                var slotEquipTo = (EquipmentSlots.Slot) chosenItem.Data;
             ItemDialogActions itemDialogActions = (ItemDialogActions)chosenItem.Id;
                switch (itemDialogActions)
                {
                    case ItemDialogActions.Drop:
                        var position = playerEntity.GetComponentOfType<Position>();
                        var command = new DropItemCommand(new List<IEntity>() { equipmentItem.Parent }, itemsHolder, position.p);
                        playerEntity.AddComponent(command);
                        break;
                    case ItemDialogActions.Equip:
                        equipment.Equip(equipmentItem, slotEquipTo);
                        break;
                    case ItemDialogActions.Cancel:
                        break;
                    default:
                        break;
                }
                CloseDialog(ItemChoiceDialog);

            };

            EquipmentChoiceDialog.Closed += (sender, args) => { SelectTable(EquipmentBox);
                dialogOpened = false;
            };

            OnCloseDialog += () => { FillItems(this.game); };
        }

       


       

        public void SwitchSelectedTable()
        {
            if (SelectedTable == ItemBox)
            {
                SelectedTable.SelectedIndex = null;
                SelectTable(EquipmentBox);
                SelectedTable.SelectedIndex = 0;
            }
            else if (SelectedTable == EquipmentBox)
            {
                SelectedTable.SelectedIndex = null;
                SelectTable(ItemBox);
                SelectedTable.SelectedIndex = 0;
            }
        }

    

     
        public void FillItems(NamelessGame game)
        {


            var selectedIndex = SelectedTable?.Items.IndexOf(selectedItem);


            EquipmentBox.Items.Clear();
            ItemBox.Items.Clear();

            var playerEntity = game.GetEntityByComponentClass<Player>();

            var itemsHolder = playerEntity.GetComponentOfType<ItemsHolder>();
            var equipment = playerEntity.GetComponentOfType<EquipmentSlots>();

            char hotkey = Char.MinValue;

            var headerEquipmentItem = new TableItem(3);
            headerEquipmentItem.Cells[0].Widgets.Add(new Label() { Text = "Hotkey", HorizontalAlignment  = HorizontalAlignment.Center});
            headerEquipmentItem.Cells[1].Widgets.Add(new Label() { Text = "Slot", });
            headerEquipmentItem.Cells[2].Widgets.Add(new Label() { Text = "Name", });
            EquipmentBox.Items.Add(headerEquipmentItem);
            {
                int i = 0;
                foreach (var equipmentSlot in equipment.Slots.Except(equipment.Slots.Where(x=>x.Item1==EquipmentSlots.Slot.BothHands)))
                {

                    if (i == 0)
                    {
                        hotkey = HotkeyHelper.alphabet.First();
                    }
                    else
                    {
                        hotkey = HotkeyHelper.GetNextKey(hotkey);
                    }


                    Description desc = equipmentSlot.Item2.Equipment?.Parent.GetComponentOfType<Description>();
                    var text = desc != null ? desc.Name : "Nothing";
                    var tableItem = new TableItem(3);
                    tableItem.Hotkey = hotkey;
                    tableItem.Tag = equipmentSlot.Item1;
                    tableItem.Cells[0].Widgets.Add(new Label() { Text = hotkey.ToString(), HorizontalAlignment = HorizontalAlignment.Center });
                    tableItem.Cells[1].Widgets.Add(new Label() {Text = equipmentSlot.Item1.ToString(),});
                    tableItem.Cells[2].Widgets.Add(new Label() {Text = text});
                    EquipmentBox.Items.Add(tableItem);
                    i++;
                }
            }

            var headerItem = new TableItem(4);
            headerItem.Cells[0].Widgets.Add(new Label() { Text = "Hotkey", HorizontalAlignment = HorizontalAlignment.Center });
            headerItem.Cells[1].Widgets.Add(new Label() { Text = "Name", });
            headerItem.Cells[2].Widgets.Add(new Label() { Text = "Weight", });
            headerItem.Cells[3].Widgets.Add(new Label() { Text = "Type", });
            ItemBox.Items.Add(headerItem);

            List<IEntity> list = itemsHolder.GetItems();

            for (int i = 0; i < list.Count; i++)
            {
                IEntity entity = list[i];

                hotkey = HotkeyHelper.GetNextKey(hotkey);

                Description desc = entity.GetComponentOfType<Description>();
                Item item = entity.GetComponentOfType<Item>();

                var tableItem = new TableItem(4);
                tableItem.Tag = entity;
                tableItem.Hotkey = hotkey;
                tableItem.Cells[0].Widgets.Add(new Label()
                    {Text = hotkey.ToString(), HorizontalAlignment = HorizontalAlignment.Center});
                tableItem.Cells[1].Widgets.Add(new Label() {Text = desc.Name,});
                tableItem.Cells[2].Widgets.Add(new Label() {Text = item.Weight.ToString(),});
                tableItem.Cells[3].Widgets.Add(new Label() {Text = item.Type.ToString(),});
                ItemBox.Items.Add(tableItem);
            }


            if (selectedIndex > SelectedTable?.Items.Count)
            {
                if (SelectedTable != null)
                {
                    SelectedTable.SelectedIndex = 0;
                    selectedItem = SelectedTable.SelectedItem;
                }
            }
            else
            {
                if (SelectedTable != null)
                {
                    SelectedTable.SelectedIndex = selectedIndex;
                    selectedItem = SelectedTable.SelectedItem;
                }
            }

        }

        public void FillEquipmentChoiceDialog(Equipment equipment)
        {

            List<ChoiceOption> choices = new List<ChoiceOption>();
            if (equipment.PossibleSlots.Count > 1)
            {
                foreach (var equipmentPossibleSlot in equipment.PossibleSlots)
                {
                    choices.Add(new ChoiceOption()
                    {
                        Data = equipmentPossibleSlot,
                        Id = ItemDialogActions.Equip,
                        Text = $"Equip into slot {equipmentPossibleSlot}"
                    });
                }
            }
            else
            {
                choices.Add(new ChoiceOption() {Id = ItemDialogActions.Equip, Text = "Equip", Data = equipment.PossibleSlots.First()});
            }
            choices.Add(new ChoiceOption() { Id = ItemDialogActions.Drop, Text = "Drop" });
            choices.Add(new ChoiceOption() { Id = ItemDialogActions.Cancel, Text = "Cancel" });
            ItemChoiceDialog.FillChoiceOptions(choices.ToArray());
        }

        private void OnClickReturnToGame(object sender, EventArgs e)
        {
            Actions.Add(InventoryScreenAction.ReturnToGame);
        }
    }
}

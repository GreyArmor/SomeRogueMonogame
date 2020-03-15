using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
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
using Color = Microsoft.Xna.Framework.Color;

namespace NamelessRogue.Engine.Engine.UiScreens
{
    public enum InventoryScreenAction
    {
        ReturnToGame
    }

    public enum ItemDialogActions
    {
        DropAmount,
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

        public AmountDialog AmountDialog;

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
                VerticalAlignment = VerticalAlignment.Stretch,
                BorderThickness = new Thickness(1),
                Border = new SolidBrush(Color.White)
            };
            ItemBox = new Table()
            {
                GridColumn = 1, GridRow = 0, HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                BorderThickness = new Thickness(1),
                Border = new SolidBrush(Color.White)
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
            ItemChoiceDialog.Title = "Item commands";


            EquipmentChoiceDialog = new ChoiceDialog(
                new ChoiceOption() { Id = EquipmentDialogActions.Unequip, Text = "Unequip" },
                new ChoiceOption() { Id = EquipmentDialogActions.Drop, Text = "Drop" },
                new ChoiceOption() { Id = EquipmentDialogActions.Cancel, Text = "Cancel" }
            );
            EquipmentChoiceDialog.Title = "Equipment commands";

            ItemBox.OnItemClick += (TableItem selectedItem) =>
            {
   
                int selectedIndex = ItemBox.Items.IndexOf(selectedItem);
                if (selectedIndex > 0)
                {
                    this.SelectedItem = selectedItem;

                    var itemEntity = (IEntity)selectedItem.Tag;
                    FillItemChoiceDialog(itemEntity);
                    OpenDialog(ItemChoiceDialog);
                }
            };

            EquipmentBox.OnItemClick += (TableItem selectedItem) =>
            {

                int selectedIndex = EquipmentBox.Items.IndexOf(selectedItem);
                if (selectedIndex > 0)
                {
                    this.SelectedItem = selectedItem;
                    OpenDialog(EquipmentChoiceDialog);
                }
            };

            EquipmentChoiceDialog.OptionsTable.OnItemClick += (TableItem selectedItemOptions) =>
            {

                if (SelectedItem.Tag == null)
                {
                    CloseDialog(EquipmentChoiceDialog);
                    return;
                }

                var playerEntity = game.GetEntitiesByComponentClass<Player>().First();
                var slot = (EquipmentSlots.Slot)SelectedItem.Tag;
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
                        equipment.TakeOff(equipmentItem);
                        var position = playerEntity.GetComponentOfType<Position>();
                        var command = new DropItemCommand(new List<IEntity>(){ game.GetEntity(equipment.ParentEntityId) },itemsHolder, position.p);
                        playerEntity.AddComponent(command);
                        break;  
                    case EquipmentDialogActions.Unequip:
                        equipment.TakeOff(equipmentItem);
                        break;
                    case EquipmentDialogActions.Cancel:
                        break;
                    default:
                        break;
                }
                CloseDialog(EquipmentChoiceDialog);
                playerEntity.AddComponentDelayed(new UpdateInventoryCommand());
             
            };

            ItemChoiceDialog.Closed += (sender, args) => { SelectTable(ItemBox); dialogOpened = false; };

            ItemChoiceDialog.OptionsTable.OnItemClick += (TableItem selectedItemOptions) =>
            {

                var playerEntity = game.GetEntitiesByComponentClass<Player>().First();
                var itemEntity = (IEntity)SelectedItem.Tag;
                var itemsHolder = playerEntity.GetComponentOfType<ItemsHolder>();
                var equipment = playerEntity.GetComponentOfType<EquipmentSlots>();

                var equipmentItem = itemEntity.GetComponentOfType<Equipment>();

             

                var chosenItem = (ChoiceOption)selectedItemOptions.Tag;
 
                
             ItemDialogActions itemDialogActions = (ItemDialogActions)chosenItem.Id;
                switch (itemDialogActions)
                {
                    case ItemDialogActions.DropAmount:
                    {
                        AmountDialog = new AmountDialog();
                        AmountDialog.ShowModal();
                        AmountDialog.Amount.OnTouchDown();
                        AmountDialog.Closed += (sender, args) =>
                        {
                            if (AmountDialog.Result)
                            {
                                var position = playerEntity.GetComponentOfType<Position>();
                                var amount = AmountDialog.Amount.Text == null ? 0 : int.Parse(AmountDialog.Amount.Text);
                                var itemComponent = itemEntity.GetComponentOfType<Item>();
                                if (amount >= itemComponent.Amount)
                                {
                                    var command = new DropItemCommand(new List<IEntity>() { itemEntity }, itemsHolder,
                                        position.p);
                                    playerEntity.AddComponent(command);
                                    CloseDialog(ItemChoiceDialog);
                                    playerEntity.AddComponent(new UpdateInventoryCommand());
                                }
                                else if (amount < 1)
                                {}
                                else
                                {
                                    var clonedEntity = itemEntity.CloneEntity();

                                    game.EntitiesToAdd.Add(clonedEntity);

                                    var clonedItemComponent = clonedEntity.GetComponentOfType<Item>();

                                    itemComponent.Amount -= amount;
                                    clonedItemComponent.Amount = amount;


                                    var command = new DropItemCommand(new List<IEntity>() { clonedEntity }, itemsHolder,
                                        position.p);
                                    playerEntity.AddComponent(command);
                                    CloseDialog(ItemChoiceDialog);
                                    playerEntity.AddComponent(new UpdateInventoryCommand());
                                }

                                AmountDialog = null;

                            }
                        };
                    
                    }
                        break;
                    case ItemDialogActions.Drop:
                    {
                        var position = playerEntity.GetComponentOfType<Position>();
                        var command = new DropItemCommand(new List<IEntity>() {itemEntity}, itemsHolder, position.p);
                        playerEntity.AddComponent(command);
                        CloseDialog(ItemChoiceDialog);
                        playerEntity.AddComponentDelayed(new UpdateInventoryCommand());
                    }
                        break;
                    case ItemDialogActions.Equip:
                        List<EquipmentSlots.Slot> slotsEquipTo;
                        slotsEquipTo = (List<EquipmentSlots.Slot>) chosenItem.Data;
                        equipment.Equip(equipmentItem, slotsEquipTo);
                        CloseDialog(ItemChoiceDialog);
                        playerEntity.AddComponentDelayed(new UpdateInventoryCommand());
                        break;
                    case ItemDialogActions.Cancel:
                        CloseDialog(ItemChoiceDialog);
                        break;
                    default:
                        break;
                }


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


            var selectedIndex = SelectedTable?.Items.IndexOf(SelectedItem);


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
                foreach (var equipmentSlot in equipment.Slots)
                {

                    if (i == 0)
                    {
                        hotkey = HotkeyHelper.alphabet.First();
                    }
                    else
                    {
                        hotkey = HotkeyHelper.GetNextKey(hotkey);
                    }

                    var eq = equipmentSlot.Item2.Equipment;
                    Description desc = null;
                    if (eq != null)
                    {
                        var itemEntity = game.GetEntity(eq.ParentEntityId);
                        desc = itemEntity.GetComponentOfType<Description>();
                    }
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

            var headerItem = new TableItem(6);
            headerItem.Cells[0].Widgets.Add(new Label() { Text = "Hotkey", HorizontalAlignment = HorizontalAlignment.Center });
            headerItem.Cells[1].Widgets.Add(new Label() { Text = "Name", });
            headerItem.Cells[2].Widgets.Add(new Label() { Text = "Quality", });
            headerItem.Cells[3].Widgets.Add(new Label() { Text = "Weight", });
            headerItem.Cells[4].Widgets.Add(new Label() { Text = "Type", });
            headerItem.Cells[5].Widgets.Add(new Label() { Text = "Amount", });
            ItemBox.Items.Add(headerItem);

            List<IEntity> list = itemsHolder.GetItems();

            for (int i = 0; i < list.Count; i++)
            {
                IEntity entity = list[i];

                hotkey = HotkeyHelper.GetNextKey(hotkey);

                Description desc = entity.GetComponentOfType<Description>();
                Item item = entity.GetComponentOfType<Item>();

                var tableItem = new TableItem(6);
                tableItem.Tag = entity;
                tableItem.Hotkey = hotkey;
                tableItem.Cells[0].Widgets.Add(new Label()
                    {Text = hotkey.ToString(), HorizontalAlignment = HorizontalAlignment.Center});
                tableItem.Cells[1].Widgets.Add(new Label() {Text = desc.Name,});
                tableItem.Cells[2].Widgets.Add(new Label() { Text = item.Quality.ToString(), });
                tableItem.Cells[3].Widgets.Add(new Label() {Text = (item.Weight * item.Amount).ToString(),});
                tableItem.Cells[4].Widgets.Add(new Label() {Text = item.Type.ToString(),});
                tableItem.Cells[5].Widgets.Add(new Label() { Text = item.Amount.ToString(), });
                ItemBox.Items.Add(tableItem);
            }


            if (selectedIndex >= SelectedTable?.Items.Count)
            {
                if (SelectedTable != null)
                {
                    SelectedTable.SelectedIndex = 0;
                    SelectedItem = SelectedTable.SelectedItem;
                }
            }
            else
            {
                if (SelectedTable != null)
                {
                    SelectedTable.SelectedIndex = selectedIndex;
                    SelectedItem = SelectedTable.SelectedItem;
                }
            }

        }

        public void FillItemChoiceDialog(IEntity itemEntity)
        {
            List<ChoiceOption> choices = new List<ChoiceOption>();

            var equipment = itemEntity.GetComponentOfType<Equipment>();
            var ammo = itemEntity.GetComponentOfType<Ammo>();
            var item = itemEntity.GetComponentOfType<Item>();
            if (equipment != null)
            {
                if (equipment.PossibleSlots.Count > 1)
                {
                    foreach (var equipmentPossibleSlot in equipment.PossibleSlots)
                    {
                        string slotsString = "";
                        foreach (var slot in equipmentPossibleSlot)
                        {
                            slotsString += slot.ToString() + " ";
                        }
                        choices.Add(new ChoiceOption()
                        {
                            Data = equipmentPossibleSlot,
                            Id = ItemDialogActions.Equip,
                            Text = $"Equip into {slotsString}"
                        });
                    }
                }
                else
                {
                    choices.Add(new ChoiceOption()
                        {Id = ItemDialogActions.Equip, Text = "Equip", Data = equipment.PossibleSlots.First()});
                }
            }

            if (item != null)
            {
                if (item.Amount > 1)
                {
                    choices.Add(new ChoiceOption() {Id = ItemDialogActions.DropAmount, Text = "Drop amount"});
                }
            }


            choices.Add(new ChoiceOption() { Id = ItemDialogActions.Drop, Text = "Drop" });
            choices.Add(new ChoiceOption() { Id = ItemDialogActions.Cancel, Text = "Cancel" });
            ItemChoiceDialog.
                FillChoiceOptions(choices.ToArray());
        }

        private void OnClickReturnToGame(object sender, EventArgs e)
        {
            Actions.Add(InventoryScreenAction.ReturnToGame);
        }
    }
}

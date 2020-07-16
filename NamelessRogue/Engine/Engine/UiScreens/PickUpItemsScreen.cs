using System;
using System.Collections.Generic;
using System.Linq;
using Myra.Graphics2D.UI;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.Interaction;
using NamelessRogue.Engine.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Engine.Components.Physical;
using NamelessRogue.Engine.Engine.Components.UI;
using NamelessRogue.Engine.Engine.Generation.World;
using NamelessRogue.Engine.Engine.UiScreens.UI;
using NamelessRogue.Engine.Engine.Utility;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.UiScreens
{
    public enum PickUpItemsScreenAction
    {
        ReturnToGame,
    }
    public class PickUpItemsScreen : TableScreen
    {
        private readonly NamelessGame game;
        public ImageTextButton ReturnToGame { get; set; }
        public List<PickUpItemsScreenAction> Actions { get; private set; } = new List<PickUpItemsScreenAction>();
        public Table ItemsTable { get; }

        public PickUpItemsScreen(NamelessGame game)
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
                GridColumn = 0,
                ContentHorizontalAlignment = HorizontalAlignment.Center,
                ContentVerticalAlignment = VerticalAlignment.Center,
                Text = "Back",
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Right,
                Width = 200,
                Height = 50
            };
            ReturnToGame.Click += OnClickReturnToGame;

            var grid = new Grid() { VerticalAlignment = VerticalAlignment.Stretch, RowSpacing = 2 };
            grid.RowsProportions.Add(new Proportion(ProportionType.Fill));
            grid.RowsProportions.Add(new Proportion(ProportionType.Pixels, 50));

            ItemsTable = new Table()
            {
                GridColumn = 0,
                GridRow = 0,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            grid.Widgets.Add(ItemsTable);
            grid.Widgets.Add(ReturnToGame);
            Panel.Widgets.Add(grid);

            Desktop.Widgets.Add(Panel);

            SelectTable(ItemsTable);

            ItemsTable.OnItemClick += (item) =>
            {

                if (item == null)
                {
                    return;
                }

                var playerEntity = game.PlayerEntity;
                var itemsHolder = playerEntity.GetComponentOfType<ItemsHolder>();
                var position = playerEntity.GetComponentOfType<Position>();

                var itemEntity = item.Tag as IEntity;

                if (itemEntity == null)
                {
                    playerEntity.AddComponent(new UpdatePickupDialogCommand());
                    return;
                }

                int selectedIndex = ItemsTable.Items.IndexOf(item);
                if (selectedIndex > 0)
                {
                    this.SelectedItem = item;
                }

                var command = new PickUpItemCommand(new List<IEntity>(){itemEntity},itemsHolder, position.p);
                playerEntity.AddComponent(command);
                if (ItemsTable.Items.Count == 2)
                {
                    Actions.Add(PickUpItemsScreenAction.ReturnToGame);
                }
                playerEntity.AddComponentDelayed(new UpdatePickupDialogCommand());
            };
        }

        public void FillItems(NamelessGame namelessGame)
        {
            var selectedIndex = SelectedTable?.Items.IndexOf(SelectedItem);

            ItemsTable.Items.Clear();

            var playerEntity = game.PlayerEntity;

            var itemsHolder = playerEntity.GetComponentOfType<ItemsHolder>();
            var equipment = playerEntity.GetComponentOfType<EquipmentSlots>();

            char hotkey = Char.MinValue;

            var headerItem = new TableItem(4);
            headerItem.Cells[0].Widgets.Add(new Label() { Text = "Hotkey", HorizontalAlignment = HorizontalAlignment.Center });
            headerItem.Cells[1].Widgets.Add(new Label() { Text = "Name", });
            headerItem.Cells[2].Widgets.Add(new Label() { Text = "Weight", });
            headerItem.Cells[3].Widgets.Add(new Label() { Text = "Type", });
            ItemsTable.Items.Add(headerItem);


            IEntity worldEntity = namelessGame.TimelineEntity;
            IWorldProvider worldProvider = null;
            if (worldEntity != null)
            {
                worldProvider = worldEntity.GetComponentOfType<TimeLine>().CurrentTimelineLayer
                    .Chunks;
            }


            var position = playerEntity.GetComponentOfType<Position>();
            var itemHolder = playerEntity.GetComponentOfType<ItemsHolder>();
            var tile = worldProvider.GetTile(position.p.X, position.p.Y);

            List<IEntity> itemsToPickUp = new List<IEntity>();
            foreach (var entityOnTIle in tile.GetEntities())
            {
                var itemComponent = entityOnTIle.GetComponentOfType<Item>();
                if (itemComponent != null)
                {
                    itemsToPickUp.Add(entityOnTIle);
                }
            }



            List<IEntity> list = itemsToPickUp;

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
                tableItem.Cells[0].Widgets.Add(new Label()
                { Text = hotkey.ToString(), HorizontalAlignment = HorizontalAlignment.Center });
                tableItem.Cells[1].Widgets.Add(new Label() { Text = desc.Name, });
                tableItem.Cells[2].Widgets.Add(new Label() { Text = item.Weight.ToString(), });
                tableItem.Cells[3].Widgets.Add(new Label() { Text = item.Type.ToString(), });
                ItemsTable.Items.Add(tableItem);
            }


            if (selectedIndex > SelectedTable?.Items.Count)
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

        private void OnClickReturnToGame(object sender, EventArgs e)
        {
            Actions.Add(PickUpItemsScreenAction.ReturnToGame);
        }
    }
}

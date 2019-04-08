using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeonBit.UI;
using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
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

    public class ItemPanel : Panel
    {
        public EquipmentSlots.Slot Slot { get; }

        public string SlotText
        {
            get { return _slotText; }
            set
            {
                _slotText = value;
                label.Text = _slotText + _equipmentText;
            }
        }

        public string EquipmentText
        {
            get => _equipmentText;
            set
            {
                _equipmentText = value;
                label.Text = _slotText + _equipmentText;
            }
        }

        Label label;
        private string _slotText = "";
        private string _equipmentText = "";

        public ItemPanel(EquipmentSlots.Slot slot, Vector2 size, Vector2 position, string text):base(size,PanelSkin.Simple,Anchor.TopLeft, position)
        {
            label = new Label(text, anchor: Anchor.AutoInline);
            Slot = slot;
            SlotText = text;
            this.AddChild(label);
        }
    }

    public class InventoryScreen : BaseGuiScreen
    {
        private readonly NamelessGame game;

        public List<InventoryScreenAction> Actions { get; } = new List<InventoryScreenAction>();

        private PickableItemList pickableItemList;
        List<IEntity> items;
        public Button ReturnToGame { get; set; }

        public PickableItemList PickableItemList
        {
            get { return pickableItemList; }
        }

        private Paragraph descriptionHeader;
        private Paragraph descriptionText;
        List<ItemPanel> slotPanels = new List<ItemPanel>();
        private IEntity draggedEntity;
        public InventoryScreen(NamelessGame game)
        {
            this.game = game;
            Panel = new Panel(new Vector2(game.GetActualWidth(), game.GetActualHeight()), PanelSkin.Default, Anchor.BottomRight);
            pickableItemList = new PickableItemList(new Vector2(game.GetActualWidth()/2, game.GetActualHeight()-120), new Vector2(game.GetActualWidth() / 2 - 50, 0));

            ReturnToGame = new Button("Back", size: new Vector2(game.GetSettings().HudWidth() / 2, 50), anchor: Anchor.BottomRight);
            ReturnToGame.ButtonParagraph.Scale = 0.7f;
            ReturnToGame.OnClick += ReturnToGameOnClick;

            Panel.AddChild(PickableItemList);
            Panel.AddChild(ReturnToGame);

            descriptionHeader = new Paragraph();
            descriptionText = new Paragraph();

            slotPanels.Add(new ItemPanel(EquipmentSlots.Slot.Head, new Vector2(game.GetActualWidth() / 2 - 100, 60),new Vector2(), "Head:"));
            slotPanels.Add(new ItemPanel(EquipmentSlots.Slot.Neck, new Vector2(game.GetActualWidth() / 2 - 100, 60), new Vector2(0,60), "Neck:"));
            slotPanels.Add(new ItemPanel(EquipmentSlots.Slot.LeftArm, new Vector2(game.GetActualWidth() / 2 - 100, 60), new Vector2(0, 120), "Left arm:"));
            slotPanels.Add(new ItemPanel(EquipmentSlots.Slot.RightArm, new Vector2(game.GetActualWidth() / 2 - 100, 60), new Vector2(0, 180), "Right arm:"));
            slotPanels.Add(new ItemPanel(EquipmentSlots.Slot.Torso, new Vector2(game.GetActualWidth() / 2 - 100, 60), new Vector2(0, 240), "Torso:"));
            slotPanels.Add(new ItemPanel(EquipmentSlots.Slot.Legs, new Vector2(game.GetActualWidth() / 2 - 100, 60), new Vector2(0, 300), "Legs:"));
            slotPanels.Add(new ItemPanel(EquipmentSlots.Slot.Feet, new Vector2(game.GetActualWidth() / 2 - 100, 60), new Vector2(0, 360), "Feet:"));
            slotPanels.Add(new ItemPanel(EquipmentSlots.Slot.Ring1, new Vector2(game.GetActualWidth() / 2 - 100, 60), new Vector2(0, 420), "Ring:"));
            slotPanels.Add(new ItemPanel(EquipmentSlots.Slot.Ring2, new Vector2(game.GetActualWidth() / 2 - 100, 60), new Vector2(0, 480), "Ring:"));

            foreach (var slotPanel in slotPanels)
            {
                Panel.AddChild(slotPanel);
            }

            UserInterface.Active.AddEntity(Panel);

        }




        private void ReturnToGameOnClick(Entity entity)
        {
            Actions.Add(InventoryScreenAction.ReturnToGame);
        }

        public void UpdateValues()
        {
            var playerEntity = game.GetEntitiesByComponentClass<Player>().First();
            var itemsHolder = playerEntity.GetComponentOfType<ItemsHolder>();
            var equipment = playerEntity.GetComponentOfType<EquipmentSlots>();
            PickableItemList.ClearItems();
            items = itemsHolder.GetItems();
            foreach (var entity in items)
            {
                var description = entity.GetComponentOfType<Description>();
                PickableItemList.AddItem(new PickableItemList.Item(description.Name, entity));
            }

            PickableItemList.Select(0);

            //if (pickableItemList.Items.Any())
            //{
            //    PickableItemList.SelectedIndex = 0;
            //}


            foreach (EquipmentSlots.Slot slot in System.Enum.GetValues(typeof(EquipmentSlots.Slot)))
            {
                var slotPanel = slotPanels.FirstOrDefault(x => x.Slot == slot);
                equipment.Slots.TryGetValue(slot, out EquipmentSlot slotEquipment);
                if (slotEquipment.Equipment != null)
                {
                    var parentEntity = slotEquipment.Equipment.Parent;
                    var description = parentEntity.GetComponentOfType<Description>();
                    slotPanel.EquipmentText = description.Name;
                }
                else
                {
                    slotPanel.EquipmentText ="Nothing";
                }
            }

        }



    }
}

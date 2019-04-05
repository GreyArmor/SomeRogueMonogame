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
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.UiScreens
{
    public enum InventoryScreenAction
    {
        ReturnToGame
    }

    public class InventoryScreen : BaseGuiScreen
    {

        public List<InventoryScreenAction> Actions { get; } = new List<InventoryScreenAction>();

        private SelectList itemsList;
        List<IEntity> items;

        private Panel descriptionPanel;
        public Button ReturnToGame { get; set; }
        private Paragraph descriptionHeader;
        private Paragraph descriptionText;

        public InventoryScreen(NamelessGame game)
        {
            Panel = new Panel(new Vector2(game.GetActualWidth(), game.GetActualHeight()), PanelSkin.Default, Anchor.BottomRight);
            itemsList = new SelectList(new Vector2(game.GetActualWidth()/2, game.GetActualHeight()-120), Anchor.TopRight);


            ReturnToGame = new Button("Back", size: new Vector2(game.GetSettings().HudWidth() / 2, 50), anchor: Anchor.BottomRight);
            ReturnToGame.ButtonParagraph.Scale = 0.7f;
            ReturnToGame.OnClick += ReturnToGameOnClick;

            Panel.AddChild(itemsList);
            Panel.AddChild(ReturnToGame);

            descriptionPanel = new Panel(new Vector2(200, 200), PanelSkin.Simple);
            descriptionPanel.Visible = false;
            descriptionPanel.ClickThrough = true;
            descriptionHeader = new Paragraph();
            descriptionText = new Paragraph();
            descriptionPanel.AddChild(descriptionHeader);
            descriptionPanel.AddChild(new HorizontalLine());
            descriptionPanel.AddChild(descriptionText);


            UserInterface.Active.AddEntity(Panel);
            UserInterface.Active.AddEntity(descriptionPanel);
        }
       

        private void ReturnToGameOnClick(Entity entity)
        {
            Actions.Add(InventoryScreenAction.ReturnToGame);
        }

        public void UpdateValues(NamelessGame namelessGame)
        {
            var playerEntity = namelessGame.GetEntitiesByComponentClass<Player>().First();
            var itemsHolder = playerEntity.GetComponentOfType<ItemsHolder>();
            itemsList.ClearItems();
            items = itemsHolder.GetItems();
            foreach (var entity in items)
            {
                var description = entity.GetComponentOfType<Description>();
                itemsList.AddItem(description.Name);
            }

            UserInterface.Active.WhileMouseHover = OnEnter;
            UserInterface.Active.OnMouseLeave = OnLeave;
        }

        private void OnLeave(Entity entity)
        {
            if (itemsList.Children.Contains(entity))
            {
                descriptionPanel.Visible = false;
            }
        }

        private void OnEnter(Entity entity)
        {
            if (itemsList.Children.Contains(entity))
            {
                var index = itemsList.Children.IndexOf(entity);
                var item = items[index];
                var desc = item.GetComponentOfType<Description>();
                
                var mouseState = Mouse.GetState();
                descriptionPanel.SetPosition(Anchor.TopLeft,new Vector2(mouseState.X, mouseState.Y));
                descriptionPanel.Visible = true;
                descriptionHeader.Text = desc.Name;
                descriptionText.Text = desc.Text;
            }
        }

        private void OnClickItem(Entity entity)
        {
           
        }
    }
}

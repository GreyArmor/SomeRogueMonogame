using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeonBit.UI;
using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Engine.Factories;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.UiScreens
{
    public enum HudAction
    {
        OpenWorldMap
    }
    public class Hud : BaseGuiScreen
    {
        public ProgressBar HealthBar { get; private set; }
        public ProgressBar StaminaBar { get; private set; }

        public Label StrLabel { get; private set; }
        public Label PerLabel { get; private set; }
        public Label RefLabel { get; private set; }
        public Label ImgLabel { get; private set; }
        public Label WillLabel { get; private set; }
        public Label WitLabel { get; private set; }

        public Label TurnLabel { get; private set; }

        public Button WorldMapButton { get; private set; }
        public Button InventoryButton { get; private set; }

        public SelectList EventLog { get; private set; }

        public HashSet<HudAction> ActionsThisTick
        {
            get { return _actionsThisTick; }
        }

        public Hud(NamelessGame game)
        {
            Panel = new Panel(new Vector2(game.GetSettings().HudWidth(), game.GetActualCharacterHeight()), PanelSkin.Default, Anchor.BottomRight);
            HealthBar = new ProgressBar(0, 100);
            HealthBar.Size = new Vector2(100, 10);
            HealthBar.ProgressFill.FillColor = Color.Red;

            StaminaBar = new ProgressBar(0, 100);
            StaminaBar.Size = new Vector2(100, 10);

            StrLabel = new Label("Str");
            PerLabel = new Label("Per");
            RefLabel = new Label("Ref");
            ImgLabel = new Label("Img");
            WillLabel = new Label("Wil");
            WitLabel = new Label("Wit");
            TurnLabel = new Label("Turn");
            SelectList list = new SelectList(new Vector2(0, 150));
            list.Locked = true;
            list.ItemsScale = 0.5f;
            list.ExtraSpaceBetweenLines = -10;
            EventLog = list;

            EventLog.OnListChange = (Entity entity) =>
            {
                SelectList list1 = (SelectList)entity;
                if (list1.Count > 100)
                {
                    list1.RemoveItem(0);
                }
                EventLog.scrollToEnd();
            };





            //  EventLog.ClearItems();



            float labelScale = 0.5f;
            StrLabel.Scale = labelScale;
            PerLabel.Scale = labelScale;
            RefLabel.Scale = labelScale;
            ImgLabel.Scale = labelScale;
            WillLabel.Scale = labelScale;
            WitLabel.Scale = labelScale;
            TurnLabel.Scale = labelScale;

            WorldMapButton = new Button("World map", size: new Vector2(200, 50), anchor: Anchor.BottomRight);
            WorldMapButton.ButtonParagraph.Scale = 0.7f;
            WorldMapButton.OnClick += OnClickWorldMap;

            InventoryButton = new Button("Inventory", size: new Vector2(200, 50), anchor: Anchor.BottomLeft);
            InventoryButton.ButtonParagraph.Scale = 0.7f;
            InventoryButton.OnClick += OnClickWorldMap;


            Panel.AddChild(TurnLabel);
            Panel.AddChild(HealthBar);
            Panel.AddChild(StaminaBar);
            Panel.AddChild(StrLabel);
            Panel.AddChild(PerLabel);
            Panel.AddChild(RefLabel);
            Panel.AddChild(ImgLabel);
            Panel.AddChild(WillLabel);
            Panel.AddChild(WitLabel);

            Panel.AddChild(EventLog);

            Panel.AddChild(InventoryButton);
            Panel.AddChild(WorldMapButton);


            UserInterface.Active.AddEntity(Panel);
        }

        public HashSet<HudAction> _actionsThisTick = new HashSet<HudAction>();

        private void OnClickWorldMap(Entity entity)
        {
            _actionsThisTick.Add(HudAction.OpenWorldMap);
        }
    }
}

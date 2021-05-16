using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.Systems.Ingame;
using NamelessRogue.Engine.UiScreens.UI;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.UiScreens
{
    public enum HudAction
    {
        OpenWorldMap, OpenInventory
    }
    public class Hud : BaseGuiScreen<HudSystem>
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

        public ImageTextButton WorldMapButton { get; private set; }
        public ImageTextButton InventoryButton { get; private set; }

        public ScrollableListBox EventLog { get; private set; }

        public HashSet<HudAction> ActionsThisTick
        {
            get { return _actionsThisTick; }
        }

        public Hud(NamelessGame game)
        {
            Panel = new Panel()
            {
                Width = (int)game.GetSettings().HudWidth(), Height = game.GetActualCharacterHeight(), HorizontalAlignment = HorizontalAlignment.Right,VerticalAlignment = VerticalAlignment.Top
            };

            var vPanel = new VerticalStackPanel();

            HealthBar = new HorizontalProgressBar();
            HealthBar.Width = (int)game.GetSettings().HudWidth();
            HealthBar.Height = 10;
            HealthBar.Maximum = 100;
            HealthBar.Minimum = 0;
            HealthBar.Value = 0.5f;
            HealthBar.VerticalAlignment = VerticalAlignment.Stretch;
            HealthBar.HorizontalAlignment = HorizontalAlignment.Left;
            HealthBar.SetColor(game.GraphicsDevice, Color.Red);

            StaminaBar = new HorizontalProgressBar();
            StaminaBar.Width = (int)game.GetSettings().HudWidth();
            StaminaBar.Height = 10;
            StaminaBar.Maximum = 100;
            StaminaBar.Minimum = 0;
            StaminaBar.VerticalAlignment = VerticalAlignment.Stretch;
            StaminaBar.HorizontalAlignment = HorizontalAlignment.Left;
            StaminaBar.SetColor(game.GraphicsDevice, Color.Green);

            StrLabel = new Label(){Text = "Str"};
            PerLabel = new Label(){Text = "Per"};
            RefLabel = new Label(){Text = "Ref"};
            ImgLabel = new Label(){Text = "Img"};
            WillLabel = new Label() { Text = "Wil"};
            WitLabel = new Label() { Text = "Wit"};
            TurnLabel = new Label(){Text = "Turn"};

            var separator1 = new HorizontalSeparator();
            var separator2 = new HorizontalSeparator();
            ScrollableListBox list = new ScrollableListBox();
            list.Width = (int)game.GetSettings().HudWidth();
            list.Height = 300;

            EventLog = list;

            WorldMapButton = new ImageTextButton()
            {
                GridColumn = 2,
                ContentHorizontalAlignment = HorizontalAlignment.Center,
                ContentVerticalAlignment = VerticalAlignment.Center,
                Text = "Map",
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Right,
                Width = 200,
                Height = 50
            };
            WorldMapButton.Click += OnClickWorldMap;

            InventoryButton = new ImageTextButton()
            {
                GridColumn = 0,
                Text = "Inventory",
                ContentHorizontalAlignment = HorizontalAlignment.Center,
                ContentVerticalAlignment = VerticalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Left,
                Width = 200,
                Height = 50
            };
            InventoryButton.Click+= (sender, args) => { ActionsThisTick.Add(HudAction.OpenInventory); };


            vPanel.Widgets.Add(TurnLabel);
            vPanel.Widgets.Add(HealthBar);
            vPanel.Widgets.Add(StaminaBar);
            vPanel.Widgets.Add(StrLabel);
            vPanel.Widgets.Add(PerLabel);
            vPanel.Widgets.Add(RefLabel);
            vPanel.Widgets.Add(ImgLabel);
            vPanel.Widgets.Add(WillLabel);
            vPanel.Widgets.Add(WitLabel);

            vPanel.Widgets.Add(separator1);
            vPanel.Widgets.Add(EventLog);
            vPanel.Widgets.Add(separator2);


            var grid = new Grid(){VerticalAlignment = VerticalAlignment.Bottom, ColumnSpacing = 3};



            grid.Widgets.Add(InventoryButton);
            grid.Widgets.Add(WorldMapButton);

            Panel.Widgets.Add(vPanel);
            Panel.Widgets.Add(grid);

            game.Desktop.Widgets.Add(Panel);



        }

        public void LogMessage(string message)
        {
            if (EventLog.Items.Any())
            {
                EventLog.Items.Remove(EventLog.Items.Last());
            }

            EventLog.Items.Add(new ListItem(message));

            if (EventLog.Items.Count > 100)
            {
                EventLog.Items.RemoveAt(0);
            }

            EventLog.Scroll.ScrollPosition = new Point(0, int.MaxValue);
            EventLog.Items.Add(new ListItem(""));
        }

        private void OnClickWorldMap(object sender, EventArgs e)
        {
            _actionsThisTick.Add(HudAction.OpenWorldMap);
        }

        public HashSet<HudAction> _actionsThisTick = new HashSet<HudAction>();

    }
}

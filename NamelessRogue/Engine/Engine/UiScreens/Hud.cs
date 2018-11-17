using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeonBit.UI;
using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;
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
        public Label EndLabel { get; private set; }
        public Label AgiLabel { get; private set; }
        public Label ImgLabel { get; private set; }
        public Label WillLabel { get; private set; }
        public Label WitLabel { get; private set; }

        public Button WorldMapButton { get; private set; }

        public HashSet<HudAction> ActionsThisTick
        {
            get { return _actionsThisTick; }
        }

        public Hud(NamelessGame game)
        {
            Panel = new Panel(new Vector2(200, game.GetActualCharacterHeight()), PanelSkin.Default, Anchor.BottomRight);
            
            HealthBar = new ProgressBar(0, 100);
            HealthBar.Size = new Vector2(100, 10);
            HealthBar.ProgressFill.FillColor = Color.Red;

            StaminaBar = new ProgressBar(0, 100);
            StaminaBar.Size = new Vector2(100, 10);

            StrLabel = new Label("Str");
            EndLabel = new Label("End");
            AgiLabel = new Label("Agi");
            ImgLabel = new Label("Img");
            WillLabel = new Label("Will");
            WitLabel = new Label("Wit");


            float labelScale = 0.7f;
            StrLabel.Scale = labelScale;
            EndLabel.Scale = labelScale;
            AgiLabel.Scale = labelScale;
            ImgLabel.Scale = labelScale;
            WillLabel.Scale = labelScale;
            WitLabel.Scale = labelScale;

            WorldMapButton = new Button("World map", size:new Vector2(150, 50), anchor:Anchor.BottomCenter);
            WorldMapButton.ButtonParagraph.Scale = 0.7f;
            WorldMapButton.OnClick += OnClickWorldMap;

            Panel.AddChild(HealthBar);
            Panel.AddChild(StaminaBar);
            Panel.AddChild(StrLabel);
            Panel.AddChild(EndLabel);
            Panel.AddChild(AgiLabel);
            Panel.AddChild(ImgLabel);
            Panel.AddChild(WillLabel);
            Panel.AddChild(WitLabel);
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

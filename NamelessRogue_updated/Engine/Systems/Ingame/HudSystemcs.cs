using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NamelessRogue.Engine.Systems.Ingame
{
    public class HudSystem : BaseSystem
    {

        public HudSystem()
        {
            Signature = new HashSet<Type>();
            Signature.Add(typeof(Player));
            Signature.Add(typeof(Stats));
            Signature.Add(typeof(Position));
        }

        public override HashSet<Type> Signature { get; }

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            foreach (IEntity entity in RegisteredEntities)
            { 
                /*
                Player player = entity.GetComponentOfType<Player>();
                var stats = entity.GetComponentOfType<Stats>();


                var turn = namelessGame.CurrentGame.Turn;

                float healthValue = (float) stats.Health.Value / stats.Health.MaxValue;
                UiFactory.HudInstance.HealthBar.Value = (int) (healthValue * 100f);

                float staminaValue = (float) stats.Stamina.Value / stats.Stamina.MaxValue;
                UiFactory.HudInstance.StaminaBar.Value = (int) (staminaValue * 100f);


                UiFactory.HudInstance.StrLabel.Text = $"Str: {stats.Strength.Value}";
                UiFactory.HudInstance.ImgLabel.Text = $"Img: {stats.Imagination.Value}";
                UiFactory.HudInstance.RefLabel.Text = $"Ref: {stats.Reflexes.Value}";
                UiFactory.HudInstance.WillLabel.Text = $"Wil: {stats.Willpower.Value}";
                UiFactory.HudInstance.PerLabel.Text = $"Per: {stats.Perception.Value}";
                UiFactory.HudInstance.WitLabel.Text = $"Wit: {stats.Wit.Value}";
                UiFactory.HudInstance.TurnLabel.Text = $"Turn  {turn}";

                foreach (var hudAction in UiFactory.HudInstance.ActionsThisTick)
                {
                    switch (hudAction)
                    {
                        case HudAction.OpenWorldMap:
                            var playerPosition = entity.GetComponentOfType<Position>();
                            namelessGame.ContextToSwitch = ContextFactory.GetWorldBoardContext(namelessGame);
                            var cursorPosition = namelessGame.CursorEntity
                                .GetComponentOfType<Position>();

                            cursorPosition.Point = new Microsoft.Xna.Framework.Point((int)(playerPosition.Point.X / Constants.ChunkSize), (int)(playerPosition.Point.Y / Constants.ChunkSize));
                            break;
                        case HudAction.OpenInventory:
                            namelessGame.ContextToSwitch = ContextFactory.GetInventoryContext(namelessGame);
                            UiFactory.InventoryScreen.FillItems(namelessGame);
                            if (UiFactory.InventoryScreen.ItemBox.Items.Any())
                            {
                                UiFactory.InventoryScreen.ItemBox.SelectedIndex = 0;
                            }

                            break;
                            ;
                        default:
                            break;
                    }
                }

                UiFactory.HudInstance.ActionsThisTick.Clear();


                while (namelessGame.Commander.DequeueCommand(out HudLogMessageCommand logMessage))
                {
                    UiFactory.HudInstance.LogMessage(logMessage.LogMessage);
                }
                */
            }
        }

	}
}

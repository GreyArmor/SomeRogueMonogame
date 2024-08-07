﻿
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.UI;
using NamelessRogue.Engine.Utility;
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

        public override void Update(GameTime gameTime, NamelessGame game)
        {
            foreach (IEntity entity in RegisteredEntities)
            { 
                
                Player player = entity.GetComponentOfType<Player>();
                var stats = entity.GetComponentOfType<Stats>();


                var turn = game.CurrentGame.Turn;

                float healthValue = (float) stats.Health.Value / stats.Health.MaxValue;
                //UIController.HudScreen.HealthBar.Value = (int) (healthValue * 100f);

                float staminaValue = (float) stats.Stamina.Value / stats.Stamina.MaxValue;
               // UIController.HudScreen.StaminaBar.Value = (int) (staminaValue * 100f);

                //UIController.HudScreen.StrLabel.Text = $"Str: {stats.Strength.Value}";
                //UIController.HudScreen.ImgLabel.Text = $"Img: {stats.Imagination.Value}";
                //UIController.HudScreen.RefLabel.Text = $"Ref: {stats.Reflexes.Value}";
                //UIController.HudScreen.WillLabel.Text = $"Wil: {stats.Willpower.Value}";
                //UIController.HudScreen.PerLabel.Text = $"Per: {stats.Perception.Value}";
                //UIController.HudScreen.WitLabel.Text = $"Wit: {stats.Wit.Value}";
                //UIController.HudScreen.TurnLabel.Text = $"Turn  {turn}";

                    switch (UIController.Instance.HudScreen.Action)
                    {
                        case HudAction.OpenWorldMap:
                            var playerPosition = entity.GetComponentOfType<Position>();
                            game.ContextToSwitch = ContextFactory.GetWorldBoardContext(game);
                            var cursorPosition = game.CursorEntity
                                .GetComponentOfType<Position>();

                            cursorPosition.Point = new Point((int)(playerPosition.Point.X / Constants.ChunkSize), (int)(playerPosition.Point.Y / Constants.ChunkSize));
                            break;
                        case HudAction.OpenInventory:
                            //game.ContextToSwitch = ContextFactory.GetInventoryContext(game);
                            //UIController.InventoryScreen.FillItems(game);
                            //if (UIController.InventoryScreen.ItemBox.Items.Any())
                            //{
                            //    UIController.InventoryScreen.ItemBox.SelectedIndex = 0;
                            //}

                            break;
                        default:
                            break;
                    }
                UIController.Instance.HudScreen.Action = HudAction.None;
                //while (game.Commander.DequeueCommand(out HudLogMessageCommand logMessage))
                //{
                //    UIController.HudScreen.LogMessage(logMessage.LogMessage);
                //}

            }
        }

	}
}

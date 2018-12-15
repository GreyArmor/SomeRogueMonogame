using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Engine.Components.Interaction;
using NamelessRogue.Engine.Engine.Components.Physical;
using NamelessRogue.Engine.Engine.Components.Stats;
using NamelessRogue.Engine.Engine.Factories;
using NamelessRogue.Engine.Engine.Infrastructure;
using NamelessRogue.Engine.Engine.UiScreens;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Systems
{
    public class HudSystem : ISystem
    {
        public void Update(long gameTime, NamelessGame namelessGame)
        {
            foreach (IEntity entity in namelessGame.GetEntities())
            {
                Player player = entity.GetComponentOfType<Player>();
                if (player != null)
                {
                    var health = entity.GetComponentOfType<Health>();
                    var stamina = entity.GetComponentOfType<Stamina>();


                    var str = entity.GetComponentOfType<Strength>();
                    var img = entity.GetComponentOfType<Imagination>();
                    var agi = entity.GetComponentOfType<Agility>();
                    var will = entity.GetComponentOfType<Willpower>();
                    var end = entity.GetComponentOfType<Endurance>();
                    var wit = entity.GetComponentOfType<Wit>();

                    float healthValue = (float)health.getValue() / health.getMaxValue();
                    UiFactory.HudInstance.HealthBar.Value = (int) (healthValue * 100f);

                    float staminaValue = (float)stamina.getValue() / stamina.getMaxValue();
                    UiFactory.HudInstance.StaminaBar.Value = (int)(staminaValue * 100f);


                    UiFactory.HudInstance.StrLabel.Text = $"Str: {str.getValue()}";
                    UiFactory.HudInstance.ImgLabel.Text = $"Img: {img.getValue()}";
                    UiFactory.HudInstance.AgiLabel.Text = $"Agi: {agi.getValue()}";
                    UiFactory.HudInstance.WillLabel.Text = $"Wil: {will.getValue()}";
                    UiFactory.HudInstance.EndLabel.Text = $"End: {end.getValue()}";
                    UiFactory.HudInstance.WitLabel.Text = $"Wit: {wit.getValue()}";

                    foreach (var hudAction in UiFactory.HudInstance.ActionsThisTick)
                    {
                        switch (hudAction)
                        {
                            case HudAction.OpenWorldMap:
                                var playerPosition = entity.GetComponentOfType<Position>();
                                namelessGame.ContextToSwitch = ContextFactory.GetWorldBoardContext(namelessGame);
                                var cursorPosition = namelessGame.GetEntityByComponentClass<Cursor>().GetComponentOfType<Position>();
                                cursorPosition.p.X = (int) (playerPosition.p.X / Constants.ChunkSize);
                                cursorPosition.p.Y = (int) (playerPosition.p.Y / Constants.ChunkSize);
                                break;
                            default:
                                break;
                        }
                    }
                    UiFactory.HudInstance.ActionsThisTick.Clear();

                }


                HudLogMessageCommand logMEssgae = entity.GetComponentOfType<HudLogMessageCommand>();
                if (logMEssgae != null)
                {
                    UiFactory.HudInstance.EventLog.AddItem(logMEssgae.LogMessage);
                    entity.RemoveComponentOfType<HudLogMessageCommand>();
                }

            }
        }
    }
}

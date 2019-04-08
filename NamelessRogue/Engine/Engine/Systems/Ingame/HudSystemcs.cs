using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.Interaction;
using NamelessRogue.Engine.Engine.Components.Physical;
using NamelessRogue.Engine.Engine.Components.Stats;
using NamelessRogue.Engine.Engine.Factories;
using NamelessRogue.Engine.Engine.Infrastructure;
using NamelessRogue.Engine.Engine.UiScreens;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Systems.Ingame
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
                    var stats = entity.GetComponentOfType<Stats>();
    

                    var turn = namelessGame.CurrentGame.Turn;

                    float healthValue = (float)stats.Health.Value / stats.Health.MaxValue;
                    UiFactory.HudInstance.HealthBar.Value = (int) (healthValue * 100f);

                    float staminaValue = (float)stats.Stamina.Value / stats.Stamina.MaxValue;
                    UiFactory.HudInstance.StaminaBar.Value = (int)(staminaValue * 100f);


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
                                var cursorPosition = namelessGame.GetEntityByComponentClass<Cursor>().GetComponentOfType<Position>();
                                cursorPosition.p.X = (int) (playerPosition.p.X / Constants.ChunkSize);
                                cursorPosition.p.Y = (int) (playerPosition.p.Y / Constants.ChunkSize);
                                break;
                            case HudAction.OpenInventory:
                                namelessGame.ContextToSwitch = ContextFactory.GetInventoryContext(namelessGame);
                                UiFactory.InventoryScreen.UpdateValues();
                                break;;
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

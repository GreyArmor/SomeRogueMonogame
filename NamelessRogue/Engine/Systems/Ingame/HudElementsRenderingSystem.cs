using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NamelessRogue.Engine.Components;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Stats;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.UI;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Systems.Ingame
{
    internal class HudElementsRenderingSystem : RenderingSystem
    {
        AnimatedSpriteNR healthBar = null;
        AnimatedSpriteNR energyBar = null;
        int healthValue = 0;
        string haealthBarAnimation = "h0";
        public static Vector2 HealthPosition = new Vector2(20) { };

        public static Vector2 EnergyPosition = new Vector2(20, 70) { };
        public HudElementsRenderingSystem(GameSettings settings) : base(settings)
        {
            healthBar = SpriteLibrary.SpritesAnimatedIdle["healthbar"];
            energyBar = SpriteLibrary.SpritesAnimatedIdle["energybar"];
        }

        public override void Update(GameTime gameTime, NamelessGame game)
        {
            Player player = game.PlayerEntity.GetComponentOfType<Player>();
            var stats = game.PlayerEntity.GetComponentOfType<CharacterStats>();


            var turn = game.CurrentGame.Turn;

            var healthValue = stats.Health.Value;

            float healthRatio = stats.Health.Value / (float)stats.Health.MaxValue;
            float energyRatio = stats.Energy.Value / (float)stats.Energy.MaxValue;

            var haealthBarAnimation = healthRatio < 0.1 ? "h0" : healthRatio < 0.2 ? "h1" : healthRatio < 0.3 ? "h2" : healthRatio < 0.4 ? "h3" : healthRatio < 0.5 ? "h4" : healthRatio < 0.6 ? "h5" : healthRatio < 0.7 ? "h6" : healthRatio < 0.8 ? "h7" : healthRatio < 0.9 ? "h8" : healthRatio < 1 ? "h9" : "idle";
            var energyBarAnimation = energyRatio < 0.1 ? "e0" : energyRatio < 0.2 ? "e1" : energyRatio < 0.3 ? "e2" : energyRatio < 0.4 ? "e3" : energyRatio < 0.5 ? "e4" : energyRatio < 0.6 ? "e5" : energyRatio < 0.7 ? "e6" : energyRatio < 0.8 ? "e7" : energyRatio < 0.9 ? "e8" : energyRatio < 1 ? "e9" : "idle";

            if (Keyboard.GetState().IsKeyDown(Keys.K))
            {
                stats.Energy.Value--;
            }
            else if ((Keyboard.GetState().IsKeyDown(Keys.I)))
            {
                stats.Energy.Value++;
            }


            healthBar.SetCurrent(haealthBarAnimation);
            energyBar.SetCurrent(energyBarAnimation);
            game.Batch.Begin();
            healthBar.Draw(game, gameTime, HealthPosition, new Vector2(1, 1f), Microsoft.Xna.Framework.Color.White);
            energyBar.Draw(game, gameTime, EnergyPosition, new Vector2(1, 1f), Microsoft.Xna.Framework.Color.White);
            game.Batch.End();
        }
    }
}

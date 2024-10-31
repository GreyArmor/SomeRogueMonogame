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
        int healthValue = 0;
        string haealthBarAnimation = "h0";
        public static Vector2 HealthPosition = new Vector2(20) { };
        public HudElementsRenderingSystem(GameSettings settings) : base(settings)
        {
            healthBar = SpriteLibrary.SpritesAnimatedIdle["healthbar"];
        }

        public override void Update(GameTime gameTime, NamelessGame game)
        {
            Player player = game.PlayerEntity.GetComponentOfType<Player>();
            var stats = game.PlayerEntity.GetComponentOfType<Stats>();


            var turn = game.CurrentGame.Turn;

            var healthValue = stats.Health.Value;

            float ratio = stats.Health.Value / (float)stats.Health.MaxValue;

            var haealthBarAnimation = ratio < 0.1 ? "h0" : ratio < 0.2 ? "h1" : ratio < 0.3 ? "h2" : ratio < 0.4 ? "h3" : ratio < 0.5 ? "h4" : ratio < 0.6 ? "h5" : ratio < 0.7 ? "h6" : ratio < 0.8 ? "h7" : ratio < 0.9 ? "h8" : ratio < 1 ? "h9" : "idle";

            // float staminaValue = (float) stats.Stamina.Value / stats.Stamina.MaxValue;

            if (Keyboard.GetState().IsKeyDown(Keys.K))
            {
                stats.Health.Value--;
            }
            else if((Keyboard.GetState().IsKeyDown(Keys.I)))
            {
                stats.Health.Value++;
            }


            healthBar.SetCurrent(haealthBarAnimation);
            game.Batch.Begin();
            healthBar.Draw(game, gameTime, HealthPosition, new Vector2(1, 0.5f), Microsoft.Xna.Framework.Color.White);
            game.Batch.End();
        }
    }
}

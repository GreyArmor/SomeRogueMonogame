using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Systems.Editors
{
    internal class EditorCharacterScreenSpriteRenderSystem : BaseSystem
    {
        private AnimatedSpriteNR currentSprite = null;

        public static Vector2 SpritePosition = new Vector2();
        public static Vector2 SpriteSize = new Vector2(24);
        public override HashSet<Type> Signature { get; } = new HashSet<Type>();
        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            while (namelessGame.Commander.DequeueCommand(out CharacterScreenChangeSpriteCommand command))
            {
                currentSprite = SpriteLibrary.SpritesAnimated[command.SpriteId];
            }

            while (namelessGame.Commander.DequeueCommand(out CharacterScreeChangeSpriteAnimationCommand command))
            {
                if (currentSprite != null)
                { 
                    currentSprite.SetCurrentLoop(command.AnimationId);
                }
            }

            if (currentSprite == null)
            {
                return;
            }
            currentSprite.Update(gameTime);
            namelessGame.Batch.Begin(samplerState: SamplerState.PointClamp);
            currentSprite.Draw(namelessGame, gameTime, SpritePosition, SpriteSize, Vector2.One, Microsoft.Xna.Framework.Color.White);
            namelessGame.Batch.End();
        }
    }
}

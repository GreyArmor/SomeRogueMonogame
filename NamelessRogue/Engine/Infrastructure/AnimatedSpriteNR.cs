using Microsoft.Xna.Framework;
using MonoGame.Aseprite;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Infrastructure
{
    internal class AnimatedSpriteNR
    {

        public AnimatedSpriteNR() {}

        public Dictionary<string, AnimatedSprite> _animations = new Dictionary<string, AnimatedSprite>();

        AnimatedSprite currentAnimation;
        public void Add(string name, AnimatedSprite sprite)
        {
            _animations.Add(name, sprite);
        }

        public void Remove(string name)
        {
            _animations.Remove(name);
        }

        public void SetCurrent(string animationName)
        {
            currentAnimation = _animations[animationName];
            currentAnimation.Reset();
            currentAnimation.Play();
        }

        public void Update(GameTime time)
        {
            currentAnimation.Update(time);
        }

        public void SetFrame(int frame)
        {
            currentAnimation.SetFrame(frame);
        }

        public void Draw(NamelessGame game, GameTime time, Vector2 position, Vector2 scale, Microsoft.Xna.Framework.Color color = default)
        {
            currentAnimation.Scale = scale;

            if(color == default)
            {
                currentAnimation.Color = Microsoft.Xna.Framework.Color.White;
            }
            else
            {
                currentAnimation.Color = color;
            }

           
            currentAnimation.Draw(game.Batch, position);
        }
    }
}

using Microsoft.Xna.Framework;
using MonoGame.Aseprite;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = Microsoft.Xna.Framework.Color;

namespace NamelessRogue.Engine.Infrastructure
{
    internal class AnimatedSpriteNR
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Vector2 Size { get; private set; } 
        public AnimatedSpriteNR(int width, int height) {
            Width = width;
            Height = height;
            Size = new Vector2(width, height);
        }

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

        public void SetCurrentLoop(string animationName)
        {
            currentAnimation = _animations[animationName];
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

        public void Draw(NamelessGame game, GameTime time, Vector2 position, Vector2 size, Vector2 scale, Microsoft.Xna.Framework.Color color = default)
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
            
            game.Batch.Draw(currentAnimation.TextureRegion, new Microsoft.Xna.Framework.Rectangle(position.ToPoint(), (size * scale).ToPoint()), color);

        }
    }
}

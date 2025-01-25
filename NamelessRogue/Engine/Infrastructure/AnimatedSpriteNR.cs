using Microsoft.Xna.Framework;
using MonoGame.Aseprite;
using NamelessRogue.Engine.Components._3D;
using NamelessRogue.shell;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = Microsoft.Xna.Framework.Color;

namespace NamelessRogue.Engine.Infrastructure
{

    public enum AnimationType
    {
        Idle,
        Attack,
        Death,
        Dead,
    }

    internal class AnimatedSpriteNR
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Vector2 Size { get; private set; }
        public AnimatedSprite CurrentAnimation { get => currentAnimation; set => currentAnimation = value; }

        public AnimatedSpriteNR(int width, int height) {
            Width = width;
            Height = height;
            Size = new Vector2(width, height);

            foreach (AnimationType enumValue in Enum.GetValues(typeof(AnimationType)))
            {
                _animationsByType[enumValue] = new List<string>();
            }
        }

        public Dictionary<string, AnimatedSprite> _animations = new Dictionary<string, AnimatedSprite>();

        public Dictionary<AnimationType, List<string>> _animationsByType = new Dictionary<AnimationType, List<string>>();

        AnimatedSprite currentAnimation;
        public void Add(string name, AnimatedSprite sprite)
        {
            _animations.Add(name, sprite);

            AddToTypeCollactionIfAppropriate(name, "idle", AnimationType.Idle);
            AddToTypeCollactionIfAppropriate(name, "attack", AnimationType.Attack);
            AddToTypeCollactionIfAppropriate(name, "death", AnimationType.Death);
            AddToTypeCollactionIfAppropriate(name, "dead", AnimationType.Dead);
        }

        private void AddToTypeCollactionIfAppropriate(string name, string type, AnimationType animationType)
        {
            if (name.Contains(type))
            {
                _animationsByType[animationType].Add(name);
            }
        }

        public void Remove(string name)
        {
            _animations.Remove(name);
            foreach (var animationList in _animationsByType.Values)
            {
                animationList.Remove(name);
            }
        }

        public void SetCurrentLoop(string animationName)
        {
            CurrentAnimation = _animations[animationName];
            CurrentAnimation.Play();
        }

        public void Update(GameTime time)
        {
            CurrentAnimation.Update(time);
        }

        public void SetFrame(int frame)
        {
            CurrentAnimation.SetFrame(frame);
        }

        public void Draw(NamelessGame game, GameTime time, Vector2 position, Vector2 size, Vector2 scale, Microsoft.Xna.Framework.Color color = default)
        {
            CurrentAnimation.Scale = scale;
            if(color == default)
            {
                CurrentAnimation.Color = Microsoft.Xna.Framework.Color.White;
            }
            else
            {
                CurrentAnimation.Color = color;
            }            
            game.Batch.Draw(CurrentAnimation.TextureRegion, new Microsoft.Xna.Framework.Rectangle(position.ToPoint(), (size * scale).ToPoint()), color);
        }
    }
}

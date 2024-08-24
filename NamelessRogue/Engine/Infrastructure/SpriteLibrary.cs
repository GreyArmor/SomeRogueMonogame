using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using NamelessRogue.shell;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sprite = MonoGame.Extended.Sprites.Sprite;

namespace NamelessRogue.Engine.Infrastructure
{
	internal class SpriteLibrary
	{
		public static readonly Dictionary<string, AnimatedSprite> SpritesAnimatedIdle = new Dictionary<string, AnimatedSprite>();
        public static readonly Dictionary<string, Sprite> SpritesStatic = new Dictionary<string, Sprite>();

        public static void Initialize(NamelessGame game)
		{

			void _addAnimatedSprite(string id, string path)
			{
				var sprite = new AnimatedSprite(game.Content.Load<SpriteSheet>(path, new JsonContentLoader()));
				sprite.Play("idleFront");
				sprite.Update(1);
                SpritesAnimatedIdle.Add(id, sprite);
			}

            void _addStaticSprite(string id, string path)
            {
                var sprite = new Sprite(game.Content.Load<Texture2D>(path));
                SpritesStatic.Add(id, sprite);
            }


            _addAnimatedSprite("treeEvergreen", "Doodads\\treeEvergreen.sf");
			_addAnimatedSprite("cacti", "Doodads\\cacti.sf");
			_addAnimatedSprite("palmTree", "Doodads\\palmTree.sf");
			_addAnimatedSprite("stump", "Doodads\\stump.sf");
			_addAnimatedSprite("smallTree", "Doodads\\smallTree.sf");
			_addAnimatedSprite("stone", "Doodads\\stone.sf");
            _addAnimatedSprite("star", "Doodads\\star.sf");
            _addAnimatedSprite("seashells1", "Doodads\\seashells1.sf");

            _addStaticSprite("box", "Sprites\\box");
            _addStaticSprite("boxMetal", "Sprites\\boxMetal");
            _addStaticSprite("barrel", "Sprites\\barrel");

			for(int i = 1; i <= 21; i++ )
			{
                _addStaticSprite($@"garbage{i}", $@"Sprites\\Garbage\\garbage{i}");
            }

        }
	}
}

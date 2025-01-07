using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Serialization.Json;
using NamelessRogue.shell;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sprite = MonoGame.Extended.Graphics.Sprite;
using AsepriteDotNet.Aseprite;
using AsepriteDotNet.IO;
using MonoGame.Aseprite;
using System.IO;
using AsepriteDotNet.Processors;
using AnimatedSprite = MonoGame.Aseprite.AnimatedSprite;

namespace NamelessRogue.Engine.Infrastructure
{

	internal class SpriteLibrary
	{
		public static readonly Dictionary<string, AnimatedSpriteNR> SpritesAnimated = new Dictionary<string, AnimatedSpriteNR>();
        public static readonly Dictionary<string, Sprite> SpritesStatic = new Dictionary<string, Sprite>();
        static NamelessGame game;

        public static void AddAnimatedSprite(string id, string path)
        {
            AsepriteFile aseFile;
            using (Stream stream = File.OpenRead(path))
            {
                aseFile = AsepriteFileLoader.FromStream(Path.GetFileName(path), stream, preMultiplyAlpha: true);
            }

            AnimatedSpriteNR sprite = new AnimatedSpriteNR(aseFile.CanvasWidth, aseFile.CanvasHeight);

            var spriteSheet = aseFile.CreateSpriteSheet(game.GraphicsDevice);

            var firstAnimation = "";

            foreach (var animTag in spriteSheet.GetAnimationTagNames())
            {
                if (firstAnimation == "")
                {
                    firstAnimation = animTag;
                }
                var animation = spriteSheet.CreateAnimatedSprite(animTag);
                sprite.Add(animTag, animation);
            }

            sprite.SetCurrentLoop(firstAnimation);
            SpritesAnimated.Add(id, sprite);
        }

        public static void RemoveAnimatedSprite(string id)
        {
            SpritesAnimated.Remove(id);
        }

            public static void AddStaticSprite(string id, string path)
        {
            var sprite = new Sprite(game.Content.Load<Texture2D>(path));
            SpritesStatic.Add(id, sprite);
        }

        public static void Initialize(NamelessGame namelessGame)
        {
            game = namelessGame;
            AddAnimatedSprite("ZeroAndOne", "Content\\Sprites\\ZeroAndOne.ase");
            AddAnimatedSprite("ZeroAndOne2", "Content\\Sprites\\ZeroAndOne2.ase");
            AddAnimatedSprite("computer1", "Content\\Sprites\\AnimatedFurniture\\computer1.ase");
            AddAnimatedSprite("healthbar", "Content\\Sprites\\healthbar.ase");
            AddAnimatedSprite("energybar", "Content\\Sprites\\energybar.ase");
            AddAnimatedSprite("drone_recon", "Content\\Sprites\\drone_recon.ase");

            AddStaticSprite("box", "Sprites\\box");
            AddStaticSprite("boxMetal", "Sprites\\boxMetal");
            AddStaticSprite("barrel", "Sprites\\barrel");
            AddStaticSprite("bullet", "Sprites\\bullet");

            for (int i = 1; i <= 21; i++)
            {
                AddStaticSprite($@"garbage{i}", $@"Sprites\\Garbage\\garbage{i}");
            }
        }
	}
}

﻿
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Infrastructure
{
	internal class SpriteLibrary
	{
		public static readonly Dictionary<string, AnimatedSprite> SpritesIdle = new Dictionary<string, AnimatedSprite>();

		public static void Initialize(NamelessGame game)
		{

			void _addSprite(string id, string path)
			{
				//var sprite = new AnimatedSprite(NamelessGame.Content.Load<SpriteSheet>(path, new JsonContentLoader()));
				//sprite.Play("idleFront");
				//sprite.Update(1);
				//SpritesIdle.Add(id, sprite);
			}

			_addSprite("treeEvergreen", "Doodads\\treeEvergreen.sf");
			_addSprite("cacti", "Doodads\\cacti.sf");
			_addSprite("palmTree", "Doodads\\palmTree.sf");
			_addSprite("stump", "Doodads\\stump.sf");
			_addSprite("smallTree", "Doodads\\smallTree.sf");
			_addSprite("stone", "Doodads\\stone.sf");
            _addSprite("star", "Doodads\\star.sf");
            _addSprite("seashells1", "Doodads\\seashells1.sf");
        }
	}
}

﻿using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.shell;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Physical
{
	internal class Position3D : Component
	{
		public Vector3 Position { get; set; }
		//the unit looks in this direction;
		public Vector2 Normal { get; set; }
		public Position3D(Vector3 position, Vector2 normal)
		{
			Position = position;
			Normal = normal;
		}

		public Position3D() { }

		public Tile Tile { get; set; }

		public Vector3? WorldPosition { get; set; } = null;


		public void InitWorldPosition(NamelessGame namelessGame, int offset)
		{
			if (Tile == null)
			{
				var tile = namelessGame.WorldProvider.GetTile((int)Position.X, (int)Position.Y);
				Tile = tile;
			}

			var tileToDraw = Tile;
			var position = new Point((int)(Position.X - offset), (int)(Position.Y - offset));
			var world = Constants.ScaleDownMatrix * Matrix.CreateTranslation(position.X * Constants.ScaleDownCoeficient, position.Y * Constants.ScaleDownCoeficient, tileToDraw.ElevationVisual * Constants.ScaleDownCoeficient);
			WorldPosition = Vector3.Transform(Vector3.One, world);
		}

	}
}

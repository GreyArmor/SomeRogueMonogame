using Microsoft.Xna.Framework;
using MonoGame.Aseprite;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Environment;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;

namespace NamelessRogue.Engine.Systems.Ingame
{
	public class StreetlightsRenderingSystem : BaseSystem
	{
		public override HashSet<Type> Signature { get; } = new HashSet<Type>() { typeof(Drawable), typeof(Streetlight) };
        const int StreeLightSize = 16;
		Rectangle streetLighRect = new Rectangle(-StreeLightSize, -StreeLightSize, StreeLightSize, StreeLightSize);
		
		public override void Update(GameTime gameTime, NamelessGame game)
		{
			var cameraEntity = game.CameraEntity;
			ConsoleCamera camera = cameraEntity.GetComponentOfType<ConsoleCamera>();
            var streetlightKeeperEntity = game.StreetLightsKeeper;
            var isVertical = streetlightKeeperEntity.GetComponentOfType<StreetlightsStatusKeeper>().VerticalMovementAllowed;
			int tileHeight = game.GetSettings().GetFontSizeZoomed();
			int tileWidth = game.GetSettings().GetFontSizeZoomed();
			var zoom = game.GetSettings().Zoom;
			Vector2 halfTile = new Vector2(tileWidth / 2, tileHeight / 2);
			var playerPosition = game.PlayerEntity.GetComponentOfType<Position>();
			game.Batch.Begin();
			var zoomedSize = StreeLightSize / zoom;
			foreach (IEntity entity in RegisteredEntities)
			{
                var position = entity.GetComponentOfType<Position>();
				
				var distance = (position.Point.ToPoint() - playerPosition.Point.ToPoint()).ToVector2().Length();

				if (distance > Constants.DebugVisionRangePlusOne)
				{ continue; }

				var streetlightComponent = entity.GetComponentOfType<Streetlight>();
				if (streetlightComponent != null)
				{
					Vector2 screenPoint = new Vector2((position.X - camera.Position.X) * tileWidth, (position.Y - camera.Position.Y) * tileHeight) + halfTile;
					var rectNorth = streetLighRect with { X = screenPoint.ToPoint().X, Y = screenPoint.ToPoint().Y - zoomedSize, Width = zoomedSize, Height = zoomedSize };
					var rectSouth = streetLighRect with { X = screenPoint.ToPoint().X, Y = screenPoint.ToPoint().Y + zoomedSize, Width = zoomedSize, Height = zoomedSize };
					var rectWest = streetLighRect with { X = screenPoint.ToPoint().X - zoomedSize, Y = screenPoint.ToPoint().Y, Width = zoomedSize, Height = zoomedSize };
					var rectEast = streetLighRect with { X = screenPoint.ToPoint().X + zoomedSize, Y = screenPoint.ToPoint().Y, Width = zoomedSize, Height = zoomedSize };
					var verticalColor = Microsoft.Xna.Framework.Color.Red with { A = 128 };
					var horizontalColor = Microsoft.Xna.Framework.Color.Green with { A = 128 };
					if (isVertical)
					{
						(verticalColor, horizontalColor) = (horizontalColor, verticalColor);
					}
					game.Batch.FillRectangle(rectNorth, verticalColor);
					game.Batch.FillRectangle(rectSouth, verticalColor);
					game.Batch.FillRectangle(rectWest, horizontalColor);
					game.Batch.FillRectangle(rectEast, horizontalColor);
				}
			}
			game.Batch.End();
		}
	}
}

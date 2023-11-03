using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Utility;
using NamelessRogue.shell;
using SharpDX;
using System;
using System.Collections.Generic;
using BoundingBox = Microsoft.Xna.Framework.BoundingBox;
using Color = Microsoft.Xna.Framework.Color;
using Matrix = Microsoft.Xna.Framework.Matrix;
using SpriteBatch = Microsoft.Xna.Framework.Graphics.SpriteBatch;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using Viewport = Microsoft.Xna.Framework.Graphics.Viewport;

namespace NamelessRogue.Engine.Systems.Ingame
{
	internal class SelectionRenderingSystem : BaseSystem, IDisposable
	{
		LineDrawer LineDrawer;
		public SelectionRenderingSystem(NamelessGame game)
		{
			whiteRectangle = new Texture2D(game.GraphicsDevice, 1, 1);
			whiteRectangle.SetData(new[] { Color.White });
			LineDrawer = new LineDrawer(game.GraphicsDevice);
			Viewport viewport = game.GraphicsDevice.Viewport;
			screenSpaceProjection = Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0, 1);
		}
		public override HashSet<Type> Signature { get; }

		Texture2D whiteRectangle;
		Matrix screenSpaceProjection;
		public override void Update(GameTime gameTime, NamelessGame namelessGame)
		{
			var player = namelessGame.PlayerEntity;
			var selectionData = player.GetComponentOfType<SelectionData>();
			if (selectionData.SelectionState == SelectionState.Drag)
			{
				LineDrawer.Begin(Matrix.Identity, screenSpaceProjection);
				var box = new BoundingBox(new Vector3(selectionData.SelectionStart.X, selectionData.SelectionStart.Y, 0), new Vector3(selectionData.SelectionEnd.X, selectionData.SelectionEnd.Y, 0));
				LineDrawer.DrawWireBox(box, Color.LightGreen);
				LineDrawer.End();
			}
		}

		public void Dispose()
		{
			whiteRectangle.Dispose();
		}
	}
}

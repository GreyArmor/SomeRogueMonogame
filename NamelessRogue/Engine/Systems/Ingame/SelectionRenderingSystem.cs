using SharpDX;

using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Utility;
using NamelessRogue.shell;
using SharpDX;
using System;
using System.Collections.Generic;
using NamelessRogue.Engine.Infrastructure;

namespace NamelessRogue.Engine.Systems.Ingame
{
	internal class SelectionRenderingSystem : BaseSystem, IDisposable
	{
	//	LineDrawer LineDrawer;
		public SelectionRenderingSystem(NamelessGame game)
		{
			//whiteRectangle = new Texture2D(NamelessGame.GraphicsDevice, 1, 1);
			//whiteRectangle.SetData(new[] { Color.White });
			//LineDrawer = new LineDrawer(NamelessGame.GraphicsDevice);
			//Viewport viewport = game.GraphicsDevice.Viewport;
			//screenSpaceProjection = Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0, 1);
		}
		public override HashSet<Type> Signature { get; }
//		Texture2D whiteRectangle;
		Matrix screenSpaceProjection;
		public override void Update(GameTime gameTime, NamelessGame game)
		{
			//var player = game.PlayerEntity;
			//var selectionData = player.GetComponentOfType<SelectionData>();
			//if (selectionData.SelectionState == SelectionState.Drag)
			//{
			//	LineDrawer.Begin(Matrix.Identity, screenSpaceProjection);
			//	var box = new BoundingBox(new Vector3(selectionData.SelectionStart.X, selectionData.SelectionStart.Y, 0), new Vector3(selectionData.SelectionEnd.X, selectionData.SelectionEnd.Y, 0));
			//	LineDrawer.DrawWireBox(box, Color.LightGreen);
			//	LineDrawer.End();
			//}
		}

		public void Dispose()
		{
		//	whiteRectangle.Dispose();
		}
	}
}

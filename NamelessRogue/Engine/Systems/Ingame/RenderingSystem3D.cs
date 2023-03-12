using System;
using System.Collections.Generic;
using System.Linq;
using RogueSharp.Random;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components;
using NamelessRogue.Engine.Components.AI.NonPlayerCharacter;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Components.Environment;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Utility;
using NamelessRogue.FieldOfView;
using NamelessRogue.shell;
using BoundingBox = NamelessRogue.Engine.Utility.BoundingBox;
using System.Diagnostics;
using NamelessRogue.Engine.Components._3D;
using System.Reflection.Metadata;
using System.Windows.Forms;
using AStarNavigator;
using Tile = NamelessRogue.Engine.Components.ChunksAndTiles.Tile;
using NamelessRogue.Engine.Systems._3DView;
using VertexBuffer = Microsoft.Xna.Framework.Graphics.VertexBuffer;
using System.Runtime.InteropServices;
using SharpDX.XAudio2;
using Color = Microsoft.Xna.Framework.Color;

namespace NamelessRogue.Engine.Systems.Ingame
{
	public class RenderingSystem3D : BaseSystem
    {
        public override HashSet<Type> Signature { get; }

        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector4, VertexElementUsage.Color, 0),
            new VertexElement(sizeof(float) * 7, VertexElementFormat.Vector4, VertexElementUsage.Color, 1),
            new VertexElement(sizeof(float) * 11, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(sizeof(float) * 13, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
        );

		public static readonly VertexDeclaration VertexShaderInstanceInput = new VertexDeclaration
		(
			//this is 4x4 matrix, transferred as instructed in https://learn.microsoft.com/en-us/windows/win32/direct3d9/efficiently-drawing-multiple-instances-of-geometry
			new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1),
			new VertexElement(sizeof(float) * 4, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 2),
			new VertexElement(sizeof(float) * 8, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 3),
			new VertexElement(sizeof(float) * 12, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 4)
		);

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct VertexShaderInstanceMatrix
		{
			// ReSharper disable NotAccessedField.Local
			private Vector4 row1;
			private Vector4 row2;
			private Vector4 row3;
			private Vector4 row4;

			public VertexShaderInstanceMatrix(Matrix matrix)
			{
				row1 = new Vector4(matrix[0,0], matrix[0, 1], matrix[0, 2], matrix[0, 3]);
				row2 = new Vector4(matrix[1, 0], matrix[1, 1], matrix[1, 2], matrix[1, 3]);
				row3 = new Vector4(matrix[2, 0], matrix[2, 1], matrix[2, 2], matrix[2, 3]);
				row4 = new Vector4(matrix[3, 0], matrix[3, 1], matrix[3, 2], matrix[3, 3]);
			}
		}



		Effect effect;
		Effect shadedInstancedEffect;

        SamplerState sampler = new SamplerState()
        {
            AddressU = TextureAddressMode.Clamp,
            AddressV = TextureAddressMode.Clamp,
            AddressW = TextureAddressMode.Clamp,
            Filter = TextureFilter.Anisotropic,
            FilterMode = TextureFilterMode.Default,
            MaxMipLevel = 0,
            MaxAnisotropy = 0,

        };

		LightSource sunLight;
		float angleRotation = 70;
		public RenderingSystem3D(GameSettings settings, NamelessGame game)
		{
			Signature = new HashSet<Type>
			{
				typeof(Drawable),
				typeof(Position)
			};

			sunLight = new LightSource(game.GraphicsDevice, 2048, 2048, new Vector3(10, -10f, 5f), new Vector3(1, 0, 0), new Vector4(1, 1, 1, 1));
		}

		void CalculateSun(float angle, Vector3 landCentre)
		{
			float offset = 3;
			sunLight.Position = new Vector3(LandCenter.X + (MathF.Cos(angle)* offset), LandCenter.Y, LandCenter.Z + (MathF.Sin(angle)* offset));
			sunLight.LookAtPoint = LandCenter;
			sunLight.RecalculateMatrix();
		}


        class ModelInstance
        {
            public string modelId;
            public Matrix position;
        }
        //for test
		List<ModelInstance> objectsToDraw = new List<ModelInstance>();
        bool once = true;

		VertexBuffer hackBuffer;
		IndexBuffer hackBufferIndices;
		private void CreateshadowMapEdgeHackMesh(NamelessGame game)
		{
			List<Vector3> points = new List<Vector3>();
			var size = 10000f;
			points.Add(new Vector3(-size, -size, 0));
			points.Add(new Vector3(size, -size, 0));
			points.Add(new Vector3(size, size, 0));
			points.Add(new Vector3(-size, size, 0));

			List<int> indices = new List<int>() {0,1,2,2,3,1 };

			Queue<Vertex3D> vertices = new Queue<Vertex3D>();
			foreach (var point in points)
			{
				var vertex = new Systems.Ingame.Vertex3D(point,
				new Vector4(0, 0, 0, 1),
				new Vector4(0, 0, 0, 1), new Vector2(0, 0), Vector3.UnitZ);
				vertices.Enqueue(vertex);
			}
			hackBuffer = new Microsoft.Xna.Framework.Graphics.VertexBuffer(game.GraphicsDevice, RenderingSystem3D.VertexDeclaration, vertices.Count, Microsoft.Xna.Framework.Graphics.BufferUsage.None);
			hackBuffer.SetData<Vertex3D>(vertices.ToArray());
			hackBufferIndices = new Microsoft.Xna.Framework.Graphics.IndexBuffer(game.GraphicsDevice, Microsoft.Xna.Framework.Graphics.IndexElementSize.ThirtyTwoBits, indices.Count, Microsoft.Xna.Framework.Graphics.BufferUsage.None);
			hackBufferIndices.SetData<int>(indices.ToArray());
		}
		Vector3 LandCenter;
		public override void Update(GameTime gameTime, NamelessGame game)
        {
			game.GraphicsDevice.BlendState = BlendState.Opaque;
            game.GraphicsDevice.SamplerStates[0] = sampler;
            game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;


			IEntity worldEntity = game.TimelineEntity;
			IWorldProvider worldProvider = null;
			if (worldEntity != null)
			{
				worldProvider = worldEntity.GetComponentOfType<TimeLine>().CurrentTimelineLayer.Chunks;
            }
			if (once)
			{
				effect = game.Content.Load<Effect>("ChunkShader3D");
				shadedInstancedEffect = game.Content.Load<Effect>("ObjectsShader");
				objectsToDraw = GetWorldObjectsToDraw(new Point(300, 300), worldProvider);
				CreateshadowMapEdgeHackMesh(game);
				SetupDebugDraw(game.GraphicsDevice);
				once = false;
				var chunkGeometries = game.ChunkGeometryEntiry.GetComponentOfType<Chunk3dGeometryHolder>();
				var landBounds = Microsoft.Xna.Framework.BoundingBox.CreateFromPoints(chunkGeometries.ChunkGeometries.Values.Select(x => (x.Bounds.Max + x.Bounds.Min) / 2));
				LandCenter = (landBounds.Max + landBounds.Min) / 2;
			}

			//angleRotation += 0.01f;
			if (angleRotation > 360) angleRotation -= 360;
			CalculateSun(angleRotation, LandCenter);

			Camera3D camera = game.PlayerEntity.GetComponentOfType<Camera3D>();
			void _setParameters(Effect shader)
			{
				shader.Parameters["xViewProjection"].SetValue(camera.View * camera.Projection);
				shader.Parameters["xWorldViewProjection"].SetValue(camera.View * camera.Projection);
				shader.Parameters["xView"].SetValue(camera.View);
				shader.Parameters["xProjection"].SetValue(camera.Projection);
				shader.Parameters["CameraPosition"].SetValue(camera.Position);
				shader.Parameters["xWorldMatrix"].SetValue(Matrix.Identity);
				shader.Parameters["xLightPos"].SetValue(sunLight.Position);
				shader.Parameters["xLightPower"].SetValue(1f);
				shader.Parameters["xAmbient"].SetValue(0.2f);
				shader.Parameters["xLightsWorldViewProjection"].SetValue(sunLight.LightsViewProjectionMatrix);
			}

			_setParameters(effect);
			_setParameters(shadedInstancedEffect);
			shadedInstancedEffect.Parameters["xLightPower"].SetValue(1f);
			shadedInstancedEffect.Parameters["xAmbient"].SetValue(0.7f);
			var device = game.GraphicsDevice;
			device.SetRenderTarget(sunLight.ShadowMapRenderTarget);
			device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Microsoft.Xna.Framework.Color.Black, 1.0f, 0);

			effect.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
			effect.GraphicsDevice.BlendState = BlendState.AlphaBlend;

			//fighting shadow map imprecision with mad skillz, to remove artifacts on terrain border
			RenderHackBufferToShadowMap(game);

			RenderChunksToShadowMap(game);
			RenderObjectsToShadowMap(game);
		
			device.SetRenderTarget(null);
			var shadowMap = (Texture2D)sunLight.ShadowMapRenderTarget;

			effect.Parameters["shadowMap"].SetValue(shadowMap);
			shadedInstancedEffect.Parameters["shadowMap"].SetValue(shadowMap);

			RenderChunksWithShadows(game);
			//RenderObjectsWithShadow(game);
			RenderTestPlayer(game, camera);

			var matrix = Matrix.CreateTranslation(sunLight.Position);
			effect.Parameters["xWorldViewProjection"].SetValue(matrix * camera.View * camera.Projection);
			RenderDebug(game);
			//RenderObjects(game);

			//effect.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
			//effect.GraphicsDevice.BlendState = BlendState.AlphaBlend;

			DrawDebugAxis(device,camera);

			//using (SpriteBatch sprite = new SpriteBatch(device))
			//{
			//	sprite.Begin();
			//	sprite.Draw(shadowMap, new Vector2(0, 0), null, Microsoft.Xna.Framework.Color.White, 0, new Vector2(0, 0), 0.2f, SpriteEffects.None, 1);
			//	sprite.End();
			//}

		

		}

		DebugDraw debugDraw;
		protected void SetupDebugDraw(GraphicsDevice device)
		{
			debugDraw = new DebugDraw(device);
		}

		void DrawDebugAxis(GraphicsDevice device, Camera3D camera)
		{
			debugDraw.Begin(camera.View, camera.Projection);

			BoundingFrustum frustum = new BoundingFrustum(sunLight.LightsViewProjectionMatrix);

			debugDraw.DrawWireFrustum(frustum, Color.Red);

			debugDraw.End();


		}
		private List<ModelInstance> GetWorldObjectsToDraw(Point positon, IWorldProvider world)
		{
			var postionOffsetX = positon.X * Constants.ChunkSize;
			var postionOffsetY = positon.Y * Constants.ChunkSize;
			var objects = new List<ModelInstance>();

			for (int x = 0; x < 100 * Constants.ChunkSize; x++)
			{
				for (int y = 0; y < 100 * Constants.ChunkSize; y++)
				{
					Tile tileToDraw = world.GetTile(x + postionOffsetX, y + postionOffsetY);
					foreach (var entity in tileToDraw.GetEntities())
					{
						var furniture = entity.GetComponentOfType<Furniture>();
						var drawable = entity.GetComponentOfType<Drawable>();
						if (furniture != null && drawable != null)
						{
							if (drawable.Representation == 'T')
							{
								//var modelShift = Matrix.CreateTranslation(0.5f, 0.5f, 2f);
								//Matrix.CreateTranslation(Constants.ScaleDownCoeficient * x, Constants.ScaleDownCoeficient * y, (float)tileToDraw.Elevation, out Matrix pos);
								objects.Add(new ModelInstance() { modelId = "smallTree", position = Constants.ScaleDownMatrix * Matrix.CreateTranslation(x * Constants.ScaleDownCoeficient, y * Constants.ScaleDownCoeficient, tileToDraw.ElevationVisual * Constants.ScaleDownCoeficient) });
								//return objects;s
							}
							if (drawable.Representation == 't')
							{

								//Matrix.CreateTranslation(Constants.ScaleDownCoeficient * x, Constants.ScaleDownCoeficient * y, (float)tileToDraw.Elevation, out Matrix pos);
								objects.Add(new ModelInstance() { modelId = "smallTree", position = Constants.ScaleDownMatrix * Matrix.CreateTranslation(x * Constants.ScaleDownCoeficient, y * Constants.ScaleDownCoeficient, tileToDraw.ElevationVisual * Constants.ScaleDownCoeficient) });
								//return objects;
							}
						}
					}
				}
			}

			return objects;
		}

		private void RenderDebug(NamelessGame game)
		{
			var device = game.GraphicsDevice;
			var chunkGeometries = game.ChunkGeometryEntiry.GetComponentOfType<Chunk3dGeometryHolder>();
			effect.CurrentTechnique = effect.Techniques["ColorTech"];
			var geometry = ModelsLibrary.Models["cube"];

			foreach (EffectPass pass in effect.CurrentTechnique.Passes)
			{
				pass.Apply();

				device.SetVertexBuffer(geometry.Buffer);
				device.Indices = geometry.IndexBuffer;

				device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, geometry.TriangleCount);
			}
		}

		private void RenderChunks(NamelessGame game)
        {
			var device = game.GraphicsDevice;
            var chunkGeometries = game.ChunkGeometryEntiry.GetComponentOfType<Chunk3dGeometryHolder>();
			effect.CurrentTechnique = effect.Techniques["ColorTech"];
			foreach (var geometry in chunkGeometries.ChunkGeometries.Values)
			{
				foreach (EffectPass pass in effect.CurrentTechnique.Passes)
				{
					pass.Apply();

					device.SetVertexBuffer(geometry.Buffer);
					device.Indices = geometry.IndexBuffer;

					device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, geometry.TriangleCount);
				}
			}
        }

		private void RenderChunksWithShadows(NamelessGame game)
		{
			var device = game.GraphicsDevice;
			var chunkGeometries = game.ChunkGeometryEntiry.GetComponentOfType<Chunk3dGeometryHolder>();
			effect.CurrentTechnique = effect.Techniques["ColorTechShadowMap"];
			foreach (var geometry in chunkGeometries.ChunkGeometries.Values)
			{
				EffectPass pass = effect.CurrentTechnique.Passes[1];
				pass.Apply();

				device.SetVertexBuffer(geometry.Buffer);
				device.Indices = geometry.IndexBuffer;

				device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, geometry.TriangleCount);
			}
		}

		private void RenderChunksToShadowMap(NamelessGame game)
		{
			var device = game.GraphicsDevice;
			var chunkGeometries = game.ChunkGeometryEntiry.GetComponentOfType<Chunk3dGeometryHolder>();
			effect.CurrentTechnique = effect.Techniques["ColorTechShadowMap"];
			foreach (var geometry in chunkGeometries.ChunkGeometries.Values)
			{
				EffectPass pass = effect.CurrentTechnique.Passes[0];
				pass.Apply();

				device.SetVertexBuffer(geometry.Buffer);
				device.Indices = geometry.IndexBuffer;

				device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, geometry.TriangleCount);
			}
		}

		private void RenderHackBufferToShadowMap(NamelessGame game)
		{
			var device = game.GraphicsDevice;
			var chunkGeometries = game.ChunkGeometryEntiry.GetComponentOfType<Chunk3dGeometryHolder>();
			effect.CurrentTechnique = effect.Techniques["ColorTechShadowMap"];
			EffectPass pass = effect.CurrentTechnique.Passes[0];
			pass.Apply();

			device.SetVertexBuffer(hackBuffer);
			device.Indices = hackBufferIndices;

			device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 2);

		}

		Dictionary<string, int> cacheCheckDictionary = new Dictionary<string, int>();
		Dictionary<string, VertexBuffer> instanceBufferCache = new Dictionary<string, VertexBuffer>();
		private void RenderObjects(NamelessGame game)
		{
					
			effect.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
			effect.GraphicsDevice.BlendState = BlendState.AlphaBlend;

			var device = game.GraphicsDevice;
			var chunkGeometries = game.ChunkGeometryEntiry.GetComponentOfType<Chunk3dGeometryHolder>();
			effect.CurrentTechnique = effect.Techniques["ColorTechInstanced"];

			//avoid setting the same buffer a million times, only transform is changed
			var objectGroups = objectsToDraw.GroupBy(x => x.modelId);

			foreach (var group in objectGroups)
			{
				VertexBuffer instanceBuffer;
				var groupCount = group.Count();
				if (cacheCheckDictionary.TryGetValue(group.Key, out int numberOfInstances) && numberOfInstances == groupCount)
				{
					instanceBuffer = instanceBufferCache[group.Key];
				}
				else
				{
					var instanceTransforms = new List<VertexShaderInstanceMatrix>(group.Count());
					//create insatnce data buffer
					foreach (var gameObject in objectsToDraw)
					{
						instanceTransforms.Add(new VertexShaderInstanceMatrix(gameObject.position));
					}
					instanceBuffer = new VertexBuffer(device, VertexShaderInstanceInput, instanceTransforms.Count, BufferUsage.WriteOnly);
					instanceBuffer.SetData(instanceTransforms.ToArray());
					cacheCheckDictionary[group.Key] = groupCount;
					instanceBufferCache[group.Key] = instanceBuffer;
				}

				var geometry = ModelsLibrary.Models[group.First().modelId];

				var bindings = new VertexBufferBinding[2];
				bindings[0] = new VertexBufferBinding(geometry.Buffer);
				bindings[1] = new VertexBufferBinding(instanceBuffer,0,1);
				device.SetVertexBuffers(bindings);
				device.Indices = geometry.IndexBuffer;
				foreach (EffectPass pass in effect.CurrentTechnique.Passes)
				{
					pass.Apply();
					device.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, geometry.TriangleCount, groupCount);
				}
			}
		}

		private void RenderTestPlayer(NamelessGame game, Camera3D camera)
		{

			var device = game.GraphicsDevice;
			var chunkGeometries = game.ChunkGeometryEntiry.GetComponentOfType<Chunk3dGeometryHolder>();
			effect.CurrentTechnique = effect.Techniques["ColorTechShadowMap"];
			//foreach (var geometry in chunkGeometries.ChunkGeometries.Values)
			//{

			Geometry3D geometry = ModelsLibrary.Models["smallTree"];
			var offset = Constants.ChunkSize * 290;
			var player = game.PlayerEntity; 
			var p = player.GetComponentOfType<Position>().Point;
			var position = new Point(p.X - offset, p.Y - offset);
			var tileToDraw = game.WorldProvider.GetTile(p.X, p.Y);
			//Matrix.CreateTranslation(Constants.ScaleDownCoeficient * position.X, Constants.ScaleDownCoeficient * position.Y, (float)tileToDraw.Elevation, out Matrix pos);
			var world = Constants.ScaleDownMatrix * Matrix.CreateTranslation(position.X * Constants.ScaleDownCoeficient, position.Y * Constants.ScaleDownCoeficient, tileToDraw.ElevationVisual * Constants.ScaleDownCoeficient);
			effect.Parameters["xWorldViewProjection"].SetValue(world * camera.View * camera.Projection);

			EffectPass pass = effect.CurrentTechnique.Passes[1];
			pass.Apply();

			device.SetVertexBuffer(geometry.Buffer);
			device.Indices = geometry.IndexBuffer;

			device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, geometry.TriangleCount);
			//}
		}

		private void RenderObjectsToShadowMap(NamelessGame game)
		{

			shadedInstancedEffect.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
			shadedInstancedEffect.GraphicsDevice.BlendState = BlendState.AlphaBlend;

			var device = game.GraphicsDevice;
			var chunkGeometries = game.ChunkGeometryEntiry.GetComponentOfType<Chunk3dGeometryHolder>();
			shadedInstancedEffect.CurrentTechnique = shadedInstancedEffect.Techniques["ColorTechShadowMapInstanced"];

			//avoid setting the same buffer a million times, only transform is changed
			var objectGroups = objectsToDraw.GroupBy(x => x.modelId);

			foreach (var group in objectGroups)
			{
				VertexBuffer instanceBuffer;
				var groupCount = group.Count();
				if (cacheCheckDictionary.TryGetValue(group.Key, out int numberOfInstances) && numberOfInstances == groupCount)
				{
					instanceBuffer = instanceBufferCache[group.Key];
				}
				else
				{
					var instanceTransforms = new List<VertexShaderInstanceMatrix>(group.Count());
					//create insatnce data buffer
					foreach (var gameObject in objectsToDraw)
					{
						instanceTransforms.Add(new VertexShaderInstanceMatrix(gameObject.position));
					}
					instanceBuffer = new VertexBuffer(device, VertexShaderInstanceInput, instanceTransforms.Count, BufferUsage.WriteOnly);
					instanceBuffer.SetData(instanceTransforms.ToArray());
					cacheCheckDictionary[group.Key] = groupCount;
					instanceBufferCache[group.Key] = instanceBuffer;
				}

				var geometry = ModelsLibrary.Models[group.First().modelId];

				var bindings = new VertexBufferBinding[2];
				bindings[0] = new VertexBufferBinding(geometry.Buffer);
				bindings[1] = new VertexBufferBinding(instanceBuffer, 0, 1);
				device.SetVertexBuffers(bindings);
				device.Indices = geometry.IndexBuffer;
				var pass = shadedInstancedEffect.CurrentTechnique.Passes[0];
				pass.Apply();
				device.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, geometry.TriangleCount, groupCount);

			}
		}
		//TODO: remove copypaste
		private void RenderObjectsWithShadow(NamelessGame game)
		{

			shadedInstancedEffect.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
			shadedInstancedEffect.GraphicsDevice.BlendState = BlendState.AlphaBlend;

			var device = game.GraphicsDevice;
			var chunkGeometries = game.ChunkGeometryEntiry.GetComponentOfType<Chunk3dGeometryHolder>();
			shadedInstancedEffect.CurrentTechnique = shadedInstancedEffect.Techniques["ColorTechShadowMapInstanced"];

			//avoid setting the same buffer a million times, only transform is changed
			var objectGroups = objectsToDraw.GroupBy(x => x.modelId);

			foreach (var group in objectGroups)
			{
				VertexBuffer instanceBuffer;
				var groupCount = group.Count();
				if (cacheCheckDictionary.TryGetValue(group.Key, out int numberOfInstances) && numberOfInstances == groupCount)
				{
					instanceBuffer = instanceBufferCache[group.Key];
				}
				else
				{
					var instanceTransforms = new List<VertexShaderInstanceMatrix>(group.Count());
					//create insatnce data buffer
					foreach (var gameObject in objectsToDraw)
					{
						instanceTransforms.Add(new VertexShaderInstanceMatrix(gameObject.position));
					}
					instanceBuffer = new VertexBuffer(device, VertexShaderInstanceInput, instanceTransforms.Count, BufferUsage.WriteOnly);
					instanceBuffer.SetData(instanceTransforms.ToArray());
					cacheCheckDictionary[group.Key] = groupCount;
					instanceBufferCache[group.Key] = instanceBuffer;
				}

				var geometry = ModelsLibrary.Models[group.First().modelId];

				var bindings = new VertexBufferBinding[2];
				bindings[0] = new VertexBufferBinding(geometry.Buffer);
				bindings[1] = new VertexBufferBinding(instanceBuffer, 0, 1);
				device.SetVertexBuffers(bindings);
				device.Indices = geometry.IndexBuffer;
				var pass = shadedInstancedEffect.CurrentTechnique.Passes[1];
				pass.Apply();
				device.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, geometry.TriangleCount, groupCount);

			}
		}

		#region debug
		RasterizerState ApplyWireframe(GraphicsDevice device)
		{
			var oldState = device.RasterizerState;
			RasterizerState rasterizerState = new RasterizerState();
			rasterizerState.FillMode = FillMode.WireFrame;
			device.RasterizerState = rasterizerState;
			return oldState;
		}

		void RestoreState(GraphicsDevice device, RasterizerState oldstate)
		{
			device.RasterizerState = oldstate;
		}
		#endregion
    }
}
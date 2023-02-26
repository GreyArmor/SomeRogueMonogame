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
using Color = NamelessRogue.Engine.Utility.Color;
using System.Diagnostics;
using NamelessRogue.Engine.Components._3D;
using System.Reflection.Metadata;
using System.Windows.Forms;
using AStarNavigator;
using Tile = NamelessRogue.Engine.Components.ChunksAndTiles.Tile;
using NamelessRogue.Engine.Systems._3DView;
using VertexBuffer = Microsoft.Xna.Framework.Graphics.VertexBuffer;
using System.Runtime.InteropServices;

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

        public RenderingSystem3D(GameSettings settings, NamelessGame game)
        {
			Signature = new HashSet<Type>
			{
				typeof(Drawable),
				typeof(Position)
			};

			sunLight = new LightSource(game.GraphicsDevice, 2000, 2000, new Vector3(-1f, -1f,  2f), new Vector3(1, 1 , 0), new Vector4(1, 1, 1, 1));


		}

        class ModelInstance
        {
            public string modelId;
            public Matrix position;
        }
        //for test
		List<ModelInstance> objectsToDraw = new List<ModelInstance>();
        bool once = true;
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
				objectsToDraw = GetWorldObjectsToDraw(new Point(300, 300), worldProvider);
				once = false;
            }

			Camera3D camera = game.PlayerEntity.GetComponentOfType<Camera3D>();
			effect.Parameters["xViewProjection"].SetValue(camera.View * camera.Projection);
			effect.Parameters["xWorldViewProjection"].SetValue(camera.View * camera.Projection);
			effect.Parameters["xView"].SetValue(camera.View);
			effect.Parameters["xProjection"].SetValue(camera.Projection);
			effect.Parameters["CameraPosition"].SetValue(camera.Position);
			effect.Parameters["xWorldMatrix"].SetValue(Matrix.Identity);


			effect.Parameters["xLightPos"].SetValue(sunLight.Position);
			effect.Parameters["xLightPower"].SetValue(1f);
			effect.Parameters["xAmbient"].SetValue(0.2f);
			effect.Parameters["xLightsWorldViewProjection"].SetValue(sunLight.LightsViewProjectionMatrix);
		

			var device = game.GraphicsDevice;
			device.SetRenderTarget(sunLight.ShadowMapRenderTarget);
			device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Microsoft.Xna.Framework.Color.Black, 1.0f, 0);

			effect.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
			effect.GraphicsDevice.BlendState = BlendState.AlphaBlend;


			RenderChunksToShadowMap(game);

			device.SetRenderTarget(null);
			var shadowMap = (Texture2D)sunLight.ShadowMapRenderTarget;

			effect.Parameters["shadowMap"].SetValue(shadowMap);

			//device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);
			//debug
			
			//using (SpriteBatch sprite = new SpriteBatch(device))
			//{
			//	sprite.Begin();
			//	sprite.Draw(shadowMap, new Vector2(0, 0), null, Microsoft.Xna.Framework.Color.White, 0, new Vector2(0, 0), 0.4f, SpriteEffects.None, 1);
			//	sprite.End();
			//}

			RenderChunksWithShadows(game);

			//RenderChunks(game);
			RenderObjects(game);
		}

        private List<ModelInstance> GetWorldObjectsToDraw(Point positon, IWorldProvider world)
        {
            var postionOffsetX = positon.X * Constants.ChunkSize;
			var postionOffsetY = positon.Y * Constants.ChunkSize;
            var objects = new List<ModelInstance>();
			for (int x = 0; x < 100*Constants.ChunkSize; x++)
            {
                for (int y = 0; y < 100* Constants.ChunkSize; y++)
                {
					Tile tileToDraw = world.GetTile(x+ postionOffsetX, y+ postionOffsetY);
					foreach (var entity in tileToDraw.GetEntities())
					{
						var furniture = entity.GetComponentOfType<Furniture>();
						var drawable = entity.GetComponentOfType<Drawable>();
						if (furniture != null && drawable != null)
						{
							if (drawable.Representation == 'T')
							{
								//var modelShift = Matrix.CreateTranslation(0.5f, 0.5f, 2f);
								Matrix.CreateTranslation(Constants.ScaleDownCoeficient * x, Constants.ScaleDownCoeficient * y, (float)tileToDraw.Elevation, out Matrix pos);
								objects.Add(new ModelInstance() { modelId = "smallTree", position = Constants.ScaleDownMatrix * Matrix.CreateTranslation(x * Constants.ScaleDownCoeficient, y * Constants.ScaleDownCoeficient, tileToDraw.ElevationVisual * Constants.ScaleDownCoeficient) });
								//return objects;s
							}
							if (drawable.Representation == 't')
							{
								
								Matrix.CreateTranslation(Constants.ScaleDownCoeficient * x, Constants.ScaleDownCoeficient * y, (float)tileToDraw.Elevation, out Matrix pos);
								objects.Add(new ModelInstance() { modelId = "smallTree", position = Constants.ScaleDownMatrix * Matrix.CreateTranslation(x * Constants.ScaleDownCoeficient, y * Constants.ScaleDownCoeficient, tileToDraw.ElevationVisual * Constants.ScaleDownCoeficient) });
								//return objects;
							}
						}
					}
				}
            }
            return objects;
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
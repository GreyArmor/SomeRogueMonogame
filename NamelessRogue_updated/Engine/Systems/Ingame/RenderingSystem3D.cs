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

        public RenderingSystem3D(GameSettings settings)
        {
            Signature = new HashSet<Type>();
            Signature.Add(typeof(Drawable));
            Signature.Add(typeof(Position));
        }
        public override void Update(GameTime gameTime, NamelessGame game)
        {

            //this.gameTime = (long)gameTime.TotalGameTime.TotalMilliseconds;

            game.GraphicsDevice.BlendState = BlendState.Opaque;
            game.GraphicsDevice.SamplerStates[0] = sampler;
            game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            //todo move to constructor or some other place better suited for initialization
            if (tileAtlas == null)
            {
                InitializeTexture(game);
            }
            //var defautstate = game.GraphicsDevice.RasterizerState;
            //var rsterizer = new RasterizerState();

            Render(game);
        }


        static Microsoft.Xna.Framework.Vector3 sunColor = Microsoft.Xna.Framework.Color.White.ToVector3();
        static Microsoft.Xna.Framework.Color groundColor = Microsoft.Xna.Framework.Color.Black;
        static Vector3 downColor = groundColor.ToVector3();
        private void Render(NamelessGame game)
        {
            //effect.Parameters["tileAtlas"].SetValue(tileAtlas);
          
            Camera3D camera = game.PlayerEntity.GetComponentOfType<Camera3D>();
            effect.Parameters["xViewProjection"].SetValue(camera.View * camera.Projection);
            effect.Parameters["SunLightIntensity"].SetValue(1f);
            effect.Parameters["CameraPosition"].SetValue(camera.Position);
            effect.Parameters["SunLightDirection"].SetValue(new Vector3(0));
            effect.Parameters["SunLightColor"].SetValue(sunColor);

            effect.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            effect.GraphicsDevice.BlendState = BlendState.AlphaBlend;


			//RasterizerState originalState = game.GraphicsDevice.RasterizerState;

			//RasterizerState rasterizerState = new RasterizerState();
			//rasterizerState.FillMode = FillMode.WireFrame;
			//rasterizerState.DepthBias = 0.1f;
			//game.GraphicsDevice.RasterizerState = rasterizerState;




			var device = game.GraphicsDevice;
           var chunkGeometries = game.ChunkGeometryEntiry.GetComponentOfType<Chunk3dGeometryHolder>();
			effect.CurrentTechnique = effect.Techniques["ColorTech"];
			foreach (var geometry in chunkGeometries.ChunkGeometries.Values)
			{
				if (geometry.TriangleCount > 0)
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



			//effect.CurrentTechnique = effect.Techniques["Wireframe"];
			//foreach (var geometry in chunkGeometries.ChunkGeometries.Values)
			//{
			//	if (geometry.TriangleCount > 0)
			//	{
			//		foreach (EffectPass pass in effect.CurrentTechnique.Passes)
			//		{
			//			pass.Apply();

			//			device.SetVertexBuffer(geometry.Buffer);
			//			device.Indices = geometry.IndexBuffer;

			//			device.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, geometry.TriangleCount);
			//		}
			//	}
			//}

			//game.GraphicsDevice.RasterizerState = originalState;

        }

        class AtlasTileData
        {
            public int X;
            public int Y;

            public AtlasTileData(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        Texture2D tileAtlas = null;



        private Texture InitializeTexture(NamelessGame game)
        {

            tileAtlas = null;
            tileAtlas = game.Content.Load<Texture2D>("SideTexture");
            effect = game.Content.Load<Effect>("ChunkShader3D");

            //effect.Parameters["tileAtlas"].SetValue(tileAtlas);
            return tileAtlas;
        }
    }
}
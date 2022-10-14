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
            new VertexElement(sizeof(float) * 11, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
        );

        Dictionary<char, AtlasTileData> characterToTileDictionary;
        private float gameTime;
        private float angle = 0;
        private float step = 0.04f;
        private InternalRandom graphicalRandom = new InternalRandom();
        Effect effect;
        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;

        SamplerState sampler = new SamplerState()
        {
            AddressU = TextureAddressMode.Clamp,
            AddressV = TextureAddressMode.Clamp,
            AddressW = TextureAddressMode.Clamp,
            Filter = TextureFilter.Point,
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

            this.gameTime = (long)gameTime.TotalGameTime.TotalMilliseconds;

            game.GraphicsDevice.BlendState = BlendState.Opaque;
            game.GraphicsDevice.SamplerStates[0] = sampler;
            game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            //todo move to constructor or some other place better suited for initialization
            if (tileAtlas == null)
            {
                InitializeTexture(game);
            }

            Render(game);
        }


 
        private void Render(NamelessGame game)
        {
            effect.Parameters["tileAtlas"].SetValue(tileAtlas);
            Camera3D camera = game.PlayerEntity.GetComponentOfType<Camera3D>();

            effect.Parameters["xViewProjection"].SetValue(camera.View * camera.Projection);

            effect.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            effect.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            var device = game.GraphicsDevice;

            effect.CurrentTechnique = effect.Techniques["Point"];

            var chunkGeometries = game.ChunkGeometryEntiry.GetComponentOfType<Chunk3dGeometryHolder>();
            foreach (var geometry in chunkGeometries.ChunkGeometries.Values)
            {
                if (geometry.Positions.Any())
                {
                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();

                        device.SetVertexBuffer(geometry.Buffer);
                        device.Indices = geometry.IndexBuffer;

                        device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, geometry.Positions.Count-1 / 3);                  }
                }
            }
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
            tileAtlas = game.Content.Load<Texture2D>("DFfont");
            effect = game.Content.Load<Effect>("ChunkShader3D");

            effect.Parameters["tileAtlas"].SetValue(tileAtlas);
            return tileAtlas;
        }
    }
}
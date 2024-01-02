using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components;
using NamelessRogue.Engine.Components._3D;
using NamelessRogue.Engine.Components.Environment;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Systems._3DView;
using NamelessRogue.shell;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using Effect = NamelessRogue.Engine.Infrastructure.Effect;
using Filter = SharpDX.Direct3D11.Filter;
using InputElement = SharpDX.Direct3D11.InputElement;
using Point = SharpDX.Point;
using Tile = NamelessRogue.Engine.Components.ChunksAndTiles.Tile;

namespace NamelessRogue.Engine.Systems.Ingame
{
    public class RenderingSystem3D : BaseSystem
    {

        [StructLayout(LayoutKind.Sequential)]
        internal struct CommonConstantBuffer
        {
            public Vector3 CameraPosition;
            public Vector3 CameraUp;
            public Vector3 CameraRight;
            public Texture2D tileAtlas;
            public Matrix xView;
            public Matrix xProjection;
            public Matrix xViewProjection;
            public Matrix xWorldViewProjection;
            public Matrix xWorldMatrix;
            public Matrix xBillboard;
            public Vector2 windCoef;
            public bool windFlag;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct TerrainConstantBuffer
        {
            public Vector3 CameraPosition;
            public Vector3 CameraUp;
            public Vector3 CameraRight;
            public Matrix xView;
            public Matrix xProjection;
            public Matrix xViewProjection;
            public Matrix xWorldViewProjection;
            public Matrix xWorldMatrix;
            public Matrix xBillboard;
            public Vector2 windCoef;
            public bool windFlag;
            public Matrix xLightsWorldViewProjection;
            public Vector3 xLightPos;
            public float xLightPower;
            public float xAmbient;
            internal int substractionCoef;
            internal int rowIndexEnd;
            internal int verticesPerRow;
        }

        public override HashSet<Type> Signature { get; }

        //public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
        //(
        //	new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
        //	new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector4, VertexElementUsage.Color, 0),
        //	new VertexElement(sizeof(float) * 7, VertexElementFormat.Vector4, VertexElementUsage.Color, 1),
        //	new VertexElement(sizeof(float) * 11, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
        //	new VertexElement(sizeof(float) * 13, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
        //);

        public static int VertexDeclarationSize = 15;
        public readonly InputElement[] VertexDeclaration = new InputElement[] {
              new InputElement("POSITION", 0, Format.R32G32B32_Float, 0),
              new InputElement("COLOR", 0, Format.R32G32B32A32_Float, sizeof(float) * 3, 0),
              new InputElement("COLOR", 0, Format.R32G32B32A32_Float, sizeof(float) * 7, 1),
              new InputElement("TEXCOORD", 0, Format.R32G32_Float, sizeof(float) * 11, 0),
              new InputElement("NORMAL", 0, Format.R32G32B32_Float, sizeof(float) * 13, 0),
        };

        public static int TerrainVertexDeclarationSize = 3;
        public readonly InputElement[] TerrainVertexDeclaration = new InputElement[] {
              new InputElement("POSITION", 0, Format.R32G32B32_Float, 0),
        };

        public static readonly InputElement[] VertexShaderInstanceInput = new InputElement[]
            {
        
			//this is 4x4 matrix, transferred as instructed in https://learn.microsoft.com/en-us/windows/win32/direct3d9/efficiently-drawing-multiple-instances-of-geometry
			new InputElement("TEXCOORD", 0, Format.R32G32B32A32_Float, 0),
            new InputElement("TEXCOORD", 0, Format.R32G32B32A32_Float, sizeof(float)* 4, 2),
            new InputElement("TEXCOORD", 0, Format.R32G32B32A32_Float, sizeof(float)* 8, 3),
            new InputElement("TEXCOORD", 0, Format.R32G32B32A32_Float, sizeof(float)* 12, 4),
        };

        public static readonly InputElement[] VertexShaderBillboardInstanceDataInput = new InputElement[]
        {
            new InputElement("TEXCOORD", 0, Format.R32G32B32A32_Float, 1)
        };


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
                row1 = new Vector4(matrix[0, 0], matrix[0, 1], matrix[0, 2], matrix[0, 3]);
                row2 = new Vector4(matrix[1, 0], matrix[1, 1], matrix[1, 2], matrix[1, 3]);
                row3 = new Vector4(matrix[2, 0], matrix[2, 1], matrix[2, 2], matrix[2, 3]);
                row4 = new Vector4(matrix[3, 0], matrix[3, 1], matrix[3, 2], matrix[3, 3]);
            }
        }


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct VertexShaderBillboardInstanceData
        {
            // ReSharper disable NotAccessedField.Local
            public Vector3 position;

            public VertexShaderBillboardInstanceData(Vector3 pos)
            {
                position = new Vector3(pos.X, pos.Y, pos.Z);
            }
        }



        Effect shadowMapStageShader;
        Effect shadowMapSceneShader;

        Effect shadedInstancedEffect;

        SamplerStateDescription ssDescription = new SamplerStateDescription()
        {
            AddressU = TextureAddressMode.Clamp,
            AddressV = TextureAddressMode.Clamp,
            AddressW = TextureAddressMode.Clamp,
            Filter = Filter.Anisotropic,
        };

        SamplerState sampler;

        LightSource sunLight;
        float angleRotation = 70;
        public RenderingSystem3D(GameSettings settings, NamelessGame game)
        {
            Signature = new HashSet<Type>
            {
                typeof(Drawable),
                typeof(Position)
            };

            sunLight = new LightSource(game, 2048, 2048, new Vector3(10, -10f, 5f), new Vector3(1, 0, 0), new Vector4(1, 1, 1, 1));
            sampler = new SamplerState(game.GraphicsDevice, ssDescription);
        }

        void CalculateSun(float angle, Vector3 landCentre)
        {
            float offset = 3;
            sunLight.Position = new Vector3(LandCenter.X + (MathF.Cos(angle) * offset), LandCenter.Y, LandCenter.Z + (MathF.Sin(angle) * offset));
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

        Buffer hackBuffer;
        Buffer hackBufferIndices;
        private void CreateshadowMapEdgeHackMesh(NamelessGame game)
        {
            List<Vector3> points = new List<Vector3>();
            var size = 10000f;
            points.Add(new Vector3(-size, -size, 0));
            points.Add(new Vector3(size, -size, 0));
            points.Add(new Vector3(size, size, 0));
            points.Add(new Vector3(-size, size, 0));

            List<int> indices = new List<int>() { 0, 1, 2, 2, 3, 1 };

            Queue<Vertex3D> vertices = new Queue<Vertex3D>();
            foreach (var point in points)
            {
                var vertex = new Systems.Ingame.Vertex3D(point,
                new Vector4(0, 0, 0, 1),
                new Vector4(0, 0, 0, 1), new Vector2(0, 0), Vector3.UnitZ);
                vertices.Enqueue(vertex);
            }
            hackBuffer = Buffer.Create(game.GraphicsDevice, BindFlags.VertexBuffer, vertices.ToArray());
            //hackBuffer.SetData<Vertex3D>(vertices.ToArray());
            hackBufferIndices = Buffer.Create(game.GraphicsDevice, BindFlags.IndexBuffer, indices.ToArray());
        }

        BlendState opaque = null;
        RasterizerState rasterizerState = null;
        DepthStencilState depthStencilState = null;
        Vector3 LandCenter;
        Buffer cbuffer = null;
        TerrainConstantBuffer constantBuffer = new TerrainConstantBuffer();
        Buffer CreateConstantBuffer(Device device)
        {
            return new SharpDX.Direct3D11.Buffer(device, Utilities.SizeOf<TerrainConstantBuffer>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
        }
        public override void Update(GameTime gameTime, NamelessGame game)
        {
            var device = game.Window.Device;
            if (cbuffer == null)
            {
                cbuffer = CreateConstantBuffer(game.Window.Device);
            }

            game.Window.DeviceContext.OutputMerger.BlendState = opaque;
            //	game.Context.OutputMerger.SamplerStates[0] = sampler;
            game.Window.DeviceContext.OutputMerger.DepthStencilState = depthStencilState;
            game.Window.DeviceContext.Rasterizer.State = rasterizerState;



            IEntity worldEntity = game.TimelineEntity;
            IWorldProvider worldProvider = null;
            if (worldEntity != null)
            {
                worldProvider = worldEntity.GetComponentOfType<TimeLine>().CurrentTimelineLayer.Chunks;
            }
            if (once)
            {
                shadowMapStageShader = new Effect(device, "ChunkShader3D", "TerrainShadowMapVertexShader", "ShadowMapPixelShader");
                shadowMapSceneShader = new Effect(device, "ChunkShader3D", "TerrainShadowedSceneVertexShader", "TerrainShadowedScenePixelShader");
             //   shadedInstancedEffect = new Effect("ObjectsShader", "", "");
                objectsToDraw = GetWorldObjectsToDraw(new Point(300, 300), worldProvider);
                CreateshadowMapEdgeHackMesh(game);
                //    SetupDebugDraw(game.GraphicsDevice);
                once = false;
                var chunkGeometries = game.ChunkGeometryEntiry.GetComponentOfType<Chunk3dGeometryHolder>();
                BoundingBox.FromPoints(chunkGeometries.ChunkGeometries.Values.Select(x => (x.Item1.Bounds.Maximum + x.Item1.Bounds.Minimum) / 2).ToArray(), out var landBounds);
                LandCenter = (landBounds.Maximum + landBounds.Minimum) / 2;
            }
            Camera3D camera = game.PlayerEntity.GetComponentOfType<Camera3D>();
            BoundingFrustum cameraFrustrum = new BoundingFrustum(camera.View * camera.Projection);

            //	angleRotation += 0.01f;
            if (angleRotation > 360) angleRotation -= 360;
            CalculateSun(angleRotation, LandCenter);     

            constantBuffer.xViewProjection = (camera.View * camera.Projection);
            constantBuffer.xWorldViewProjection = (camera.View * camera.Projection);
            constantBuffer.xView = (camera.View);
            constantBuffer.xProjection = (camera.Projection);
            constantBuffer.CameraPosition = (camera.Position);
            constantBuffer.xWorldMatrix = (Matrix.Identity);
            constantBuffer.xLightPos = (sunLight.Position);
            constantBuffer.xLightPower = (1f);
            constantBuffer.xAmbient = (0.2f);

            constantBuffer.substractionCoef = (1);
            constantBuffer.rowIndexEnd = (Constants.ChunkSize * 2) + 1;
            constantBuffer.verticesPerRow = (Constants.ChunkSize * 2) + 4;
            
            game.Window.DeviceContext.OutputMerger.SetRenderTargets(sunLight.ShadowMapRenderTargetView);
            // device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            //effect.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            // effect.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            game.Window.DeviceContext.UpdateSubresource(ref constantBuffer, cbuffer);

       
         

            var chunkGeometries1 = game.ChunkGeometryEntiry.GetComponentOfType<Chunk3dGeometryHolder>();
                 
            //fighting shadow map imprecision with mad skillz, to remove artifacts on terrain border


            //RenderHackBufferToShadowMap(game);

            RenderChunksToShadowMap(game, camera);
            //	RenderObjectsToShadowMap(game);

            game.Window.DeviceContext.OutputMerger.SetRenderTargets(game.Window.RenderTargetView);
            var shadowMap = sunLight.ShadowMapResourceView;

            game.Window.DeviceContext.PixelShader.SetShaderResource(1, shadowMap);
           // shadedInstancedEffect.Parameters["shadowMap"].SetValue(shadowMap);
          //  game.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            RenderChunksWithShadows(game, camera);
            //RenderObjectsWithShadow(game);

            // game.GraphicsDevice.RasterizerState = oldstate;

            var matrix = Matrix.Translation(sunLight.Position);

          //  constantBuffer.xWorldViewProjection = matrix * camera.View * camera.Projection;
         //  game.Window.DeviceContext.UpdateSubresource(ref constantBuffer, cbuffer);

       //     game.Window.DeviceContext.VertexShader.SetConstantBuffer().Parameters["xWorldViewProjection"].SetValue();



            //RenderDebug(game);

      //     DrawDebugAxis(device, camera);

            //using (SpriteBatch sprite = new SpriteBatch(device))
            //{
            //    sprite.Begin();
            //    sprite.Draw(shadowMap, new Vector2(0, 0), null, Color.White, 0, new Vector2(0, 0), 0.2f, SpriteEffects.None, 1);
            //    sprite.End();
            //}

        }

        //LineDrawer debugDraw;
        //protected void SetupDebugDraw(GraphicsDevice device)
        //{
        //	debugDraw = new LineDrawer(device);
        //}

        //void DrawDebugAxis(GraphicsDevice device, Camera3D camera)
        //{
        //	debugDraw.Begin(camera.View, camera.Projection);

        //	BoundingFrustum frustum = new BoundingFrustum(sunLight.LightsViewProjectionMatrix);

        //	debugDraw.DrawWireFrustum(frustum, Color.Red);

        //	debugDraw.End();


        //}
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
                                objects.Add(new ModelInstance() { modelId = "smallTree", position = Constants.ScaleDownMatrix * Matrix.Translation(x * Constants.ScaleDownCoeficient, y * Constants.ScaleDownCoeficient, tileToDraw.ElevationVisual * Constants.ScaleDownCoeficient) });
                                //return objects;s
                            }
                            if (drawable.Representation == 't')
                            {

                                //Matrix.CreateTranslation(Constants.ScaleDownCoeficient * x, Constants.ScaleDownCoeficient * y, (float)tileToDraw.Elevation, out Matrix pos);
                                objects.Add(new ModelInstance() { modelId = "smallTree", position = Constants.ScaleDownMatrix * Matrix.Translation(x * Constants.ScaleDownCoeficient, y * Constants.ScaleDownCoeficient, tileToDraw.ElevationVisual * Constants.ScaleDownCoeficient) });
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
            //var device = game.GraphicsDevice;
            //var chunkGeometries = game.ChunkGeometryEntiry.GetComponentOfType<Chunk3dGeometryHolder>();
            //effect.CurrentTechnique = effect.Techniques["ColorTech"];
            //var geometry = ModelsLibrary.Models["cube"];

            //foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            //{
            //    pass.Apply();

            //    device.SetVertexBuffer(geometry.Buffer);
            //    device.Indices = geometry.IndexBuffer;

            //    device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, geometry.TriangleCount);
            //}
        }

        private void RenderChunks(NamelessGame game)
        {
            //var device = game.GraphicsDevice;
            //var chunkGeometries = game.ChunkGeometryEntiry.GetComponentOfType<Chunk3dGeometryHolder>();
            //effect.CurrentTechnique = effect.Techniques["TerrrainTextureTechShadowMap"];
            //foreach (var geometryTuple in chunkGeometries.ChunkGeometries.Values)
            //{
            //    var geometry = geometryTuple.Item1;
            //    var terrainGeometry = geometryTuple.Item2;
            //    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            //    {
            //        pass.Apply();

            //        device.SetVertexBuffer(terrainGeometry.Buffer);
            //        device.Indices = geometry.IndexBuffer;

            //        device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, 0, geometry.TriangleCount);
            //    }
            //}
        }

        int terrainTriangleCount = ((Constants.ChunkSize * 2) + 4) * Constants.ChunkSize;
        private void RenderChunksToShadowMap(NamelessGame game, Camera3D camera)
        {
            shadowMapStageShader.Apply(game.Window.DeviceContext);
            game.Window.DeviceContext.VertexShader.SetConstantBuffer(0, cbuffer);
            game.Window.DeviceContext.PixelShader.SetConstantBuffer(0, cbuffer);

            var chunkGeometries = game.ChunkGeometryEntiry.GetComponentOfType<Chunk3dGeometryHolder>();
            game.Window.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
            foreach (var geometryTuple in chunkGeometries.ChunkGeometries.Values)
            {
                var geometry = geometryTuple.Item1;
                var terrainGeometry = geometryTuple.Item2;
                game.Window.DeviceContext.PixelShader.SetShaderResource(0, geometry.MaterialResourceView);

                constantBuffer.xWorldMatrix = (Constants.ScaleDownMatrix * terrainGeometry.WorldOffset);
                game.Window.DeviceContext.UpdateSubresource(ref constantBuffer, cbuffer);

                game.Window.DeviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(terrainGeometry.Buffer, TerrainVertexDeclarationSize, 0));
                game.Window.DeviceContext.Draw(terrainTriangleCount, 0);
            }
        }
        private void RenderChunksWithShadows(NamelessGame game, Camera3D camera)
        {
            shadowMapSceneShader.Apply(game.Window.DeviceContext);
            game.Window.DeviceContext.VertexShader.SetConstantBuffer(0, cbuffer);
            game.Window.DeviceContext.PixelShader.SetConstantBuffer(0, cbuffer);
            var chunkGeometries = game.ChunkGeometryEntiry.GetComponentOfType<Chunk3dGeometryHolder>();
            game.Window.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
            foreach (var geometryTuple in chunkGeometries.ChunkGeometries.Values)
            {
                var geometry = geometryTuple.Item1;
                var terrainGeometry = geometryTuple.Item2;
                game.Window.DeviceContext.PixelShader.SetShaderResource(0, geometry.MaterialResourceView);

                constantBuffer.xWorldMatrix = (Constants.ScaleDownMatrix * terrainGeometry.WorldOffset);
                game.Window.DeviceContext.UpdateSubresource(ref constantBuffer, cbuffer);

                game.Window.DeviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(terrainGeometry.Buffer, TerrainVertexDeclarationSize, 0));
                game.Window.DeviceContext.Draw(terrainTriangleCount, 0);
            }
        }

        private void RenderHackBufferToShadowMap(NamelessGame game)
        {
            //shadowMapStageShader.Apply(game.Window.DeviceContext);
            //var device = game.GraphicsDevice;
            //var chunkGeometries = game.ChunkGeometryEntiry.GetComponentOfType<Chunk3dGeometryHolder>();
            //effect.CurrentTechnique = effect.Techniques["ColorTechShadowMap"];
            //EffectPass pass = effect.CurrentTechnique.Passes[0];
            //pass.Apply();

            //device.SetVertexBuffer(hackBuffer);
            //device.Indices = hackBufferIndices;

            //device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 2);

        }

        Dictionary<string, int> cacheCheckDictionary = new Dictionary<string, int>();
        Dictionary<string, VertexBuffer> instanceBufferCache = new Dictionary<string, VertexBuffer>();
        private void RenderObjects(NamelessGame game)
        {

            //effect.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            //effect.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            //var device = game.GraphicsDevice;
            //effect.CurrentTechnique = effect.Techniques["ColorTechInstanced"];

            ////avoid setting the same buffer a million times, only transform is changed
            //var objectGroups = objectsToDraw.GroupBy(x => x.modelId);

            //foreach (var group in objectGroups)
            //{
            //    VertexBuffer instanceBuffer;
            //    var groupCount = group.Count();
            //    if (cacheCheckDictionary.TryGetValue(group.Key, out int numberOfInstances) && numberOfInstances == groupCount)
            //    {
            //        instanceBuffer = instanceBufferCache[group.Key];
            //    }
            //    else
            //    {
            //        var instanceTransforms = new List<VertexShaderInstanceMatrix>(group.Count());
            //        //create insatnce data buffer
            //        foreach (var gameObject in objectsToDraw)
            //        {
            //            instanceTransforms.Add(new VertexShaderInstanceMatrix(NamelessGameObject.position));
            //        }
            //        instanceBuffer = new VertexBuffer(device, VertexShaderInstanceInput, instanceTransforms.Count, BufferUsage.WriteOnly);
            //        instanceBuffer.SetData(instanceTransforms.ToArray());
            //        cacheCheckDictionary[group.Key] = groupCount;
            //        instanceBufferCache[group.Key] = instanceBuffer;
            //    }

            //    var geometry = ModelsLibrary.Models[group.First().modelId];

            //    var bindings = new VertexBufferBinding[2];
            //    bindings[0] = new VertexBufferBinding(geometry.Buffer);
            //    bindings[1] = new VertexBufferBinding(instanceBuffer, 0, 1);
            //    device.SetVertexBuffers(bindings);
            //    device.Indices = geometry.IndexBuffer;
            //    //Matrix.CreateBillboard
            //    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            //    {
            //        pass.Apply();
            //        device.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, geometry.TriangleCount, groupCount);
            //    }
            //}
        }

        private void RenderTestPlayer(NamelessGame game, Camera3D camera)
        {

            //var device = game.GraphicsDevice;
            //var chunkGeometries = game.ChunkGeometryEntiry.GetComponentOfType<Chunk3dGeometryHolder>();
            //effect.CurrentTechnique = effect.Techniques["ColorTechShadowMap"];
            ////foreach (var geometry in chunkGeometries.ChunkGeometries.Values)
            ////{

            //Geometry3D geometry = ModelsLibrary.Models["smallTree"];
            //var offset = Constants.ChunkSize * (300 - Constants.RealityBubbleRangeInChunks);
            //var player = game.PlayerEntity;
            //var p = player.GetComponentOfType<Position>().Point;
            //var position = new Point(p.X - offset, p.Y - offset);
            //var tileToDraw = game.WorldProvider.GetTile(p.X, p.Y);
            ////Matrix.CreateTranslation(Constants.ScaleDownCoeficient * position.X, Constants.ScaleDownCoeficient * position.Y, (float)tileToDraw.Elevation, out Matrix pos);
            //var world = Constants.ScaleDownMatrix * Matrix.CreateTranslation(position.X * Constants.ScaleDownCoeficient, position.Y * Constants.ScaleDownCoeficient, tileToDraw.ElevationVisual * Constants.ScaleDownCoeficient);
            //effect.Parameters["xWorldViewProjection"].SetValue(world * camera.View * camera.Projection);

            //EffectPass pass = effect.CurrentTechnique.Passes[1];
            //pass.Apply();

            //device.SetVertexBuffer(geometry.Buffer);
            //device.Indices = geometry.IndexBuffer;

            //device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, geometry.TriangleCount);
            ////}
        }

        private void RenderObjectsToShadowMap(NamelessGame game)
        {

            //shadedInstancedEffect.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            //shadedInstancedEffect.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            //var device = game.GraphicsDevice;
            //var chunkGeometries = game.ChunkGeometryEntiry.GetComponentOfType<Chunk3dGeometryHolder>();
            //shadedInstancedEffect.CurrentTechnique = shadedInstancedEffect.Techniques["ColorTechShadowMapInstanced"];

            ////avoid setting the same buffer a million times, only transform is changed
            //var objectGroups = objectsToDraw.GroupBy(x => x.modelId);

            //foreach (var group in objectGroups)
            //{
            //    VertexBuffer instanceBuffer;
            //    var groupCount = group.Count();
            //    if (cacheCheckDictionary.TryGetValue(group.Key, out int numberOfInstances) && numberOfInstances == groupCount)
            //    {
            //        instanceBuffer = instanceBufferCache[group.Key];
            //    }
            //    else
            //    {
            //        var instanceTransforms = new List<VertexShaderInstanceMatrix>(group.Count());
            //        //create insatnce data buffer
            //        foreach (var gameObject in objectsToDraw)
            //        {
            //            instanceTransforms.Add(new VertexShaderInstanceMatrix(NamelessGameObject.position));
            //        }
            //        instanceBuffer = new VertexBuffer(device, VertexShaderInstanceInput, instanceTransforms.Count, BufferUsage.WriteOnly);
            //        instanceBuffer.SetData(instanceTransforms.ToArray());
            //        cacheCheckDictionary[group.Key] = groupCount;
            //        instanceBufferCache[group.Key] = instanceBuffer;
            //    }

            //    var geometry = ModelsLibrary.Models[group.First().modelId];

            //    var bindings = new VertexBufferBinding[2];
            //    bindings[0] = new VertexBufferBinding(geometry.Buffer);
            //    bindings[1] = new VertexBufferBinding(instanceBuffer, 0, 1);
            //    device.SetVertexBuffers(bindings);
            //    device.Indices = geometry.IndexBuffer;
            //    var pass = shadedInstancedEffect.CurrentTechnique.Passes[0];
            //    pass.Apply();
            //    device.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, geometry.TriangleCount, groupCount);

            //}
        }
        //TODO: remove copypaste
        private void RenderObjectsWithShadow(NamelessGame game)
        {

            //shadedInstancedEffect.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            //shadedInstancedEffect.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            //var device = game.GraphicsDevice;
            //var chunkGeometries = game.ChunkGeometryEntiry.GetComponentOfType<Chunk3dGeometryHolder>();
            //shadedInstancedEffect.CurrentTechnique = shadedInstancedEffect.Techniques["ColorTechShadowMapInstanced"];

            ////avoid setting the same buffer a million times, only transform is changed
            //var objectGroups = objectsToDraw.GroupBy(x => x.modelId);

            //foreach (var group in objectGroups)
            //{
            //    VertexBuffer instanceBuffer;
            //    var groupCount = group.Count();
            //    if (cacheCheckDictionary.TryGetValue(group.Key, out int numberOfInstances) && numberOfInstances == groupCount)
            //    {
            //        instanceBuffer = instanceBufferCache[group.Key];
            //    }
            //    else
            //    {
            //        var instanceTransforms = new List<VertexShaderInstanceMatrix>(group.Count());
            //        //create insatnce data buffer
            //        foreach (var gameObject in objectsToDraw)
            //        {
            //            instanceTransforms.Add(new VertexShaderInstanceMatrix(NamelessGameObject.position));
            //        }
            //        instanceBuffer = new VertexBuffer(device, VertexShaderInstanceInput, instanceTransforms.Count, BufferUsage.WriteOnly);
            //        instanceBuffer.SetData(instanceTransforms.ToArray());
            //        cacheCheckDictionary[group.Key] = groupCount;
            //        instanceBufferCache[group.Key] = instanceBuffer;
            //    }

            //    var geometry = ModelsLibrary.Models[group.First().modelId];

            //    var bindings = new VertexBufferBinding[2];
            //    bindings[0] = new VertexBufferBinding(geometry.Buffer);
            //    bindings[1] = new VertexBufferBinding(instanceBuffer, 0, 1);
            //    device.SetVertexBuffers(bindings);
            //    device.Indices = geometry.IndexBuffer;
            //    var pass = shadedInstancedEffect.CurrentTechnique.Passes[1];
            //    pass.Apply();
            //    device.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, geometry.TriangleCount, groupCount);

            //}
        }

        #region debug
        //RasterizerState ApplyWireframe(GraphicsDevice device)
        //{
        //	var oldState = device.RasterizerState;
        //	RasterizerState rasterizerState = new RasterizerState();
        //	rasterizerState.FillMode = FillMode.WireFrame;
        //	device.RasterizerState = rasterizerState;
        //	return oldState;
        //}

        //void RestoreState(GraphicsDevice device, RasterizerState oldstate)
        //{
        //	device.RasterizerState = oldstate;
        //}
        #endregion
    }
}
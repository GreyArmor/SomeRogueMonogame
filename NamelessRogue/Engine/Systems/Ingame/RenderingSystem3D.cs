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
using RogueSharp.DiceNotation;
using SharpGen.Runtime;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using Veldrid;
using Veldrid.SPIRV;
using Veldrid.Utilities;
using Vulkan;
using Effect = NamelessRogue.Engine.Infrastructure.Effect;
using Point = Veldrid.Point;
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
            public TextureView tileAtlas;
            public Matrix4x4 xView;
            public Matrix4x4 xProjection;
            public Matrix4x4 xViewProjection;
            public Matrix4x4 xWorldViewProjection;
            public Matrix4x4 xWorldMatrix;
            public Matrix4x4 xBillboard;
            public Vector2 windCoef;
            public bool windFlag;
        }

        //cbuffer UnchangeableValiblesBuffer : register(b0)
        //{
        //    float3 CameraPosition;
        //    float3 CameraUp;
        //    float3 CameraRight;

        //    float4x4 xView;
        //    float4x4 xProjection;
        //    float4x4 xViewProjection;

        //    float2 windCoef;
        //    bool windFlag;

        //    float3 xLightPos;
        //    float xLightPower;
        //    float xAmbient;

        //    float rowIndexEnd = 33;
        //    float substractionCoef = 1;
        //    float verticesPerRow = 36;
        //};

        //cbuffer ObjectTransformBuffer : register(b1)
        //{
        //    float4x4 xLightsWorldViewProjection;
        //    float4x4 xWorldViewProjection;
        //    float4x4 xWorldMatrix;
        //    float4x4 xBillboard;
        //};

        [StructLayout(LayoutKind.Sequential)]
        public struct TerrainUnchangeableValiblesBuffer
        {
            public Vector3 CameraPosition;
            public Vector3 CameraUp;
            public Vector3 CameraRight;

            public Matrix4x4 xView;
            public Matrix4x4 xProjection;
            public Matrix4x4 xViewProjection;

            public Vector2 windCoef;
            public bool windFlag;

            public Vector3 xLightPos;
            public float xLightPower;
            public float xAmbient;


            internal int substractionCoef;
            internal int rowIndexEnd;
            internal int verticesPerRow;

            public Matrix4x4 xLightsWorldViewProjection;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TerrainObjectValiblesBuffer
        { 
            public Matrix4x4 xWorldViewProjection;
            public Matrix4x4 xWorldMatrix;
            public Matrix4x4 xBillboard;
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


        Pipeline _pipeline;
        Pipeline _shadowMapPipeline;
        
        public static int VertexDeclarationSize = 15 * 4;
        VertexLayoutDescription vertexLayout = new VertexLayoutDescription(
                 new VertexElementDescription("POSITION", VertexElementSemantic.Position, VertexElementFormat.Float3),
                 new VertexElementDescription("COLOR0", VertexElementSemantic.Color, VertexElementFormat.Float4),
                 new VertexElementDescription("COLOR1", VertexElementSemantic.Color, VertexElementFormat.Float4),
                 new VertexElementDescription("TEXCOORD", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
                 new VertexElementDescription("NORMAL", VertexElementSemantic.Normal, VertexElementFormat.Float3)
            );    

        public static int TerrainVertexDeclarationSize = 12;
        VertexLayoutDescription TerrainVertexDeclaration = new VertexLayoutDescription(new[] {
              new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
              new VertexElementDescription("Color", VertexElementSemantic.TextureCoordinate, VertexElementFormat.UInt1),
              }
         );

        //this is 4x4 matrix, transferred as instructed in https://learn.microsoft.com/en-us/windows/win32/direct3d9/efficiently-drawing-multiple-instances-of-geometry

        VertexLayoutDescription VertexShaderInstanceInput = new VertexLayoutDescription(
               new VertexElementDescription("TEXCOORD0", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4),
               new VertexElementDescription("TEXCOORD1", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4),
               new VertexElementDescription("TEXCOORD2", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4),
               new VertexElementDescription("TEXCOORD3", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4)
          );

        VertexLayoutDescription VertexShaderBillboardInstanceDataInput = new VertexLayoutDescription(
          new VertexElementDescription("TEXCOORD0", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4)
     );



        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct VertexShaderInstanceMatrix
        {
            // ReSharper disable NotAccessedField.Local
            private Vector4 row1;
            private Vector4 row2;
            private Vector4 row3;
            private Vector4 row4;

            public VertexShaderInstanceMatrix(Matrix4x4 matrix)
            {
                row1 = new Vector4(matrix.M11, matrix.M12, matrix.M13, matrix.M14);
                row2 = new Vector4(matrix.M21, matrix.M22, matrix.M23, matrix.M24);
                row3 = new Vector4(matrix.M31, matrix.M32, matrix.M33, matrix.M34);
                row4 = new Vector4(matrix.M41, matrix.M42, matrix.M43, matrix.M44);
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




        SamplerDescription ssDescription = new SamplerDescription()
        {
            AddressModeU = SamplerAddressMode.Wrap,
            AddressModeV = SamplerAddressMode.Wrap,
            AddressModeW = SamplerAddressMode.Wrap,
            Filter = SamplerFilter.Anisotropic
        };

        SamplerDescription pointSamplerDescription = new SamplerDescription()
        {
            AddressModeU = SamplerAddressMode.Wrap,
            AddressModeV = SamplerAddressMode.Wrap,
            AddressModeW = SamplerAddressMode.Wrap,
            Filter = SamplerFilter.MinPoint_MagPoint_MipPoint
        };

        Sampler sampler;

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
            sampler = game.GraphicsDevice.ResourceFactory.CreateSampler(ssDescription);
            pointSampler = game.GraphicsDevice.ResourceFactory.CreateSampler(pointSamplerDescription);
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
            public Matrix4x4 position;
        }
        //for test
        List<ModelInstance> objectsToDraw = new List<ModelInstance>();
        bool once = true;

        DeviceBuffer hackBuffer;
        DeviceBuffer hackBufferIndices;
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
            var factory = game.GraphicsDevice.ResourceFactory;
            hackBuffer = factory.CreateBuffer(new BufferDescription((uint)(points.Count * 12), BufferUsage.VertexBuffer));
            game.GraphicsDevice.UpdateBuffer(hackBuffer, 0, points.ToArray());
            //hackBuffer.SetData<Vertex3D>(vertices.ToArray());

            hackBufferIndices = factory.CreateBuffer(new BufferDescription((uint)(indices.Count * sizeof(int)), BufferUsage.IndexBuffer));
            game.GraphicsDevice.UpdateBuffer(hackBufferIndices, 0, indices.ToArray());

        }

        Sampler pointSampler;
        Vector3 LandCenter;
        DeviceBuffer unchengeableBuffer = null;
        DeviceBuffer objectBuffer = null;
        TerrainUnchangeableValiblesBuffer constantBuffer0 = new TerrainUnchangeableValiblesBuffer();
        TerrainObjectValiblesBuffer constantBuffer1 = new TerrainObjectValiblesBuffer();

        ResourceSet sceneBufferSet;
        ResourceSet shadowMapResourceSet;
        ResourceLayout sceneBufferLayout;

        DeviceBuffer CreateConstantBuffer0(GraphicsDevice device)
        {
            return device.ResourceFactory.CreateBuffer(new BufferDescription((uint)Marshal.SizeOf(typeof(TerrainUnchangeableValiblesBuffer)), BufferUsage.UniformBuffer));
        }

        DeviceBuffer CreateConstantBuffer1(GraphicsDevice device)
        {
            return device.ResourceFactory.CreateBuffer(new BufferDescription((uint)Marshal.SizeOf(typeof(TerrainObjectValiblesBuffer)), BufferUsage.UniformBuffer));
        }
        public override void Update(GameTime gameTime, NamelessGame game)
        {
            var device = game.GraphicsDevice;
            if (unchengeableBuffer == null)
            {
                unchengeableBuffer = CreateConstantBuffer0(device);
                objectBuffer = CreateConstantBuffer1(device);
            }
           
            //game.Window.DeviceContext.OutputMerger.BlendState = opaque;
            ////	game.Context.OutputMerger.SamplerStates[0] = sampler;
            //game.Window.DeviceContext.OutputMerger.DepthStencilState = depthStencilState;
            //game.Window.DeviceContext.Rasterizer.State = rasterizerState;         


            IEntity worldEntity = game.TimelineEntity;
            IWorldProvider worldProvider = null;
            if (worldEntity != null)
            {
                worldProvider = worldEntity.GetComponentOfType<TimeLine>().CurrentTimelineLayer.Chunks;
            }
            if (once)
            {
                CreateResources(game, device);
            }

            game.CommandList.SetPipeline(_shadowMapPipeline);

            Camera3D camera = game.PlayerEntity.GetComponentOfType<Camera3D>();
            BoundingFrustum cameraFrustrum = new BoundingFrustum(camera.View * camera.Projection);

            //	angleRotation += 0.01f;
            if (angleRotation > 360) angleRotation -= 360;
            CalculateSun(angleRotation, LandCenter);

            constantBuffer0.xViewProjection = (camera.View * camera.Projection);
         
            constantBuffer0.xView = (camera.View);
            constantBuffer0.xProjection = (camera.Projection);
            constantBuffer0.CameraPosition = (camera.Position);

            constantBuffer0.xLightPos = (sunLight.Position);
            constantBuffer0.xLightPower = (1f);
            constantBuffer0.xAmbient = (0.2f);        

            constantBuffer0.substractionCoef = (1);
            constantBuffer0.rowIndexEnd = (Constants.ChunkSize * 2) + 1;
            constantBuffer0.verticesPerRow = (Constants.ChunkSize * 2) + 4;


            game.CommandList.SetPipeline(_shadowMapPipeline);
            device.UpdateBuffer(unchengeableBuffer, 0, constantBuffer0);
            game.CommandList.SetFramebuffer(sunLight.ShadowMapRenderTargetTexture);
            game.CommandList.ClearDepthStencil(0);
            game.CommandList.SetGraphicsResourceSet(0, shadowMapResourceSet);
            //   game.Window.DeviceContext.OutputMerger.SetRenderTargets();
            // device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            //effect.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            // effect.GraphicsDevice.BlendState = BlendState.AlphaBlend;


//            var chunkGeometries1 = game.ChunkGeometryEntiry.GetComponentOfType<Chunk3dGeometryHolder>();

            //fighting shadow map imprecision with mad skillz, to remove artifacts on terrain border


            //RenderHackBufferToShadowMap(game);

            RenderChunksToShadowMap(game, camera);
            //	RenderObjectsToShadowMap(game);

            game.CommandList.SetFramebuffer(device.SwapchainFramebuffer);
            game.CommandList.SetPipeline(_pipeline);
            //var shadowMap = sunLight.ShadowMapRenderTargetTexture;
            game.CommandList.SetGraphicsResourceSet(0, sceneBufferSet);
          //  game.Window.DeviceContext.PixelShader.SetShaderResource(1, shadowMap);
            //shadedInstancedEffect.Parameters["shadowMap"].SetValue(shadowMap);
            //game.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            RenderChunksWithShadows(game, camera);
            //RenderObjectsWithShadow(game);

            // game.GraphicsDevice.RasterizerState = oldstate;

            var matrix = Matrix4x4.CreateTranslation(sunLight.Position);

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

        struct VertexPositionColor
        {
            public Vector2 Position; // This is the position, in normalized device coordinates.
            public RgbaFloat Color; // This is the color of the vertex.
            public VertexPositionColor(Vector2 position, RgbaFloat color)
            {
                Position = position;
                Color = color;
            }
            public const uint SizeInBytes = 24;
        }

        private void CreateResources(NamelessGame game, GraphicsDevice device)
        {
            shadowMapStageShader = new Effect(device, "Content\\ChunkShader3D.fx", "TerrainShadowMapVertexShader", "ShadowMapPixelShader");
            shadowMapSceneShader = new Effect(device, "Content\\ChunkShader3D.fx", "TerrainShadowedSceneVertexShader", "TerrainShadowedScenePixelShader");
            //   shadedInstancedEffect = new Effect("ObjectsShader", "", "");
            // objectsToDraw = GetWorldObjectsToDraw(new Point(300, 300), worldProvider);
            CreateshadowMapEdgeHackMesh(game);
            //    SetupDebugDraw(game.GraphicsDevice);
            once = false;
            //    var chunkGeometries = game.ChunkGeometryEntiry.GetComponentOfType<Chunk3dGeometryHolder>();
            //  var landBounds = BoundingBox.CreateFromVertices(chunkGeometries.ChunkGeometries.Values.Select(x => (x.Item1.Bounds.Max + x.Item1.Bounds.Min) / 2).ToArray());
            LandCenter = Vector3.One;// (landBounds.Max + landBounds.Min) / 2;
            var _shaders = new Shader[] { shadowMapStageShader.VertexShader, shadowMapStageShader.PixelShader };

            ResourceLayout shadowMapLayout = device.ResourceFactory.CreateResourceLayout(
            new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("MainBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                new ResourceLayoutElementDescription("ObjectBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex)
            ));

            sceneBufferLayout = device.ResourceFactory.CreateResourceLayout(
                new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("MainBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                new ResourceLayoutElementDescription("ObjectBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                new ResourceLayoutElementDescription("tileAtlas", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("shadowMap", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("textureSampler", ResourceKind.Sampler, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("ShadowMapSampler", ResourceKind.Sampler, ShaderStages.Fragment)
            ));



            GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription();

            pipelineDescription.BlendState = BlendStateDescription.SingleOverrideBlend;

            pipelineDescription.DepthStencilState = new DepthStencilStateDescription(
            depthTestEnabled: true,
            depthWriteEnabled: true,
            comparisonKind: ComparisonKind.LessEqual);

            pipelineDescription.RasterizerState = new RasterizerStateDescription(
            cullMode: FaceCullMode.Back,
            fillMode: PolygonFillMode.Solid,
            frontFace: FrontFace.Clockwise,
            depthClipEnabled: true,
            scissorTestEnabled: false);

            pipelineDescription.DepthStencilState = new DepthStencilStateDescription(
            depthTestEnabled: true,
            depthWriteEnabled: true,
            comparisonKind: ComparisonKind.LessEqual);

            pointSampler = device.ResourceFactory.CreateSampler(this.pointSamplerDescription);

            shadowMapResourceSet = device.ResourceFactory.CreateResourceSet(new ResourceSetDescription(shadowMapLayout, unchengeableBuffer, objectBuffer));
            sceneBufferSet = device.ResourceFactory.CreateResourceSet(new ResourceSetDescription(sceneBufferLayout, unchengeableBuffer, objectBuffer, sunLight.ShadowMapTextureView, sunLight.ShadowMapTextureView, pointSampler, pointSampler));


            pipelineDescription.ResourceLayouts = new ResourceLayout[] { shadowMapLayout };

            pipelineDescription.ShaderSet = new ShaderSetDescription(
            vertexLayouts: new VertexLayoutDescription[] { TerrainVertexDeclaration },
            shaders: _shaders);

            pipelineDescription.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
            pipelineDescription.ResourceBindingModel = ResourceBindingModel.Default;
            pipelineDescription.Outputs = sunLight.ShadowMapRenderTargetTexture.OutputDescription;


            _shadowMapPipeline = device.ResourceFactory.CreateGraphicsPipeline(pipelineDescription);


            pipelineDescription.Outputs = device.SwapchainFramebuffer.OutputDescription;
            pipelineDescription.ResourceLayouts = new ResourceLayout[] { sceneBufferLayout };
            pipelineDescription.ShaderSet = new ShaderSetDescription(
            vertexLayouts: new VertexLayoutDescription[] { TerrainVertexDeclaration },
            shaders: new Shader[] { shadowMapStageShader.VertexShader, shadowMapStageShader.PixelShader });

            _pipeline = device.ResourceFactory.CreateGraphicsPipeline(pipelineDescription);

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
            //var postionOffsetX = positon.X * Constants.ChunkSize;
            //var postionOffsetY = positon.Y * Constants.ChunkSize;
            var objects = new List<ModelInstance>();

            //for (int x = 0; x < 100 * Constants.ChunkSize; x++)
            //{
            //    for (int y = 0; y < 100 * Constants.ChunkSize; y++)
            //    {
            //        Tile tileToDraw = world.GetTile(x + postionOffsetX, y + postionOffsetY);
            //        foreach (var entity in tileToDraw.GetEntities())
            //        {
            //            var furniture = entity.GetComponentOfType<Furniture>();
            //            var drawable = entity.GetComponentOfType<Drawable>();
            //            if (furniture != null && drawable != null)
            //            {
            //                if (drawable.Representation == 'T')
            //                {
            //                    //var modelShift = Matrix4x4.CreateCreateTranslation(0.5f, 0.5f, 2f);
            //                    //Matrix4x4.CreateCreateTranslation(Constants.ScaleDownCoeficient * x, Constants.ScaleDownCoeficient * y, (float)tileToDraw.Elevation, out Matrix4x4 pos);
            //                    objects.Add(new ModelInstance() { modelId = "smallTree", position = Constants.ScaleDownMatrix4x4 * Matrix4x4.CreateTranslation(x * Constants.ScaleDownCoeficient, y * Constants.ScaleDownCoeficient, tileToDraw.ElevationVisual * Constants.ScaleDownCoeficient) });
            //                    //return objects;s
            //                }
            //                if (drawable.Representation == 't')
            //                {

            //                    //Matrix4x4.CreateCreateTranslation(Constants.ScaleDownCoeficient * x, Constants.ScaleDownCoeficient * y, (float)tileToDraw.Elevation, out Matrix4x4 pos);
            //                    objects.Add(new ModelInstance() { modelId = "smallTree", position = Constants.ScaleDownMatrix4x4 * Matrix4x4.CreateTranslation(x * Constants.ScaleDownCoeficient, y * Constants.ScaleDownCoeficient, tileToDraw.ElevationVisual * Constants.ScaleDownCoeficient) });
            //                    //return objects;
            //                }
            //            }
            //        }
            //    }
            //}

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

            game.CommandList.SetPipeline(_shadowMapPipeline);
            game.CommandList.SetGraphicsResourceSet(0, shadowMapResourceSet);

        //    shadowMapStageShader.Apply(game.Window.DeviceContext);
           // game.Window.DeviceContext.VertexShader.SetConstantBuffer(0, cbuffer);
            //game.Window.DeviceContext.PixelShader.SetConstantBuffer(0, cbuffer);

            var chunkGeometries = game.ChunkGeometryEntiry.GetComponentOfType<Chunk3dGeometryHolder>();
            foreach (var geometryTuple in chunkGeometries.ChunkGeometries.Values)
            {
                var geometry = geometryTuple.Item1;
                var terrainGeometry = geometryTuple.Item2;
                constantBuffer1.xWorldMatrix = (Constants.ScaleDownMatrix * terrainGeometry.WorldOffset);
                game.GraphicsDevice.UpdateBuffer(objectBuffer, 0, constantBuffer1);

                game.CommandList.SetVertexBuffer(0, terrainGeometry.Buffer);
                game.CommandList.Draw((uint)terrainGeometry.VerticesCount);
                break;
            }
        }
        private void RenderChunksWithShadows(NamelessGame game, Camera3D camera)
        {
            game.CommandList.SetPipeline(_pipeline);
            var chunkGeometries = game.ChunkGeometryEntiry.GetComponentOfType<Chunk3dGeometryHolder>();
            foreach (var geometryTuple in chunkGeometries.ChunkGeometries.Values)
            {
                var geometry = geometryTuple.Item1;
                var terrainGeometry = geometryTuple.Item2;
                constantBuffer1.xWorldMatrix = (Constants.ScaleDownMatrix * terrainGeometry.WorldOffset);
                game.GraphicsDevice.UpdateBuffer(objectBuffer, 0, constantBuffer1);

                sceneBufferSet = game.GraphicsDevice.ResourceFactory.CreateResourceSet(
                    new ResourceSetDescription(sceneBufferLayout, unchengeableBuffer, objectBuffer, geometry.Material, sunLight.ShadowMapTextureView, pointSampler, pointSampler));
                game.CommandList.SetGraphicsResourceSet(0, sceneBufferSet);

                game.CommandList.SetVertexBuffer(0, terrainGeometry.Buffer);
                game.CommandList.Draw((uint)terrainGeometry.VerticesCount);
                break;
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

        //Dictionary<string, int> cacheCheckDictionary = new Dictionary<string, int>();
        //Dictionary<string, VertexBuffer> instanceBufferCache = new Dictionary<string, VertexBuffer>();
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
            //    //Matrix4x4.CreateBillboard
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
            ////Matrix4x4.CreateCreateTranslation(Constants.ScaleDownCoeficient * position.X, Constants.ScaleDownCoeficient * position.Y, (float)tileToDraw.Elevation, out Matrix4x4 pos);
            //var world = Constants.ScaleDownMatrix4x4 * Matrix4x4.CreateCreateTranslation(position.X * Constants.ScaleDownCoeficient, position.Y * Constants.ScaleDownCoeficient, tileToDraw.ElevationVisual * Constants.ScaleDownCoeficient);
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
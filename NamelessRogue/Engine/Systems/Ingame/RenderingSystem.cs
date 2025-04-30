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
using System.Runtime.InteropServices;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.ECS;
using static Assimp.Metadata;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Drawing;
using Point = Microsoft.Xna.Framework.Point;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using NamelessRogue.Engine.Components.ItemComponents;
using Tile = NamelessRogue.Engine.Components.ChunksAndTiles.Tile;
using NamelessRogue.Engine.Components.Status;
using MonoGame.Aseprite;
using MonoGame.Extended;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Shapes;
using Microsoft.VisualBasic.Logging;
using AsepriteDotNet;
using MonoGame.Extended.Particles.Modifiers.Containers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Profiles;
using System.Reflection.Metadata;
using XNAColor = Microsoft.Xna.Framework.Color;

namespace NamelessRogue.Engine.Systems.Ingame
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vertex
    {
        // ReSharper disable NotAccessedField.Local
        private Vector3 position;
        private Vector4 color;
        private Vector4 backgroundColor;
        private Vector2 textureCoordinate;

        public Vertex(Vector3 position, Vector4 color, Vector4 backgroundColor, Vector2 textureCoordinate)
        {
            this.position = position;
            this.color = color;
            this.backgroundColor = backgroundColor;
            this.textureCoordinate = textureCoordinate;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vertex3D
    {
        // ReSharper disable NotAccessedField.Local
        private Vector3 position;
        private Vector4 color;
        private Vector4 backgroundColor;
        private Vector2 textureCoordinate;
        private Vector3 normal;

        public Vertex3D(Vector3 position, Vector4 color, Vector4 backgroundColor, Vector2 textureCoordinate, Vector3 normal)
        {
            this.position = position;
            this.color = color;
            this.backgroundColor = backgroundColor;
            this.textureCoordinate = textureCoordinate;
            this.normal = normal;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    // we use height data for vertex z value, all else is calculated by gpu, we use yaw and pitch to calculate triangle normal
    public struct TerrainVertex
    {
        // ReSharper disable NotAccessedField.Local
        public Vector3 vertexHeightYawPitch;

        public TerrainVertex(float height, float yaw, float pitch)
        {
            this.vertexHeightYawPitch = new Vector3(height, yaw, pitch);
        }
    }

    public class SFXLightningModel
    {
        public int timeToPlay;
        public float rotation;
        public Vector2 screenLocation;
    }

    public class TileModel : IDisposable {
        public Vertex[] Vertices { get; }
        public int[] Indices { get; }
        public VertexBuffer Buffer { get; set; }
        public IndexBuffer IndexBuffer { get; set; }
        public TileModel(int height, int width)
        {
            Vertices = new Vertex[height * width * 4];
            Indices = new int[height * width * 6];

            //var indices = new int[6] { 0, 1, 2, 2, 1, 3 };
            var vertexCounter = 0;
            for (int i = 0; i < height * width * 6; i += 6)
            {
                Indices[i] = vertexCounter;
                Indices[i + 1] = vertexCounter + 1;
                Indices[i + 2] = vertexCounter + 2;
                Indices[i + 3] = vertexCounter + 2;
                Indices[i + 4] = vertexCounter + 1;
                Indices[i + 5] = vertexCounter + 3;
                vertexCounter += 4;
            }
           
        }

        public void Dispose()
        {
            Buffer?.Dispose();
            IndexBuffer?.Dispose();
        }

        public void UpdateBuffers(GraphicsDevice device)
        {
            Buffer?.Dispose();
            IndexBuffer?.Dispose();

            Buffer = new VertexBuffer(device, RenderingSystem.VertexDeclaration, Vertices.Length, BufferUsage.None);
            IndexBuffer = new IndexBuffer(device, IndexElementSize.ThirtyTwoBits, Indices.Length, BufferUsage.None);

            Buffer.SetData(Vertices);
            IndexBuffer.SetData(Indices);
        }
    }

    public class VisualChunk
    {
        public TileModel TileModel { get; set; }
        public Point WorldPosition { get; }
        public ScreenTile[,] ScreenBuffer { get; private set; }
        public VisualChunk(Point worldPosition)
        {
            var size = Constants.ChunkSize;
            WorldPosition = worldPosition;
            ScreenBuffer = new ScreenTile[size, size];
            TileModel = new TileModel(size, size);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    ScreenBuffer[i, j] = new ScreenTile();
                }
            }
        }

        public void UpdateChunk(Dictionary<string, RenderingSystem.AtlasTileData> characterToTileDictionary, Texture2D tileAtlas, int playerZ, NamelessGame game)
        {

            var worldProvider = game.WorldProvider;
            FillWithWorld(worldProvider, playerZ);

            var stackDepth = 0;
            for (int y = 0; y < Constants.ChunkSize; y++)
            {
                for (int x = 0; x < Constants.ChunkSize; x++)
                {
                    var objectToDraw = ScreenBuffer[x, y].StackedObjects[stackDepth];
                    if (objectToDraw.Type == ScreenObjectSource.Tileset)
                    {
                        var objectId = objectToDraw.Id;
                        RenderingSystem.AtlasTileData tileData;
                        if (!characterToTileDictionary.TryGetValue(objectId, out tileData))
                        {
                            characterToTileDictionary.TryGetValue("Nothingness", out tileData);
                        }
                        var white = new Color(1f, 1f, 1f, 1f);

                        var tileMask = white;

                        int tileHeight = 64;
                        int tileWidth = 64;

                        RenderingSystem.DrawTile(tileHeight, tileWidth, x, y, Constants.ChunkSize,
                                x * Constants.ChunkSize,
                                y * Constants.ChunkSize,
                                tileData,
                                tileMask,
                                tileMask, TileModel, tileAtlas);
                    }
                }
            }

            TileModel.UpdateBuffers(game.GraphicsDevice);
        }

        private void FillWithWorld(IWorldProvider world, int playerZ)
        {
            var realSpaceChunkX = WorldPosition.X * Constants.ChunkSize;
            var realSpaceChunkY = WorldPosition.Y * Constants.ChunkSize;

            for (int x = 0; x < Constants.ChunkSize; x++)
            {
                for (int y = 0; y < Constants.ChunkSize; y++)
                {
                    var realSpaceX = realSpaceChunkX + x;
                    var realSpaceY = realSpaceChunkY + y;
                    Tile tileToDraw = world.GetTile(realSpaceX, realSpaceY, playerZ);

                    if (tileToDraw != null && tileToDraw.Terrain != TerrainTypes.Nothingness)
                    {
                        ScreenBuffer[x, y].AddObject(TerrainLibrary.Terrains[tileToDraw.Terrain].Representation.ObjectID, ScreenObjectSource.Tileset, new Color(255, 255, 255), false, false, false);
                    }
                    else
                    {
                        ScreenBuffer[x, y].AddObject("Nothingness", ScreenObjectSource.Tileset, new Color(), false, false);
                    }
                }
            }
        }
    }


    public class UpdateVisualChunkCommand : ICommand
    {
        public UpdateVisualChunkCommand(Point chunkCoordinate)
        {
            ChunkCoordinate = chunkCoordinate;
        }

        public Point ChunkCoordinate { get; }
    }

    public class RenderingSystem : BaseSystem
    {
        private List<SFXLightningModel> lightningModels = new List<SFXLightningModel>();
        public override HashSet<Type> Signature { get; }

        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector4, VertexElementUsage.Color, 0),
            new VertexElement(sizeof(float) * 7, VertexElementFormat.Vector4, VertexElementUsage.Color, 1),
            new VertexElement(sizeof(float) * 11, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
        );

        Dictionary<string, AtlasTileData> characterToTileDictionary;
        private float gameTime;
        private float angle = 0;
        private float step = 0.04f;
        private InternalRandom graphicalRandom = new InternalRandom();
        Effect effect;
        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;
        private int playerPosZ;
        Microsoft.Xna.Framework.Color shadowColor = new Microsoft.Xna.Framework.Color(0, 0, 0, 96);

      List<VisualChunk> visualChunks = new List<VisualChunk>();

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

        public RenderingSystem(GameSettings settings){
            InitializeCharacterTileDictionary();

            Signature = [typeof(Drawable), typeof(Position)];
           

        }

        //TODO move this hardcode to configuration file for tileset
        void InitializeCharacterTileDictionary()
        {


            var atlasTileData = new AtlasTileData(8, 9);
            characterToTileDictionary = new Dictionary<string, AtlasTileData>();
            characterToTileDictionary.Add("Nothingness", atlasTileData);
            characterToTileDictionary.Add("Dirt", new AtlasTileData(2, 4));
            characterToTileDictionary.Add("Character", new AtlasTileData(0, 7));
            characterToTileDictionary.Add("Asphault", new AtlasTileData(1, 3));
            characterToTileDictionary.Add("Sidewalk", new AtlasTileData(0, 3));
            characterToTileDictionary.Add("PaintedAsphault", new AtlasTileData(2, 3));
            characterToTileDictionary.Add("FloorGrate", new AtlasTileData(0, 4));
            characterToTileDictionary.Add("smallCursor", new AtlasTileData(0, 6));
            characterToTileDictionary.Add("Cursor", new AtlasTileData(1, 6));

            characterToTileDictionary.Add("arrowDown", new AtlasTileData(1,7));

            characterToTileDictionary.Add("wall", atlasTileData);
            characterToTileDictionary.Add("door", atlasTileData);
            characterToTileDictionary.Add("window", atlasTileData);
            characterToTileDictionary.Add("stairs_down", new AtlasTileData(8, 0));
            characterToTileDictionary.Add("stairs_up", new AtlasTileData(8, 0));

             characterToTileDictionary.Add("railing_metal_lt",  new AtlasTileData(5, 3));
             characterToTileDictionary.Add("railing_metal_t",   new AtlasTileData(6, 3));
             characterToTileDictionary.Add("railing_metal_rt",  new AtlasTileData(7, 3));
             characterToTileDictionary.Add("railing_metal_l",   new AtlasTileData(5, 4));
             characterToTileDictionary.Add("railing_metal_r",   new AtlasTileData(7, 4));
             characterToTileDictionary.Add("railing_metal_rb",  new AtlasTileData(7, 5));
             characterToTileDictionary.Add("railing_metal_b",   new AtlasTileData(6, 5));
             characterToTileDictionary.Add("railing_metal_lb",  new AtlasTileData(5, 5));
             characterToTileDictionary.Add("floor_metal", new AtlasTileData(6, 4));


            characterToTileDictionary.Add("railing_stairs_down", new AtlasTileData(5, 6));
            characterToTileDictionary.Add("railing_stairs_up", new AtlasTileData(6, 6));

            characterToTileDictionary.Add("sattelite_dish", new AtlasTileData(3, 9));
            characterToTileDictionary.Add("air_conditioner", new AtlasTileData(4, 9));
            characterToTileDictionary.Add("antenna_1", new AtlasTileData(3, 10));
            characterToTileDictionary.Add("antenna_2", new AtlasTileData(4, 10));
            characterToTileDictionary.Add("antenna_3", new AtlasTileData(5, 10));
            characterToTileDictionary.Add("antenna_4", new AtlasTileData(6, 10));
            characterToTileDictionary.Add("antenna_5", new AtlasTileData(3, 11));
            characterToTileDictionary.Add("antenna_6", new AtlasTileData(4, 11));
            characterToTileDictionary.Add("antenna_7", new AtlasTileData(5, 11));

            characterToTileDictionary.Add("table", new AtlasTileData(2, 8));

            characterToTileDictionary.Add("bed", new AtlasTileData(1, 9));

            characterToTileDictionary.Add("toilet", new AtlasTileData(3, 8));
            characterToTileDictionary.Add("shower", new AtlasTileData(0, 8));

            BindWallPositions(0, 0, "wall", "door", "openDoor", "window");
            BindWallPositions(10, 0, "wall_brick", "door_brick", "openDoor_brick", "window_brick");

        }

        private void BindWallPositions(int displacementX, int displacementY, string wallId, string doorId, string openDoorId, string windowId )
        {
            characterToTileDictionary.Add(wallId + TileBitmaskingEncoding.WallHorizontal0, new AtlasTileData(1 + displacementX, 0 + displacementY));
            characterToTileDictionary.Add(wallId + TileBitmaskingEncoding.WallHorizontal1, new AtlasTileData(1 + displacementX, 0 + displacementY));
            characterToTileDictionary.Add(wallId + TileBitmaskingEncoding.WallHorizontal2, new AtlasTileData(1 + displacementX, 0 + displacementY));

            characterToTileDictionary.Add(wallId + TileBitmaskingEncoding.WallVertical0, new AtlasTileData(0 + displacementX, 1 + displacementY));
            characterToTileDictionary.Add(wallId + TileBitmaskingEncoding.WallVertical1, new AtlasTileData(0 + displacementX, 1 + displacementY));
            characterToTileDictionary.Add(wallId + TileBitmaskingEncoding.WallVertical2, new AtlasTileData(4 + displacementX, 0 + displacementY));

            characterToTileDictionary.Add(wallId + TileBitmaskingEncoding.CornerTopLeft, new AtlasTileData(0 + displacementX, 0 + displacementY));
            characterToTileDictionary.Add(wallId + TileBitmaskingEncoding.CornerBotLeft, new AtlasTileData(0 + displacementX, 2 + displacementY));
            characterToTileDictionary.Add(wallId + TileBitmaskingEncoding.CornerTopRight, new AtlasTileData(2 + displacementX, 0 + displacementY));
            characterToTileDictionary.Add(wallId + TileBitmaskingEncoding.CornerBotRight, new AtlasTileData(2 + displacementX, 2 + displacementY));

            characterToTileDictionary.Add(wallId + TileBitmaskingEncoding.InterserctionLeft, new AtlasTileData(3 + displacementX, 1 + displacementY));
            characterToTileDictionary.Add(wallId + TileBitmaskingEncoding.InterserctionRight, new AtlasTileData(3 + displacementX, 1 + displacementY, true));
            characterToTileDictionary.Add(wallId + TileBitmaskingEncoding.InterserctionTop, new AtlasTileData(3 + displacementX, 0 + displacementY));
            characterToTileDictionary.Add(wallId + TileBitmaskingEncoding.InterserctionBot, new AtlasTileData(3 + displacementX, 2 + displacementY));
            characterToTileDictionary.Add(wallId + TileBitmaskingEncoding.InterserctionCenter, new AtlasTileData(4 + displacementX, 2 + displacementY));
            characterToTileDictionary.Add(wallId + TileBitmaskingEncoding.Pillar, new AtlasTileData(4 + displacementX, 1));

            var doorData = new AtlasTileData(5 + displacementX, 0 + displacementY);
            var doorDataVertical = new AtlasTileData(5 + displacementX, 1 + displacementY);

            characterToTileDictionary.Add(doorId + TileBitmaskingEncoding.WallHorizontal0, doorData);
            characterToTileDictionary.Add(doorId + TileBitmaskingEncoding.WallHorizontal1, doorData);
            characterToTileDictionary.Add(doorId + TileBitmaskingEncoding.WallHorizontal2, doorData);

            characterToTileDictionary.Add(doorId + TileBitmaskingEncoding.WallVertical0, doorDataVertical);
            characterToTileDictionary.Add(doorId + TileBitmaskingEncoding.WallVertical1, doorDataVertical);
            characterToTileDictionary.Add(doorId + TileBitmaskingEncoding.WallVertical2, doorDataVertical);

            var openDoorData = new AtlasTileData(7 + displacementX, 0 + displacementY);
            var openDoorDataVertical = new AtlasTileData(7 + displacementX, 1 + displacementY);

            characterToTileDictionary.Add(openDoorId + TileBitmaskingEncoding.WallHorizontal0, openDoorData);
            characterToTileDictionary.Add(openDoorId + TileBitmaskingEncoding.WallHorizontal1, openDoorData);
            characterToTileDictionary.Add(openDoorId + TileBitmaskingEncoding.WallHorizontal2, openDoorData);

            characterToTileDictionary.Add(openDoorId + TileBitmaskingEncoding.WallVertical0, openDoorDataVertical);
            characterToTileDictionary.Add(openDoorId + TileBitmaskingEncoding.WallVertical1, openDoorDataVertical);
            characterToTileDictionary.Add(openDoorId + TileBitmaskingEncoding.WallVertical2, openDoorDataVertical);

            characterToTileDictionary.Add(windowId + TileBitmaskingEncoding.WallHorizontal0, new AtlasTileData(6 + displacementX, 0 + displacementY));
            characterToTileDictionary.Add(windowId + TileBitmaskingEncoding.WallHorizontal1, new AtlasTileData(6 + displacementX, 0 + displacementY));
            characterToTileDictionary.Add(windowId + TileBitmaskingEncoding.WallHorizontal2, new AtlasTileData(6 + displacementX, 0 + displacementY));

            characterToTileDictionary.Add(windowId + TileBitmaskingEncoding.WallVertical0, new AtlasTileData(6 + displacementX, 1 + displacementY));
            characterToTileDictionary.Add(windowId + TileBitmaskingEncoding.WallVertical1, new AtlasTileData(6 + displacementX, 1 + displacementY));
            characterToTileDictionary.Add(windowId + TileBitmaskingEncoding.WallVertical2, new AtlasTileData(6 + displacementX, 1 + displacementY));
        }

        TileModel unseenTiles;
        TileModel unknownTiles;
        public override void Update(GameTime gameTime, NamelessGame game)
        {

            game.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            game.GraphicsDevice.SamplerStates[0] = sampler;
            game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            Position playerPosition = game.FollowedByCameraEntity
               .GetComponentOfType<Position>();

            playerPosZ = playerPosition.Z;

            //todo move to constructor or some other place better suited for initialization
            if (tileAtlas == null)
            {
                InitializeTexture(game);
            }         


            IEntity worldEntity = game.TimelineEntity;
            IWorldProvider worldProvider = null;
            if (worldEntity != null)
            {
                worldProvider = worldEntity.GetComponentOfType<TimeLine>().CurrentTimelineLayer.Chunks;
            }

            while (game.Commander.DequeueCommand(out UpdateVisualChunkCommand command))
            {
                var chunkPoint = command.ChunkCoordinate;

                var chunk = visualChunks.FirstOrDefault(x => x.WorldPosition == chunkPoint);
                if (chunk == null)
                {
                    chunk = new VisualChunk(chunkPoint);
                }

                chunk.UpdateChunk(characterToTileDictionary, tileAtlas, playerPosZ, game);
                visualChunks.Add(chunk);
            }


            var entity = game.CameraEntity;

            ConsoleCamera camera = entity.GetComponentOfType<ConsoleCamera>();
            Screen screen = entity.GetComponentOfType<Screen>();
            Commander commander = game.Commander;
            screen = UpdateZoom(game, commander, entity, screen , out bool zoomUpdate);
            int fsz = game.Settings.GetFontSizeZoomed();

            if(zoomUpdate)
            {

            }

            if (camera != null && screen != null && worldProvider != null)
            {

                //ProcessSXFCommands(game);

                MoveCamera(game, camera);
                ClearScreen(screen, camera, game.GetSettings(), worldProvider);
                FillcharacterBufferVisibility(game, screen, camera, game.GetSettings(), worldProvider);


                //FillcharacterBuffersWithWorld(screen, camera, game.GetSettings(), worldProvider);
                //FillcharacterBuffersWithTileObjects(screen, camera, game.GetSettings(), game, gameTime, worldProvider);
                //FillcharacterBuffersWithWorldObjects(screen, camera, game.GetSettings(), game, gameTime);

                bool renderNewScreen = true;
                if (renderNewScreen)
                {
                    effect.Parameters["tileAtlas"].SetValue(tileAtlas);
                    var projectionMatrix = Matrix.CreateOrthographicOffCenter(0, game.GetActualWidth(), game.GetActualHeight(), 0, 0, 2);

                    effect.Parameters["xViewProjection"].SetValue(projectionMatrix);

                    effect.GraphicsDevice.SamplerStates[0] = sampler;
                    effect.GraphicsDevice.BlendState = BlendState.AlphaBlend;

                    var playerChunkPosition = new Point((playerPosition.Point.X / Constants.ChunkSize), (playerPosition.Point.Y / Constants.ChunkSize));
                    foreach (var visualChunk in visualChunks)
                    {
                        var tileModel = visualChunk.TileModel;

                        if ((playerChunkPosition - visualChunk.WorldPosition).ToVector2().Length()>(4)) //- visualChunk.WorldPosition).ToVector2().Length() > 1)
                        {
                            continue;
                        }

                        var chunkPosition = new Vector3((visualChunk.WorldPosition.X) * Constants.ChunkSize, (visualChunk.WorldPosition.Y) * Constants.ChunkSize, 0);

                        var chunkScreenPoint = camera.PointToScreen(new Point((int)chunkPosition.X, (int)chunkPosition.Y));

                        var chunkPositionMatrix = Matrix.CreateTranslation(new Vector3(chunkScreenPoint.X*Constants.ChunkSize, chunkScreenPoint.Y* Constants.ChunkSize, 0));
                        effect.Parameters["xWorld"].SetValue(chunkPositionMatrix * Matrix.CreateScale(1f/game.Settings.Zoom));

                        game.GraphicsDevice.SetVertexBuffer(visualChunk.TileModel.Buffer);
                        game.GraphicsDevice.Indices = visualChunk.TileModel.IndexBuffer;

                        foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                        {
                            pass.Apply();
                            game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, visualChunk.TileModel.Indices.Length / 3);
                          //  game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, tileModel.Vertices, 0, tileModel.Vertices.Length,
                          //      tileModel.Indices, 0, tileModel.Indices.Length / 3, this.VertexDeclaration);
                        }
                    }
                }

                game.Batch.Begin(samplerState: SamplerState.PointClamp, depthStencilState: DepthStencilState.Default);
                for (int x = 0; x < game.GetSettings().GetWidthZoomed(); x++)
                {
                    for (int y = 0; y < game.GetSettings().GetHeightZoomed(); y++)
                    {
                        Point screenPoint = camera.PointToScreen(x, y);

                        float alpha = 0;

                        var remembered = screen.ScreenBuffer[x, y].isRemembered;
                        var visible = screen.ScreenBuffer[x, y].isVisible;
                        if (visible)
                        {
                            //game.Batch.Draw(pixel, new Rectangle(x * fsz, y * fsz, fsz, fsz), new XNAColor(Microsoft.Xna.Framework.Color.Wheat, 1));
                            continue;
                        }
                        alpha = remembered ? 0.5f : 1;
                        game.Batch.Draw(pixel, new Rectangle(x * fsz, y * fsz, fsz, fsz), new XNAColor(Microsoft.Xna.Framework.Color.Black, alpha));

                    }
                }

                //game.Batch.End();


                if (!renderNewScreen)
                {
                    int stackDepth = 0;
                    while (RenderScreen(game, screen, game.GetSettings(), stackDepth))
                    {
                        stackDepth++;
                    }
                }

                //RenderSpriteShadows(game, screen, game.GetSettings(), gameTime);

                //game.Batch.Begin(samplerState: SamplerState.PointClamp);
                //RenderSpriteScreen(game, screen, game.GetSettings(), gameTime);
                //RenderProjectiles(game, screen, camera, game.GetSettings(), gameTime);
                //RenderSFX(game, screen, camera, game.GetSettings(), gameTime);
                //game.Batch.End();
            }

           

            game.GraphicsDevice.Clear(ClearOptions.DepthBuffer, new Microsoft.Xna.Framework.Color(1), 1, 0);
        }

        private void ProcessSXFCommands(NamelessGame game)
        {
            const int lightningSegmentLenght = 32;
            while (game.Commander.DequeueCommand(out SFXLightningCommand command))
            {        
                var directionVector = command.Start.ToPoint().ToVector2() - command.End.ToPoint().ToVector2();
                directionVector.Normalize();
                var angle = MathUtil.AngleBetween(Vector2.UnitX, directionVector);

                var dist = (command.Start - command.End).Length() * lightningSegmentLenght;
                var maxDist = dist;
                while (dist > 0)
                {
                    var lerpValue = (float)dist / maxDist;
                    var fromVector = new Vector2(command.Start.X, command.Start.Y);
                    var toVector = new Vector2(command.End.X, command.End.Y);
                    var interpolatedValue = Vector2.Lerp(fromVector, toVector, lerpValue);
                    lightningModels.Add(new SFXLightningModel() { screenLocation = interpolatedValue, rotation = (float)angle, timeToPlay = 4000 });
                    dist -= lightningSegmentLenght;
                }
            }
        }

        private static Screen UpdateZoom(NamelessGame game, Commander commander, IEntity entity, Screen screen, out bool zoomUpdate)
        {
            zoomUpdate = false;
            if (commander.DequeueCommand(out ZoomCommand zoom))
            {
                var settings = game.GetSettings();


                entity.RemoveComponent(screen);

                if (zoom.ZoomOut)
                {
                    if (settings.Zoom < 16)
                    {
                        settings.Zoom *= 2;
                    }
                    else
                    {
                        settings.Zoom = 1;
                    }
                }
                else
                {
                    if (settings.Zoom > 1)
                    {
                        settings.Zoom /= 2;
                    }
                    else
                    {
                        settings.Zoom = 16;
                    }
                }
                screen = new Screen(settings.GetWidthZoomed(), settings.GetHeightZoomed());
                entity.AddComponent(screen);
                zoomUpdate = true;
            }
            
            return screen;
        }


        private void MoveCamera(NamelessGame game, ConsoleCamera camera)
        {
                Position playerPosition = game.FollowedByCameraEntity
                    .GetComponentOfType<Position>();

                Point p = camera.getPosition();
            p.X = (playerPosition.Point.X - game.GetSettings().GetWidthZoomed() / 2);
            p.Y = (playerPosition.Point.Y - game.GetSettings().GetHeightZoomed() / 2);
            camera.setPosition(p);
        }
        PermissiveVisibility fov;
        Screen screenCopy;
        private void FillcharacterBufferVisibility(NamelessGame game, Screen screen, ConsoleCamera camera,
            GameSettings settings, IWorldProvider world)
        {
            int camX = camera.getPosition().X;
            int camY = camera.getPosition().Y;
            Position playerPosition = game.PlayerEntity.GetComponentOfType<Position>();
            BoundingBox b = new BoundingBox(camera.getPosition(),
                new Point(settings.GetWidthZoomed() + camX, settings.GetHeightZoomed() + camY));

            for (int x = 0; x < settings.GetWidthZoomed(); x++)
            {
                for (int y = 0; y < settings.GetHeightZoomed(); y++)
                {
                    screen.ScreenBuffer[x, y].isVisible = false;
                    screen.ScreenBuffer[x, y].isRemembered = false;
                }
            }
            screenCopy = screen;
            // return;

            for (int x = camX; x < settings.GetWidthZoomed() + camX; x++)
            {
                for (int y = camY; y < settings.GetHeightZoomed() + camY; y++)
                {
                    Point screenPoint = camera.PointToScreen(x, y);
                    var tile = world.GetTile(x, y, playerPosZ);
                    if (tile != null)
                    {
                        screen.ScreenBuffer[screenPoint.X, screenPoint.Y].isVisible = tile.IsVisible;
                        screen.ScreenBuffer[screenPoint.X, screenPoint.Y].isRemembered = tile.IsRemembered;
                    }
                }
            }
        }


        private void FillcharacterBuffersWithTileObjects(Screen screen, ConsoleCamera camera, GameSettings settings,
            NamelessGame game, GameTime gameTime, IWorldProvider world)
        {
           // return;
            int camX = camera.getPosition().X;
            int camY = camera.getPosition().Y;
            if (angle > 360)
            {
                angle = 0;
            }

            angle += step;

            for (int x = camX; x < settings.GetWidthZoomed() + camX; x++)
            {
                for (int y = camY; y < settings.GetHeightZoomed() + camY; y++)
                {
                    Point screenPoint = camera.PointToScreen(x, y);
                    if (screen.ScreenBuffer[screenPoint.X, screenPoint.Y].isRemembered && x > 0 && y > 0)
                    {
                        Tile tileToDraw = world.GetTile(x, y, playerPosZ);

                        if (tileToDraw != null)
                        {
                            foreach (var entity in tileToDraw.GetEntities())
                            {
                                var item = entity.GetComponentOfType<Item>();                              
                                var drawable = entity.GetComponentOfType<Drawable>();
                                var sprited = entity.GetComponentOfType<SpritedObject>();


                                var character  = entity.GetComponentOfType<Character>();
                                if(character!=null)
                                {
                                    continue;
                                }

                                if (drawable != null && sprited == null)
                                {
                                    screen.ScreenBuffer[screenPoint.X, screenPoint.Y].AddObject(drawable.ObjectID + drawable.TilesetPosition, ScreenObjectSource.Tileset, drawable.CharColor,  drawable.CastsShadow, drawable.IsFlying);
                                }
                                else if(drawable != null && sprited != null)
                                {
                                    if (sprited.IsStatic)
                                    {
                                        screen.ScreenBuffer[screenPoint.X, screenPoint.Y].AddObject(drawable.ObjectID + drawable.TilesetPosition, ScreenObjectSource.StaticSprite, drawable.CharColor, drawable.CastsShadow, drawable.IsFlying);
                                    }
                                    else
                                    {
                                        var animation = sprited.IdleAnimation;
                                        if (sprited.CurrentAnimationTimeLeft > 0)
                                        {
                                            animation = sprited.CurrentAnimation;
                                            sprited.CurrentAnimationTimeLeft -= gameTime.ElapsedGameTime.Milliseconds;
                                        }

                                        if(sprited.InfinteAnimation)
                                        {
                                            sprited.CurrentAnimationTimeLeft = 1000;
                                        }
                                        screen.ScreenBuffer[screenPoint.X, screenPoint.Y].AddObject(drawable.ObjectID + drawable.TilesetPosition, ScreenObjectSource.AnimatedSprite, drawable.CharColor, drawable.CastsShadow, drawable.IsFlying, false, sprited.CurrentAnimationTimeLeft, animation);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void FillcharacterBuffersWithWorld(Screen screen, ConsoleCamera camera, GameSettings settings,
            IWorldProvider world)
        {
            int camX = camera.getPosition().X;
            int camY = camera.getPosition().Y;
            if (angle > 360)
            {
                angle = 0;
            }

            angle += step;

            for (int x = camX; x < settings.GetWidthZoomed() + camX; x++)
            {
                for (int y = camY; y < settings.GetHeightZoomed() + camY; y++)
                {
                    Point screenPoint = camera.PointToScreen(x, y);
                    if (screen.ScreenBuffer[screenPoint.X, screenPoint.Y].isRemembered && x>0 && y>0)
                    {
                        Tile tileToDraw = world.GetTile(x, y, playerPosZ);

                        if (tileToDraw != null && tileToDraw.Terrain!=TerrainTypes.Nothingness)
                        {
                            GetTerrainTile(screen, TerrainLibrary.Terrains[tileToDraw.Terrain], screenPoint);
                        }
                        else
                        {
                            var currentElevation = playerPosZ;
                            while (currentElevation > 0)
                            {
                                currentElevation--;
                                tileToDraw = world.GetTile(x, y, currentElevation);
                                if (tileToDraw != null || tileToDraw.Terrain != TerrainTypes.Nothingness)
                                {
                                    GetTerrainBlurredTile(screen, TerrainLibrary.Terrains[tileToDraw.Terrain], screenPoint);
                                }
                            }
                            if (tileToDraw == null || tileToDraw.Terrain == TerrainTypes.Nothingness)
                            {
                                screen.ScreenBuffer[screenPoint.X, screenPoint.Y].AddObject("Nothingness", ScreenObjectSource.Tileset, new Color(), false, false);
                            }
                        }

                    }
                    else
                    {
                        screen.ScreenBuffer[screenPoint.X, screenPoint.Y].AddObject("Nothingness", ScreenObjectSource.Tileset, new Color(), false, false);
                    }
                }
            }
        }

        public void ClearScreen(Screen screen, ConsoleCamera camera, GameSettings settings,
            IWorldProvider world)
        {
            int camX = camera.getPosition().X;
            int camY = camera.getPosition().Y;
            if (angle > 360)
            {
                angle = 0;
            }

            angle += step;

            for (int x = camX; x < settings.GetWidthZoomed() + camX; x++)
            {
                for (int y = camY; y < settings.GetHeightZoomed() + camY; y++)
                {
                    Point screenPoint = camera.PointToScreen(x, y);

                    screen.ScreenBuffer[screenPoint.X, screenPoint.Y].StackedObjects.Clear();

                }
            }
        }

        void GetTerrainTile(Screen screen, Terrain terrain, Point point)
        {
            screen.ScreenBuffer[point.X, point.Y].AddObject(terrain.Representation.ObjectID, ScreenObjectSource.Tileset, new Color(255,255,255), false, false, false);
        }

        void GetTerrainBlurredTile(Screen screen, Terrain terrain, Point point)
        {
            screen.ScreenBuffer[point.X, point.Y].AddObject(terrain.Representation.ObjectID, ScreenObjectSource.Tileset, new Color(255, 255, 255), false, false, false);
        }

        private void FillcharacterBuffersWithWorldObjects(Screen screen, ConsoleCamera camera, GameSettings settings,
            NamelessGame game, GameTime gameTime)
        {
            {
                var cursorEntity = game.CursorEntity;
                Position cursorPosition = cursorEntity.GetComponentOfType<Position>(); 
                Position playerPosition = game.PlayerEntity.GetComponentOfType<Position>();

                LineToPlayer lineToPlayer = cursorEntity.GetComponentOfType<LineToPlayer>();
                Drawable cursorDrawable = cursorEntity.GetComponentOfType<Drawable>();
                var targeter = game.TargeterEntity.GetComponentOfType<TergeterComponent>();
                if (cursorDrawable.Visible)
                {
                    {                      
                        var distance = (cursorPosition.Point - playerPosition.Point).Length();
                        var screenPoint = camera.PointToScreen(cursorPosition.X, cursorPosition.Y);
                        int x = screenPoint.X;
                        int y = screenPoint.Y;
                        if (x >= 0 && x < settings.GetWidthZoomed() && y >= 0 && y < settings.GetHeightZoomed())
                        {
                            if (screen.ScreenBuffer[screenPoint.X, screenPoint.Y].isVisible)
                            {

                                var color = new Color(255, 255, 255);

                                if (distance > targeter.CurrentTargetingRange)
                                {
                                    color = new Color(255, 0, 0);
                                }

                                screen.ScreenBuffer[screenPoint.X, screenPoint.Y].AddObject(cursorDrawable.ObjectID, ScreenObjectSource.Tileset, color, false, false);

                            }
                        }
                    }

                    if (lineToPlayer != null)
                    {
                     
                        List<Point> line = PointUtil.getLine(playerPosition.Point.ToPoint(), cursorPosition.Point.ToPoint());
                        for (int i = 0; i < line.Count - 1; i++)
                        {

                            Point p = new Point(line[i].X, line[i].Y);
                            Point screenPoint = camera.PointToScreen(p.X, p.Y);

                            var distance = (p - playerPosition.Point.ToPoint()).ToVector2().Length();

                            int x = screenPoint.X;
                            int y = screenPoint.Y;
                            if (x >= 0 && x < settings.GetWidthZoomed() && y >= 0 && y < settings.GetHeightZoomed())
                            {

                                var color = new Color(255, 255, 255);
                                if (distance > targeter.CurrentTargetingRange)
                                {
                                    color = new Color(255, 0, 0);
                                }

                                string objectId = i == (line.Count() - 1) ? "Cursor" : "smallCursor";
                                screen.ScreenBuffer[screenPoint.X, screenPoint.Y].AddObject(objectId, ScreenObjectSource.Tileset, color, false, false);
   
                            }
                        }
                    }
                }
            }

            List<IEntity> characters = new List<IEntity>();
            List<IEntity> selectorCursors = new List<IEntity>();
            foreach (IEntity entity in RegisteredEntities)
            {
                Drawable drawable = entity.GetComponentOfType<Drawable>();

                var character = entity.GetComponentOfType<Character>();
                var characterPosition = entity.GetComponentOfType<Position>();

                if (character != null && characterPosition.Z == playerPosZ)
                {
                    characters.Add(entity);
                    continue;
                }

                var interactionSelectorLink = entity.GetComponentOfType<InteractionSelectorLink>();
                if (interactionSelectorLink != null)
                {
                    selectorCursors.Add(entity);
                    continue;
                }
            }

            foreach (IEntity entity in characters)
            {
                Drawable drawable = entity.GetComponentOfType<Drawable>();

                if (drawable == null)
                {
                    continue;
                }

                Position position = entity.GetComponentOfType<Position>();
                if (drawable.Visible)
                {
                    Point screenPoint = camera.PointToScreen(position.X, position.Y);
                    int x = screenPoint.X;
                    int y = screenPoint.Y;
                    if (x >= 0 && x < settings.GetWidthZoomed() && y >= 0 && y < settings.GetHeightZoomed())
                    {
                        if (screen.ScreenBuffer[screenPoint.X, screenPoint.Y].isVisible)
                        {
                            var sprited = entity.GetComponentOfType<SpritedObject>();
                            if (sprited == null)
                            {
                                screen.ScreenBuffer[screenPoint.X, screenPoint.Y].AddObject("Character", ScreenObjectSource.Tileset, drawable.CharColor, false, false);
                            }
                            else
                            {
                                var animation = sprited.IdleAnimation;

                                if(sprited.CurrentAnimationTimeLeft>0)
                                {
                                    animation = sprited.CurrentAnimation;
                                    sprited.CurrentAnimationTimeLeft -= gameTime.ElapsedGameTime.Milliseconds;
                                }

                                if (entity.GetComponentOfType<Dead>() != null)
                                {
                                    screen.ScreenBuffer[screenPoint.X, screenPoint.Y].AddObjectToBottom(drawable.ObjectID + drawable.TilesetPosition, ScreenObjectSource.AnimatedSprite, drawable.CharColor, drawable.CastsShadow, drawable.IsFlying, sprited.CurrentAnimationTimeLeft, animation);

                                }
                                else
                                {
                                    screen.ScreenBuffer[screenPoint.X, screenPoint.Y].AddObject(drawable.ObjectID + drawable.TilesetPosition, ScreenObjectSource.AnimatedSprite, drawable.CharColor, drawable.CastsShadow, drawable.IsFlying, false,  sprited.CurrentAnimationTimeLeft, animation);
                                }
                            }
                        }
                        else                                               
                        {
                            //screen.ScreenBuffer[screenPoint.X, screenPoint.Y].AddObject("Nothingness", ScreenObjectSource.Tileset);
                        }
                    }

                }
            }

            foreach (IEntity entity in selectorCursors) 
            {
                Drawable drawable = entity.GetComponentOfType<Drawable>();

                if (drawable == null)
                {
                    continue;
                }

                Position position = entity.GetComponentOfType<Position>();
                if (drawable.Visible)
                {
                    Point screenPoint = camera.PointToScreen(position.X, position.Y);
                    int x = screenPoint.X;
                    int y = screenPoint.Y;
                    if (x >= 0 && x < settings.GetWidthZoomed() && y >= 0 && y < settings.GetHeightZoomed())
                    {
                        if (screen.ScreenBuffer[screenPoint.X, screenPoint.Y].isVisible)
                        {
                            screen.ScreenBuffer[screenPoint.X, screenPoint.Y].AddObject("Cursor", ScreenObjectSource.Tileset, drawable.CharColor, false, false);
                        }
                        else
                        {
                            //screen.ScreenBuffer[screenPoint.X, screenPoint.Y].AddObject("Nothingness", ScreenObjectSource.Tileset);
                        }
                    } 
                }
            }
        }
        private void RenderProjectiles(NamelessGame game, Screen screen, ConsoleCamera camera, GameSettings settings, GameTime gameTime)
        {
            foreach (IEntity entity in RegisteredEntities)
            {
                var projectileComponent = entity.GetComponentOfType<ProjectileComponent>();
                if (projectileComponent != null)
                {
                    Drawable drawable = entity.GetComponentOfType<Drawable>();
                    var spriteId = drawable.ObjectID;
                    if (SpriteLibrary.SpritesStatic.TryGetValue(spriteId, out var sprite))
                    {
                        int tileHeight = game.GetSettings().GetFontSizeZoomed();
                        int tileWidth = game.GetSettings().GetFontSizeZoomed();
                        var lerpValue = (float)projectileComponent.CurrentFrame / (float)projectileComponent.FramesToReachDestination;
                        var fromVector = projectileComponent.From.ToPoint().ToVector2();
                        var toVector = projectileComponent.To.ToPoint().ToVector2();
                        var interpolatedValue = Vector2.Lerp(fromVector, toVector, lerpValue);
                        Vector2 screenPoint = new Vector2((interpolatedValue.X - camera.Position.X) * tileWidth, (interpolatedValue.Y - camera.Position.Y) * tileHeight);
                        var rect = new Rectangle(screenPoint.ToPoint(), new Vector2(tileWidth, tileHeight).ToPoint());
                        game.Batch.Draw(sprite.TextureRegion, rect, Microsoft.Xna.Framework.Color.White);
                     //   sprite.Draw(game, gameTime, new Vector2((x * tileWidth) + 5, (y * tileHeight) + 5), new Vector2(tileWidth, tileHeight), new Vector2(1f), Microsoft.Xna.Framework.Color.Black);

                        //        sprite.Draw(game.Batch, screenPoint, MathHelper.ToRadians(angle), );
                    }
                }
            }
        }

        private void RenderSFX(NamelessGame game, Screen screen, ConsoleCamera camera, GameSettings settings, GameTime gameTime)
        {
            int tileHeight = game.GetSettings().GetFontSizeZoomed();

            List<SFXLightningModel> list = lightningModels.ToList();
            for (int lightningIndex = 0; lightningIndex < list.Count; lightningIndex++)
            {
                SFXLightningModel lightningModel = list[lightningIndex];
                var polygonVertices = new List<Vector2>();
                var rotatedPolygon = new List<Vector2>();
                //skip this to let the last segment be in the center of the target
                if (lightningIndex > 0)
                {
                    polygonVertices.Add(new Vector2(0, tileHeight / 2));
                    polygonVertices.Add(new Vector2(tileHeight * 0.2f, (float)(Random.Shared.NextDouble() - 0.5) * (tileHeight / 2) + tileHeight / 2));
                }
                polygonVertices.Add(new Vector2(tileHeight * 0.5f, (float)(Random.Shared.NextDouble() - 0.5) * (tileHeight / 2) + tileHeight / 2));
                polygonVertices.Add(new Vector2(tileHeight * 0.8f, (float)(Random.Shared.NextDouble() - 0.5) * (tileHeight / 2) + tileHeight / 2));
                polygonVertices.Add(new Vector2(tileHeight, tileHeight / 2));

                var origin = new Vector2(tileHeight / 2, tileHeight / 2);
                for (int i = 0; i < polygonVertices.Count; i++)
                {
                    var point = polygonVertices[i];
                    point.RotateAround(origin, MathHelper.ToRadians(lightningModel.rotation));
                    rotatedPolygon.Add(point);
                }

                int tileWidth = game.GetSettings().GetFontSizeZoomed();
                Vector2 screenPoint = new Vector2((lightningModel.screenLocation.X - camera.Position.X) * tileWidth, (lightningModel.screenLocation.Y - camera.Position.Y) * tileHeight);

                for (int i = 0; i < rotatedPolygon.Count-1; i++) {
                    var pointA = rotatedPolygon[i];
                    var pointB = rotatedPolygon[i+1];                    
                    game.Batch.DrawLine(screenPoint + pointA, screenPoint + pointB, Microsoft.Xna.Framework.Color.Blue, 4);
                    game.Batch.DrawLine(screenPoint + pointA, screenPoint + pointB, Microsoft.Xna.Framework.Color.White, 2);
                }             

                lightningModel.timeToPlay -= gameTime.ElapsedGameTime.Milliseconds;
                if(lightningModel.timeToPlay<=0)
                {
                    lightningModels.Remove(lightningModel);
                }
            }         

          //  _particleEffect.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

        }

        private bool RenderScreen(NamelessGame game, Screen screen, GameSettings settings, int stackDepth)
        {
            bool moreItemsToRender = false;
            effect.Parameters["tileAtlas"].SetValue(tileAtlas);
            var projectionMatrix = //Matrix.CreateOrthographic(game.getActualWidth(),game.getActualHeight(),0,1);
    Matrix.CreateOrthographicOffCenter(0, game.GetActualWidth(), game.GetActualHeight(), 0, 0, 2);

            effect.Parameters["xViewProjection"].SetValue(projectionMatrix);

            effect.GraphicsDevice.SamplerStates[0] = sampler;
            effect.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            var device = game.GraphicsDevice;
            //Stopwatch s = Stopwatch.StartNew();
            for (int y = 0; y < settings.GetHeightZoomed(); y++)
            {
                for (int x = 0; x < settings.GetWidthZoomed(); x++)
                {
                    if (screen.ScreenBuffer[x, y].StackedObjects.Count > stackDepth)
                    {
                        moreItemsToRender = true;
                        var objectToDraw = screen.ScreenBuffer[x, y].StackedObjects[stackDepth];
                        if (objectToDraw.Type == ScreenObjectSource.Tileset)
                        {
                            var objectId = objectToDraw.Id;
                            AtlasTileData tileData;
                            if (!characterToTileDictionary.TryGetValue(objectId, out tileData))
                            {
                                characterToTileDictionary.TryGetValue("Nothingness", out tileData);
                            }
                            var white = new Color(1f, 1f, 1f, 1f);
                            var grey = new Color(0.5f, 0.5f, 0.5f, 1f);

                            var tileMask = screen.ScreenBuffer[x, y].isRemembered && screen.ScreenBuffer[x, y].isVisible ? objectToDraw.CharColor : grey;


                            int tileHeight = 64;
                            int tileWidth = 64;

                      

                            //DrawTile(tileHeight, tileWidth, x, y,
                            //     game.Settings.GetWidthZoomed(),
                            //    x * settings.GetFontSizeZoomed(),
                            //    y * settings.GetFontSizeZoomed(),                                
                            //    tileData,                            
                            //    tileMask,
                            //    tileMask,
                            //    foregroundModel, 
                            //    tileAtlas);
                        }
                    }                 
                }
            }

			effect.CurrentTechnique = effect.Techniques["Point"];

            //var tileModel = foregroundModel;

            //foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            //{
            //    pass.Apply();
            //    device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, tileModel.Vertices, 0, tileModel.Vertices.Length,
            //         tileModel.Indices.ToArray(), 0, tileModel.Indices.Count()/3, VertexDeclaration);
            //}
            return moreItemsToRender;
        }
        
        private void RenderSpriteScreen(NamelessGame game, Screen screen, GameSettings settings, GameTime gameTime)
        {
            for (int y = 0; y < settings.GetHeightZoomed(); y++)
            {
                for (int x = 0; x < settings.GetWidthZoomed(); x++)
                {
                    foreach (var objectToDraw in screen.ScreenBuffer[x, y].StackedObjects)
                    {
                        if (objectToDraw.Type == ScreenObjectSource.AnimatedSprite || objectToDraw.Type == ScreenObjectSource.StaticSprite)
                        {
                            var spriteId = objectToDraw.Id;
                            int tileHeight = game.GetSettings().GetFontSizeZoomed();
                            int tileWidth = game.GetSettings().GetFontSizeZoomed();
                            if (objectToDraw.Type == ScreenObjectSource.AnimatedSprite)
                            {
                                if (SpriteLibrary.SpritesAnimated.TryGetValue(spriteId, out var sprite))
                                {
                                    sprite.SetCurrentLoopWithTimeConstrains(objectToDraw.AnimationName, objectToDraw.AnimationTime);
                                   // sprite.Update(gameTime);
                                    sprite.Draw(game, gameTime, new Vector2(x * tileWidth, y * tileHeight), new Vector2(tileWidth, tileHeight), new Vector2(1f), Microsoft.Xna.Framework.Color.White);
                                }
                            }
                            else
                            {
                                if (SpriteLibrary.SpritesStatic.TryGetValue(spriteId, out var sprite))
                                {
                                    {
                                        var position = new Vector2((x * tileWidth), (y * tileHeight));
                                        var size = new Vector2(tileWidth, tileHeight);
                                        var scale = Vector2.One;
                                        game.Batch.Draw(sprite.TextureRegion, new Microsoft.Xna.Framework.Rectangle(position.ToPoint(), (size * scale).ToPoint()), Microsoft.Xna.Framework.Color.White);
                                    }
                                }
                            }                                           
                        }
                    }
                }
            }
        }

        private void RenderSpriteShadows(NamelessGame game, Screen screen, GameSettings settings, GameTime gameTime)
        {
            for (int y = 0; y < settings.GetHeightZoomed(); y++)
            {
                for (int x = 0; x < settings.GetWidthZoomed(); x++)
                {
                    foreach (var objectToDraw in screen.ScreenBuffer[x, y].StackedObjects)
                    {

                        if (objectToDraw.Type == ScreenObjectSource.AnimatedSprite || objectToDraw.Type == ScreenObjectSource.StaticSprite)
                        {
                            var spriteId = objectToDraw.Id;
                            int tileHeight = game.GetSettings().GetFontSizeZoomed();
                            int tileWidth = game.GetSettings().GetFontSizeZoomed();
                            if (objectToDraw.Type == ScreenObjectSource.AnimatedSprite)
                            {
                                if (SpriteLibrary.SpritesAnimated.TryGetValue(spriteId, out var sprite))
                                {
                                    sprite.SetCurrentLoopWithTimeConstrains(objectToDraw.AnimationName, objectToDraw.AnimationTime);
                                    sprite.Update(gameTime);
                                    int shadowOffset = 10 / game.GetSettings().Zoom;
                                    if (objectToDraw.HasShadow)
                                    {
                                        Vector2 positionOnScreen = new Vector2((x * tileWidth) + tileWidth * 0.4f, (y * tileHeight) + tileHeight / 4);
                                        if (objectToDraw.IsFlying)
                                        {
                                            positionOnScreen = new Vector2((x * tileWidth) + tileWidth * 0.4f, (y * tileHeight) + tileHeight / 4);
                                            game.Batch.Begin(samplerState: SamplerState.PointClamp);
                                        }
                                        else
                                        {
                                            positionOnScreen = new Vector2((x * tileWidth) + tileWidth * 0.4f, (y * tileHeight) + tileHeight / 4);
                                            var angleX = -45;
                                            Matrix slant = Matrix.CreateTranslation(-positionOnScreen.X, -positionOnScreen.Y, 0f) *
                                            Matrix.CreateRotationX(MathHelper.ToRadians(angleX)) *
                                            Matrix.CreateRotationY(MathHelper.ToRadians(30)) *
                                            Matrix.CreateScale(1.4f, 1f, 0) *
                                            Matrix.CreateTranslation(positionOnScreen.X, positionOnScreen.Y, 0f);
                                            game.Batch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: slant);
                                        }
                                       
                                        sprite.Draw(game, gameTime, positionOnScreen, new Vector2(tileWidth, tileHeight), new Vector2(1f), shadowColor);
                                        game.Batch.End();
                                    }
                                }
                            }
                            else
                            {
                                if (SpriteLibrary.SpritesStatic.TryGetValue(spriteId, out var sprite))
                                {
                                    int shadowOffset = 10 / game.GetSettings().Zoom;
                                    if (objectToDraw.HasShadow)
                                    {

                                        var position = new Vector2((x * tileWidth) + shadowOffset, (y * tileHeight) + shadowOffset);
                                        var size = new Vector2(tileWidth, tileHeight);
                                        var scale = Vector2.One;
                                        game.Batch.Draw(sprite.TextureRegion, new Microsoft.Xna.Framework.Rectangle(position.ToPoint(), (size * scale).ToPoint()), shadowColor);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public class AtlasTileData
        {
            public int X;
            public int Y;

            public AtlasTileData(int x, int y, bool mirrorVertical = false, bool mirrorHorizontal = false)
            {
                X = x;
                Y = y;
                MirrorVerical = mirrorVertical;
                MirrorHorizontal = mirrorHorizontal;
            }

            public bool MirrorVerical;
            public bool MirrorHorizontal;
        }

        Texture2D tileAtlas = null;
        private Texture2D _particleTexture;
        private ParticleEffect _particleEffect;
        private Texture2D pixel;

        private Microsoft.Xna.Framework.Graphics.Texture InitializeTexture(NamelessGame game)
        {

            pixel = new Texture2D(game.GraphicsDevice, 1, 1);
            pixel.SetData<XNAColor>(new XNAColor[] { XNAColor.White });

            tileAtlas = null;
            tileAtlas = game.Content.Load<Texture2D>("Sprites/tileset2");            
            effect = game.Content.Load<Effect>("Shader");

            effect.Parameters["tileAtlas"].SetValue(tileAtlas);

            _particleTexture = new Texture2D(game.GraphicsDevice, 1, 1);
            _particleTexture.SetData(new[] { XNAColor.White });
            Texture2DRegion textureRegion = new Texture2DRegion(_particleTexture);


            _particleEffect = new ParticleEffect()
            {
                Position = new Vector2(400, 240),
                Emitters = new List<ParticleEmitter>
                {
                    new ParticleEmitter(textureRegion, 500, TimeSpan.FromSeconds(2.5),
                        Profile.BoxFill(32,32))
                    {
                        Parameters = new ParticleReleaseParameters
                        {
                            Speed = new Range<float>(0f, 25),
                            Quantity = 3,
                            Rotation = new Range<float>(-1f, 1f),
                            Scale = new Range<float>(1f, 2f)
                        },
                        Modifiers =
                        {
                            new AgeModifier
                            {
                                Interpolators =
                                {
                                    new ColorInterpolator
                                    {
                                        StartValue = Microsoft.Xna.Framework.Color.White.ToHsl(),
                                        EndValue =  Microsoft.Xna.Framework.Color.Gray.ToHsl(),
                                    }
                                }
                            },
                            new RotationModifier {RotationRate = -2.1f},
                            new DragModifier(){ Density = 1f, DragCoefficient = 1f},
                            new LinearGravityModifier {Direction = -Vector2.UnitY, Strength = 30f},
                            new OpacityFastFadeModifier(),
                            new VortexModifier()
                            {
                                Mass = 10f,
                                MaxSpeed = 1f,
                                Position = new Vector2(0,-15)
                            },
                            new VortexModifier()
                            {
                                Mass = 10f,
                                MaxSpeed = 1f,
                                Position = new Vector2(-15,0)
                            }
                        }
                    }
                }
            };

            return tileAtlas;
        }


        public static void DrawTile(int tileHeight, int tileWidth, int screenPositionX, int screenPositionY, int screenWidth, int positionX, int positionY,
    AtlasTileData atlasTileData,
    Color color, Color backGroundColor, TileModel foregroundModel, Texture2D tileAtlas)
        {

            if (atlasTileData == null)
            {
                atlasTileData = new AtlasTileData(1, 1);
            }        

            float textureX = atlasTileData.X * (Constants.tileAtlasTileSize / (float)tileAtlas.Width);
            float textureY = atlasTileData.Y * (Constants.tileAtlasTileSize / (float)tileAtlas.Height);

            float textureXend = (atlasTileData.X + 1f) * (Constants.tileAtlasTileSize / (float)tileAtlas.Width);

            float textureYend = (atlasTileData.Y + 1f) * (Constants.tileAtlasTileSize / (float)tileAtlas.Height);

            var arrayPosition = (screenPositionX * 4) + (screenPositionY * screenWidth * 4);


            if (atlasTileData.MirrorVerical)
            {
                var temp = textureX;
                textureX = textureXend;
                textureXend = temp;
            }
            if (atlasTileData.MirrorHorizontal)
            {
                var temp = textureY;
                textureY = textureYend;
                textureYend = temp;
            }


            var foregroundvertices = foregroundModel.Vertices;

            foregroundvertices[arrayPosition] = new Vertex(new Vector3(positionX, positionY, 0), color.ToVector4(),
                backGroundColor.ToVector4(), new Vector2(textureX, textureY));
            foregroundvertices[arrayPosition + 1] = new Vertex(new Vector3(positionX + tileWidth, positionY, 0), color.ToVector4(),
                backGroundColor.ToVector4(), new Vector2(textureXend, textureY));
            foregroundvertices[arrayPosition + 2] = new Vertex(new Vector3(positionX, positionY + tileHeight, 0), color.ToVector4(),
                backGroundColor.ToVector4(), new Vector2(textureX, textureYend));
            foregroundvertices[arrayPosition + 3] = new Vertex(new Vector3(positionX + tileWidth, positionY + tileHeight, 0), color.ToVector4(),
                backGroundColor.ToVector4(), new Vector2(textureXend, textureYend));

           

        } 
    }
}
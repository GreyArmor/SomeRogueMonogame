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
using AsepriteDotNet.Common;
using MonoGame.Extended.Timers;
using static NamelessRogue.Engine.Systems.Ingame.RenderingSystem;
using static log4net.Appender.ColoredConsoleAppender;

namespace NamelessRogue.Engine.Systems.Ingame
{

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexTileModel
    {
        // ReSharper disable NotAccessedField.Local
        private Vector3 position;
        private Vector2 textureCoordinate;

        public VertexTileModel(Vector3 position, Vector2 textureCoordinate)
        {
            this.position = position;
            this.textureCoordinate = textureCoordinate;
        }
    }


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

    public class SFXLightningModel
    {
        public int timeToPlay;
        public float rotation;
        public Vector2 screenLocation;
    }



    public class TileModel : IDisposable {
        public VertexTileModel[] Vertices { get; }
        public int[] Indices { get; }
        public VertexBuffer Buffer { get; set; }
        public IndexBuffer IndexBuffer { get; set; }
        public TileModel(int height, int width)
        {
            Vertices = new VertexTileModel[height * width * 4];
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

            Buffer = new VertexBuffer(device, RenderingSystem.TileModelVertexDeclaration, Vertices.Length, BufferUsage.None);
            IndexBuffer = new IndexBuffer(device, IndexElementSize.ThirtyTwoBits, Indices.Length, BufferUsage.None);

            Buffer.SetData(Vertices);
            IndexBuffer.SetData(Indices);
        }
    }

    public class VisualChunk
    {
        public List<TileModel> TileLayers { get; set; }
        public Vector3Int WorldPosition { get; }
        public ScreenTile[,] ScreenBuffer { get; private set; }
        public VisualChunk(Vector3Int worldPosition)
        {
            var size = Constants.ChunkSize;
            WorldPosition = worldPosition;
            ScreenBuffer = new ScreenTile[size, size];
            TileLayers = new List<TileModel>();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    ScreenBuffer[i, j] = new ScreenTile();
                }
            }
        }

        public void UpdateChunk(Dictionary<string, RenderingSystem.AtlasTileData> characterToTileDictionary, Texture2D tileAtlas, NamelessGame game)
        {

            var worldProvider = game.WorldProvider;


            for (int x = 0; x < Constants.ChunkSize; x++)
            {
                for (int y = 0; y < Constants.ChunkSize; y++)
                {
                    ScreenBuffer[x, y].StackedObjects.Clear();
                }
            }


            FillWithWorld(worldProvider, WorldPosition.Z);

            var size = Constants.ChunkSize;
            var currentDepth = 0;
            for (int y = 0; y < Constants.ChunkSize; y++)
            {
                for (int x = 0; x < Constants.ChunkSize; x++)
                {
                    var depth = ScreenBuffer[x, y].StackedObjects.Count();

                    if (currentDepth < depth)
                    {
                        for (; currentDepth < depth; currentDepth++)
                        {
                            var tileModel = new TileModel(size, size);
                            this.TileLayers.Add(tileModel);
                        }
                    }

                    for (int depthIndex = 0; depthIndex < depth; depthIndex++)
                    {
                        var tileModel = TileLayers[depthIndex];
                        var objectToDraw = ScreenBuffer[x, y].StackedObjects[depthIndex];
                        if (objectToDraw.Type == ScreenObjectSource.Tileset)
                        {
                            var objectId = objectToDraw.Id;
                            RenderingSystem.AtlasTileData tileData;
                            if(objectId.Contains("door"))
                            {
                                objectId.ToString();
                            }
                            if (!characterToTileDictionary.TryGetValue(objectId, out tileData))
                            {
                                characterToTileDictionary.TryGetValue("Nothingness", out tileData);
                            }

                            int tileHeight = 64;
                            int tileWidth = 64;

                            RenderingSystem.DrawTile(tileHeight, tileWidth, x, y, Constants.ChunkSize,
                                    x * Constants.ChunkSize,
                                    y * Constants.ChunkSize,
                                    tileData, tileModel, tileAtlas);
                        }
                    }
                }
            }

            foreach (TileModel tileModel in TileLayers)
            {
                tileModel.UpdateBuffers(game.GraphicsDevice);
            }
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

                    if (tileToDraw != null)
                    {
                        foreach (var entity in tileToDraw.GetEntities())
                        {
                            var item = entity.GetComponentOfType<Item>();
                            var drawable = entity.GetComponentOfType<Drawable>();
                            var sprited = entity.GetComponentOfType<AnimatedSpriteObject>();


                            var character = entity.GetComponentOfType<Character>();
                            if (character != null)
                            {
                                continue;
                            }

                            if (drawable != null && sprited == null)
                            {
                                ScreenBuffer[x, y].AddObject(drawable.ObjectID + tileToDraw.TilesetPosition, ScreenObjectSource.Tileset, drawable.CharColor, drawable.CastsShadow, drawable.IsFlying);
                            }
                            else if (drawable != null && sprited != null)
                            {
                                //if (sprited.IsStatic)
                                //{
                                //    ScreenBuffer[screenPoint.X, screenPoint.Y].AddObject(drawable.ObjectID + drawable.TilesetPosition, ScreenObjectSource.StaticSprite, drawable.CharColor, drawable.CastsShadow, drawable.IsFlying);
                                //}
                            }
                        }
                    }
                }
            }
        }
    }


    public class VisibilityModel
    {
        public TileModel TileModel { get; set; }
        public Texture2D VisibilityMask { get; set; }

        private XNAColor[] colors { get; set; }

        private XNAColor grey { get; set; } = new XNAColor(0, 0, 0, 0.5f);
        private XNAColor black { get; set; } = new XNAColor(0, 0, 0, 1f);
        private XNAColor transprent { get; set; } = new XNAColor(0, 0, 0, 0f);
        public VisibilityModel(int width, int height, NamelessGame game)
        {
            TileModel = new TileModel(width, height);
            var worldProvider = game.WorldProvider;

            int tileHeight = game.Settings.GetFontSizeZoomed();
            int tileWidth = game.Settings.GetFontSizeZoomed();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var tileMask = new Color(0);
                    RenderingSystem.DrawVisibilityTile(tileHeight, tileWidth, x, y, width, height,
                            x * tileWidth,
                            y * tileWidth,
                            tileMask, TileModel);
                }
            }

            colors = new XNAColor[height*width];

            VisibilityMask = new Texture2D(game.GraphicsDevice, width, height);
            TileModel.UpdateBuffers(game.GraphicsDevice);
        }

        public void UpdateVisibility(Screen screen, NamelessGame game)
        {
            int tileHeight = game.Settings.GetFontSizeZoomed();
            int tileWidth = game.Settings.GetFontSizeZoomed();
            for (int y = 0; y < screen.Height; y++)
            {
                for (int x = 0; x < screen.Width; x++)
                {
                    var isVisible = screen.ScreenBuffer[x, y].isVisible;

                    if (!isVisible)
                    {
                        var isRemembered = screen.ScreenBuffer[x, y].isRemembered;
                        var tileMask = isRemembered ? grey:black;
                        colors[y*screen.Width + x] = tileMask;
                    }
                    else
                    {
                        colors[y * screen.Width + x] = transprent;
                    }
                }
            }
            VisibilityMask.SetData(colors);
        }
    }



    public class UpdateVisualChunkCommand : ICommand
    {
        public UpdateVisualChunkCommand(Vector3Int chunkCoordinate)
        {
            ChunkCoordinate = chunkCoordinate;
        }

        public Vector3Int ChunkCoordinate { get; }
    }

    public class RenderingSystem : BaseSystem
    {
        public override HashSet<Type> Signature { get; }

        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector4, VertexElementUsage.Color, 0),
            new VertexElement(sizeof(float) * 7, VertexElementFormat.Vector4, VertexElementUsage.Color, 1),
            new VertexElement(sizeof(float) * 11, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
        );

       public static readonly VertexDeclaration TileModelVertexDeclaration = new VertexDeclaration
       (
           new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
           new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
       );

        Dictionary<string, AtlasTileData> characterToTileDictionary;
        private float gameTime;
        private float angle = 0;
        private float step = 0.04f;
        private InternalRandom graphicalRandom = new InternalRandom();
        Effect effect;
        private VertexBuffer visibilityVertexBuffer;
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

        public RenderingSystem(GameSettings settings) {
            InitializeCharacterTileDictionary();

            Signature = [typeof(Drawable), typeof(Position), typeof(AnimatedSpriteObject)];


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

            characterToTileDictionary.Add("arrowDown", new AtlasTileData(1, 7));

            characterToTileDictionary.Add("wall", atlasTileData);
            characterToTileDictionary.Add("door", atlasTileData);
            characterToTileDictionary.Add("window", atlasTileData);
            characterToTileDictionary.Add("stairs_down", new AtlasTileData(8, 0));
            characterToTileDictionary.Add("stairs_up", new AtlasTileData(8, 0));

            characterToTileDictionary.Add("railing_metal_lt", new AtlasTileData(5, 3));
            characterToTileDictionary.Add("railing_metal_t", new AtlasTileData(6, 3));
            characterToTileDictionary.Add("railing_metal_rt", new AtlasTileData(7, 3));
            characterToTileDictionary.Add("railing_metal_l", new AtlasTileData(5, 4));
            characterToTileDictionary.Add("railing_metal_r", new AtlasTileData(7, 4));
            characterToTileDictionary.Add("railing_metal_rb", new AtlasTileData(7, 5));
            characterToTileDictionary.Add("railing_metal_b", new AtlasTileData(6, 5));
            characterToTileDictionary.Add("railing_metal_lb", new AtlasTileData(5, 5));
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

            BindWallPositions(0, 0, "wall", "closed_door", "open_door", "window");
            BindWallPositions(10, 0, "wall_brick", "closed_door_brick", "open_door_brick", "window_brick");
            BindWallPositions(10, 3, "chainlink", "closed_door_chainlink", "open_door_chainlink", "window_chainlink");
            BindWallPositions(10, 6, "tent_wall", "closed_door_tent", "open_door_tent", "window_tent");

        }

        private void BindWallPositions(int displacementX, int displacementY, string wallId, string doorId, string openDoorId, string windowId)
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

        VisibilityModel visibility;
        Vector3Int previousPlayerPosition = default;
        List<VisualChunk> chunksToUpdate = new List<VisualChunk>();

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
                Initialize(game);
            }


            IEntity worldEntity = game.TimelineEntity;
            IWorldProvider worldProvider = null;
            if (worldEntity != null)
            {
                worldProvider = worldEntity.GetComponentOfType<WorldTemplate>().WorldMap.Chunks;
            }
            bool chunkUpdate = false;



            while (game.Commander.DequeueCommand(out UpdateVisualChunkCommand command))
            {
                var chunkPoint = command.ChunkCoordinate;

                var chunk = visualChunks.FirstOrDefault(x => x.WorldPosition == chunkPoint);
                if (chunk == null)
                {
                    chunk = new VisualChunk(chunkPoint);
                    visualChunks.Add(chunk);
                }

                chunksToUpdate.Add(chunk);
                chunkUpdate = true;
            }

            if (chunkUpdate)
            {
                chunksToUpdate = chunksToUpdate.GroupBy(ch=>ch.WorldPosition).Select(g=>g.First()).ToList();

                foreach (var chunk in chunksToUpdate)
                {
                    chunk.UpdateChunk(characterToTileDictionary, tileAtlas, game);
                }
                chunksToUpdate.Clear();
            }

            var cameraEntity = game.CameraEntity;

            ConsoleCamera camera = cameraEntity.GetComponentOfType<ConsoleCamera>();
            Screen screen = cameraEntity.GetComponentOfType<Screen>();
            Commander commander = game.Commander;
            screen = UpdateZoom(game, commander, cameraEntity, screen, out bool zoomUpdate);
            int fsz = game.Settings.GetFontSizeZoomed();

            if (visibility == null || zoomUpdate)
            {
                visibility = new VisibilityModel(screen.Width, screen.Height, game);
            }

            if (camera != null && screen != null && worldProvider != null)
            {

                //ProcessSXFCommands(game);

                MoveCamera(game, camera);
                ClearScreen(screen, camera, game.GetSettings(), worldProvider);

                if (previousPlayerPosition != playerPosition.Point || zoomUpdate || chunkUpdate)
                {
                    FillcharacterBufferVisibility(game, screen, camera, game.GetSettings(), worldProvider);
                    visibility.UpdateVisibility(screen, game);
                }


                //  FillcharacterBuffersWithWorld(screen, camera, game.GetSettings(), worldProvider);
                //FillcharacterBuffersWithTileObjects(screen, camera, game.GetSettings(), game, gameTime, worldProvider);
                //FillcharacterBuffersWithWorldObjects(screen, camera, game.GetSettings(), game, gameTime);


                var projectionMatrix = Matrix.CreateOrthographicOffCenter(0, game.GetActualWidth(), game.GetActualHeight(), 0, 0, 2);
                effect.Parameters["xViewProjection"].SetValue(projectionMatrix);

                effect.GraphicsDevice.SamplerStates[0] = sampler;
                effect.GraphicsDevice.BlendState = BlendState.AlphaBlend;
                effect.GraphicsDevice.DepthStencilState = DepthStencilState.None;

                effect.Parameters["tileAtlas"].SetValue(tileAtlas);

                var playerChunkPosition = new Point((playerPosition.Point.X / Constants.ChunkSize), (playerPosition.Point.Y / Constants.ChunkSize));
                foreach (var visualChunk in visualChunks)
                {
                    if (visualChunk.WorldPosition.Z != playerPosZ)
                    {
                        continue;
                    }

                    if ((playerChunkPosition - visualChunk.WorldPosition.ToPoint()).ToVector2().Length() > (4)) //- visualChunk.WorldPosition).ToVector2().Length() > 1)
                    {
                        continue;
                    }

                    var chunkPosition = new Vector3((visualChunk.WorldPosition.X) * Constants.ChunkSize, (visualChunk.WorldPosition.Y) * Constants.ChunkSize, 0);

                    var chunkScreenPoint = camera.PointToScreen(new Point((int)chunkPosition.X, (int)chunkPosition.Y));

                    var chunkPositionMatrix = Matrix.CreateTranslation(new Vector3(chunkScreenPoint.X * Constants.ChunkSize, chunkScreenPoint.Y * Constants.ChunkSize, 0));
                    effect.Parameters["xWorld"].SetValue(chunkPositionMatrix * Matrix.CreateScale(1f / game.Settings.Zoom));
                    foreach (var tileModel in visualChunk.TileLayers)
                    {
                        game.GraphicsDevice.SetVertexBuffer(tileModel.Buffer);
                        game.GraphicsDevice.Indices = tileModel.IndexBuffer;

                        foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                        {
                            pass.Apply();
                            game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, tileModel.Indices.Length / 3);
                        }
                    }
                }

                effect.Parameters["tileAtlas"].SetValue(visibility.VisibilityMask);
                effect.Parameters["xWorld"].SetValue(Matrix.Identity);
            

                game.GraphicsDevice.SetVertexBuffer(visibility.TileModel.Buffer);
                game.GraphicsDevice.Indices = visibility.TileModel.IndexBuffer;

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, visibility.TileModel.Indices.Length / 3);
                }

                foreach (var entity in RegisteredEntities)
                {

                    var projectile = entity.GetComponentOfType<ProjectileComponent>();
                    if (projectile == null)
                    {
                        continue;
                    }

                    var sprited = entity.GetComponentOfType<AnimatedSpriteObject>();
                    sprited.CurrentAnimationTimeLeft -= gameTime.ElapsedGameTime.Microseconds;

                    if (sprited.InfinteAnimation)
                    {
                        sprited.CurrentAnimationTimeLeft = 1000;
                    }
                    sprited.Sprite.Update(gameTime);


                }

                RenderSpriteShadows(game, camera, game.GetSettings(), gameTime);

                game.Batch.Begin(samplerState: SamplerState.PointClamp);

                var screenActualWidth = game.Settings.GetWidthZoomed() * game.Settings.GetFontSizeZoomed();
                game.Batch.Draw(pixel, new Rectangle(screenActualWidth, 0, game.GetActualWidth() - screenActualWidth, game.GetActualHeight()), XNAColor.Black);

                RenderSpriteScreen(game, camera, game.GetSettings(), gameTime);
                RenderCursor(game, screen, camera, game.GetSettings(), gameTime);
                game.Batch.End();
            }

            previousPlayerPosition = playerPosition.Point;

            game.GraphicsDevice.Clear(ClearOptions.DepthBuffer, new Microsoft.Xna.Framework.Color(1), 1, 0);
        }




        private static Screen UpdateZoom(NamelessGame game, Commander commander, IEntity cameraEntity, Screen screen, out bool zoomUpdate)
        {
            zoomUpdate = false;
            if (commander.DequeueCommand(out ZoomCommand zoom))
            {
                var settings = game.GetSettings();


                cameraEntity.RemoveComponent(screen);

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
                cameraEntity.AddComponent(screen);
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


        private void RenderCursor(NamelessGame game, Screen screen, ConsoleCamera camera, GameSettings settings, GameTime gameTime)
        {
            var cursorEntity = game.CursorEntity;
            Drawable cursorDrawable = cursorEntity.GetComponentOfType<Drawable>();
            if (cursorDrawable.Visible)
            {
                Position cursorPosition = cursorEntity.GetComponentOfType<Position>();
                Position playerPosition = game.PlayerEntity.GetComponentOfType<Position>();

                LineToPlayer lineToPlayer = cursorEntity.GetComponentOfType<LineToPlayer>();
                var targeter = game.TargeterEntity.GetComponentOfType<TergeterComponent>();

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
                            var tileData = this.characterToTileDictionary[objectId];

                            var destination = new Rectangle(new Point(x * settings.GetFontSizeZoomed(), y * settings.GetFontSizeZoomed()), new Point(settings.GetFontSizeZoomed()));
                            var source = new Rectangle(new Point(tileData.X * settings.GetFontSizeZoomed(), tileData.Y * settings.GetFontSizeZoomed()), new Point(settings.GetFontSizeZoomed()));

                            game.Batch.Draw(cursorSmall, destination, color.ToXnaColor());


                        }
                    }
                }
                {
                    Point screenPointBigCursor = camera.PointToScreen(cursorPosition.X, cursorPosition.Y);
                    var distance = (cursorPosition.Point.ToPoint() - playerPosition.Point.ToPoint()).ToVector2().Length();
                    var cursorBigDestination = new Rectangle(new Point(screenPointBigCursor.X * settings.GetFontSizeZoomed(), screenPointBigCursor.Y * settings.GetFontSizeZoomed()), new Point(settings.GetFontSizeZoomed()));

                    var color = new Color(255, 255, 255);
                    if (distance > targeter.CurrentTargetingRange)
                    {
                        color = new Color(255, 0, 0);
                    }
                    game.Batch.Draw(cursorBig, cursorBigDestination, color.ToXnaColor());
                }
            }
        }
        

        private void RenderSFX(NamelessGame game, Screen screen, ConsoleCamera camera, GameSettings settings, GameTime gameTime)
        {


            //  _particleEffect.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

        }

          private void RenderSpriteScreen(NamelessGame game, ConsoleCamera camera, GameSettings settings, GameTime gameTime)
        {
            foreach (var entity in RegisteredEntities)
            {

                var projectile = entity.GetComponentOfType<ProjectileComponent>();
                if (projectile != null)
                {
                    continue;
                }

                var sprited = entity.GetComponentOfType<AnimatedSpriteObject>();
                var position = entity.GetComponentOfType<Position>();

                if(position.Z != playerPosZ)
                { continue; }

                Point screenPoint = camera.PointToScreen(position.X, position.Y);
                int tileHeight = game.GetSettings().GetFontSizeZoomed();
                int tileWidth = game.GetSettings().GetFontSizeZoomed();

                var animation = sprited.CurrentAnimation;
                if (sprited.CurrentAnimationTimeLeft > 0)
                {

                }
                var sprite = sprited.Sprite;
                sprite.Draw(game, gameTime, new Vector2(screenPoint.X * tileWidth, screenPoint.Y * tileHeight), new Vector2(tileWidth, tileHeight), new Vector2(1f), Microsoft.Xna.Framework.Color.White);
            }
        }

        private void RenderSpriteShadows(NamelessGame game, ConsoleCamera camera, GameSettings settings, GameTime gameTime)
        {
            foreach (var entity in RegisteredEntities)
            {

                var projectile = entity.GetComponentOfType<ProjectileComponent>();
                if (projectile != null)
                {
                    continue;
                }

                var sprited = entity.GetComponentOfType<AnimatedSpriteObject>();
                var position = entity.GetComponentOfType<Position>();
                Point screenPoint = camera.PointToScreen(position.X, position.Y);
                var spriteId = sprited;
                int tileHeight = game.GetSettings().GetFontSizeZoomed();
                int tileWidth = game.GetSettings().GetFontSizeZoomed();

                var animation = sprited.CurrentAnimation;

                    Vector2 positionOnScreen = new Vector2((screenPoint.X * tileWidth) + tileWidth * 0.4f, (screenPoint.Y * tileHeight) + tileHeight / 4);
                    var angleX = -45;
                    Matrix slant = Matrix.CreateTranslation(-positionOnScreen.X, -positionOnScreen.Y, 0f) *
                    Matrix.CreateRotationX(MathHelper.ToRadians(angleX)) *
                    Matrix.CreateRotationY(MathHelper.ToRadians(30)) *
                    Matrix.CreateScale(1.4f, 1f, 0) *
                    Matrix.CreateTranslation(positionOnScreen.X, positionOnScreen.Y, 0f);
                    game.Batch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: slant);
                    var sprite = sprited.Sprite;                
                    sprite.Draw(game, gameTime, positionOnScreen, new Vector2(tileWidth, tileHeight), new Vector2(1f), shadowColor);
                    game.Batch.End();
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
        Texture2D cursorSmall = null;
        Texture2D cursorBig = null;
        private Texture2D _particleTexture;
        private ParticleEffect _particleEffect;
        private Texture2D pixel;

        private Microsoft.Xna.Framework.Graphics.Texture Initialize(NamelessGame game)
        {

            pixel = new Texture2D(game.GraphicsDevice, 1, 1);
            pixel.SetData<XNAColor>(new XNAColor[] { XNAColor.White });

            tileAtlas = null;
            tileAtlas = game.Content.Load<Texture2D>("Sprites/tileset2");

            cursorSmall = null;
            cursorSmall = game.Content.Load<Texture2D>("Sprites/cursorSmall");

            cursorBig = null;
            cursorBig = game.Content.Load<Texture2D>("Sprites/cursorBig");

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

        public static void DrawVisibilityTile(int tileHeight, int tileWidth, int screenPositionX, int screenPositionY, int screenWidth, int screenHeight, int positionX, int positionY,  Color color, TileModel foregroundModel)
        {


            float textureX = (float)(screenPositionX / (float)screenWidth);
            float textureY = (float)(screenPositionY / (float)screenHeight);

            float textureXend = (float)((screenPositionX+1) / (float)screenWidth); ;

            float textureYend = (float)((screenPositionY+1) / (float)screenHeight);

            var arrayPosition = (screenPositionX * 4) + (screenPositionY * screenWidth * 4);

            var foregroundvertices = foregroundModel.Vertices;

            foregroundvertices[arrayPosition] =     new VertexTileModel(new Vector3(positionX, positionY, 0), new Vector2(textureX, textureY));
            foregroundvertices[arrayPosition + 1] = new VertexTileModel(new Vector3(positionX + tileWidth, positionY, 0), new Vector2(textureXend, textureY));
            foregroundvertices[arrayPosition + 2] = new VertexTileModel(new Vector3(positionX, positionY + tileHeight, 0),new Vector2(textureX, textureYend));
            foregroundvertices[arrayPosition + 3] = new VertexTileModel(new Vector3(positionX + tileWidth, positionY + tileHeight, 0),new Vector2(textureXend, textureYend));

        }

        public static void DrawTile(int tileHeight, int tileWidth, int screenPositionX, int screenPositionY, int screenWidth, int positionX, int positionY,
    AtlasTileData atlasTileData, TileModel foregroundModel, Texture2D tileAtlas)
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

            foregroundvertices[arrayPosition] =     new VertexTileModel (new Vector3(positionX, positionY, 0), new Vector2(textureX, textureY));
            foregroundvertices[arrayPosition + 1] = new VertexTileModel(new Vector3(positionX + tileWidth, positionY, 0), new Vector2(textureXend, textureY));
            foregroundvertices[arrayPosition + 2] = new VertexTileModel(new Vector3(positionX, positionY + tileHeight, 0), new Vector2(textureX, textureYend));
            foregroundvertices[arrayPosition + 3] = new VertexTileModel(new Vector3(positionX + tileWidth, positionY + tileHeight, 0), new Vector2(textureXend, textureYend));

           

        } 
    }
}
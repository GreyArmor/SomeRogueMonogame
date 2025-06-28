using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RogueSharp.Random;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Generation;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Systems.Ingame;
using NamelessRogue.shell;
using Color = NamelessRogue.Engine.Utility.Color;
using NamelessRogue.Engine.Utility;
using MonoGame.Extended.ECS;
using static System.Runtime.InteropServices.JavaScript.JSType;
using NamelessRogue.Engine.Generation.World.TerrainFeatures;
using NamelessRogue.Engine.Factories;
using static NamelessRogue.Engine.Systems.Ingame.RenderingSystem;
using AtlasTileData = NamelessRogue.Engine.Systems.Ingame.RenderingSystem.AtlasTileData;
using XNAColor = Microsoft.Xna.Framework.Color;
namespace NamelessRogue.Engine.Systems.Map
{

    public enum WorldBoardRenderingSystemMode
    {
        Terrain,
        Regions,
        Political,
        Artifact,
    }

    public class MapRenderingSystem : BaseSystem
    {
        public WorldBoardRenderingSystemMode Mode
        {
            get => _mode;
            set
            {
                _mode = value;
                worldMap?.Dispose();
                worldMap = null;
            }
        }


        Dictionary<string, AtlasTileData> characterToTileDictionary;
        private float gameTime;
        private float angle = 0;
        private float step = 0.04f;
        private InternalRandom graphicalRandom = new InternalRandom();
        Effect effect;
        TileModel worldMapTileModel;

        public bool LocalMapRendering { get; set; } = false;

        public override HashSet<Type> Signature { get; } = new HashSet<Type>();

        SamplerState sampler = new SamplerState()
        {
            AddressU = TextureAddressMode.Wrap,
            AddressV = TextureAddressMode.Wrap,
            AddressW = TextureAddressMode.Clamp,
            Filter = TextureFilter.Point,
            FilterMode = TextureFilterMode.Default,
            MaxMipLevel = 0,
            MaxAnisotropy = 4,

        };

        public MapRenderingSystem(GameSettings settings, WorldSettings gameWorldSettings)
        {
            InitializeCharacterTileDictionary();
            worldMapScreen = new Screen(gameWorldSettings.WorldBoardWidth, gameWorldSettings.WorldBoardHeight);
        }

        //TODO move this hardcode to configuration file for tileset
        void InitializeCharacterTileDictionary()
        {
            characterToTileDictionary = new Dictionary<string, AtlasTileData>();
            characterToTileDictionary.Add("Nothingness", new AtlasTileData(0,0));
            characterToTileDictionary.Add("Road", new AtlasTileData(1, 0));
            characterToTileDictionary.Add("Dirt", new AtlasTileData(2, 0));
            characterToTileDictionary.Add("Water", new AtlasTileData(3, 0));
        }


        public override void Update(GameTime gameTime, NamelessGame game)
        {

            this.gameTime = (long)gameTime.TotalGameTime.TotalMilliseconds;

            game.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            game.GraphicsDevice.SamplerStates[0] = sampler;
            game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;


            IEntity timeline = game.TimelineEntity;
            WorldBoard worldProvider = null;
            if (timeline != null)
            {
                worldProvider = timeline.GetComponentOfType<TimeLine>().CurrentTimelineLayer;
            }

            //todo move to constructor or some other place better suited for initialization
            if (tileAtlas == null)
            {
                InitializeTexture(game);
                _spriteBatch = new SpriteBatch(game.GraphicsDevice, 6400);
                worldMapTileModel = new TileModel(WorldGenConstants.Resolution, WorldGenConstants.Resolution);


   
                for (int y = 0; y < WorldGenConstants.Resolution; y++)
                {
                    for (int x = 0; x < WorldGenConstants.Resolution; x++)
                    {
                        var worldTile = worldProvider.WorldTiles[x, y];

                        int tileHeight = 64;
                        int tileWidth = 64;

                        var terrainRep = TerrainLibrary.Terrains[worldTile.Terrain].Representation;

                        RenderingSystem.AtlasTileData tileData;
                        if (!characterToTileDictionary.TryGetValue(terrainRep.ObjectID, out tileData))
                        {
                            characterToTileDictionary.TryGetValue("Nothingness", out tileData);
                        }

                        RenderingSystem.DrawTile(tileHeight, tileWidth, x, y, WorldGenConstants.Resolution,
                                x * Constants.ChunkSize,
                                y * Constants.ChunkSize,
                                tileData,
                                 worldMapTileModel, tileAtlas);
                    }
                }
                worldMapTileModel.UpdateBuffers(game.GraphicsDevice);
            }

            var zoom = game.WorldMapCameraEntity.GetComponentOfType<WorldMapCameraComponent>().Zoom;

            var chunkPositionMatrix = Matrix.CreateTranslation(new Vector3(0, 0, 0));
            effect.Parameters["xWorld"].SetValue(chunkPositionMatrix * Matrix.CreateScale(1f/ zoom));
            game.GraphicsDevice.SetVertexBuffer(worldMapTileModel.Buffer);
            game.GraphicsDevice.Indices = worldMapTileModel.IndexBuffer;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, worldMapTileModel.Indices.Length / 3);
            }

            game.Batch.Begin(samplerState: SamplerState.PointClamp);

            var screenActualWidth = game.Settings.GetWidthZoomed() * game.Settings.GetFontSizeZoomed();
            game.Batch.Draw(pixel, new Rectangle(screenActualWidth, 0, game.GetActualWidth() - screenActualWidth, game.GetActualHeight()), XNAColor.Black);

            game.Batch.End();

            game.GraphicsDevice.Clear(ClearOptions.DepthBuffer, new Microsoft.Xna.Framework.Color(1), 1, 0);
        }
        

        private void MoveCamera(NamelessGame game, ConsoleCamera camera)
        {
            Position playerPosition = game.CursorEntity
                .GetComponentOfType<Position>();

            Point p = camera.getPosition();
            p.X = (playerPosition.Point.X - game.GetSettings().GetWidthZoomed() / 2);
            p.Y = (playerPosition.Point.Y - game.GetSettings().GetHeightZoomed() / 2);
            camera.setPosition(p);
        }

        private void FillcharacterBuffersWithWorld(Screen screen, ConsoleCamera camera, GameSettings settings,
            WorldSettings worldSEttings,
            WorldBoard world)
        {

            int camX = camera.getPosition().X;
            int camY = camera.getPosition().Y;

            for (int x = camX; x < screen.Width + camX; x++)
            {
                for (int y = camY; y < screen.Height + camY; y++)
                {
                    Point screenPoint = camera.PointToScreen(x, y);

                    if (screenPoint.X < 0 || screenPoint.Y < 0 || x < 0 || x >= worldSEttings.WorldBoardWidth ||
                        y < 0 || y >= worldSEttings.WorldBoardHeight)
                    {
                        continue;
                    }
                    GetTerrainTile(screen, screenPoint, world.WorldTiles[x, y]);
                }
            }

        }

        void GetTerrainTile(Screen screen, Point point, WorldTile tile)
        {
            var terrainRepresentation = TerrainLibrary.Terrains[tile.Terrain].Representation;
            screen.ScreenBuffer[point.X, point.Y].AddObject(terrainRepresentation.ObjectID, ScreenObjectSource.Tileset, terrainRepresentation.CharColor, false, false);
        }

        Texture2D tileAtlas = null;
        Texture2D worldMap  = null;
        Texture2D pixel = null;
        private WorldBoardRenderingSystemMode _mode = WorldBoardRenderingSystemMode.Terrain;
        Texture2D whiteRectangle = null;
        private SpriteBatch _spriteBatch;
        private Screen worldMapScreen;

        private Texture InitializeTexture(NamelessGame game)
        {
            pixel = new Texture2D(game.GraphicsDevice, 1, 1);
            pixel.SetData<XNAColor>(new XNAColor[] { XNAColor.White });

            tileAtlas = null;
            tileAtlas = game.Content.Load<Texture2D>("Sprites/worldmap_tileset");
            effect = game.Content.Load<Effect>("Shader");
            effect.Parameters["tileAtlas"].SetValue(tileAtlas);

            whiteRectangle = new Texture2D(game.GraphicsDevice, 1, 1);
            whiteRectangle.SetData(new[] { Microsoft.Xna.Framework.Color.White });

            return tileAtlas;
        }

    }
}

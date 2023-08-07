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

        public readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
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
        private Screen worldMapScreen;

        public bool LocalMapRendering { get; set; } = false;

        public override HashSet<Type> Signature { get; } = new HashSet<Type>();

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

        public MapRenderingSystem(GameSettings settings, WorldSettings gameWorldSettings)
        {
            InitializeCharacterTileDictionary();
            worldMapScreen = new Screen(gameWorldSettings.WorldBoardWidth, gameWorldSettings.WorldBoardHeight);
        }

        //TODO move this hardcode to configuration file for tileset
        void InitializeCharacterTileDictionary()
        {
            characterToTileDictionary = new Dictionary<char, AtlasTileData>();
            characterToTileDictionary.Add(' ', new AtlasTileData(0, 0));
            characterToTileDictionary.Add('.', new AtlasTileData(14, 2));
            characterToTileDictionary.Add('@', new AtlasTileData(0, 4));
            characterToTileDictionary.Add('&', new AtlasTileData(6, 2));
            characterToTileDictionary.Add('~', new AtlasTileData(14, 7));
            characterToTileDictionary.Add('#', new AtlasTileData(3, 2));
            characterToTileDictionary.Add('$', new AtlasTileData(4, 2));
            characterToTileDictionary.Add('%', new AtlasTileData(5, 2));
            //alphabet                Add
            characterToTileDictionary.Add('A', new AtlasTileData(1, 4));
            characterToTileDictionary.Add('B', new AtlasTileData(2, 4));
            characterToTileDictionary.Add('C', new AtlasTileData(3, 4));
            characterToTileDictionary.Add('D', new AtlasTileData(4, 4));
            characterToTileDictionary.Add('E', new AtlasTileData(5, 4));
            characterToTileDictionary.Add('F', new AtlasTileData(6, 4));
            characterToTileDictionary.Add('G', new AtlasTileData(7, 4));
            characterToTileDictionary.Add('H', new AtlasTileData(8, 4));
            characterToTileDictionary.Add('I', new AtlasTileData(9, 4));
            characterToTileDictionary.Add('J', new AtlasTileData(10, 4));
            characterToTileDictionary.Add('K', new AtlasTileData(11, 4));
            characterToTileDictionary.Add('L', new AtlasTileData(12, 4));
            characterToTileDictionary.Add('M', new AtlasTileData(13, 4));
            characterToTileDictionary.Add('N', new AtlasTileData(14, 4));
            characterToTileDictionary.Add('O', new AtlasTileData(15, 4));
            //row change              Add
            characterToTileDictionary.Add('P', new AtlasTileData(0, 5));
            characterToTileDictionary.Add('Q', new AtlasTileData(1, 5));
            characterToTileDictionary.Add('R', new AtlasTileData(2, 5));
            characterToTileDictionary.Add('S', new AtlasTileData(3, 5));
            characterToTileDictionary.Add('T', new AtlasTileData(4, 5));
            characterToTileDictionary.Add('U', new AtlasTileData(5, 5));
            characterToTileDictionary.Add('V', new AtlasTileData(6, 5));
            characterToTileDictionary.Add('W', new AtlasTileData(7, 5));
            characterToTileDictionary.Add('X', new AtlasTileData(8, 5));
            characterToTileDictionary.Add('Y', new AtlasTileData(9, 5));
            characterToTileDictionary.Add('Z', new AtlasTileData(10, 5));
            //row change              Add
            characterToTileDictionary.Add('a', new AtlasTileData(1, 6));
            characterToTileDictionary.Add('b', new AtlasTileData(2, 6));
            characterToTileDictionary.Add('c', new AtlasTileData(3, 6));
            characterToTileDictionary.Add('d', new AtlasTileData(4, 6));
            characterToTileDictionary.Add('e', new AtlasTileData(5, 6));
            characterToTileDictionary.Add('f', new AtlasTileData(6, 6));
            characterToTileDictionary.Add('g', new AtlasTileData(7, 6));
            characterToTileDictionary.Add('h', new AtlasTileData(8, 6));
            characterToTileDictionary.Add('i', new AtlasTileData(9, 6));
            characterToTileDictionary.Add('j', new AtlasTileData(10, 6));
            characterToTileDictionary.Add('k', new AtlasTileData(11, 6));
            characterToTileDictionary.Add('l', new AtlasTileData(12, 6));
            characterToTileDictionary.Add('m', new AtlasTileData(13, 6));
            characterToTileDictionary.Add('n', new AtlasTileData(14, 6));
            characterToTileDictionary.Add('o', new AtlasTileData(15, 6));
            //row change              Add
            characterToTileDictionary.Add('p', new AtlasTileData(0, 7));
            characterToTileDictionary.Add('q', new AtlasTileData(1, 7));
            characterToTileDictionary.Add('r', new AtlasTileData(2, 7));
            characterToTileDictionary.Add('s', new AtlasTileData(3, 7));
            characterToTileDictionary.Add('t', new AtlasTileData(4, 7));
            characterToTileDictionary.Add('u', new AtlasTileData(5, 7));
            characterToTileDictionary.Add('v', new AtlasTileData(6, 7));
            characterToTileDictionary.Add('w', new AtlasTileData(7, 7));
            characterToTileDictionary.Add('x', new AtlasTileData(8, 7));
            characterToTileDictionary.Add('y', new AtlasTileData(9, 7));
            characterToTileDictionary.Add('z', new AtlasTileData(10, 7));

            characterToTileDictionary.Add('★', new AtlasTileData(15, 0));

        }


        public override void Update(GameTime gameTime, NamelessGame game)
        {

            this.gameTime = (long)gameTime.TotalGameTime.TotalMilliseconds;

            game.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            game.GraphicsDevice.SamplerStates[0] = sampler;
            game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            //todo move to constructor or some other place better suited for initialization
            if (tileAtlas == null)
            {
                InitializeTexture(game);
                _spriteBatch = new SpriteBatch(game.GraphicsDevice, 6400);
            }

            IEntity timeline = game.TimelineEntity;
            WorldBoard worldProvider = null;
            if (timeline != null)
            {
                worldProvider = timeline.GetComponentOfType<TimeLine>().CurrentTimelineLayer;
            }

            if (LocalMapRendering)
            {
                _spriteBatch.Begin(SpriteSortMode.Deferred, null, null, DepthStencilState.None);
                var entity = game.CameraEntity;

                ConsoleCamera camera = entity.GetComponentOfType<ConsoleCamera>();
                Screen screen = entity.GetComponentOfType<Screen>();

                if (camera != null && screen != null && worldProvider != null)
                {
                    MoveCamera(game, camera);
                    FillcharacterBuffersWithWorld(screen, camera, game.GetSettings(), game.WorldSettings,
                        worldProvider);

                    Position playerPosition = game.CursorEntity
                        .GetComponentOfType<Position>();
                    var screenPoint = camera.PointToScreen(playerPosition.Point);

                    if (screenPoint.X > 0 && screenPoint.X < game.GetSettings().GetWidthZoomed() &&
                        screenPoint.X > 0 &&
                        screenPoint.Y < game.GetSettings().GetWidthZoomed())
                    {
                        screen.ScreenBuffer[screenPoint.X, screenPoint.Y].Char = 'X';
                        screen.ScreenBuffer[screenPoint.X, screenPoint.Y].CharColor = new Color(1f, 1f, 1f, 1f);
                    }

                    RenderScreen(game, screen, game.GetSettings().GetFontSize(), game.GetSettings());
                }
                _spriteBatch.End();
            }
            else
            {
              
                if (worldProvider != null)
                {

                    Screen screen = worldMapScreen;
                    ConsoleCamera camera = new ConsoleCamera(new Point(0,0));
                    if (worldMap == null)
                    {
                        FillcharacterBuffersWithWorld(screen, camera, game.GetSettings(), game.WorldSettings,
                            worldProvider);

                        Position cursorPosition = game.CursorEntity
                            .GetComponentOfType<Position>();
                        var screenPoint = camera.PointToScreen(cursorPosition.Point);

                        if (screenPoint.X > 0 && screenPoint.X < game.GetSettings().GetWidthZoomed() &&
                            screenPoint.X > 0 &&
                            screenPoint.Y < game.GetSettings().GetWidthZoomed())
                        {
                            screen.ScreenBuffer[screenPoint.X, screenPoint.Y].Char = 'X';
                            screen.ScreenBuffer[screenPoint.X, screenPoint.Y].CharColor = new Color(1f, 1f, 1f, 1f);
                        }

                        var arr = new Color[worldMapScreen.Width, worldMapScreen.Height];
                        var defaultcolor = new Color();
                        for (int x = 0; x < screen.Width; x++)
                        {
                            for (int y = 0; y < screen.Height; y++)
                            {
                                var playerPosition = game.PlayerEntity.GetComponentOfType<Position>();
                                if ((playerPosition.Point.X / Constants.ChunkSize) == x || playerPosition.Point.Y / Constants.ChunkSize == y)
                                {
                                    arr[x, y] = new Color(1f, 0, 0, 1f);
                                }
                                else
                                {
                                    if (screen.ScreenBuffer[x, y].BackGroundColor.Red != 0 || screen.ScreenBuffer[x, y].BackGroundColor.Green != 0 || screen.ScreenBuffer[x, y].BackGroundColor.Blue != 0)
                                    {
                                        arr[x, y] = screen.ScreenBuffer[x, y].BackGroundColor;
                                    }
                                    else
                                    {
                                        arr[x, y] = screen.ScreenBuffer[x, y].CharColor;
                                    }
                                }
                            }
                        }

                        byte[] arrBytes = new byte[screen.Width * screen.Height * 4];
                        for (int y = 0; y < screen.Height; y++)
                        {
                            for (int x = 0; x < screen.Width; x++)
                            {

                                var red =  arr[x, y].Red * 255f;
                                var green =  arr[x, y].Green * 255f;
                                var blue =  arr[x, y].Blue * 255f;
                                arrBytes[y * screen.Height * 4 + x * 4] = (byte) red;
                                arrBytes[y * screen.Height * 4 + x * 4 + 1] = (byte) green;
                                arrBytes[y * screen.Height * 4 + x * 4 + 2] = (byte) blue;

                                //arrBytes[y * screen.Height * 4 + x * 4] = (byte)255;
                                //arrBytes[y * screen.Height * 4 + x * 4 + 1] = 0;
                                //arrBytes[y * screen.Height * 4 + x * 4 + 2] = 0;


                                arrBytes[y * screen.Height * 4 + x * 4 + 3] = 255;
                            }
                        }

                        Texture2D tex = new Texture2D(NamelessGame.DebugDevice, screen.Width, screen.Height, false,
                            SurfaceFormat.Color);
                        tex.SetData(arrBytes);
//#if DEBUG
//                        using (var stream = File.OpenWrite("C:\\11\\colored.png"))
//                        {
//                            tex.SaveAsPng(stream, tex.Width, tex.Height);
//                        }
//#endif

                        worldMap = tex;
                    }
                    RenderWorldScreen(game, screen);
                }



            }

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
            if (angle > 360)
            {
                angle = 0;
            }

            angle += step;

            for (int x = 0; x < screen.Width; x++)
            {
                for (int y = 0; y < screen.Height; y++)
                {
                    screen.ScreenBuffer[x, y].Char = ' ';
                    screen.ScreenBuffer[x, y].CharColor = new Color();
                    screen.ScreenBuffer[x, y].BackGroundColor = new Color();
                }
            }

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
            screen.ScreenBuffer[point.X, point.Y].Char = BiomesLibrary.Biomes[tile.Biome].Representation.Representation;
            screen.ScreenBuffer[point.X, point.Y].CharColor = BiomesLibrary.Biomes[tile.Biome].Representation.CharColor;
            screen.ScreenBuffer[point.X, point.Y].BackGroundColor = BiomesLibrary.Biomes[tile.Biome].Representation.BackgroundColor;


            if (Mode == WorldBoardRenderingSystemMode.Regions)
            {
                if (tile.Continent != null)
                {
                    screen.ScreenBuffer[point.X, point.Y].BackGroundColor = tile.Continent.Color;
                }
                else
                {
                    screen.ScreenBuffer[point.X, point.Y].BackGroundColor = new Color();
                }
            }
            else if (Mode == WorldBoardRenderingSystemMode.Political)
            {
                if (tile.Owner != null)
                {
                    if (tile.Settlement != null)
                    {
                        screen.ScreenBuffer[point.X, point.Y].Char = 'T';
                        screen.ScreenBuffer[point.X, point.Y].CharColor = new Color(1, 1, 1, 1);
                    }

                    screen.ScreenBuffer[point.X, point.Y].BackGroundColor = new Color(tile.Owner.CivColor);
                }
                else
                {
                    screen.ScreenBuffer[point.X, point.Y].Char = ' ';
                    screen.ScreenBuffer[point.X, point.Y].CharColor = new Color(0, 0, 0, 0);
                }
            }
            else if (Mode == WorldBoardRenderingSystemMode.Artifact)
            {
                if (tile.Artifact != null)
                {
                    screen.ScreenBuffer[point.X, point.Y].Char = tile.Artifact.Representation;
                    screen.ScreenBuffer[point.X, point.Y].CharColor = tile.Artifact.CharColor;
                }
            }
        }

        private void RenderScreen(NamelessGame gameInstance, Screen screen, int tileSize, GameSettings settings)
        {
            for (int x = 0; x < settings.GetWidthZoomed(); x++)
            {
                for (int y = 0; y < settings.GetHeightZoomed(); y++)
                {
                        DrawTile(gameInstance.GraphicsDevice, gameInstance,
                            x * settings.GetFontSizeZoomed(),
                            y * settings.GetFontSizeZoomed(),
                            characterToTileDictionary[screen.ScreenBuffer[x, y].Char],
                            screen.ScreenBuffer[x, y].CharColor,
                            screen.ScreenBuffer[x, y].BackGroundColor
                            );
                }
            }
        }




        private void RenderWorldScreen(NamelessGame gameInstance, Screen screen)
        {
            effect.Parameters["tileAtlas"].SetValue(worldMap);
            var positionX = 0;
            var positionY = 0;
            var tileHeight = gameInstance.GetActualCharacterHeight();
            var tileWidth = gameInstance.GetActualCharacterWidth();
            float textureX = 0;
            float textureY = 1;

            float textureXend = 1;

            float textureYend = 0;
            var projectionMatrix = //Matrix.CreateOrthographic(game.getActualWidth(),game.getActualHeight(),0,1);
                Matrix.CreateOrthographicOffCenter(0, gameInstance.GetActualWidth(),
                    0, gameInstance.GetActualHeight(), 0, 2);

            effect.Parameters["xViewProjection"].SetValue(projectionMatrix);
            var indices = new int[6] {0, 1, 2, 2, 3, 0};
            effect.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            effect.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            var vertices = new Vertex[4];

            vertices[0] = new Vertex(new Vector3(positionX, positionY + tileHeight, 0), new Vector4(),
                new Vector4(), new Vector2(textureX, textureY));
            vertices[1] = new Vertex(new Vector3(positionX, positionY, 0), new Vector4(),
                new Vector4(), new Vector2(textureX, textureYend));
            vertices[2] = new Vertex(new Vector3(positionX + tileWidth, positionY, 0), new Vector4(),
                new Vector4(), new Vector2(textureXend, textureYend));
            vertices[3] = new Vertex(new Vector3(positionX + tileWidth, positionY + tileHeight, 0), new Vector4(),
                new Vector4(), new Vector2(textureXend, textureY));

            effect.CurrentTechnique = effect.Techniques["TextureTecnique"];
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                gameInstance.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0,
                    vertices.Length,
                    indices.Reverse().ToArray(), 0, 2, this.VertexDeclaration);
            }

        }
        SpriteBatch _spriteBatch;
        void DrawTile(GraphicsDevice device, NamelessGame game, int positionX, int positionY,
    AtlasTileData atlasTileData,
    Color color, Color backGroundColor)
        {

            if (atlasTileData == null)
            {
                atlasTileData = new AtlasTileData(1, 1);
            }
            var fontsize = game.GetSettings().GetFontSizeZoomed();
            var tilesize = tileAtlas.Width / 16;
            if (fontsize < 4)
            {
                _spriteBatch.Draw(whiteRectangle, new Rectangle(positionX, game.GetActualCharacterHeight() - positionY, fontsize, fontsize), null, color.ToXnaColor(), 0, default(Vector2), SpriteEffects.None, 0);
            }
            else
            {
                _spriteBatch.Draw(whiteRectangle, new Rectangle(positionX, game.GetActualCharacterHeight() - positionY, fontsize, fontsize), null, backGroundColor.ToXnaColor(), 0, default(Vector2), SpriteEffects.None, 0);
                _spriteBatch.Draw(tileAtlas, new Rectangle(positionX, game.GetActualCharacterHeight() - positionY, fontsize, fontsize), new Rectangle(atlasTileData.X * tilesize, atlasTileData.Y * tilesize, tilesize, tilesize), color.ToXnaColor(), 0, default(Vector2), SpriteEffects.None, 1);
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
        Texture2D worldMap  = null;
        private WorldBoardRenderingSystemMode _mode = WorldBoardRenderingSystemMode.Terrain;
        Texture2D whiteRectangle = null;

        private Texture InitializeTexture(NamelessGame game)
        {

            tileAtlas = null;
            tileAtlas = game.Content.Load<Texture2D>("DFfont");
            effect = game.Content.Load<Effect>("Shader");
            effect.Parameters["tileAtlas"].SetValue(tileAtlas);

            whiteRectangle = new Texture2D(game.GraphicsDevice, 1, 1);
            whiteRectangle.SetData(new[] { Microsoft.Xna.Framework.Color.White });

            return tileAtlas;
        }

        void DrawTile(GraphicsDevice device, NamelessGame game, int positionX, int positionY,
            AtlasTileData atlasTileData,
            Color color, Color backGroundColor, int tileSize)
        {

            if (atlasTileData == null)
            {
                atlasTileData = new AtlasTileData(1, 1);
            }


            int tileHeight = tileSize;
            int tileWidth = tileSize;


            float textureX = atlasTileData.X * (Constants.tileAtlasTileSize / (float) tileAtlas.Width);
            float textureY = atlasTileData.Y * (Constants.tileAtlasTileSize / (float) tileAtlas.Height);

            float textureXend = (atlasTileData.X + 1f) * (Constants.tileAtlasTileSize / (float) tileAtlas.Width);

            float textureYend = (atlasTileData.Y + 1f) * (Constants.tileAtlasTileSize / (float) tileAtlas.Height);


            var settings = game.GetSettings();
            var projectionMatrix = //Matrix.CreateOrthographic(game.getActualWidth(),game.getActualHeight(),0,1);
                Matrix.CreateOrthographicOffCenter(0, game.GetActualWidth(),
                    0, game.GetActualHeight(), 0, 2);

            effect.Parameters["xViewProjection"].SetValue(projectionMatrix);
            var indices = new int[6] {0, 1, 2, 2, 3, 0};
            effect.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            effect.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            var vertices = new Vertex[4];

            vertices[0] = new Vertex(new Vector3(positionX, positionY + tileHeight, 0), color.ToVector4(),
                backGroundColor.ToVector4(), new Vector2(textureX, textureY));
            vertices[1] = new Vertex(new Vector3(positionX, positionY, 0), color.ToVector4(),
                backGroundColor.ToVector4(), new Vector2(textureX, textureYend));
            vertices[2] = new Vertex(new Vector3(positionX + tileWidth, positionY, 0), color.ToVector4(),
                backGroundColor.ToVector4(), new Vector2(textureXend, textureYend));
            vertices[3] = new Vertex(new Vector3(positionX + tileWidth, positionY + tileHeight, 0), color.ToVector4(),
                backGroundColor.ToVector4(), new Vector2(textureXend, textureY));

            effect.CurrentTechnique = effect.Techniques["Background"];
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length,
                    indices.Reverse().ToArray(), 0, 2, this.VertexDeclaration);
            }


            vertices[0] = new Vertex(new Vector3(positionX, positionY + tileHeight, 0), color.ToVector4(),
                backGroundColor.ToVector4(), new Vector2(textureX, textureY));
            vertices[1] = new Vertex(new Vector3(positionX, positionY, 0), color.ToVector4(),
                backGroundColor.ToVector4(), new Vector2(textureX, textureYend));
            vertices[2] = new Vertex(new Vector3(positionX + tileWidth, positionY, 0), color.ToVector4(),
                backGroundColor.ToVector4(), new Vector2(textureXend, textureYend));
            vertices[3] = new Vertex(new Vector3(positionX + tileWidth, positionY + tileHeight, 0), color.ToVector4(),
                backGroundColor.ToVector4(), new Vector2(textureXend, textureY));

            effect.CurrentTechnique = effect.Techniques["Point"];
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length,
                    indices.Reverse().ToArray(), 0, 2, this.VertexDeclaration);
            }
        }

    }
}

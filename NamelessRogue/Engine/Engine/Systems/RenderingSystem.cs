using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.AI.NonPlayerCharacter;
using NamelessRogue.Engine.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Engine.Components.Interaction;
using NamelessRogue.Engine.Engine.Components.Physical;
using NamelessRogue.Engine.Engine.Components.Rendering;
using NamelessRogue.Engine.Engine.Infrastructure;
using NamelessRogue.Engine.Engine.Utility;
using NamelessRogue.FieldOfView;
using NamelessRogue.shell;
using NamelessRogue.Storage.data;
using BoundingBox = NamelessRogue.Engine.Engine.Utility.BoundingBox;
using Color = NamelessRogue.Engine.Engine.Utility.Color;

namespace NamelessRogue.Engine.Engine.Systems
{

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



    public class RenderingSystem : ISystem
    {


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
        private Random graphicalRandom = new Random();
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

        public RenderingSystem(GameSettings settings)
        {

            InitializeCharacterTileDictionary();
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
            characterToTileDictionary.Add('p', new AtlasTileData(0, 8));
            characterToTileDictionary.Add('q', new AtlasTileData(1, 8));
            characterToTileDictionary.Add('r', new AtlasTileData(2, 8));
            characterToTileDictionary.Add('s', new AtlasTileData(3, 8));
            characterToTileDictionary.Add('t', new AtlasTileData(4, 8));
            characterToTileDictionary.Add('u', new AtlasTileData(5, 8));
            characterToTileDictionary.Add('v', new AtlasTileData(6, 8));
            characterToTileDictionary.Add('w', new AtlasTileData(7, 8));
            characterToTileDictionary.Add('x', new AtlasTileData(8, 8));
            characterToTileDictionary.Add('y', new AtlasTileData(9, 8));
            characterToTileDictionary.Add('z', new AtlasTileData(10, 8));
        }

        public void Update(long gameTime, NamelessGame game)
        {

            this.gameTime = gameTime;

            game.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            game.GraphicsDevice.SamplerStates[0] = sampler;
            game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            //todo move to constructor or some other place better suited for initialization
            if (tileAtlas == null)
            {
                InitializeTexture(game);
            }

            IEntity worldEntity = game.GetEntityByComponentClass<ChunkData>();
            IChunkProvider worldProvider = null;
            if (worldEntity != null)
            {
                worldProvider = worldEntity.GetComponentOfType<ChunkData>();
            }

            foreach (IEntity entity in game.GetEntities())
            {

                ConsoleCamera camera = entity.GetComponentOfType<ConsoleCamera>();
                Screen screen = entity.GetComponentOfType<Screen>();
                if (camera != null && screen != null && worldProvider != null)
                {
                    MoveCamera(game, camera);
                    FillcharacterBufferVisibility(game, screen, camera, game.GetSettings(), worldProvider);
                    FillcharacterBuffersWithWorld(screen, camera, game.GetSettings(), worldProvider);
                    FillcharacterBuffersWithWorldObjects(screen, camera, game.GetSettings(), game);

                    RenderScreen(game, screen, game.GetSettings());
                    break;
                }
            }
        }

        private void MoveCamera(NamelessGame game, ConsoleCamera camera)
        {
            IEntity input = game.GetEntityByComponentClass<InputComponent>();
            if (input != null)
            {
                Position playerPosition = game.GetEntityByComponentClass<FollowedByCamera>()
                    .GetComponentOfType<Position>();

                Point p = camera.getPosition();
                p.X = (playerPosition.p.X - game.GetSettings().getWidth() / 2);
                p.Y = (playerPosition.p.Y - game.GetSettings().getHeight() / 2);
                camera.setPosition(p);
            }
        }
        PermissiveVisibility fov;
        private void FillcharacterBufferVisibility(NamelessGame game, Screen screen, ConsoleCamera camera,
            GameSettings settings, IChunkProvider world)
        {
            int camX = camera.getPosition().X;
            int camY = camera.getPosition().Y;
            Position playerPosition = game.GetEntityByComponentClass<Player>().GetComponentOfType<Position>();
            BoundingBox b = new BoundingBox(camera.getPosition(),
                new Point(settings.getWidth() + camX, settings.getHeight() + camY));

            for (int x = 0; x < settings.getWidth(); x++)
            {
                for (int y = 0; y < settings.getHeight(); y++)
                {
                    screen.ScreenBuffer[x, y].isVisible = false;
                }
            }


            if (fov == null)
            {
                fov = new PermissiveVisibility((x, y) => { return !world.getTile(x, y).GetBlocksVision(game); },
                    (x, y) =>
                    {
                        Point screenPoint = camera.PointToScreen(x, y);
                        if (screenPoint.X >= 0 && screenPoint.X < settings.getWidth() && screenPoint.Y >= 0 &&
                            screenPoint.Y < settings.getHeight())
                        {
                            screen.ScreenBuffer[screenPoint.X, screenPoint.Y].isVisible = true;
                        }
                    }, (x, y) => { return Math.Abs(x) + Math.Abs(y); }
                );
            }

            fov.Compute(playerPosition.p,60);
        }

        private void FillcharacterBuffersWithWorld(Screen screen, ConsoleCamera camera, GameSettings settings,
            IChunkProvider world)
        {
            int camX = camera.getPosition().X;
            int camY = camera.getPosition().Y;
            if (angle > 360)
            {
                angle = 0;
            }

            angle += step;

            for (int x = camX; x < settings.getWidth() + camX; x++)
            {
                for (int y = camY; y < settings.getHeight() + camY; y++)
                {
                    Point screenPoint = camera.PointToScreen(x, y);
                    if (screen.ScreenBuffer[screenPoint.X, screenPoint.Y].isVisible)
                    {
                        Tile tileToDraw = world.getTile(x, y);
                        GetTerrainTile(screen, tileToDraw.getTerrainType(), screenPoint);
                    }
                    else
                    {
                        screen.ScreenBuffer[screenPoint.X, screenPoint.Y].Char = ' ';
                        screen.ScreenBuffer[screenPoint.X, screenPoint.Y].BackGroundColor = new Color();
                    }
                }
            }
        }

        void GetTerrainTile(Screen screen, TerrainTypes terrainTypes, Point point)
        {
            if (terrainTypes == TerrainTypes.Water)
            {
                screen.ScreenBuffer[point.X, point.Y].Char = '~';

                //float waveValue1 = (float) ((Math.Sin(((x + y) / 1.5) + angle)) + 1) / 2;
                //float waveValue2 = (float) ((Math.Sin(((x + y)) + angle)) / 2 + 1) / 2;
                //float waveValue3 = (float) ((Math.Sin(((x + y) / 3) + angle)) + 1) / 2;
                //float resultingColor = 0.3f + (0.5f * (waveValue1 + waveValue2 + waveValue3) / 3);

                screen.ScreenBuffer[point.X, point.Y].CharColor = new Color(0, 0, 255);
                screen.ScreenBuffer[point.X, point.Y].BackGroundColor =
                    new Color(0, 1, 1);
            }
            else if (terrainTypes == TerrainTypes.Road)
            {
                screen.ScreenBuffer[point.X, point.Y].Char = '.';
                screen.ScreenBuffer[point.X, point.Y].CharColor = new Color(0.5, 0.5, 0.5);
                screen.ScreenBuffer[point.X, point.Y].BackGroundColor = new Color();
            }
            else if (terrainTypes == TerrainTypes.Dirt)
            {
                screen.ScreenBuffer[point.X, point.Y].Char = '&';
                screen.ScreenBuffer[point.X, point.Y].CharColor = new Color(1f, 1f, 0);
                screen.ScreenBuffer[point.X, point.Y].BackGroundColor = new Color();
            }
            else if (terrainTypes == TerrainTypes.Grass)
            {
                screen.ScreenBuffer[point.X, point.Y].Char = '.';
                screen.ScreenBuffer[point.X, point.Y].CharColor = new Color(0, 0.8f, 0);
                screen.ScreenBuffer[point.X, point.Y].BackGroundColor = new Color();
            }
            else if (terrainTypes == TerrainTypes.HardRocks)
            {
                screen.ScreenBuffer[point.X, point.Y].Char = 'A';
                screen.ScreenBuffer[point.X, point.Y].CharColor = new Color(0.2, 0.2, 0.2);
                screen.ScreenBuffer[point.X, point.Y].BackGroundColor = new Color();
            }
            else if (terrainTypes == TerrainTypes.Rocks)
            {
                screen.ScreenBuffer[point.X, point.Y].Char = 'A';
                screen.ScreenBuffer[point.X, point.Y].CharColor = new Color(0.8, 0.8, 0.8);
                screen.ScreenBuffer[point.X, point.Y].BackGroundColor = new Color();
            }
            else if (terrainTypes == TerrainTypes.LightRocks)
            {
                screen.ScreenBuffer[point.X, point.Y].Char = 'A';
                screen.ScreenBuffer[point.X, point.Y].CharColor = new Color(0.5, 0.5, 0.5);
                screen.ScreenBuffer[point.X, point.Y].BackGroundColor = new Color();
            }
            else if (terrainTypes == TerrainTypes.Sand)
            {
                screen.ScreenBuffer[point.X, point.Y].Char = '~';
                screen.ScreenBuffer[point.X, point.Y].CharColor = new Color(1f, 1f, 0);
                screen.ScreenBuffer[point.X, point.Y].BackGroundColor = new Color();
            }
            else if (terrainTypes == TerrainTypes.Snow)
            {
                screen.ScreenBuffer[point.X, point.Y].Char = 's';
                screen.ScreenBuffer[point.X, point.Y].CharColor = new Color(1f, 1f, 1f);
                screen.ScreenBuffer[point.X, point.Y].BackGroundColor = new Color();
            }

            else
            {
                screen.ScreenBuffer[point.X, point.Y].Char = ' ';
                screen.ScreenBuffer[point.X, point.Y].CharColor = new Color();
                screen.ScreenBuffer[point.X, point.Y].BackGroundColor = new Color();
            }
        }


        private void FillcharacterBuffersWithWorldObjects(Screen screen, ConsoleCamera camera, GameSettings settings,
            NamelessGame game)
        {
            foreach (IEntity entity in game.GetEntities())
            {

                Position position = entity.GetComponentOfType<Position>();
                Drawable drawable = entity.GetComponentOfType<Drawable>();
                LineToPlayer lineToPlayer = entity.GetComponentOfType<LineToPlayer>();
                if (drawable != null && position != null)
                {
                    if (true) //drawable.isVisible())
                    {
                        Point screenPoint = camera.PointToScreen(position.p.X, position.p.Y);
                        int x = screenPoint.X;
                        int y = screenPoint.Y;
                        if (x >= 0 && x < settings.getWidth() && y >= 0 && y < settings.getHeight())
                        {
                            if (screen.ScreenBuffer[screenPoint.X, screenPoint.Y].isVisible)
                            {
                                screen.ScreenBuffer[screenPoint.X, screenPoint.Y].Char = drawable.getRepresentation();
                                screen.ScreenBuffer[screenPoint.X, screenPoint.Y].CharColor = drawable.getCharColor();
                            }
                            else
                            {
                                screen.ScreenBuffer[screenPoint.X, screenPoint.Y].Char = ' ';
                                screen.ScreenBuffer[screenPoint.X, screenPoint.Y].CharColor = new Color();
                                screen.ScreenBuffer[screenPoint.X, screenPoint.Y].BackGroundColor = new Color();
                            }
                        }

                    }
                }

                if (drawable != null && position != null && lineToPlayer != null)
                {
                    if (drawable.isVisible())
                    {
                        Position playerPosition =
                            game.GetEntityByComponentClass<Player>().GetComponentOfType<Position>();
                        List<Point> line = PointUtil.getLine(playerPosition.p, position.p);
                        for (int i = 1; i < line.Count - 1; i++)
                        {
                            Point p = line[i];
                            Point screenPoint = camera.PointToScreen(p.X, p.Y);
                            int x = screenPoint.X;
                            int y = screenPoint.Y;
                            if (x >= 0 && x < settings.getWidth() && y >= 0 && y < settings.getHeight())
                            {
                                screen.ScreenBuffer[screenPoint.X, screenPoint.Y].Char = 'x';
                                screen.ScreenBuffer[screenPoint.X, screenPoint.Y].CharColor = drawable.getCharColor();
                            }
                        }
                    }
                }
            }
        }


        private void RenderScreen(NamelessGame gameInstance, Screen screen, GameSettings settings)
        {

            for (int x = 0; x < settings.getWidth(); x++)
            {
                for (int y = 0; y < settings.getHeight(); y++)
                {

                    DrawTile(gameInstance.GraphicsDevice, gameInstance,
                        x * settings.getFontSize(),
                        y * settings.getFontSize(),
                        characterToTileDictionary[screen.ScreenBuffer[x, y].Char],
                        screen.ScreenBuffer[x, y].CharColor,
                        screen.ScreenBuffer[x, y].BackGroundColor
                        );
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
            effect = game.Content.Load<Effect>("Shader");
            effect.Parameters["tileAtlas"].SetValue(tileAtlas);
            return tileAtlas;
        }

        void DrawTile(GraphicsDevice device, NamelessGame game, int positionX, int positionY,
            AtlasTileData atlasTileData,
            Color color, Color backGroundColor)
        {

            if (atlasTileData == null)
            {
                atlasTileData = new AtlasTileData(1, 1);
            }


            int tileHeight = game.GetSettings().getFontSize();
            int tileWidth = game.GetSettings().getFontSize();


            float textureX = atlasTileData.X * (Constants.tileAtlasTileSize / (float) tileAtlas.Width);
            float textureY = atlasTileData.Y * (Constants.tileAtlasTileSize / (float) tileAtlas.Height);

            float textureXend = (atlasTileData.X + 1f) * (Constants.tileAtlasTileSize / (float) tileAtlas.Width);

            float textureYend = (atlasTileData.Y + 1f) * (Constants.tileAtlasTileSize / (float) tileAtlas.Height);


            var settings = game.GetSettings();
            var projectionMatrix = //Matrix.CreateOrthographic(game.getActualWidth(),game.getActualHeight(),0,1);
                Matrix.CreateOrthographicOffCenter(0, game.GetActualWidth(),
                    0, game.GetActualHeight(), 0, 2);

            effect.Parameters["xViewProjection"].SetValue(projectionMatrix);
            var indices = new int[6] { 0, 1, 2, 2, 3, 0 };
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
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
            this.vertexHeightYawPitch = new Vector3(height,yaw, pitch);
        }
    }


    public class TileModel { 
        public Vertex[] Vertices { get; }
        public int[] Indices { get; }
		public TileModel(int height, int width)
		{
            Vertices = new Vertex[height * width * 4];
            Indices = new int[height * width * 6];

            //var indices = new int[6] { 0, 1, 2, 2, 1, 3 };
            var vertexCounter = 0;
            for (int i = 0; i < height * width * 6; i+=6)
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
	}



    public class RenderingSystem : BaseSystem
    {
        public override HashSet<Type> Signature { get; }

        public readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
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

            Signature = new HashSet<Type>();
            Signature.Add(typeof(Drawable));
            Signature.Add(typeof(Position));

        }

        //TODO move this hardcode to configuration file for tileset
        void InitializeCharacterTileDictionary()
        {

            var atlasTileData = new AtlasTileData(3, 1);
            characterToTileDictionary = new Dictionary<String, AtlasTileData>();
            characterToTileDictionary.Add("Nothingness", atlasTileData);
            characterToTileDictionary.Add("Grass", atlasTileData);
            characterToTileDictionary.Add("Character", new AtlasTileData(3, 0));
            characterToTileDictionary.Add("Asphault", atlasTileData);
            characterToTileDictionary.Add("Sidewalk", new AtlasTileData(3,1));
            characterToTileDictionary.Add("PaintedAsphault", atlasTileData);

        }

        TileModel foregroundModel;
        TileModel backgroundModel;
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
            }

            IEntity worldEntity = game.TimelineEntity;
            IWorldProvider worldProvider = null;
            if (worldEntity != null)
            {
                worldProvider = worldEntity.GetComponentOfType<TimeLine>().CurrentTimelineLayer.Chunks;
            }

            var entity = game.CameraEntity;

            ConsoleCamera camera = entity.GetComponentOfType<ConsoleCamera>();
            Screen screen = entity.GetComponentOfType<Screen>();
            Commander commander = game.Commander;
            screen = UpdateZoom(game, commander, entity, screen , out bool zoomUpdate);

            if (foregroundModel == null || zoomUpdate)
            {
                foregroundModel = new TileModel(screen.Height, screen.Width);
            }

            if (backgroundModel == null || zoomUpdate)
            {
                backgroundModel = new TileModel(screen.Height, screen.Width);
            }


            if (camera != null && screen != null && worldProvider != null)
            {
                MoveCamera(game, camera);
                FillcharacterBufferVisibility(game, screen, camera, game.GetSettings(), worldProvider);
                FillcharacterBuffersWithWorld(screen, camera, game.GetSettings(), worldProvider);
            //    FillcharacterBuffersWithTileObjects(screen, camera, game.GetSettings(), game, worldProvider);
            //    FillcharacterBuffersWithWorldObjects(screen, camera, game.GetSettings(), game);
                RenderScreen(game, screen, game.GetSettings());
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
                    screen.ScreenBuffer[x, y].isVisible = true;
                }
            }

            return;
            if (fov == null)
            {
                fov = new PermissiveVisibility((x, y) => { return !world.GetTile(x, y).GetBlocksVision(game); },
                    (x, y) =>
                    {
                        Point screenPoint = camera.PointToScreen(x, y);
                        if (screenPoint.X >= 0 && screenPoint.X < settings.GetWidth() && screenPoint.Y >= 0 &&
                            screenPoint.Y < settings.GetHeight())
                        {
                            screen.ScreenBuffer[screenPoint.X, screenPoint.Y].isVisible = true;
                        }
                    }, (x, y) => { return Math.Abs(x) + Math.Abs(y); }
                );
            }

            fov.Compute(playerPosition.Point,60);
        }


        private void FillcharacterBuffersWithTileObjects(Screen screen, ConsoleCamera camera, GameSettings settings,
            NamelessGame game, IWorldProvider world)
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
                    if (screen.ScreenBuffer[screenPoint.X, screenPoint.Y].isVisible && x > 0 && y > 0)
                    {
                        Tile tileToDraw = world.GetTile(x, y);

                        foreach (var entity in tileToDraw.GetEntities())
                        {
                            var furniture = entity.GetComponentOfType<Furniture>();
                            var drawable = entity.GetComponentOfType<Drawable>();
                            if (furniture != null && drawable != null)
                            {
                                screen.ScreenBuffer[screenPoint.X, screenPoint.Y].ObjectId = drawable.Representation;
                                screen.ScreenBuffer[screenPoint.X, screenPoint.Y].CharColor = drawable.CharColor;
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
                    if (screen.ScreenBuffer[screenPoint.X, screenPoint.Y].isVisible && x>0 && y>0)
                    {
                        Tile tileToDraw = world.GetTile(x, y);
                        GetTerrainTile(screen, TerrainLibrary.Terrains[tileToDraw.Terrain], screenPoint);

                    }
                    else
                    {
                        screen.ScreenBuffer[screenPoint.X, screenPoint.Y].ObjectId = "Nothingness";
                        screen.ScreenBuffer[screenPoint.X, screenPoint.Y].BackGroundColor = new Color();
                    }
                }
            }
        }

        void GetTerrainTile(Screen screen, Terrain terrain, Point point)
        {
         
                screen.ScreenBuffer[point.X, point.Y].ObjectId = terrain.Representation.Representation;
                screen.ScreenBuffer[point.X, point.Y].CharColor = terrain.Representation.CharColor;
                screen.ScreenBuffer[point.X, point.Y].BackGroundColor = terrain.Representation.BackgroundColor;
        }


        private void FillcharacterBuffersWithWorldObjects(Screen screen, ConsoleCamera camera, GameSettings settings,
            NamelessGame game)
        {
            List<IEntity> characters = new List<IEntity>();
            foreach (IEntity entity in RegisteredEntities)
            {
                Drawable drawable = entity.GetComponentOfType<Drawable>();

                var character = entity.GetComponentOfType<Character>();
                if (character != null)
                {
                    characters.Add(entity);
                    continue;
                }

                Position position = entity.GetComponentOfType<Position>();

                LineToPlayer lineToPlayer = entity.GetComponentOfType<LineToPlayer>();
                if (drawable.Visible)
                {
                    Point screenPoint = camera.PointToScreen(position.Point.X, position.Point.Y);
                    int x = screenPoint.X;
                    int y = screenPoint.Y;
                    if (x >= 0 && x < settings.GetWidth() && y >= 0 && y < settings.GetHeight())
                    {
                        if (screen.ScreenBuffer[screenPoint.X, screenPoint.Y].isVisible)
                        {                                  
                            screen.ScreenBuffer[screenPoint.X, screenPoint.Y].ObjectId = drawable.Representation;
                            screen.ScreenBuffer[screenPoint.X, screenPoint.Y].CharColor = drawable.CharColor;
                        }                                  
                        else                               
                        {                                  
                            screen.ScreenBuffer[screenPoint.X, screenPoint.Y].ObjectId = drawable.Representation;
                            screen.ScreenBuffer[screenPoint.X, screenPoint.Y].CharColor = new Color();
                            screen.ScreenBuffer[screenPoint.X, screenPoint.Y].BackGroundColor = new Color();
                        }
                    }

                }

                if (lineToPlayer != null)
                {
                    if (drawable.Visible)
                    {
                        Position playerPosition =
                            game.PlayerEntity.GetComponentOfType<Position>();
                        List<Point> line = PointUtil.getLine(playerPosition.Point, position.Point);
                        for (int i = 1; i < line.Count - 1; i++)
                        {
                            Point p = line[i];
                            Point screenPoint = camera.PointToScreen(p.X, p.Y);
                            int x = screenPoint.X;
                            int y = screenPoint.Y;
                            if (x >= 0 && x < settings.GetWidth() && y >= 0 && y < settings.GetHeight())
                            {
                                screen.ScreenBuffer[screenPoint.X, screenPoint.Y].ObjectId = "Cursor";
                                screen.ScreenBuffer[screenPoint.X, screenPoint.Y].CharColor = drawable.CharColor;
                            }
                        }
                    }
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
                    Point screenPoint = camera.PointToScreen(position.Point.X, position.Point.Y);
                    int x = screenPoint.X;
                    int y = screenPoint.Y;
                    if (x >= 0 && x < settings.GetWidthZoomed() && y >= 0 && y < settings.GetHeightZoomed())
                    {
                        if (screen.ScreenBuffer[screenPoint.X, screenPoint.Y].isVisible)
                        {                                                  
                            screen.ScreenBuffer[screenPoint.X, screenPoint.Y].ObjectId = drawable.Representation;
                            screen.ScreenBuffer[screenPoint.X, screenPoint.Y].CharColor = drawable.CharColor;
                        }                                                 
                        else                                               
                        {
                            screen.ScreenBuffer[screenPoint.X, screenPoint.Y].ObjectId = "Nothingness";
                            screen.ScreenBuffer[screenPoint.X, screenPoint.Y].CharColor = new Color();
                            screen.ScreenBuffer[screenPoint.X, screenPoint.Y].BackGroundColor = new Color();
                        }
                    }

                }
            }
        }


        private void RenderScreen(NamelessGame game, Screen screen, GameSettings settings)
        {
            effect.Parameters["tileAtlas"].SetValue(tileAtlas);
            var projectionMatrix = //Matrix.CreateOrthographic(game.getActualWidth(),game.getActualHeight(),0,1);
    Matrix.CreateOrthographicOffCenter(0, game.GetActualWidth(), game.GetActualHeight(), 0, 0, 2);

            effect.Parameters["xViewProjection"].SetValue(projectionMatrix);

            effect.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            effect.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            var device = game.GraphicsDevice;
            Stopwatch s = Stopwatch.StartNew();
            for (int y = 0; y < settings.GetHeightZoomed(); y++)
            {
                for (int x = 0; x < settings.GetWidthZoomed(); x++)
                {
                    string objectId = screen.ScreenBuffer[x, y].ObjectId;
                    if(objectId == null)
                    {
                        objectId = "Nothingness";
                    }

                    DrawTile(game.GraphicsDevice, game, x, y,
                        x * settings.GetFontSizeZoomed(),
                        y * settings.GetFontSizeZoomed(),
                        characterToTileDictionary[objectId],
                        screen.ScreenBuffer[x, y].CharColor,
                        screen.ScreenBuffer[x, y].BackGroundColor, foregroundModel, backgroundModel
                        );
                }
            }
            s.Stop();
            var tileModel = backgroundModel;
			effect.CurrentTechnique = effect.Techniques["Background"];
			foreach (EffectPass pass in effect.CurrentTechnique.Passes)
			{
				pass.Apply();
				device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, tileModel.Vertices, 0, tileModel.Vertices.Length,
					 tileModel.Indices.ToArray(), 0, 2, this.VertexDeclaration);
			}

			effect.CurrentTechnique = effect.Techniques["Point"];

            tileModel = foregroundModel;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, tileModel.Vertices, 0, tileModel.Vertices.Length,
                     tileModel.Indices.ToArray(), 0, tileModel.Indices.Count()/3, this.VertexDeclaration);
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
            tileAtlas = game.Content.Load<Texture2D>("Sprites/surfaces");
            effect = game.Content.Load<Effect>("Shader");

            effect.Parameters["tileAtlas"].SetValue(tileAtlas);
            return tileAtlas;
        }

        void DrawTile(GraphicsDevice device, NamelessGame game, int screenPositionX, int screenPositionY, int positionX, int positionY,
            AtlasTileData atlasTileData,
            Color color, Color backGroundColor, TileModel foregroundModel, TileModel backgroundModel)
        {

            if (atlasTileData == null)
            {
                atlasTileData = new AtlasTileData(1, 1);
            }


            int tileHeight = game.GetSettings().GetFontSizeZoomed();
            int tileWidth = game.GetSettings().GetFontSizeZoomed();


            var onePixelPercentageWidth = 1 / (float)tileAtlas.Width;
            var onePixelPercentageHeight = 1 / (float)tileAtlas.Height;

            float textureX = (atlasTileData.X * (Constants.tileAtlasTileSize / (float)tileAtlas.Width)) + (onePixelPercentageWidth * 1.2f);
            float textureY = (atlasTileData.Y * (Constants.tileAtlasTileSize / (float)tileAtlas.Height))+ (onePixelPercentageHeight * 1.2f);

       

            float textureXend = ((atlasTileData.X + 1f) * (Constants.tileAtlasTileSize / (float)tileAtlas.Width)) - (onePixelPercentageWidth * 2.2f);

            float textureYend = ((atlasTileData.Y + 1f) * (Constants.tileAtlasTileSize / (float)tileAtlas.Height)) - (onePixelPercentageHeight * 2.2f);

            var settings = game.GetSettings();

            var arrayPosition = (screenPositionX * 4) + (screenPositionY * settings.GetWidthZoomed() * 4);



            var foregroundvertices = foregroundModel.Vertices;

            foregroundvertices[arrayPosition] = new Vertex(new Vector3(positionX, positionY, 0), color.ToVector4(),
                backGroundColor.ToVector4(), new Vector2(textureX, textureY));
            foregroundvertices[arrayPosition + 1] = new Vertex(new Vector3(positionX + tileWidth, positionY, 0), color.ToVector4(),
                backGroundColor.ToVector4(), new Vector2(textureXend, textureY));
            foregroundvertices[arrayPosition + 2] = new Vertex(new Vector3(positionX, positionY + tileHeight, 0), color.ToVector4(),
                backGroundColor.ToVector4(), new Vector2(textureX, textureYend));
            foregroundvertices[arrayPosition + 3] = new Vertex(new Vector3(positionX + tileWidth, positionY + tileHeight, 0), color.ToVector4(),
                backGroundColor.ToVector4(), new Vector2(textureXend, textureYend));

            var backgroundVertices = backgroundModel.Vertices;

            backgroundVertices[arrayPosition] = new Vertex(new Vector3(positionX, positionY, 0), color.ToVector4(),
                backGroundColor.ToVector4(), new Vector2(textureX, textureY));
            backgroundVertices[arrayPosition + 1] = new Vertex(new Vector3(positionX + tileWidth, positionY, 0), color.ToVector4(),
                backGroundColor.ToVector4(), new Vector2(textureXend, textureY));
            backgroundVertices[arrayPosition + 2] = new Vertex(new Vector3(positionX, positionY + tileHeight, 0), color.ToVector4(),
                backGroundColor.ToVector4(), new Vector2(textureX, textureYend));
            backgroundVertices[arrayPosition + 3] = new Vertex(new Vector3(positionX + tileWidth, positionY + tileHeight, 0), color.ToVector4(),
                backGroundColor.ToVector4(), new Vector2(textureXend, textureYend));

        }
    }
}
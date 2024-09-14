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

namespace NamelessRogue.Engine.Systems.Ingame
{

    public enum TilesetModifier
    {
        Top,
        Bottom, 
        Left,
        Right,
        Corner,
        Center,     
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
        private int playerPosZ;

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
            

            var atlasTileData = new AtlasTileData(8, 9);
            characterToTileDictionary = new Dictionary<string, AtlasTileData>();
            characterToTileDictionary.Add("Nothingness", atlasTileData);
            characterToTileDictionary.Add("Dirt", new AtlasTileData(2, 4));
            characterToTileDictionary.Add("Character", new AtlasTileData(0,7));
            characterToTileDictionary.Add("Asphault", new AtlasTileData(1,3));
            characterToTileDictionary.Add("Sidewalk", new AtlasTileData(0, 3));
            characterToTileDictionary.Add("PaintedAsphault", new AtlasTileData(2, 3));
            characterToTileDictionary.Add("FloorGrate", new AtlasTileData(0, 4));
            characterToTileDictionary.Add("smallCursor", new AtlasTileData(0, 6));
            characterToTileDictionary.Add("Cursor", new AtlasTileData(1, 6));

            characterToTileDictionary.Add("wall", atlasTileData);
            characterToTileDictionary.Add("door", atlasTileData);
            characterToTileDictionary.Add("window", atlasTileData);
            characterToTileDictionary.Add("stairs_down", new AtlasTileData(8, 0));
            characterToTileDictionary.Add("stairs_up", new AtlasTileData(8, 0));

            characterToTileDictionary.Add("table", new AtlasTileData(2, 8));

            characterToTileDictionary.Add("bed", new AtlasTileData(1, 9));

            characterToTileDictionary.Add("toilet", new AtlasTileData(3, 8));
            characterToTileDictionary.Add("shower", new AtlasTileData(0, 8));

            characterToTileDictionary.Add("wall" + TileBitmaskingEncoding.WallHorizontal0, new AtlasTileData(1, 0));
            characterToTileDictionary.Add("wall" + TileBitmaskingEncoding.WallHorizontal1, new AtlasTileData(1, 0));
            characterToTileDictionary.Add("wall" + TileBitmaskingEncoding.WallHorizontal2, new AtlasTileData(1, 0));
                                           
            characterToTileDictionary.Add("wall" + TileBitmaskingEncoding.WallVertical0, new AtlasTileData(0, 1));
            characterToTileDictionary.Add("wall" + TileBitmaskingEncoding.WallVertical1, new AtlasTileData(0, 1));
            characterToTileDictionary.Add("wall" + TileBitmaskingEncoding.WallVertical2, new AtlasTileData(4, 0));
                                           
            characterToTileDictionary.Add("wall" + TileBitmaskingEncoding.CornerTopLeft, new AtlasTileData(0, 0));
            characterToTileDictionary.Add("wall" + TileBitmaskingEncoding.CornerBotLeft, new AtlasTileData(0, 2));
            characterToTileDictionary.Add("wall" + TileBitmaskingEncoding.CornerTopRight, new AtlasTileData(2, 0));
            characterToTileDictionary.Add("wall" + TileBitmaskingEncoding.CornerBotRight, new AtlasTileData(2, 2));
                                           
            characterToTileDictionary.Add("wall" + TileBitmaskingEncoding.InterserctionLeft, new AtlasTileData(3, 1));
            characterToTileDictionary.Add("wall" + TileBitmaskingEncoding.InterserctionRight, new AtlasTileData(3, 1, true));
            characterToTileDictionary.Add("wall" + TileBitmaskingEncoding.InterserctionTop, new AtlasTileData(3, 0));
            characterToTileDictionary.Add("wall" + TileBitmaskingEncoding.InterserctionBot, new AtlasTileData(3, 2));
            characterToTileDictionary.Add("wall" + TileBitmaskingEncoding.InterserctionCenter, new AtlasTileData(4, 2));
            characterToTileDictionary.Add("wall" + TileBitmaskingEncoding.Pillar, new AtlasTileData(4, 1));

            var doorData = new AtlasTileData(5, 0);

            characterToTileDictionary.Add("door" + TileBitmaskingEncoding.WallHorizontal0, doorData);
            characterToTileDictionary.Add("door" + TileBitmaskingEncoding.WallHorizontal1, doorData);
            characterToTileDictionary.Add("door" + TileBitmaskingEncoding.WallHorizontal2, doorData);

            characterToTileDictionary.Add("door" + TileBitmaskingEncoding.WallVertical0, doorData);
            characterToTileDictionary.Add("door" + TileBitmaskingEncoding.WallVertical1, doorData);
            characterToTileDictionary.Add("door" + TileBitmaskingEncoding.WallVertical2, doorData);

            var openDoorData = new AtlasTileData(7, 0);


            characterToTileDictionary.Add("openDoor" + TileBitmaskingEncoding.WallHorizontal0, openDoorData);
            characterToTileDictionary.Add("openDoor" + TileBitmaskingEncoding.WallHorizontal1, openDoorData);
            characterToTileDictionary.Add("openDoor" + TileBitmaskingEncoding.WallHorizontal2, openDoorData);

            characterToTileDictionary.Add("openDoor" + TileBitmaskingEncoding.WallVertical0, openDoorData);
            characterToTileDictionary.Add("openDoor" + TileBitmaskingEncoding.WallVertical1, openDoorData);
            characterToTileDictionary.Add("openDoor" + TileBitmaskingEncoding.WallVertical2, openDoorData);

            characterToTileDictionary.Add("window" + TileBitmaskingEncoding.WallHorizontal0, new AtlasTileData(6, 0));
            characterToTileDictionary.Add("window" + TileBitmaskingEncoding.WallHorizontal1, new AtlasTileData(6, 0));
            characterToTileDictionary.Add("window" + TileBitmaskingEncoding.WallHorizontal2, new AtlasTileData(6, 0));

            characterToTileDictionary.Add("window" + TileBitmaskingEncoding.WallVertical0, new AtlasTileData(6, 1));
            characterToTileDictionary.Add("window" + TileBitmaskingEncoding.WallVertical1, new AtlasTileData(6, 1));
            characterToTileDictionary.Add("window" + TileBitmaskingEncoding.WallVertical2, new AtlasTileData(6, 1));


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
            Position playerPosition = game.FollowedByCameraEntity
                   .GetComponentOfType<Position>();

            playerPosZ = playerPosition.Z;

            if (camera != null && screen != null && worldProvider != null)
            {
                MoveCamera(game, camera);
                ClearScreen(screen, camera, game.GetSettings(), worldProvider);
                FillcharacterBufferVisibility(game, screen, camera, game.GetSettings(), worldProvider);
                FillcharacterBuffersWithWorld(screen, camera, game.GetSettings(), worldProvider);
                FillcharacterBuffersWithTileObjects(screen, camera, game.GetSettings(), game, worldProvider);
                FillcharacterBuffersWithWorldObjects(screen, camera, game.GetSettings(), game);
                int stackDepth = 0;
                while (RenderScreen(game, screen, game.GetSettings(), stackDepth)) {
                    stackDepth++;
                }               
                game.Batch.Begin();
                RenderSpriteScreen(game, screen, game.GetSettings(), gameTime);
                game.Batch.End();
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
                }
            }
            screenCopy = screen;
           // return;
            if (fov == null)
            {
                fov = new PermissiveVisibility((x, y) => {
                    var worldTile = world.GetTile(x, y, playerPosZ);
                    if(worldTile == null)
                    {
                        return false;
                    }
                    return !world.GetTile(x, y, playerPosZ).GetBlocksVision(game); 
                },
                    (x, y) =>
                    {
                        Point screenPoint = camera.PointToScreen(x, y);
                        if (screenPoint.X >= 0 && screenPoint.X < settings.GetWidthZoomed() && screenPoint.Y >= 0 &&
                            screenPoint.Y < settings.GetHeightZoomed())
                        {
                            screenCopy.ScreenBuffer[screenPoint.X, screenPoint.Y].isVisible = true;
                        }
                    }, (x, y) => { return Math.Abs(x) + Math.Abs(y); }
                );
            }
            fov.Compute(new Point(playerPosition.Point.X, playerPosition.Point.Y), 60);
        }


        private void FillcharacterBuffersWithTileObjects(Screen screen, ConsoleCamera camera, GameSettings settings,
            NamelessGame game, IWorldProvider world)
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
                    if (screen.ScreenBuffer[screenPoint.X, screenPoint.Y].isVisible && x > 0 && y > 0)
                    {
                        Tile tileToDraw = world.GetTile(x, y, playerPosZ);

                        if (tileToDraw != null)
                        {
                            foreach (var entity in tileToDraw.GetEntities())
                            {
                                var furniture = entity.GetComponentOfType<Furniture>();
                                var drawable = entity.GetComponentOfType<Drawable>();
                                var sprited = entity.GetComponentOfType<SpritedObject>();
                                if (furniture != null && drawable != null && sprited == null)
                                {
                                    screen.ScreenBuffer[screenPoint.X, screenPoint.Y].AddObject(drawable.ObjectID + drawable.TilesetPosition, ScreenObjectSource.Tileset);
                                    screen.ScreenBuffer[screenPoint.X, screenPoint.Y].CharColor = drawable.CharColor;
                                }
                                else if(furniture != null && drawable != null && sprited != null)
                                {
                                    screen.ScreenBuffer[screenPoint.X, screenPoint.Y].AddObject(drawable.ObjectID + drawable.TilesetPosition, ScreenObjectSource.AnimatedSprite);
                                    screen.ScreenBuffer[screenPoint.X, screenPoint.Y].CharColor = drawable.CharColor;
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
                    if (screen.ScreenBuffer[screenPoint.X, screenPoint.Y].isVisible && x>0 && y>0)
                    {
                        Tile tileToDraw = world.GetTile(x, y, playerPosZ);

                        if (tileToDraw != null)
                        {
                            GetTerrainTile(screen, TerrainLibrary.Terrains[tileToDraw.Terrain], screenPoint);
                        }
                        else
                        {
                            screen.ScreenBuffer[screenPoint.X, screenPoint.Y].AddObject("Nothingness", ScreenObjectSource.Tileset);
                            screen.ScreenBuffer[screenPoint.X, screenPoint.Y].BackGroundColor = new Color();
                        }

                    }
                    else
                    {
                        screen.ScreenBuffer[screenPoint.X, screenPoint.Y].AddObject("Nothingness", ScreenObjectSource.Tileset);
                        screen.ScreenBuffer[screenPoint.X, screenPoint.Y].BackGroundColor = new Color();
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
                    screen.ScreenBuffer[screenPoint.X, screenPoint.Y].BackGroundColor = new Color();

                }
            }
        }

        void GetTerrainTile(Screen screen, Terrain terrain, Point point)
        {

            screen.ScreenBuffer[point.X, point.Y].AddObject(terrain.Representation.ObjectID, ScreenObjectSource.Tileset);
            screen.ScreenBuffer[point.X, point.Y].CharColor = terrain.Representation.CharColor;
            screen.ScreenBuffer[point.X, point.Y].BackGroundColor = terrain.Representation.BackgroundColor;
        }


        private void FillcharacterBuffersWithWorldObjects(Screen screen, ConsoleCamera camera, GameSettings settings,
            NamelessGame game)
        {
            {
                var cursorEntity = game.CursorEntity;
                Position cursorPosition = cursorEntity.GetComponentOfType<Position>();

                LineToPlayer lineToPlayer = cursorEntity.GetComponentOfType<LineToPlayer>();
                Drawable cursorDrawable = cursorEntity.GetComponentOfType<Drawable>();
                if (cursorDrawable.Visible)
                {
                    {
                        Point screenPoint = camera.PointToScreen(cursorPosition.X, cursorPosition.Y);
                        int x = screenPoint.X;
                        int y = screenPoint.Y;
                        if (x >= 0 && x < settings.GetWidthZoomed() && y >= 0 && y < settings.GetHeightZoomed())
                        {
                            if (screen.ScreenBuffer[screenPoint.X, screenPoint.Y].isVisible)
                            {
                                screen.ScreenBuffer[screenPoint.X, screenPoint.Y].AddObject(cursorDrawable.ObjectID, ScreenObjectSource.Tileset);
                                screen.ScreenBuffer[screenPoint.X, screenPoint.Y].CharColor = cursorDrawable.CharColor;
                            }
                        }
                    }

                    if (lineToPlayer != null)
                    {
                        Position playerPosition =
                            game.PlayerEntity.GetComponentOfType<Position>();
                        List<Point> line = PointUtil.getLine(playerPosition.Point.ToPoint(), cursorPosition.Point.ToPoint());
                        for (int i = 0; i < line.Count - 1; i++)
                        {
                            //how is this switched?
                            Point p = new Point(line[i].Y, line[i].X);
                            Point screenPoint = camera.PointToScreen(p.X, p.Y);
                            int x = screenPoint.X;
                            int y = screenPoint.Y;
                            if (x >= 0 && x < settings.GetWidthZoomed() && y >= 0 && y < settings.GetHeightZoomed())
                            {
                                string objectId = i == (line.Count() - 1) ? "Cursor" : "smallCursor";
                                screen.ScreenBuffer[screenPoint.X, screenPoint.Y].AddObject(objectId, ScreenObjectSource.Tileset);
                                screen.ScreenBuffer[screenPoint.X, screenPoint.Y].CharColor = cursorDrawable.CharColor;
                            }
                        }
                    }
                }
            }

            List<IEntity> characters = new List<IEntity>();
            foreach (IEntity entity in RegisteredEntities)
            {
                Drawable drawable = entity.GetComponentOfType<Drawable>();

                var character = entity.GetComponentOfType<Character>();
                if (character == null)
                {
                   
                    continue;
                }
                characters.Add(entity);
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
                            screen.ScreenBuffer[screenPoint.X, screenPoint.Y].AddObject("Character", ScreenObjectSource.Tileset);
                            screen.ScreenBuffer[screenPoint.X, screenPoint.Y].CharColor = drawable.CharColor;
                        }                                                 
                        else                                               
                        {
                            screen.ScreenBuffer[screenPoint.X, screenPoint.Y].AddObject("Nothingness", ScreenObjectSource.Tileset);
                            screen.ScreenBuffer[screenPoint.X, screenPoint.Y].CharColor = new Color();
                            screen.ScreenBuffer[screenPoint.X, screenPoint.Y].BackGroundColor = new Color();
                        }
                    }

                }
            }
        }

        private bool RenderScreen(NamelessGame game, Screen screen, GameSettings settings, int stackDepth)
        {
            bool moreItemsToRender = false;
            effect.Parameters["tileAtlas"].SetValue(tileAtlas);
            var projectionMatrix = //Matrix.CreateOrthographic(game.getActualWidth(),game.getActualHeight(),0,1);
    Matrix.CreateOrthographicOffCenter(0, game.GetActualWidth(), game.GetActualHeight(), 0, 0, 2);

            effect.Parameters["xViewProjection"].SetValue(projectionMatrix);

            effect.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
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
                            DrawTile(game.GraphicsDevice, game, x, y,
                                x * settings.GetFontSizeZoomed(),
                                y * settings.GetFontSizeZoomed(),
                                tileData,
                                //screen.ScreenBuffer[x, y].CharColor,
                                white,
                                screen.ScreenBuffer[x, y].BackGroundColor, foregroundModel, backgroundModel
                                );
                        }
                    }
                 
                }
            }
           //s.Stop();
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
                        if (objectToDraw.Type == ScreenObjectSource.AnimatedSprite)
                        {
                            var spriteId = objectToDraw.Id;
                            int tileHeight = game.GetSettings().GetFontSizeZoomed();
                            int tileWidth = game.GetSettings().GetFontSizeZoomed();
                            if (SpriteLibrary.SpritesAnimatedIdle.TryGetValue(spriteId, out var sprite))
                            {
                                sprite.Update(gameTime);
                                sprite.Draw(game, gameTime, new Vector2(x * tileWidth, y * tileHeight), new Vector2(1f / settings.Zoom), Microsoft.Xna.Framework.Color.White);
                            }
                        }
                    }
                }
            }
        }

        class AtlasTileData
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



        private Texture InitializeTexture(NamelessGame game)
        {

            tileAtlas = null;
            tileAtlas = game.Content.Load<Texture2D>("Sprites/tileset2");
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


            float textureX = atlasTileData.X * (Constants.tileAtlasTileSize / (float)tileAtlas.Width);
            float textureY = atlasTileData.Y * (Constants.tileAtlasTileSize / (float)tileAtlas.Height);

            float textureXend = (atlasTileData.X + 1f) * (Constants.tileAtlasTileSize / (float)tileAtlas.Width);

            float textureYend = (atlasTileData.Y + 1f) * (Constants.tileAtlasTileSize / (float)tileAtlas.Height);

            var settings = game.GetSettings();

            var arrayPosition = (screenPositionX * 4) + (screenPositionY * settings.GetWidthZoomed() * 4);


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
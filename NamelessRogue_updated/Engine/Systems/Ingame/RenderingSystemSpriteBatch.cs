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

namespace NamelessRogue.Engine.Systems.Ingame
{

    public class RenderingSystemSpriteBatch : BaseSystem
    {
        public override HashSet<Type> Signature { get; }

        Dictionary<char, AtlasTileData> characterToTileDictionary;
        private float gameTime;
        private float angle = 0;
        private float step = 0.04f;

        public RenderingSystemSpriteBatch(GameSettings settings)
        {
            InitializeCharacterTileDictionary();

            Signature = new HashSet<Type>();
            Signature.Add(typeof(Drawable));
            Signature.Add(typeof(Position));
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
        SpriteBatch _spriteBatch;

        public override void Update(long gameTime, NamelessGame game)
		{



			this.gameTime = gameTime;

			//todo move to constructor or some other place better suited for initialization
			if (tileAtlas == null)
			{
				InitializeTexture(game);
				_spriteBatch = new SpriteBatch(game.GraphicsDevice);
			}


			Commander commander = game.Commander;


			var entity = game.CameraEntity;
			ConsoleCamera camera = entity.GetComponentOfType<ConsoleCamera>();
			Screen screen = entity.GetComponentOfType<Screen>();

			screen = UpdateZoom(game, commander, entity, screen);

			_spriteBatch.Begin(SpriteSortMode.Deferred, null, null, DepthStencilState.None);


			IEntity worldEntity = game.TimelineEntity;
			IWorldProvider worldProvider = null;
			if (worldEntity != null)
			{
				worldProvider = worldEntity.GetComponentOfType<TimeLine>().CurrentTimelineLayer.Chunks;
			}


			if (camera != null && screen != null && worldProvider != null)
			{
				MoveCamera(game, camera);
				FillcharacterBufferVisibility(game, screen, camera, game.GetSettings(), worldProvider);
				FillcharacterBuffersWithWorld(screen, camera, game.GetSettings(), worldProvider);
				FillcharacterBuffersWithTileObjects(screen, camera, game.GetSettings(), game, worldProvider);
				FillcharacterBuffersWithWorldObjects(screen, camera, game.GetSettings(), game);
				RenderScreen(game, screen, game.GetSettings());
			}
			_spriteBatch.End();
		}

		private static Screen UpdateZoom(NamelessGame game, Commander commander, IEntity entity, Screen screen)
		{
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
                    screen.ScreenBuffer[x, y].isVisible = false;
                }
            }

            //return;

            if (fov == null)
            {
                {
                  
                    fov = new PermissiveVisibility((x, y) => { return !world.GetTile(x, y).GetBlocksVision(game); },
                        (x, y) =>
                        {
                            Stopwatch s = Stopwatch.StartNew();
                            var lambdaLocalScreen = game.CameraEntity.GetComponentOfType<Screen>();
                            s.Stop();
                            s.ToString();

                            Point screenPoint = camera.PointToScreen(x, y);
                            if (screenPoint.X >= 0 && screenPoint.X < settings.GetWidthZoomed() && screenPoint.Y >= 0 &&
                                screenPoint.Y < settings.GetHeightZoomed())
                            {
                                lambdaLocalScreen.ScreenBuffer[screenPoint.X, screenPoint.Y].isVisible = true;
                            }
                        }, (x, y) =>
                        {
                            if (Math.Abs(x) > 60 || Math.Abs(y) > 60)
                            {
                                return 10000;
                            }
                         //   return (int)((x*x) + (y*y)); 
                        return Math.Abs(x) + Math.Abs(y);
                        }
                    );
                }
            }
            fov.Compute(playerPosition.Point, 60);
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
                    if (screen.ScreenBuffer[screenPoint.X, screenPoint.Y].isVisible)
                    {
                        Tile tileToDraw = world.GetTile(x, y);

                        foreach (var entity in tileToDraw.GetEntities())
                        {
                            var furniture = entity.GetComponentOfType<Furniture>();
                            var drawable = entity.GetComponentOfType<Drawable>();
                            if (furniture != null && drawable != null)
                            {
                                screen.ScreenBuffer[screenPoint.X, screenPoint.Y].Char = drawable.Representation;
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
                    if (screen.ScreenBuffer[screenPoint.X, screenPoint.Y].isVisible)
                    {
                        Tile tileToDraw = world.GetTile(x, y);
                        GetTerrainTile(screen, TerrainLibrary.Terrains[tileToDraw.Terrain], screenPoint);
                    }
                    else
                    {
                        screen.ScreenBuffer[screenPoint.X, screenPoint.Y].Char = ' ';
                        screen.ScreenBuffer[screenPoint.X, screenPoint.Y].BackGroundColor = new Color();
                    }
                }
            }
        }

        void GetTerrainTile(Screen screen, Terrain terrain, Point point)
        {

            screen.ScreenBuffer[point.X, point.Y].Char = terrain.Representation.Representation;
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
                    if (x >= 0 && x < settings.GetWidthZoomed() && y >= 0 && y < settings.GetHeightZoomed())
                    {
                        if (screen.ScreenBuffer[screenPoint.X, screenPoint.Y].isVisible)
                        {
                            screen.ScreenBuffer[screenPoint.X, screenPoint.Y].Char = drawable.Representation;
                            screen.ScreenBuffer[screenPoint.X, screenPoint.Y].CharColor = drawable.CharColor;
                        }
                        else
                        {
                            screen.ScreenBuffer[screenPoint.X, screenPoint.Y].Char = ' ';
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
                            if (x >= 0 && x < settings.GetWidthZoomed() && y >= 0 && y < settings.GetHeightZoomed())
                            {
                                screen.ScreenBuffer[screenPoint.X, screenPoint.Y].Char = 'x';
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
                            screen.ScreenBuffer[screenPoint.X, screenPoint.Y].Char = drawable.Representation;
                            screen.ScreenBuffer[screenPoint.X, screenPoint.Y].CharColor = drawable.CharColor;
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

        }


        private void RenderScreen(NamelessGame gameInstance, Screen screen, GameSettings settings)
        {
            for (int x = 0; x < settings.GetWidthZoomed(); x++)
            {
                for (int y = 0; y < settings.GetHeightZoomed(); y++)
                {
                    if (screen.ScreenBuffer[x, y].isVisible)
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
        Texture2D whiteRectangle = null;



        private Texture InitializeTexture(NamelessGame game)
        {

            tileAtlas = null;
            tileAtlas = game.Content.Load<Texture2D>("DFfont");

            whiteRectangle = new Texture2D(game.GraphicsDevice, 1, 1);
            whiteRectangle.SetData(new[] { Microsoft.Xna.Framework.Color.White });

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
    }
}
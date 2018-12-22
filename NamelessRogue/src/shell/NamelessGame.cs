using System;
using System.Collections.Generic;
using System.Linq;
using GeonBit.UI;
using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components;
using NamelessRogue.Engine.Engine.Context;
using NamelessRogue.Engine.Engine.Factories;
using NamelessRogue.Engine.Engine.Generation;
using NamelessRogue.Engine.Engine.Generation.World;
using NamelessRogue.Engine.Engine.Infrastructure;
using NamelessRogue.Engine.Engine.Systems;
using NamelessRogue.Engine.Engine.Utility;
using NamelessRogue.Storage.data;
using Color = Microsoft.Xna.Framework.Color;

namespace NamelessRogue.shell
{
    public class NamelessGame : Game
    {
        private static long serialVersionUID = 1L;

        List<IEntity> ActiveEntities;

        List<IEntity> OfflineEntities;

        //InputSystem inputsystem;

        public static GraphicsDevice DebugDevice;

        public List<IEntity> GetEntities()
        {
            return ActiveEntities;
        }

        public IEntity GetEntity(Guid id)
        {
            return ActiveEntities.FirstOrDefault(x => x.GetId() == id);
        }

        public void RemoveEntity(IEntity entity)
        {
            ActiveEntities.Remove(entity);
            EntityManager.RemoveEntity(entity.GetId());
        }

        public List<IEntity> GetEntitiesByComponentClass<T>() where T : IComponent
        {
            List<IEntity> results = ActiveEntities.Where(v => v.GetComponentOfType<T>() != null).ToList();
            return results;
        }

        public IEntity GetEntityByComponentClass<T>() where T : IComponent
        {
            return GetEntitiesByComponentClass<T>().FirstOrDefault();
        }

        public GameContext CurrentContext { get; private set; }
        public GameContext ContextToSwitch { get; set; } = null;

        private GameSettings settings;


        public NamelessGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Content.RootDirectory = "Content";
        }



        public int GetActualCharacterWidth()
        {
            return settings.getWidth() * settings.getFontSize();
        }

        public int GetActualCharacterHeight()
        {
            return settings.getHeight() * settings.getFontSize();
        }

        public int GetActualWidth()
        {
            return graphics.PreferredBackBufferWidth;
        }

        public int GetActualHeight()
        {
            return graphics.PreferredBackBufferHeight;
        }





        public GameSettings GetSettings()
        {
            return settings;
        }



        void SetSettings(GameSettings settings)
        {
            this.settings = settings;
        }




        public void WriteLineToConsole(String text)
        {
            //int lineCount = textConsole.getLineCount();
            //if(lineCount>10)
            //{
            //	int howMuchLinesToRemove = lineCount - 10;
            //	for (int i = 0;i<howMuchLinesToRemove;i++) {
            //		int end = 0;
            //		try {
            //			end = textConsole.getLineEndOffset(0);
            //		} catch (BadLocationException e) {
            //			e.printStackTrace();
            //		}
            //		textConsole.replaceRange("", 0, end);
            //	}
            //}
            //textConsole.append("\n");
            //textConsole.append(text);
            //textConsole.setCaretPosition(textConsole.getDocument().getLength());

        }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        WorldSettings worldSettings;
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {


            DebugDevice = this.GraphicsDevice;
            //TODO: move to config later
            int width = 60;
            int height = 40;
            settings = new GameSettings(width, height);
            graphics.PreferredBackBufferWidth = (int) (GetActualCharacterWidth() + settings.HudWidth());
            graphics.PreferredBackBufferHeight = GetActualCharacterHeight();

            graphics.IsFullScreen = false;
            graphics.PreferMultiSampling = false;
            graphics.SynchronizeWithVerticalRetrace = true;


            RenderTarget = new RenderTarget2D(
                GraphicsDevice,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24, 4, RenderTargetUsage.PlatformContents);


            graphics.ApplyChanges();

            //5 for testing
            //1489 for desert
            worldSettings = new WorldSettings(14891,1000,1000);


            ActiveEntities = new List<IEntity>();



            int xoffset = 215;
            int yoffset = 200;

            //TODO: for test
            ActiveEntities.Add(RenderFactory.CreateViewport(settings));
            ActiveEntities.Add(TimelineFactory.CreateTimeline(this));
            ActiveEntities.Add(TerrainFactory.CreateWorld(worldSettings));
            ActiveEntities.Add(InputHandlingFactory.CreateInput());

            var furnitureEntities = TerrainFurnitureFactory.CreateInstancedFurnitureEntities(this);
            foreach (var furnitureEntity in furnitureEntities)
            {
                ActiveEntities.Add(furnitureEntity);
            }


            ActiveEntities.Add(
                CharacterFactory.CreateSimplePlayerCharacter(xoffset * Constants.ChunkSize,
                    yoffset * Constants.ChunkSize));
            ActiveEntities.Add(CharacterFactory.CreateBlankNpc(xoffset * Constants.ChunkSize - 1,
                yoffset * Constants.ChunkSize));
            ActiveEntities.Add(CharacterFactory.CreateBlankNpc(xoffset * Constants.ChunkSize - 3,
                yoffset * Constants.ChunkSize));
            ActiveEntities.Add(CharacterFactory.CreateBlankNpc(xoffset * Constants.ChunkSize - 5,
                yoffset * Constants.ChunkSize));
            ActiveEntities.Add(CharacterFactory.CreateBlankNpc(xoffset * Constants.ChunkSize - 7,
                yoffset * Constants.ChunkSize));
            ActiveEntities.Add(ItemFactory.CreateItem());
            ActiveEntities.Add(GameInitializer.CreateCursor());
            ActiveEntities.Add(GameInitializer.CreateWorldMapMode());
           

            UserInterface.Initialize(Content, BuiltinThemes.hd);
            UserInterface.Active.UseRenderTarget = true;
            CurrentContext = ContextFactory.GetIngameContext(this);
            CurrentContext.ContextScreen.Show();
            this.IsMouseVisible = true;
            UserInterface.Active.ShowCursor = false;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            fpsLabel = new Label("1111", Anchor.TopLeft, new Vector2(1000, 50), new Vector2());
            UserInterface.Active.AddEntity(fpsLabel);

            for (int i = 0; i < 1; i++)
            {
                ActiveEntities.Add(BuildingFactory.CreateDummyBuilding(xoffset * Constants.ChunkSize + 1 + (i * 10),
                    yoffset * Constants.ChunkSize, 10,
                    this));
                ActiveEntities.Add(BuildingFactory.CreateDummyBuilding(xoffset * Constants.ChunkSize + 13 + (i * 10),
                    yoffset * Constants.ChunkSize, 10,
                    this));
                ActiveEntities.Add(BuildingFactory.CreateDummyBuilding(xoffset * Constants.ChunkSize + 1 + (i * 10),
                    yoffset * Constants.ChunkSize + 13,
                    10, this));
                ActiveEntities.Add(BuildingFactory.CreateDummyBuilding(xoffset * Constants.ChunkSize + 1 + 13 + (i * 10),
                    yoffset * Constants.ChunkSize + 13, 10, this));
            }

        }

        public RenderTarget2D RenderTarget { get; set; }

        public SpriteBatch Batch
        {
            get { return spriteBatch; }
        }

        public WorldSettings WorldSettings
        {
            get { return worldSettings; }
            set { worldSettings = value; }
        }

 

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() { }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
           
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (ContextToSwitch != null)
            {
                CurrentContext.ContextScreen.Hide();
                CurrentContext = ContextToSwitch;
                CurrentContext.ContextScreen.Show();
                ContextToSwitch = null;
            }

            CurrentContext.Update((long) gameTime.TotalGameTime.TotalMilliseconds, this);
            UserInterface.Active.Update(gameTime);
        }

        private FrameCounter _frameCounter = new FrameCounter();
        Label fpsLabel;
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _frameCounter.Update(deltaTime);
            fpsLabel.Text = _frameCounter.AverageFramesPerSecond.ToString();

            GraphicsDevice.Clear(Color.Black);
            CurrentContext.RenderingUpdate((long) gameTime.TotalGameTime.TotalMilliseconds, this);

            

        }
    }
}

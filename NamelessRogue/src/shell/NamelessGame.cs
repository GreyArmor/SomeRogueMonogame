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
using NamelessRogue.Engine.Engine.Infrastructure;
using NamelessRogue.Engine.Engine.Systems;
using NamelessRogue.Storage.data;

namespace NamelessRogue.shell
{
    public class NamelessGame : Game
    {
        private static long serialVersionUID = 1L;

        List<IEntity> Entities;
        //InputSystem inputsystem;

        public static GraphicsDevice DebugDevice;

        public List<IEntity> GetEntities()
        {
            return Entities;
        }

        public IEntity GetEntity(Guid id)
        {
            return Entities.FirstOrDefault(x => x.GetId() == id);
        }

        public void RemoveEntity(IEntity entity)
        {
            Entities.Remove(entity);
            EntityManager.RemoveEntity(entity.GetId());
        }

        public List<IEntity> GetEntitiesByComponentClass<T>() where T : IComponent
        {
            List<IEntity> results = Entities.Where(v => v.GetComponentOfType<T>() != null).ToList();
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
            graphics.PreferredBackBufferWidth = GetActualCharacterWidth() + 200;
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
            worldSettings = new WorldSettings(5,1000,1000);


            Entities = new List<IEntity>();



            int xoffset = 215;
            int yoffset = 200;

            //TODO: for test
            Entities.Add(RenderFactory.CreateViewport(settings));
            Entities.Add(TimelineFactory.CreateTimeline(this));
            Entities.Add(TerrainFactory.CreateWorld(worldSettings));
            Entities.Add(InputHandlingFactory.CreateInput());

            Entities.Add(BuildingFactory.CreateDummyBuilding(xoffset * Constants.ChunkSize + 1,
                yoffset * Constants.ChunkSize, 10,
                this));
            Entities.Add(BuildingFactory.CreateDummyBuilding(xoffset * Constants.ChunkSize + 13,
                yoffset * Constants.ChunkSize, 10,
                this));
            Entities.Add(BuildingFactory.CreateDummyBuilding(xoffset * Constants.ChunkSize + 1,
                yoffset * Constants.ChunkSize + 13,
                10, this));
            Entities.Add(BuildingFactory.CreateDummyBuilding(xoffset * Constants.ChunkSize + 1 + 13,
                yoffset * Constants.ChunkSize + 13, 10, this));

            Entities.Add(
                CharacterFactory.CreateSimplePlayerCharacter(xoffset * Constants.ChunkSize,
                    yoffset * Constants.ChunkSize));
            Entities.Add(CharacterFactory.CreateBlankNpc(xoffset * Constants.ChunkSize - 1,
                yoffset * Constants.ChunkSize));
            Entities.Add(CharacterFactory.CreateBlankNpc(xoffset * Constants.ChunkSize - 3,
                yoffset * Constants.ChunkSize));
            Entities.Add(CharacterFactory.CreateBlankNpc(xoffset * Constants.ChunkSize - 5,
                yoffset * Constants.ChunkSize));
            Entities.Add(CharacterFactory.CreateBlankNpc(xoffset * Constants.ChunkSize - 7,
                yoffset * Constants.ChunkSize));
            Entities.Add(ItemFactory.CreateItem());
            Entities.Add(GameInitializer.CreateCursor());


            UserInterface.Initialize(Content, BuiltinThemes.hd);
            UserInterface.Active.UseRenderTarget = true;
            CurrentContext = ContextFactory.GetIngameContext(this);
            CurrentContext.ContextScreen.Show();
            this.IsMouseVisible = true;
            UserInterface.Active.ShowCursor = false;
            spriteBatch = new SpriteBatch(GraphicsDevice);

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



        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            CurrentContext.RenderingUpdate((long) gameTime.TotalGameTime.TotalMilliseconds, this);
        }
    }
}

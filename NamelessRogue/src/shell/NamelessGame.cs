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
using NamelessRogue.Engine.Engine.Factories;
using NamelessRogue.Engine.Engine.Infrastructure;
using NamelessRogue.Engine.Engine.Systems;
using NamelessRogue.Storage.data;

namespace NamelessRogue.shell
{
    public class NamelessGame : Game
    {
        private static long serialVersionUID = 1L;

        List<IEntity> Entities;
        List<ISystem> Systems;
        InputSystem inputsystem;

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

        public List<ISystem> GetSystems()
        {
            return Systems;
        }


        private GameSettings settings;

        RenderingSystem renderingSystem;

        public NamelessGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Content.RootDirectory = "Content";
        }



        public int getActualWidth()
        {
            return settings.getWidth() * settings.getFontSize();
        }

        public int getActualHeight()
        {
            return settings.getHeight() * settings.getFontSize();
        }





        public GameSettings getSettings()
        {
            return settings;
        }



        void setSettings(GameSettings settings)
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
            graphics.PreferredBackBufferWidth = getActualWidth();
            graphics.PreferredBackBufferHeight = getActualHeight();
           
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




            inputsystem = new InputSystem();

            Entities = new List<IEntity>();
            Systems = new List<ISystem>();

            int xoffset = 280;
            int yoffset = 110;

            //TODO: for test
            Entities.Add(RenderFactory.CreateViewport(settings));
            Entities.Add(TerrainFactory.CreateWorld());
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

            //

            ChunkManagementSystem chunkManagementSystem = new ChunkManagementSystem();
            //initialize reality bubble
            chunkManagementSystem.Update(0, this);
            //
            Systems.Add(new InitializationSystem());
            Systems.Add(inputsystem);
            Systems.Add(new IntentSystem());
           // Systems.Add(new AiSystem());
            Systems.Add(new MovementSystem());
            Systems.Add(new CombatSystem());
            Systems.Add(new SwitchSystem());
            Systems.Add(new DamageHandlingSystem());
            Systems.Add(new DeathSystem());
            Systems.Add(chunkManagementSystem);

            renderingSystem = new RenderingSystem(settings);

            this.IsMouseVisible = true;

            spriteBatch = new SpriteBatch(GraphicsDevice);
            // create and init the UI manager
            UserInterface.Initialize(Content, BuiltinThemes.hd);
            UserInterface.Active.UseRenderTarget = true;

            // draw cursor outside the render target
            UserInterface.Active.IncludeCursorInRenderTarget = false;

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Panel panel = new Panel(new Vector2(400, 400), PanelSkin.Default, Anchor.Center);
            UserInterface.Active.AddEntity(panel);

            // add title and text
            panel.AddChild(new Header("Example Panel"));
            panel.AddChild(new HorizontalLine());
            panel.AddChild(new Paragraph("This is a simple panel with a button."));

            // add a button at the bottom
            panel.AddChild(new Button("Click Me!", ButtonSkin.Default, Anchor.BottomCenter));


        }

        public RenderTarget2D RenderTarget { get; set; }

        public SpriteBatch Batch
        {
            get { return spriteBatch; }
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
 
            foreach (ISystem system in this.Systems)
            {
                system.Update((long) gameTime.TotalGameTime.TotalMilliseconds, this);
            }
            UserInterface.Active.Update(gameTime);

        }

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

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

           

            UserInterface.Active.Draw(spriteBatch);

            // clear buffer
           // GraphicsDevice.Clear(Color.CornflowerBlue);

            // finalize ui rendering
            UserInterface.Active.DrawMainRenderTarget(spriteBatch);

            this.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            this.GraphicsDevice.SamplerStates[0] = sampler;
            this.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            renderingSystem.Update((long)gameTime.TotalGameTime.TotalMilliseconds, this);

        }



    }
}

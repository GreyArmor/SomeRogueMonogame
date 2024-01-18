using log4net;
using log4net.Config;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components;
using NamelessRogue.Engine.Components._3D;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Context;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.GameInstance;
using NamelessRogue.Engine.Generation;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Serialization;
using NamelessRogue.Engine.Sounds;
using NamelessRogue.Engine.Systems._3DView;
using NamelessRogue.Engine.Systems.Ingame;
using NamelessRogue.Engine.UI;
using NamelessRogue.Engine.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Windows.Forms;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using Point = Veldrid.Point;
using Rectangle = Veldrid.Rectangle;

namespace NamelessRogue.shell
{
    public class NamelessGame : IDisposable
    {
        private static long serialVersionUID = 1L;

        //RenderTarget2D renderTarget = new RenderTarget2D(
        public GameInstance CurrentGame { get; set; }

        public GraphicsDevice GraphicsDevice { get; set; }

        public IEntity GetEntity(Guid id)
        {
            return EntityInfrastructureManager.GetEntity(id);
        }

        public void AddEntity(IEntity entity)
        {
            EntityInfrastructureManager.AddEntity(entity);
        }

        public void RemoveEntity(IEntity entity)
        {
            EntityInfrastructureManager.RemoveEntity(entity);
        }
        GameTime GameTime { get; set; } = new GameTime();
        public Position TestMapPosition { get; private set; }
        public IEntity PlayerEntity { get; set; }
        public IEntity TimelineEntity { get; set; }

        public IEntity ChunkGeometryEntiry { get; set; }

        public IEntity FollowedByCameraEntity { get; set; }

        public IEntity CameraEntity { get; set; }

        public IEntity CursorEntity { get; set; }

        public Commander Commander { get; set; }


        // this lookup is very expensive, avoid using in loops
        public List<IEntity> GetEntitiesByComponentClass<T>() where T : IComponent
        {
            List<IEntity> results = EntityInfrastructureManager.Entities.Values.Where(v => v.GetComponentOfType<T>() != null).ToList();
            return results;
        }

        //this lookup is very expensive, avoid using in loops
        public IEntity GetEntityByComponentClass<T>() where T : IComponent
        {
            return GetEntitiesByComponentClass<T>().FirstOrDefault();
        }

        public GameContext CurrentContext { get; private set; }
        public bool IsMouseVisible { get; private set; }
        public GameContext ContextToSwitch { get; set; } = null;

        private GameSettings settings;
        int windowHeight, windowWidth;

    //    public RenderForm RenderForm { get; private set; }
        public bool IsFullScreen { get; private set; }

        public IWorldProvider WorldProvider
        {
            get
            {
                return TimelineEntity.GetComponentOfType<TimeLine>().CurrentTimelineLayer.Chunks;
            }
        }

     //   SwapChain swapChain;

        public NamelessGame()
        {
        }



        public int GetActualCharacterWidth()
        {
            return settings.GetWidth() * settings.GetFontSize();
        }

        public int GetActualCharacterHeight()
        {
            return settings.GetHeight() * settings.GetFontSize();
        }

        public int GetActualWidth()
        {
            return windowWidth;
        }

        public int GetActualHeight()
        {
            return windowHeight;
        }





        public GameSettings GetSettings()
        {
            return settings;
        }



        void SetSettings(GameSettings settings)
        {
            this.settings = settings;
        }




        WorldSettings worldSettings;
        public Sdl2Window Window;

        public ILog Log { get; private set; }
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected void Initialize()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "NamelessRogue.log4net.config";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                XmlConfigurator.Configure(LogManager.CreateRepository("NamelessRogue"), stream);
            }
            Log = LogManager.GetLogger(typeof(NamelessGame));
            Log.Info("Application started");
            GameTime zero = new GameTime();
            //SaveManager.Init();


            CurrentGame = new GameInstance();
            //TODO: move to config later
            int width = 40;
            int height = 30;


            var commanderEntity = new Entity();
            Commander = new Commander();
            commanderEntity.AddComponent(Commander);

            settings = new GameSettings(width, height);

            windowWidth = (int)(GetActualCharacterWidth() + settings.HudWidth);
            windowHeight = GetActualCharacterHeight();

            var window = VeldridStartup.CreateWindow(new WindowCreateInfo(100, 100, windowWidth, windowHeight, Veldrid.WindowState.Normal, "NamelessRogue"));
            var gd = VeldridStartup.CreateDefaultD3D11GraphicsDevice(new GraphicsDeviceOptions(true, PixelFormat.R32_G32_B32_A32_Float, true), window);
            
            Window =  window;
            GraphicsDevice = gd;
            CommandList = gd.ResourceFactory.CreateCommandList();

            ModelsLibrary.Initialize(this);

            //12345 123
            worldSettings = new WorldSettings(4, WorldGenConstants.Resolution, WorldGenConstants.Resolution);


            new UIController(this);
            ContextFactory.InitAllContexts(this);
            var viewportEntity = RenderFactory.CreateViewport(settings);
            CameraEntity = viewportEntity;

            if (true)
            {

                TerrainFurnitureFactory.CreateFurnitureEntities(this);
                Entity chunksHolder = new Entity();
                Chunk3dGeometryHolder holder = new Chunk3dGeometryHolder();
                chunksHolder.AddComponent(holder);
                ChunkGeometryEntiry = chunksHolder;
                TimelineEntity = TimelineFactory.CreateTimeline(this);
                var libraries = new Entity();
                var ammoLibrary = new AmmoLibrary();
                ammoLibrary.AmmoTypes.Add(new AmmoType() { Name = "Revolver ammo" });
                libraries.AddComponent(ammoLibrary);

                var timelinEntity = TimelineEntity;
                var timeline = timelinEntity.GetComponentOfType<TimeLine>();

                WorldTile firsTile = null;
                int x, y;
                if (firsTile != null)
                {

                    //place everything at the center of newly generated settlement;
                    x = firsTile.Settlement.Concrete.Center.X;
                    y = firsTile.Settlement.Concrete.Center.Y;

                }
                else
                {
                    x = 300;
                    y = 300;
                }

                ChunkManagementSystem chunkManagementSystem = new ChunkManagementSystem();
                //initialize reality bubble
                chunkManagementSystem.Update(zero, this);


                Point worldRiverPosition = new Point();
                bool anyRivers = false;
                foreach (var worldBoardWorldTile in timeline.CurrentTimelineLayer.WorldTiles)
                {
                    var pos = worldBoardWorldTile.WorldBoardPosiiton;
                    var isWater = timeline.CurrentTimelineLayer.InlandWaterConnectivity[pos.X][pos.Y].isWater;
                    if (isWater)
                    {
                        anyRivers = true;
                        worldRiverPosition = pos;
                        break;
                    }
                }


                var player = CharacterFactory.CreateSimplePlayerCharacter(x * Constants.ChunkSize, y * Constants.ChunkSize, this);
                PlayerEntity = player;
                TestMapPosition = new Position(x * Constants.ChunkSize, y * Constants.ChunkSize);

                var rect = new Rectangle(TestMapPosition.Point.X - 10, TestMapPosition.Point.Y + 10, 2, 2);

                CharacterFactory.CreateNpcFormation(rect, Vector2.UnitY, "Red", "1", this);

                rect = new Rectangle(TestMapPosition.Point.X - 60, TestMapPosition.Point.Y + 10, 20, 10);

                CharacterFactory.CreateNpcFormation(rect, Vector2.UnitY, "Red", "2", this);

                rect = new Rectangle(TestMapPosition.Point.X - 80, TestMapPosition.Point.Y + 10, 20, 10);

                CharacterFactory.CreateNpcFormation(rect, Vector2.UnitY, "Red", "3", this);

                rect = new Rectangle(TestMapPosition.Point.X - 100, TestMapPosition.Point.Y + 10, 20, 10);

                CharacterFactory.CreateNpcFormation(rect, Vector2.UnitY, "Red", "4", this);



                rect = new Rectangle(TestMapPosition.Point.X + 10, TestMapPosition.Point.Y + 10, 2, 2);

                //	CharacterFactory.CreateNpcField(rect, -Vector2.UnitY, "Blue", "2", this);



                FollowedByCameraEntity = player;

                if (false)//if (anyRivers)
                {
                    //move player to some river
                    PlayerEntity.GetComponentOfType<Position>().Point = new Point(200 * Constants.ChunkSize, 200 * Constants.ChunkSize);
                    chunkManagementSystem.Update(zero, this);
                }

                CursorEntity = GameInitializer.CreateCursor();

                for (int i = x - Constants.RealityBubbleRangeInChunks; i < x + Constants.RealityBubbleRangeInChunks; i++)
                {
                    for (int j = y - Constants.RealityBubbleRangeInChunks; j < y + Constants.RealityBubbleRangeInChunks; j++)
                    {
                        var p = new Point(i, j);
                        UpdateChunkCommand command = new UpdateChunkCommand(p);
                        Commander.EnqueueCommand(command);

                        //var chunk = WorldProvider.GetChunks()[p];
                        //WorldProvider.GetRealityBubbleChunks().Add(p, chunk);
                        //WorldProvider.RealityChunks.Add(chunk);
                    }
                }
            }

            CurrentContext = ContextFactory.GetMainMenuContext(this);
            this.IsMouseVisible = true;
            //spriteBatch = new SpriteBatch(GraphicsDevice);
            SpriteLibrary.Initialize(this);
            InitSound();
            PlayMainMenuTheme();
            IsInitialized = true;

        }
        MusicPack musicPack;
        public void InitSound()
        {
            //var packPath = @$"Content\MusicPack\PackConfig.xml";
            //if (File.Exists(packPath))
            //{
            //	var musicPackSerializer = XmlSerializer.FromTypes(new Type[] { typeof(MusicPack) }).First();
            //	musicPack = (MusicPack)musicPackSerializer.Deserialize(File.OpenRead(packPath));
            //	foreach (var track in musicPack.Tracks)
            //	{
            //		var trackPath = @$"Content\MusicPack\" + track.File;
            //		if (File.Exists(trackPath))
            //		{
            //			var song = Song.FromUri(trackPath, new Uri(trackPath, UriKind.Relative));
            //			SoundsHolder.SongDictionary.Add(track.ThemeId, song);
            //		}
            //	}
            //}
            //SoundsHolder.SoundDictionary.Add("ButtonClick", Content.Load<SoundEffect>("sounds\\annabloom_click1"));
        }

        public void PlayMainMenuTheme()
        {
            Commander.EnqueueCommand(new PlaySoundCommand(CurrentContext.MusicThemeId, true, 0.0f, true));
        }


        bool saveScheduled = false;
        internal void ScheduleSave()
        {
            saveScheduled = true;
        }

        bool loadScheduled = false;
        internal void ScheduleLoad()
        {
            loadScheduled = true;
        }

        public TextureView RenderTarget { get; set; }

        //public SpriteBatch Batch
        //{
        //	get { return spriteBatch; }
        //}

        public WorldSettings WorldSettings
        {
            get { return worldSettings; }
            set { worldSettings = value; }
        }
        public bool IsInitialized { get; internal set; }
        public GameSettings Settings { get => settings; set => settings = value; }
        public bool IsActive { get; internal set; } = true;
        public float AspectRatio { get { return windowWidth / windowHeight; } }

        public CommandList CommandList { get; internal set; }
        public InputSnapshot Input { get; internal set; }

        //public List<IEntity> EntitiesToAdd { get => entitiesToAdd; set => entitiesToAdd = value; }
        //public List<IEntity> EntitiesToRemove { get => entitiesToRemove; set => entitiesToRemove = value; }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected void LoadContent()
        {

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected void UnloadContent()
        {

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected void Update(GameTime gameTime)
        {
            if (ContextToSwitch != null)
            {
                CurrentContext = ContextToSwitch;

                Commander.EnqueueCommand(new PlaySoundCommand(CurrentContext.MusicThemeId, true, 0, true));

                ContextToSwitch = null;
            }

            //if (EntitiesToAdd.Any())
            //{
            //    foreach (var entity in EntitiesToAdd)
            //    {
            //        Entities.Add(entity);
            //    }
            //    EntitiesToAdd.Clear();
            //}

            //if (EntitiesToRemove.Any())
            //{
            //    foreach (var entity in EntitiesToRemove)
            //    {
            //        Entities.Remove(entity);
            //    }
            //    EntitiesToRemove.Clear();
            //}

            //foreach (var entity in Entities)
            //{
            //    entity.AppendDelayedComponents();
            //}	

            CurrentContext.Update(GameTime, this);

            if (saveScheduled)
            {
                saveScheduled = false;
                SaveManager.SaveGame("", this);
            }

            if (loadScheduled)
            {
                loadScheduled = false;


                EntityInfrastructureManager.ClearGame();

                ContextFactory.ReleaseAllContexts(this);
                ContextFactory.InitAllContexts(this);

                SaveManager.LoadGame("", this);

                ContextToSwitch = ContextFactory.GetIngameContext(this);
                skipNextFrame = true;
            }

         
        }

        private FrameCounter _frameCounter = new FrameCounter();
        //Label fpsLabel;
        private bool skipNextFrame;

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>

        //byte[] data;
        protected void Draw(GameTime gameTime)
        {
            //if (!this.IsActive) //Pause Game when minimized
            //	return;

            if (skipNextFrame)
            {
                skipNextFrame = false;
                return;
            }

            var deltaTime = (float)gameTime.TotalGameTime.TotalSeconds;
            _frameCounter.Update(deltaTime);

            IngameScreen.FPS = _frameCounter.AverageFramesPerSecond.ToString();


            CommandList.Begin();
            CommandList.SetFramebuffer(GraphicsDevice.MainSwapchain.Framebuffer);
            CommandList.ClearColorTarget(0, RgbaFloat.Black);
            CurrentContext.RenderingUpdate(GameTime, this);
            CommandList.End();
            GraphicsDevice.SubmitCommands(CommandList);
            GraphicsDevice.SwapBuffers(GraphicsDevice.MainSwapchain);

            // Window.BeginScene(0,0,0,1);
          
           // Window.EndScene();
          //  Window.ClearStates();
        }

        Stopwatch gameTimeStopwatch = new Stopwatch();
        internal void Run()
        {
            LoadContent();
            Initialize();
            gameTimeStopwatch.Start();
            while (Window.Exists)
            {
                Input = Window.PumpEvents();
                if (!Window.Exists) { break; }
                GameTime.ElapsedGameTime = gameTimeStopwatch.Elapsed - GameTime.TotalGameTime;
                GameTime.TotalGameTime = gameTimeStopwatch.Elapsed;
                Update(GameTime);
                Draw(GameTime);
            };
        }


        public void Dispose()
        {
            
        }

        internal void Exit()
        {
            Application.Exit();
        }
    }
}

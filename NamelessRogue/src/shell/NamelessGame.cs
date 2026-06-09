using log4net;
using log4net.Config;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components;
using NamelessRogue.Engine.Components._3D;
using NamelessRogue.Engine.Components.AI.Pathfinder;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Context;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.Generation;
using NamelessRogue.Engine.Generation.Editor;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Serialization;
using NamelessRogue.Engine.Sounds;
using NamelessRogue.Engine.Systems;
using NamelessRogue.Engine.Systems.Ingame;
using NamelessRogue.Engine.UI;
using NamelessRogue.Engine.Utility;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;
using Color = Microsoft.Xna.Framework.Color;

namespace NamelessRogue.shell
{
	public class NamelessGame : Game
	{
		private static long serialVersionUID = 1L;

		//RenderTarget2D renderTarget = new RenderTarget2D(
		public GameInstance CurrentGame { get { return gameInstance; } set { gameInstance = value; } }

		public static GraphicsDevice DebugDevice;

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

		public Position TestMapPosition { get; private set; }
		public IEntity PlayerEntity { get; set; }

        public IEntity InputEntity { get; set; }
        public IEntity TimelineEntity { get; set; }

		public IEntity FollowedByCameraEntity { get; set; }

		public IEntity CameraEntity { get; set; }

		public IEntity CursorEntity { get; set; }

        public IEntity TargeterEntity { get; set; }

        public IEntity WorldMapCameraEntity { get; set; }

        public Commander Commander { get; set; }

        public FlowFieldController FlowFieldController {get; private set; }

        // this lookup is very expensive, avoid using in loops
        public List<IEntity> GetEntitiesByComponentClass<T>() where T : IComponent
		{
			List<IEntity> results = EntityInfrastructureManager.Entities.Where(v => v.GetComponentOfType<T>() != null).ToList();
			return results;
		}

		//this lookup is very expensive, avoid using in loops
		public IEntity GetEntityByComponentClass<T>() where T : IComponent
		{
			return GetEntitiesByComponentClass<T>().FirstOrDefault();
		}

		public GameContext CurrentContext { get; private set; }
		public GameContext ContextToSwitch { get; set; } = null;

		private GameSettings settings;

		public IWorldProvider WorldProvider
		{
			get
			{
				return TimelineEntity.GetComponentOfType<WorldTemplate>().WorldMap.Chunks;
			}
		}


		public NamelessGame()
		{
			graphics = new GraphicsDeviceManager(this);
			graphics.GraphicsProfile = GraphicsProfile.HiDef;
			Content.RootDirectory = "Content";
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

		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		GameInstance gameInstance;
		public ILog Log { get; private set; }
		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "NamelessRogue.log4net.config";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                XmlConfigurator.Configure(LogManager.CreateRepository("NamelessRogue"), stream);
            }

            Log = LogManager.GetLogger(typeof(NamelessGame));

            Log.Info("Application started");

            if (!Directory.Exists("Worlds"))
            {
                Directory.CreateDirectory("Worlds");
            }

            //SaveManager.Init();

            InputEntity = new Entity();
			InputEntity.AddComponent(new InputComponent());

            CurrentGame = new GameInstance();
            DebugDevice = this.GraphicsDevice;
            gameInstance = new GameInstance(6666666);
            //TODO: move to config later
            int width = 20;
            int height = 15;

            Commander = new Commander();
         
            settings = new GameSettings(width, height);

            graphics.PreferredBackBufferWidth = (int)(GetActualCharacterWidth() + settings.HudWidth);
            graphics.PreferredBackBufferHeight = GetActualCharacterHeight();

            graphics.IsFullScreen = false;
            graphics.PreferMultiSampling = true;
            graphics.SynchronizeWithVerticalRetrace = true;

            RenderTarget = new RenderTarget2D(
                GraphicsDevice,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24, 4, RenderTargetUsage.PlatformContents);


            graphics.ApplyChanges();

           


            ModelsLibrary.Initialize(this);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            SpriteLibrary.Initialize(this);
            new UIContainer(this);
            ContextFactory.InitAllContexts(this);
            var viewportEntity = RenderFactory.CreateViewport(settings);
            CameraEntity = viewportEntity;
            TerrainFurnitureFactory.CreateFurnitureEntities(this);

            BuffLibrary.ClearData();
            BuffLibrary.LoadData(this);

            ItemLibrary.ClearData();
            ItemLibrary.LoadItemData(this);

            DialogLibrary.ClearData();
            DialogLibrary.LoadData(this);

            BuildingLibrary.ClearData();
            BuildingLibrary.LoadData(this);

			CharacterFactory.ClearData();
			CharacterFactory.LoadCharacters();



            CurrentContext = ContextFactory.GetMainMenuContext(this);
            this.IsMouseVisible = true;

            InitSound();
            PlayMainMenuTheme();

            AbilityLogicLibrary.Init(this);

         

            IsInitialized = true;

        }

        public void InitializeNewGameInstance(GameInstance gameInstance, WorldTemplate worldTemplate)
        {
			GameTime zero = new GameTime();
			this.CurrentGame = gameInstance;

            Entity worldTemplateEntity = new Entity();
            worldTemplateEntity.AddComponent(worldTemplate);
            TimelineEntity = worldTemplateEntity; //TimelineFactory.CreateTimeline(this);       

            ChunkData chunkData = new ChunkData(CurrentGame, worldTemplate.WorldMap);
            worldTemplate.WorldMap.Chunks = chunkData;

            int x, y;
			//alpha world start zone
            x = 5;
            y = 9;



            var player = CharacterFactory.CreateSimplePlayerCharacter(x * Constants.ChunkSize, y * Constants.ChunkSize, 0, this);
            PlayerEntity = player;
            TestMapPosition = new Position(x * Constants.ChunkSize, y * Constants.ChunkSize, 0);

           

            ChunkManagementSystem chunkManagementSystem = new ChunkManagementSystem();
            //initialize reality bubble
            chunkManagementSystem.Update(zero, this);

            //var characters = Directory.GetFiles(Environment.CurrentDirectory + Constants.GameObjectRelativePath + "\\Characters\\", "*.nrcf", SearchOption.AllDirectories);
            //Vector2 characterCreationOffset = new Vector2(0);

            //List<Entity> charaterEntities = new List<Entity>();

            //foreach (var charactersFile in characters)
            //{
            //    characterCreationOffset.Y--;
            //    characterCreationOffset.Y--;
            //    XmlSerializer serializer = new XmlSerializer(typeof(CharacterTemplateData));
            //    TextReader reader = new StreamReader(charactersFile);
            //    var data = (CharacterTemplateData)serializer.Deserialize(reader);
            //    var character = CharacterFactory.CreateCharacterFromData(this, new Vector3Int((int)(characterCreationOffset.X + (x * Constants.ChunkSize)), (int)(characterCreationOffset.Y + (y * Constants.ChunkSize)), 0), data);
            //    charaterEntities.Add(character);
            //}

            //foreach (var character in charaterEntities)
            //{
            //    var characterItems = character.GetComponentOfType<ItemsHolder>();
            //    for (int i = 0; i < 10; i++)
            //    {
            //        var numberOfItems = ItemLibrary.ItemData.Count;

            //        var randomItem = Random.Shared.Next(0, numberOfItems);

            //        var randomItemData = ItemLibrary.ItemData[randomItem];
            //        var item = ItemLibrary.CreateItemFromData(this, randomItemData);
            //        characterItems.Items.Add(item);
            //    }
            //}

            var itemsHolder = player.GetComponentOfType<ItemsHolder>();
            foreach (var itemData in ItemLibrary.ItemData)
            {
                if (itemData.ItemType == ItemType.Consumable)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        var item = ItemLibrary.CreateItemFromData(this, itemData);
                        itemsHolder.Items.Add(item);
                    }
                }
                else
                {
                    var item = ItemLibrary.CreateItemFromData(this, itemData);
                    itemsHolder.Items.Add(item);
                }
            }

            AbilityFactory.LoadData(this);

            var abilityHolder = PlayerEntity.GetComponentOfType<AbilityHolder>();
            var abilityBinder = PlayerEntity.GetComponentOfType<AbilityBinder>();

            int binding = 1;
            foreach (var abilityData in AbilityFactory.Data)
            {
                var ability = AbilityFactory.CreateFromData(this, abilityData);
                abilityHolder.Abilities.Add(ability);
                abilityBinder.AbilityBindings.Add(binding, ability);
                binding++;
            }

			for (int worldX = 0; worldX < worldTemplate.WorldSize.X; worldX++)
			{
				for (int worldY = 0; worldY < worldTemplate.WorldSize.Y; worldY++)
				{
					var worldTile = worldTemplate.WorldMap.WorldTiles[worldX, worldY];
                    foreach(var building in worldTile.Buildings)
					{
                        var buildingName = building.Name;
						
						var buildingData = BuildingLibrary.Data.FirstOrDefault(x=>x.Name == buildingName);
						if (buildingData!=null)
						{
							BuildingLibrary.CreateBuildingFromData(this, new System.Drawing.Point(worldX, worldY), buildingData);
						}
                    }
                }
			}

            //foreach (var buildingData in BuildingLibrary.Data)
            //{
            //    for (int i = -4; i < 4; i++)
            //    {
            //        for (int j = -4; j < 4; j++)
            //        {
            //            BuildingLibrary.CreateBuildingFromData(this, new System.Drawing.Point(x + i, y + j), buildingData);
            //        }
            //    }
            //}
            var realChunks = WorldProvider.GetRealityBubbleChunks();
            foreach (var realityBubbleChunk in realChunks)
            {
                for (int z = 0; z < Constants.ChunkHeight; z++)
                {
                    var tile = realityBubbleChunk.Value.ChunkTiles[0][0][z];
                    if (tile != null)
                    {
                        Commander.EnqueueCommand(new UpdateVisualChunkCommand(new Engine.Utility.Vector3Int(realityBubbleChunk.Key.X, realityBubbleChunk.Key.Y, z)));
                    }
                }
            }

            FollowedByCameraEntity = player;
            CursorEntity = GameInitializer.CreateCursor();
            TargeterEntity = GameInitializer.CreateTargeter();
            WorldMapCameraEntity = GameInitializer.CreateWorldMapCamera();
            FlowFieldController = new FlowFieldController(this, WorldProvider);
        }

        MusicPack musicPack;
		public void InitSound()
		{
			var packPath = @$"Content\MusicPack\PackConfig.xml";
			if (File.Exists(packPath))
			{
				var musicPackSerializer = XmlSerializer.FromTypes(new Type[] { typeof(MusicPack) }).First();
				musicPack = (MusicPack)musicPackSerializer.Deserialize(File.OpenRead(packPath));
				foreach (var track in musicPack.Tracks)
				{
					var trackPath = @$"Content\MusicPack\" + track.File;
					if (File.Exists(trackPath))
					{
						var song = Song.FromUri(trackPath, new Uri(trackPath, UriKind.Relative));
						SoundsHolder.SongDictionary.Add(track.ThemeId, song);
					}
				}
			}
			//TODO: automate the loading
			SoundsHolder.SoundDictionary.Add("ButtonClick", Content.Load<SoundEffect>("sounds\\annabloom_click1"));
            SoundsHolder.SoundDictionary.Add("DoorOpen", Content.Load<SoundEffect>("sounds\\171705__peepholecircus__sci-fi-door"));
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

		public RenderTarget2D RenderTarget { get; set; }

		public SpriteBatch Batch
		{
			get { return spriteBatch; }
		}

		public GameInstance WorldSettings
		{
			get { return gameInstance; }
			set { gameInstance = value; }
		}
		public bool IsInitialized { get; internal set; }
		public GameSettings Settings { get => settings; set => settings = value; }
        public bool TurnUpdated { get; internal set; }

        //public List<IEntity> EntitiesToAdd { get => entitiesToAdd; set => entitiesToAdd = value; }
        //public List<IEntity> EntitiesToRemove { get => entitiesToRemove; set => entitiesToRemove = value; }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
		{

		}

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
				CurrentContext = ContextToSwitch;

				Commander.EnqueueCommand(new PlaySoundCommand(CurrentContext.MusicThemeId, true, 0, true));

				ContextToSwitch = null;
			}

			CurrentContext.Update(gameTime, this);

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
		protected override void Draw(GameTime gameTime)
		{
			//if (!this.IsActive) //Pause Game when minimized
			//	return;

			if (skipNextFrame)
			{
				skipNextFrame = false;
				return;
			}

			var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
			_frameCounter.Update(deltaTime);

			IngameScreen.FPS = ((int)_frameCounter.AverageFramesPerSecond).ToString();

			GraphicsDevice.Clear(Color.Black);
			CurrentContext.RenderingUpdate(gameTime, this);
		}
	}
}

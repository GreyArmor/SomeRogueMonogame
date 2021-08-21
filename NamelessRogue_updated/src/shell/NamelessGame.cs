using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;
using log4net.Config;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra;
using Myra.Graphics2D.UI;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components;
using NamelessRogue.Engine.Components.ItemComponents;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Context;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.GameInstance;
using NamelessRogue.Engine.Generation;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Systems;
using NamelessRogue.Engine.Systems.Ingame;
using NamelessRogue.Engine.Utility;
using NamelessRogue.Engine.Infrastructure;
using Color = Microsoft.Xna.Framework.Color;
using Label = Myra.Graphics2D.UI.Label;
using NamelessRogue.Engine.Serialization;

namespace NamelessRogue.shell
{
	public class NamelessGame : Game
	{
		private static long serialVersionUID = 1L;


		public GameInstance CurrentGame { get; set; }


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

		public IEntity PlayerEntity { get; set; }
		public IEntity TimelineEntity { get; set; }

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
		public GameContext ContextToSwitch { get; set; } = null;

		private GameSettings settings;

		public IWorldProvider WorldProvider
		{
			get
			{
				return TimelineEntity.GetComponentOfType<TimeLine>().CurrentTimelineLayer.Chunks;
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
		private Desktop _desktop = new Desktop();
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


			SaveManager.Init();

			CurrentGame = new GameInstance();
			DebugDevice = this.GraphicsDevice;
			//TODO: move to config later
			int width = 80;
			int height = 60;


			var commanderEntity = new Entity();
			Commander = new Commander();
			commanderEntity.AddComponent(Commander);

			settings = new GameSettings(width, height);
			graphics.PreferredBackBufferWidth = (int)(GetActualCharacterWidth() + settings.HudWidth());
			graphics.PreferredBackBufferHeight = GetActualCharacterHeight();

			graphics.IsFullScreen = false;
			graphics.PreferMultiSampling = false;
			graphics.SynchronizeWithVerticalRetrace = true;

			MyraEnvironment.Game = this;

			RenderTarget = new RenderTarget2D(
				GraphicsDevice,
				GraphicsDevice.PresentationParameters.BackBufferWidth,
				GraphicsDevice.PresentationParameters.BackBufferHeight,
				false,
				GraphicsDevice.PresentationParameters.BackBufferFormat,
				DepthFormat.Depth24, 4, RenderTargetUsage.PlatformContents);


			graphics.ApplyChanges();

			//12345 123
			worldSettings = new WorldSettings(4, WorldGenConstants.Resolution, WorldGenConstants.Resolution);

			TerrainFurnitureFactory.CreateFurnitureEntities(this);

			ContextFactory.InitAllContexts(this);



			var viewportEntity = RenderFactory.CreateViewport(settings);

			CameraEntity = viewportEntity;

			TimelineEntity = TimelineFactory.CreateTimeline(this);


			var libraries = new Entity();
			var ammoLibrary = new AmmoLibrary();
			ammoLibrary.AmmoTypes.Add(new AmmoType() { Name = "Revolver ammo" });
			libraries.AddComponent(ammoLibrary);

			var timelinEntity = TimelineEntity;
			var timeline = timelinEntity.GetComponentOfType<TimeLine>();

			WorldTile firsTile = null;
			foreach (var worldBoardWorldTile in timeline.CurrentTimelineLayer.WorldTiles)
			{
				if (worldBoardWorldTile.Settlement != null)
				{
					firsTile = worldBoardWorldTile;
					break;

				}
			}
			int x, y;
			if (firsTile != null)
			{

				//place everything at the center of newly generated settlement;
				x = firsTile.Settlement.Concrete.Center.X;
				y = firsTile.Settlement.Concrete.Center.Y;

			}
			else
			{
				x = WorldGenConstants.Resolution / 2;
				y = WorldGenConstants.Resolution / 2;
			}
			var player = CharacterFactory.CreateSimplePlayerCharacter(x, y, this);

			PlayerEntity = player;

			FollowedByCameraEntity = player;

			ChunkManagementSystem chunkManagementSystem = new ChunkManagementSystem();
			//initialize reality bubble
			chunkManagementSystem.Update(0, this);
			//for (int i = 1; i < 10; i++)
			//{
			//    for (int j = 1; j < 10; j++)
			//    {
			//        Entities.Add(CharacterFactory.CreateBlankNpc(x - i,
			//            y - j));
			//    }
			//}

			CharacterFactory.CreateBlankNpc(x - 6,
				y, this);
			//Entities.Add(CharacterFactory.CreateBlankNpc(x - 3,
			//    y));
			//Entities.Add(CharacterFactory.CreateBlankNpc(x - 5,
			//    y));
			//Entities.Add(CharacterFactory.CreateBlankNpc(x - 7,
			//    y));


			for (int i = 0; i < 2; i++)
			{
				var sword = ItemFactory.CreateSword(x - 2,
					y, i, this); ;
			}

			var platemail = ItemFactory.CreatePlateMail(x - 2, y, 1, this);

			var pants = ItemFactory.CreatePants(x - 2, y, 1, this);

			var boots = ItemFactory.CreateBoots(x - 2, y, 1, this);

			var cape = ItemFactory.CreateCape(x - 2, y, 1, this);

			var ring = ItemFactory.CreateRing(x - 2, y, 1, this);

			var shield = ItemFactory.CreateShield(x - 2, y, 1, this);

			var helmet = ItemFactory.CreateHelmet(x - 2, y, 1, this);


			var ammo1 = ItemFactory.CreateLightAmmo(x - 1, y, 1, 20, this, ammoLibrary);


			var ammo2 = ItemFactory.CreateLightAmmo(x - 1, y + 1, 1, 20, this, ammoLibrary);


			var revolver = ItemFactory.CreateRevolver(x + 2, y + 1, 1, this, ammoLibrary);


			var pArmor = ItemFactory.CreatePowerArmor(x - 2, y, 1, this);


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

			if (anyRivers)
			{
				//move player to some river
				//PlayerEntity.GetComponentOfType<Position>().Point = new Point(worldRiverPosition.X * Constants.ChunkSize, worldRiverPosition.Y * Constants.ChunkSize);
				//chunkManagementSystem.Update(0, this);
			}

			CursorEntity = GameInitializer.CreateCursor();


			//Paragraph.BaseSize = 1.175f;
			//UserInterface.Initialize(Content, "custom");
			//UserInterface.Active.UseRenderTarget = true;
			CurrentContext = ContextFactory.GetMainMenuContext(this);
			CurrentContext.ContextScreen.Show();
			this.IsMouseVisible = true;
			// UserInterface.Active.ShowCursor = false;
			spriteBatch = new SpriteBatch(GraphicsDevice);


			fpsLabel = new Label();
			fpsLabel.HorizontalAlignment = HorizontalAlignment.Right;
			ContextFactory.GetIngameContext(this).ContextScreen.Panel.Widgets.Add(fpsLabel);

			var stackPanel = new VerticalStackPanel();
			stackPanel.Widgets.Add(fpsLabel);

			// Desktop.Widgets.Add(stackPanel);


			// Inform Myra that external text input is available
			// So it stops translating Keys to chars


			_desktop.HasExternalTextInput = true;

			// Provide that text input
			Window.TextInput += (s, a) =>
			{
				_desktop.OnChar(a.Character);
			};

			//for (int i = 0; i < 1; i++)
			//{
			//    Entities.Add(BuildingFactory.CreateDummyBuilding(xoffset * Constants.ChunkSize + 1 + (i * 10),
			//        yoffset * Constants.ChunkSize, 10, 10,
			//        this));
			//    Entities.Add(BuildingFactory.CreateDummyBuilding(xoffset * Constants.ChunkSize + 13 + (i * 10),
			//        yoffset * Constants.ChunkSize, 10,10,
			//        this));
			//    Entities.Add(BuildingFactory.CreateDummyBuilding(xoffset * Constants.ChunkSize + 1 + (i * 10),
			//        yoffset * Constants.ChunkSize + 13,10,
			//        10, this));
			//    Entities.Add(BuildingFactory.CreateDummyBuilding(xoffset * Constants.ChunkSize + 1 + 13 + (i * 10),
			//        yoffset * Constants.ChunkSize + 13, 10,10, this));
			//}




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

		public WorldSettings WorldSettings
		{
			get { return worldSettings; }
			set { worldSettings = value; }
		}

		//public List<IEntity> EntitiesToAdd { get => entitiesToAdd; set => entitiesToAdd = value; }
		//public List<IEntity> EntitiesToRemove { get => entitiesToRemove; set => entitiesToRemove = value; }
		public Desktop Desktop { get => _desktop; set => _desktop = value; }



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

			CurrentContext.Update((long)gameTime.TotalGameTime.TotalMilliseconds, this);

			if (saveScheduled)
			{
				saveScheduled = false;
				SaveManager.SaveGame ("", this);
			}

			if (loadScheduled)
			{
				loadScheduled = false;


				EntityInfrastructureManager.ClearGame();

				ContextFactory.ReleaseAllContexts(this);
				ContextFactory.InitAllContexts(this);

				SaveManager.LoadGame("", this);

				ContextToSwitch = ContextFactory.GetIngameContext(this);
			}

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
			fpsLabel.Text = "FPS = " + _frameCounter.AverageFramesPerSecond.ToString();

			GraphicsDevice.Clear(Color.Black);
			CurrentContext.RenderingUpdate((long)gameTime.TotalGameTime.TotalMilliseconds, this);
		}
	}
}

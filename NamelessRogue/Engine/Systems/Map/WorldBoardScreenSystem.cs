using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.UI;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Systems.Map
{
    public class WorldBoardScreenSystem : BaseSystem
    {
        private readonly MapRenderingSystem _mapRenderSystem;

        public WorldBoardScreenSystem(MapRenderingSystem mapRenderSystem)
        {
            _mapRenderSystem = mapRenderSystem;
            Signature = new HashSet<Type>();
        }

        public override HashSet<Type> Signature { get; }

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            ConsoleCamera camera = namelessGame.CameraEntity?.GetComponentOfType<ConsoleCamera>();
            WorldTemplate timeline = namelessGame.WorldTemplateEntity?.GetComponentOfType<WorldTemplate>();
            var tilePosition = camera.GetMouseTilePosition(namelessGame);
            var settings = namelessGame.WorldSettings;
			var mapScreen = UIContainer.Instance.MapScreen;
			if (tilePosition.X >= 0 && tilePosition.X < settings.WorldMapResolution && tilePosition.Y >= 0 && tilePosition.Y < settings.WorldMapResolution)
            {
                var tile = timeline.WorldMap.WorldTiles[tilePosition.X, tilePosition.Y];
                switch (mapScreen.Mode)
				{

					//case MapMode.ArtifactMode:
					//	{
					//		if (tile.Artifact != null)
					//		{
     //                           mapScreen.Description = (tile.Artifact.Name);
					//		}
					//	}
					//	break;
					//case MapMode.PoliticalMode:
					//	{

					//		if (tile.Owner != null)
					//		{
					//			if (tile.Settlement != null)
					//			{
     //                               mapScreen.Description = $"{tile.Owner.Name}, {tile.Settlement.Name} city";
					//			}
					//			else
					//			{
     //                               mapScreen.Description = $"{tile.Owner.Name}";
					//			}

					//		}
					//	}
					//	break;
					//case MapMode.RegionsMode:
					//	{
					//		if (tile.Continent != null)
					//		{
     //                           mapScreen.Description = $"{tile.Continent.Name} continent";
					//		}
					//		if (tile.LandmarkRegion != null)
					//		{
     //                           mapScreen.Description = $"{tile.LandmarkRegion.Name} region";
					//		}
					//	}
					//	break;
					default:
                        mapScreen.Description = "";
						break;
				}
			}

			switch (mapScreen.Action)
			{
				case MapAction.Exit:
					namelessGame.ContextToSwitch = ContextFactory.GetIngameContext(namelessGame);
					break;
				default:
					break;
			}

			_mapRenderSystem.LocalMapRendering = mapScreen.LocalMapDisplay;

			mapScreen.Action = MapAction.None;

		}
	}
}


using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.Rendering;
using NamelessRogue.Engine.Engine.Factories;
using NamelessRogue.Engine.Engine.Generation.World;
using NamelessRogue.Engine.Engine.UiScreens;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Systems.Map
{
    public class WorldBoardScreenSystem : ISystem
    {
        private readonly MapRenderingSystem _mapRenderSystem;

        public WorldBoardScreenSystem(MapRenderingSystem mapRenderSystem)
        {
            _mapRenderSystem = mapRenderSystem;
        }

        public void Update(long gameTime, NamelessGame namelessGame)
        {
            ConsoleCamera camera = namelessGame.GetEntityByComponentClass<ConsoleCamera>()?.GetComponentOfType<ConsoleCamera>();
            TimeLine timeline = namelessGame.GetEntityByComponentClass<TimeLine>()?.GetComponentOfType<TimeLine>();
            var tilePosition = camera.GetMouseTilePosition(namelessGame);
            var settings = namelessGame.WorldSettings;
            if (tilePosition.X >= 0 && tilePosition.X < settings.WorldBoardWidth && tilePosition.Y >= 0 && tilePosition.Y < settings.WorldBoardHeight)
            {
              
                var tile = timeline.CurrentTimelineLayer.WorldTiles[tilePosition.X, tilePosition.Y];

                switch (UiFactory.WorldBoardScreen.Mode)
                {

                    case WorldBoardScreenAction.ResourceMode:
                    {
                        UiFactory.WorldBoardScreen.DescriptionLog.ClearItems();
                        if (tile.Resource != null)
                        {
                            UiFactory.WorldBoardScreen.DescriptionLog.AddItem(tile.Resource.Info.Name);
                        }
                    }
                        break;
                    case WorldBoardScreenAction.ArtifactMode:
                    {
                        UiFactory.WorldBoardScreen.DescriptionLog.ClearItems();
                        if (tile.Artifact != null)
                        {
                            UiFactory.WorldBoardScreen.DescriptionLog.AddItem(tile.Artifact.Info.Name);
                        }
                    }
                        break;
                    case WorldBoardScreenAction.PoliticalMode:
                    {
                        UiFactory.WorldBoardScreen.DescriptionLog.ClearItems();
                        if (tile.Owner != null)
                        {
                            if (tile.Settlement != null)
                            {
                                UiFactory.WorldBoardScreen.DescriptionLog.AddItem(
                                    $"{tile.Owner.Name}, {tile.Settlement.Info.Name} city");
                            }
                            else
                            {
                                UiFactory.WorldBoardScreen.DescriptionLog.AddItem($"{tile.Owner.Name}");
                            }

                        }
                    }
                        break;
                    case WorldBoardScreenAction.RegionsMode:
                    {
                        UiFactory.WorldBoardScreen.DescriptionLog.ClearItems();
                        if (tile.Continent != null)
                        {
                            UiFactory.WorldBoardScreen.DescriptionLog.AddItem($"{tile.Continent.Name} continent");
                        }
                        if (tile.LandmarkRegion != null)
                        {
                            UiFactory.WorldBoardScreen.DescriptionLog.AddItem($"{tile.LandmarkRegion.Name} region");
                        }
                        }
                        break;
                    default:
                        UiFactory.WorldBoardScreen.DescriptionLog.ClearItems();
                        break;
                }
            }

            foreach (var worldBoardScreenAction in UiFactory.WorldBoardScreen.Actions)
            {
                switch (worldBoardScreenAction)
                {
                    case WorldBoardScreenAction.ReturnToGame:

                        namelessGame.ContextToSwitch = ContextFactory.GetIngameContext(namelessGame);
                        break;
                    case WorldBoardScreenAction.RegionsMode:
                    {
                        _mapRenderSystem.Mode = WorldBoardRenderingSystemMode.Regions;
                    }
                        break;
                    case WorldBoardScreenAction.PoliticalMode:
                    {
                        _mapRenderSystem.Mode = WorldBoardRenderingSystemMode.Political;
                    }
                        break;
                    case WorldBoardScreenAction.TerrainMode:
                    {
                        _mapRenderSystem.Mode = WorldBoardRenderingSystemMode.Terrain;
                    }
                        break;
                    case WorldBoardScreenAction.ArtifactMode:
                    {
                        _mapRenderSystem.Mode = WorldBoardRenderingSystemMode.Terrain;
                    }
                        break;
                    case WorldBoardScreenAction.ResourceMode:
                    {
                        _mapRenderSystem.Mode = WorldBoardRenderingSystemMode.Resources;
                    }

                        break;
                    case WorldBoardScreenAction.LocalMap:
                    {
                        _mapRenderSystem.LocalMapRendering = true;
                    }

                        break;
                    case WorldBoardScreenAction.WorldMap:
                    {
                        _mapRenderSystem.LocalMapRendering = false;
                    }

                        break;
                    default:
                        break;
                }
            }
            UiFactory.WorldBoardScreen.Actions.Clear();
        }
    }
}


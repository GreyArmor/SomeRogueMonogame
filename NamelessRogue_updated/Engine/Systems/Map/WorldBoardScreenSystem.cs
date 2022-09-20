using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.UI;
using NamelessRogue.Engine.ViewModels;
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
            TimeLine timeline = namelessGame.TimelineEntity?.GetComponentOfType<TimeLine>();
            var tilePosition = camera.GetMouseTilePosition(namelessGame);
            var settings = namelessGame.WorldSettings;
            if (tilePosition.X >= 0 && tilePosition.X < settings.WorldBoardWidth && tilePosition.Y >= 0 && tilePosition.Y < settings.WorldBoardHeight)
            {
              
                var tile = timeline.CurrentTimelineLayer.WorldTiles[tilePosition.X, tilePosition.Y];

                //switch (UIController.Instance.MapScreenViewModel.Mode)
                //{

                //    case WorldBoardScreenAction.ArtifactMode:
                //    {
                //            if (tile.Artifact != null)
                //            {
                //               // UiFactory.WorldBoardScreen.DescriptionLog.Text = (tile.Artifact.Name);
                //            }
                //        }
                //        break;
                //    case WorldBoardScreenAction.PoliticalMode:
                //    {
                           
                //            if (tile.Owner != null)
                //            {
                //                if (tile.Settlement != null)
                //                {
                //                 //   UiFactory.WorldBoardScreen.DescriptionLog.Text = $"{tile.Owner.Name}, {tile.Settlement.Name} city";
                //                }
                //                else
                //                {
                //                  //  UiFactory.WorldBoardScreen.DescriptionLog.Text = $"{tile.Owner.Name}";
                //                }

                //            }
                //        }
                //        break;
                //    case WorldBoardScreenAction.RegionsMode:
                //    {
                //            if (tile.Continent != null)
                //            {
                //              //  UiFactory.WorldBoardScreen.DescriptionLog.Text = $"{tile.Continent.Name} continent";
                //            }
                //            if (tile.LandmarkRegion != null)
                //            {
                //               // UiFactory.WorldBoardScreen.DescriptionLog.Text = $"{tile.LandmarkRegion.Name} region";
                //            }
                //        }
                //        break;
                //    default:
                //       // UiFactory.WorldBoardScreen.DescriptionLog.Text = "";
                //        break;
                //}
            }
            /*
            foreach (var worldBoardScreenAction in UiFactory.WorldBoardScreen.SimpleActions)
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
            UiFactory.WorldBoardScreen.SimpleActions.Clear();
            */
        }
    }
}


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.Rendering;
using NamelessRogue.Engine.Engine.Components.UI;
using NamelessRogue.Engine.Engine.Factories;
using NamelessRogue.Engine.Engine.Generation.World;
using NamelessRogue.Engine.Engine.UiScreens;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Systems
{
    public class WorldBoardScreenSystem : ISystem
    {
        public void Update(long gameTime, NamelessGame namelessGame)
        {
            

            ConsoleCamera camera = namelessGame.GetEntityByComponentClass<ConsoleCamera>()?.GetComponentOfType<ConsoleCamera>();
            TimeLine timeline = namelessGame.GetEntityByComponentClass<TimeLine>()?.GetComponentOfType<TimeLine>();
            var tilePosition = camera.GetMouseTilePosition(namelessGame);

            if (tilePosition.X >= 0 && tilePosition.X < 1000 && tilePosition.Y >= 0 && tilePosition.Y < 1000)
            {
              
                var tile = timeline.CurrentWorldBoard.WorldTiles[tilePosition.X, tilePosition.Y];

                switch (UiFactory.WorldBoardScreen.Mode)
                {
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
                        IEntity worldModeEntity = namelessGame.GetEntityByComponentClass<WorldMapMode>();
                        WorldMapMode worldMode = worldModeEntity.GetComponentOfType<WorldMapMode>();
                        worldMode.Mode = WorldBoardRenderingSystemMode.Regions;
                    }
                        break;
                    case WorldBoardScreenAction.PoliticalMode:
                    {
                        IEntity worldModeEntity = namelessGame.GetEntityByComponentClass<WorldMapMode>();
                        WorldMapMode worldMode = worldModeEntity.GetComponentOfType<WorldMapMode>();
                        worldMode.Mode = WorldBoardRenderingSystemMode.Political;
                    }
                        break;
                    case WorldBoardScreenAction.TerrainMode:
                    {
                        IEntity worldModeEntity = namelessGame.GetEntityByComponentClass<WorldMapMode>();
                        WorldMapMode worldMode = worldModeEntity.GetComponentOfType<WorldMapMode>();
                        worldMode.Mode = WorldBoardRenderingSystemMode.Terrain;
                    }
                        break;
                    case WorldBoardScreenAction.ArtifactMode:
                    {
                        IEntity worldModeEntity = namelessGame.GetEntityByComponentClass<WorldMapMode>();
                        WorldMapMode worldMode = worldModeEntity.GetComponentOfType<WorldMapMode>();
                        worldMode.Mode = WorldBoardRenderingSystemMode.Artifact;
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


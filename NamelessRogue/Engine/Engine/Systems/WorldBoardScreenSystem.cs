using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.UI;
using NamelessRogue.Engine.Engine.Factories;
using NamelessRogue.Engine.Engine.UiScreens;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Systems
{
    public class WorldBoardScreenSystem : ISystem
    {
        public void Update(long gameTime, NamelessGame namelessGame)
        {
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
                    default:
                        break;
                }
            }
            UiFactory.WorldBoardScreen.Actions.Clear();
        }
    }
}


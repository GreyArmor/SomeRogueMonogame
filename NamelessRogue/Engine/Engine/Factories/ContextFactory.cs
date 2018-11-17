using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeonBit.UI;
using Microsoft.Xna.Framework.Graphics;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Context;
using NamelessRogue.Engine.Engine.Systems;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Factories
{
    public class ContextFactory
    {
        private static GameContext IngameContext;
        public static GameContext GetIngameContext(NamelessGame game)
        {

            if (IngameContext != null)
            {
                return IngameContext;
            }
            else
            {
                ChunkManagementSystem chunkManagementSystem = new ChunkManagementSystem();
                //initialize reality bubble
                chunkManagementSystem.Update(0, game);
                var systems = new List<ISystem>();
                systems.Add(new InitializationSystem());
                systems.Add(new InputSystem());
                systems.Add(new IngameIntentSystem());
                // Systems.Add(new AiSystem());
                systems.Add(new MovementSystem());
                systems.Add(new CombatSystem());
                systems.Add(new SwitchSystem());
                systems.Add(new DamageHandlingSystem());
                systems.Add(new DeathSystem());
                systems.Add(chunkManagementSystem);
                systems.Add(new HudSystem());
                var renderingSystem = new RenderingSystem(game.GetSettings());
                var uiSystem = new UIRenderSystem();

                // create and init the UI manager
                UiFactory.CreateHud(game);

                IngameContext = new GameContext(systems, new List<ISystem>() {uiSystem, renderingSystem},
                    UiFactory.HudInstance);

                return IngameContext;
            }
        }

        private static GameContext WorldBoardContext;
        public static GameContext GetWorldBoardContext(NamelessGame game)
        {

            if (WorldBoardContext != null)
            {
                return WorldBoardContext;
            }
            else
            {
                var systems = new List<ISystem>();
                systems.Add(new InputSystem());
                systems.Add(new WorldBoardIntentSystem());
                systems.Add(new MovementSystem());
                systems.Add(new WorldBoardScreenSystem());
                var renderingSystem = new WorldBoardRenderingSystem(game.GetSettings());
                var uiSystem = new UIRenderSystem();

                // create and init the UI manager
                UiFactory.CreateWorldBoardScreen(game);
                WorldBoardContext = new GameContext(systems, new List<ISystem>() { uiSystem, renderingSystem }, UiFactory.WorldBoardScreen);
                return WorldBoardContext;
            }
        }

    }
}

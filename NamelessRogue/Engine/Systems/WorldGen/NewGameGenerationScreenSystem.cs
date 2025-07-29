using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.UI;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Systems.WorldGen
{
    public class NewGameGenerationScreenSystem : BaseSystem
    {
        public override HashSet<Type> Signature { get; } = new HashSet<Type>();
        Timer progressBarTestTimer = null;
        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            while (namelessGame.Commander.DequeueCommand(out ExitContextCommand command))
            {
                namelessGame.ContextToSwitch = ContextFactory.GetCharacterCreationScreenContext(namelessGame);
            }

            while (namelessGame.Commander.DequeueCommand(out StartNewGameCommand command))
            {
                var task = new Task(() =>
                {
                    var worldTemplate = command.Template;
                    namelessGame.CurrentGame = new Generation.GameInstance(worldTemplate.Seed);
                    namelessGame.CurrentGame.WorldMapResolution = worldTemplate.WorldMap.Resolution;
                   
                    namelessGame.InitializeNewGameinstance(namelessGame.CurrentGame, worldTemplate);
                    progressBarTestTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    progressBarTestTimer.Dispose();
                    
                    namelessGame.Commander.EnqueueCommand(new SwitchToIngameContextCommand());

                });
                UIContainer.Instance.WorldGenerationProgressUI.ProgressFraction = 0;
                progressBarTestTimer = new Timer((object? target) =>
                {
                    UIContainer.Instance.NewGameGenerationProgressScreen.ProgressFraction = UIContainer.Instance.NewGameGenerationProgressScreen.ProgressFraction < 1f ? UIContainer.Instance.NewGameGenerationProgressScreen.ProgressFraction += 0.1f : 1f;
                }
                , null, 0, 100);

                task.Start();
            }

            while (namelessGame.Commander.DequeueCommand(out SwitchToIngameContextCommand command))
            {               
                namelessGame.ContextToSwitch = ContextFactory.GetIngameContext(namelessGame);

            }
        }

        internal class SwitchToIngameContextCommand : ICommand
        {
        }
    }
}

using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Input;
using NamelessRogue.Engine.Serialization;
using NamelessRogue.Engine.UI;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Systems.PickUpItems
{

    public class GenerateWorldFinishedCommand : ICommand
    {
        public GenerateWorldFinishedCommand(WorldTemplate worldTemplate)
        {
            WorldTemplate = worldTemplate;
        }

        public WorldTemplate WorldTemplate { get; }
    }

    public class WorldGenerationProgressSystem : BaseSystem
    {
        public WorldGenerationProgressSystem()
        {
            Signature = new HashSet<Type>();
            Signature.Add(typeof(InputComponent));
        }
        public override HashSet<Type> Signature { get; }
        public bool InventoryNeedsUpdate { get; private set; }
        Timer progressBarTestTimer = null;
        public override void Update(GameTime gameTime, NamelessGame game)
        {

            while (game.Commander.DequeueCommand(out GenerateWorldFileCommand command))
            {
                var task = new Task(() =>
                {
                    var parameters = command.Parameters;
                    var worldTemplate = new WorldTemplate(parameters);
                    var worldMap = new WorldMap(parameters.worldSizeValue);
                    ChunkData chunkData = new ChunkData(game.WorldSettings, worldMap);
                    worldMap.Chunks = chunkData;
                    WorldBoardGenerator.PopulateWithInitialData(worldMap, game);
                    worldTemplate.WorldMap = worldMap;

                 

                    game.Commander.EnqueueCommand(new GenerateWorldFinishedCommand(worldTemplate));
                    progressBarTestTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    progressBarTestTimer.Dispose();
                });
                UIContainer.Instance.WorldGenerationProgressUI.ProgressFraction = 0;
                progressBarTestTimer = new Timer((object? target) => {
                    UIContainer.Instance.WorldGenerationProgressUI.ProgressFraction = UIContainer.Instance.WorldGenerationProgressUI.ProgressFraction < 1f ? UIContainer.Instance.WorldGenerationProgressUI.ProgressFraction += 0.1f : 1f;
                }
                , null, 0, 100);
                
                task.Start();
            }

            while (game.Commander.DequeueCommand(out GenerateWorldFinishedCommand command))
            {
                SaveManager.SaveWorldTemplate("Worlds", @$"{command.WorldTemplate.Name}.nrwf", command.WorldTemplate, game);
                game.ContextToSwitch = ContextFactory.GetWorldGenContext(game);
            }
        }
    }
}

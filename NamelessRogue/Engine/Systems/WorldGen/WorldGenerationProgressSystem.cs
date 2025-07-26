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
                });

                task.Start();
            }

            while (game.Commander.DequeueCommand(out GenerateWorldFinishedCommand command))
            {
                SaveManager.SaveWorldTemplate("Worlds", "test.nrwf", command.WorldTemplate, game);
                var savedWorldTemplate = SaveManager.LoadWorldTemplate("Worlds", "test.nrwf");
                command.ToString();               
            }

            switch (UIContainer.Instance.WorldGenScreen.Action)
            {
                case WorldGenAction.Exit:
                    game.ContextToSwitch = ContextFactory.GetMainMenuContext(game);
                    break;
                case WorldGenAction.Generate:
                    break;
                default:
                    break;
            }
            UIContainer.Instance.WorldGenScreen.Action = WorldGenAction.None;
        }
    }
}

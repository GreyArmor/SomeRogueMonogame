using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Serialization;
using NamelessRogue.Engine.Systems.PickUpItems;
using NamelessRogue.Engine.UI;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NamelessRogue.Engine.Systems.WorldGen
{
    public class CharacterCreationScreenSystem : BaseSystem
    {
        public override HashSet<Type> Signature { get; } = new HashSet<Type>();
        WorldTemplate currentWorldTemplate { get; set; } = null;

        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            while (namelessGame.Commander.DequeueCommand(out ExitContextCommand command))
            {
                namelessGame.ContextToSwitch = ContextFactory.GetNewGamePickWorldContext(namelessGame);
            }

            while (namelessGame.Commander.DequeueCommand(out StartNewGameInAWorldCommand command))
            {
                var worldFile = command.WorldPath;
                currentWorldTemplate = SaveManager.LoadWorldTemplate("Worlds", Path.GetFileName(worldFile));
            }

            while (namelessGame.Commander.DequeueCommand(out StartNewGameWithThisCharacterCommand command))
            {
                namelessGame.ContextToSwitch = ContextFactory.GetNewGameGenerationProgressContext(namelessGame);
                namelessGame.Commander.EnqueueCommand(new StartNewGameCommand(command.TempCharacterArchtype, currentWorldTemplate));
            }
        }
    }
}

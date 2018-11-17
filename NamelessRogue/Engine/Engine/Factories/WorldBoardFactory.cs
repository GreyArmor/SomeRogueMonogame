using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NamelessRogue.Engine.Engine.Generation;
using NamelessRogue.Engine.Engine.Generation.World;
using NamelessRogue.Engine.Engine.Infrastructure;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Factories
{
    public class WorldBoardFactory
    {
        public static Entity CreateWorldBoard(NamelessGame game)
        {
            Entity worldBoard = new Entity();
            var wb = new WorldBoard(100, 100);
            worldBoard.AddComponent(wb);

            WorldBoardGenerator.PopulateWithData(wb, game);
            return worldBoard;
        }
    }
}

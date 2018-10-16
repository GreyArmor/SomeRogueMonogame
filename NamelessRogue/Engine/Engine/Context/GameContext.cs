using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Context
{
    public class GameContext
    {

        public HashSet<ISystem> Systems { get; } = new HashSet<ISystem>();
        public HashSet<ISystem> RenderingSystems { get; } = new HashSet<ISystem>();

        public GameContext(IEnumerable<ISystem> systems, IEnumerable<ISystem> renderingSystems)
        {
            if (systems != null && systems.Any())
            {
                foreach (var system in systems)
                {
                    Systems.Add(system);
                }
            }
            if (renderingSystems != null && renderingSystems.Any())
            {
                foreach (var system in renderingSystems)
                {
                    RenderingSystems.Add(system);
                }
            }
        }

        public void Update(long gameTime, NamelessGame namelessGame)
        {
            foreach (var system in Systems)
            {
                system.Update(gameTime, namelessGame);
            }
        }

        public void RenderingUpdate(long gameTime, NamelessGame namelessGame)
        {
            foreach (var system in RenderingSystems)
            {
                system.Update(gameTime, namelessGame);
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.UI;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Context
{
    public class GameContext
    {
        public List<IBaseGuiScreen> ContextScreens { get; } = new List<IBaseGuiScreen>();
        public HashSet<ISystem> Systems { get; } = new HashSet<ISystem>();
        public HashSet<ISystem> RenderingSystems { get; } = new HashSet<ISystem>();
        public string MusicThemeId { get; set; }
        public GameContext(IEnumerable<ISystem> systems, IEnumerable<ISystem> renderingSystems, BaseScreen contextScreen, string musicThemeId)
            : this(systems, renderingSystems, new List<IBaseGuiScreen>() { contextScreen }, musicThemeId)
        {        }

        public GameContext(IEnumerable<ISystem> systems, IEnumerable<ISystem> renderingSystems, List<IBaseGuiScreen> contextScreens, string musicThemeId)
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

            ContextScreens.AddRange(contextScreens);
            MusicThemeId = musicThemeId;
        }

            public void Update(GameTime gameTime, NamelessGame namelessGame)
        {
            foreach (var system in Systems)
            {
                system.Update(gameTime, namelessGame);
            }
        }

        public void RenderingUpdate(GameTime gameTime, NamelessGame namelessGame)
        {
            foreach (var system in RenderingSystems)
            {
                system.Update(gameTime, namelessGame);
            }
        }

    }
}

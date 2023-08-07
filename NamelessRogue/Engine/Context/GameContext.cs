﻿using System;
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
        public IBaseGuiScreen ContextScreen { get; }
        public HashSet<ISystem> Systems { get; } = new HashSet<ISystem>();
        public HashSet<ISystem> RenderingSystems { get; } = new HashSet<ISystem>();
        public string MusicThemeId { get; set; }
        public GameContext(IEnumerable<ISystem> systems, IEnumerable<ISystem> renderingSystems, BaseScreen contextScreen, string musicThemeId)
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

            ContextScreen = contextScreen;
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

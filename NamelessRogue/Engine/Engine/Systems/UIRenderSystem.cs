using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeonBit.UI;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Systems
{
    public class UIRenderSystem : ISystem
    {
        public void Update(long gameTime, NamelessGame namelessGame)
        {
            UserInterface.Active.Draw(namelessGame.Batch);
            UserInterface.Active.DrawMainRenderTarget(namelessGame.Batch);
        }
    }
}

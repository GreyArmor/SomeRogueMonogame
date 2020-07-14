using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Myra.Graphics2D.UI;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Systems
{
    public class UIRenderSystem : BaseSystem
    {
        public override HashSet<Type> Signature { get; }  = new HashSet<Type>();

        public override void Update(long gameTime, NamelessGame namelessGame)
        {
            Desktop.Render();
        }
    }
}

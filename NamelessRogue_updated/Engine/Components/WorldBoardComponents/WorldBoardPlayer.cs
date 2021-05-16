using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.WorldBoardComponents
{
    public class WorldBoardPlayer : Component
    {
        public override IComponent Clone()
        {
            return new WorldBoardPlayer();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Engine.Components.Environment
{
    public class Fire : Component
    {
        public override IComponent Clone()
        {
            return new Fire();
        }
    }
}

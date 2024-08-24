using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Environment
{
    public class SpritedObject : Component
    {
        public override IComponent Clone()
        {
            return new SpritedObject();
        }
    }
}

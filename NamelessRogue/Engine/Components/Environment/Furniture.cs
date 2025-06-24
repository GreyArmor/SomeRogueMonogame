using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessRogue.Engine.Components.Environment
{
    public class Furniture : Component
    {
        public override IComponent Clone()
        {
            return new Furniture();
        }
    }

    public class PhantomEntity : Component
    {
        public override IComponent Clone()
        {
            return new PhantomEntity();
        }
    }
}

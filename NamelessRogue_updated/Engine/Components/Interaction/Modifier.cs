using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NamelessRogue.Engine.Abstraction;

namespace NamelessRogue.Engine.Components.Interaction
{
    public abstract class Modifier : Component
    {
        public abstract void Apply(IEntity entity);
    }
}

using System.Collections.Generic;
using NamelessRogue.Engine.Abstraction;

namespace NamelessRogue.Engine.Engine.Infrastructure
{
    public class Context {
        public List<ISystem> Systems;
        public Context(){
            Systems = new List<ISystem>();
        }
    }
}

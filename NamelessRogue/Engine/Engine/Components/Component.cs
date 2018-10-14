 

using System;

namespace NamelessRogue.Engine.Engine.Components
{
    public class Component : IComponent
    {
        private Guid Id;

        public Component()
        {
            Id = Guid.NewGuid();
        }

        public Guid GetId()
        {
            return Id;
        }
    }
}

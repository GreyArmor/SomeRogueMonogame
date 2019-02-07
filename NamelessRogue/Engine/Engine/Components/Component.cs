 

using System;
using System.Runtime.Serialization;

namespace NamelessRogue.Engine.Engine.Components
{
    [DataContract]
    public class Component : IComponent
    {
        [DataMember]
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

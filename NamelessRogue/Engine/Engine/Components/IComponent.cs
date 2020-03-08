 

using System;

namespace NamelessRogue.Engine.Engine.Components
{
    public interface IComponent {
        Guid GetId();
        Guid ParentEntityId { get; set; }
        IComponent Clone();
    }
}

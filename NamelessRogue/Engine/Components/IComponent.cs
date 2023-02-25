 

using System;

namespace NamelessRogue.Engine.Components
{
    public interface IComponent {
        Guid Id { get; set; }
        Guid ParentEntityId { get; set; }
        IComponent Clone();

        void Serialize();
        void DeSerialize();
    }
}

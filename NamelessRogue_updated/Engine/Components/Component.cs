 

using System;
using System.Runtime.Serialization;

namespace NamelessRogue.Engine.Components
{
    
    public abstract class Component : IComponent
    {
        
        public Guid Id { get; set; }
        public Guid ParentEntityId { get; set; }

        public Component()
        {
            Id = Guid.NewGuid();
        }

        public virtual IComponent Clone()
        {
            throw new NotImplementedException();
        }

		public virtual void Serialize()
		{
			throw new NotImplementedException();
		}

		public virtual void DeSerialize()
		{
			throw new NotImplementedException();
		}
	}
}

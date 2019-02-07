using System;
using System.Runtime.Serialization;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components;
using NamelessRogue.Engine.Engine.Infrastructure;
using SharpDX.WIC;

namespace NamelessRogue.Engine.Engine.Infrastructure
{

    [DataContract]
    public class Entity : IEntity
    {
        [DataMember]
        private Guid Id;

        public Entity(params IComponent[] components)
        {
            Id = Guid.NewGuid();
            foreach (IComponent component in components) {
                EntityManager.AddComponent(Id, component);
            }
            EntityManager.AddComponent(Id, new JustCreated());
        }

        public Entity()
        {
            Id = Guid.NewGuid();
            EntityManager.AddComponent(Id, new JustCreated());
        }


       

        public Guid GetId()
        {
            return Id;
        }

       
        public void AddComponent<ComponentType>(ComponentType component) where ComponentType : IComponent
        {
            EntityManager.AddComponent(Id, component);
        }

   
       
        public ComponentType GetComponentOfType<ComponentType>() where ComponentType : IComponent
        {
            return EntityManager.GetComponent<ComponentType>(Id);
        }

       
        public void RemoveComponentOfType<ComponentType>() where ComponentType : IComponent
        {
            EntityManager.RemoveComponent<ComponentType>(Id);
        }
    }
}

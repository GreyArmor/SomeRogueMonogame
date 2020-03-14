using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components;
using NamelessRogue.Engine.Engine.Infrastructure;
using SharpDX.WIC;

namespace NamelessRogue.Engine.Engine.Infrastructure
{

    
    public class Entity : IEntity
    {


        private Guid Id;

        public Entity(params IComponent[] components)
        {
            Id = Guid.NewGuid();
            foreach (IComponent component in components) {
                EntityManager.AddComponent(Id, component);
            }
        }

        public Entity()
        {
            Id = Guid.NewGuid();
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

        public List<IComponent> GetAllComponents()
        { 
            return EntityManager.GetAllComponents(Id); ;
        }

        public IEntity CloneEntity()
        {
            var newEntity = new Entity();
            var components = GetAllComponents();
            foreach (var component in components)
            {
                newEntity.AddComponent(component.Clone());
            }
            return newEntity;
        }

        List<IComponent> delayedAddComponents = new List<IComponent>();
        List<IComponent> delayedRemoveComponents = new List<IComponent>();

        public void AddComponentDelayed<T>(T component) where T : IComponent
        {
            delayedAddComponents.Add(component);
        }
        public void RemoveComponentDelayed<T>(T component) where T : IComponent
        {
            delayedRemoveComponents.Add(component);
        }
        public void AppendDelayedComponents()
        {
            foreach (var delayedAddComponent in delayedAddComponents)
            {
                AddComponent(delayedAddComponent);
            }

            foreach (var delayedRemoveComponent in delayedRemoveComponents)
            {
                RemoveComponent(delayedRemoveComponent);
            }
            delayedRemoveComponents.Clear();
            delayedAddComponents.Clear();
        }

        public void RemoveComponent<T>(T component) where T : IComponent
        {
            EntityManager.RemoveComponent(component, Id);
        }
    }
}

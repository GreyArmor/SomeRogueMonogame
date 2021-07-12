using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Serialization;
using SharpDX.WIC;

namespace NamelessRogue.Engine.Infrastructure
{

    [SkipClassGeneration]
    public class Entity : IEntity
    {
        public Entity(params IComponent[] components)
        {
            Id = Guid.NewGuid();
            foreach (IComponent component in components) {
                EntityInfrastructureManager.AddComponent(this, component);
            }
        }

        public Entity()
        {
            Id = Guid.NewGuid();
        }

        public Entity(Guid Id)
        {
            this.Id = Id;
        }

 
       
        public void AddComponent<ComponentType>(ComponentType component) where ComponentType : IComponent
        {
            EntityInfrastructureManager.AddComponent(this, component);
        }

   
       
        public ComponentType GetComponentOfType<ComponentType>() where ComponentType : IComponent
        {
            return EntityInfrastructureManager.GetComponent<ComponentType>(Id);
        }


       
        public void RemoveComponentOfType<ComponentType>() where ComponentType : IComponent
        {
            EntityInfrastructureManager.RemoveComponent<ComponentType>(this);
        }

        public List<IComponent> GetAllComponents()
        { 
            return EntityInfrastructureManager.GetAllComponents(Id); ;
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

		public Guid Id { get; set; }

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
            EntityInfrastructureManager.RemoveComponent(component, Id);
        }
    }
}

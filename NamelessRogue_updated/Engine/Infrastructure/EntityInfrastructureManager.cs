using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components;

namespace NamelessRogue.Engine.Infrastructure
{
    public class EntityInfrastructureManager {

        static Dictionary<Type, Dictionary<Guid, IComponent>> components;
        static LinkedList<ISystem> systems;

		public static Dictionary<Type, Dictionary<Guid, IComponent>> Components { get { return components; } }

		static EntityInfrastructureManager() {
            components = new Dictionary<Type, Dictionary<Guid, IComponent>>();
            systems = new LinkedList<ISystem>();
        }

        public static void AddSystem(ISystem system)
        {
            systems.AddLast(system);
        }

        public static void RemoveSystem(ISystem system)
        {
            systems.Remove(system);
        }

        public static  void AddComponent<ComponentType>(IEntity entity, ComponentType component) where ComponentType : IComponent {
            Dictionary<Guid, IComponent> componentsOfType;
            components.TryGetValue(component.GetType(), out componentsOfType);
            if (componentsOfType == null) {
                componentsOfType = new Dictionary<Guid, IComponent>();
                components.Add(component.GetType(), componentsOfType);
            }

            component.ParentEntityId = entity.Id;
            componentsOfType.Add(entity.Id, component);

            foreach (var system in systems)
            {
                if (system.IsEntityMatchesSignature(entity))
                {
                    system.AddEntity(entity);
                }
            }

        }

        public static void RemoveComponent<ComponentType>(IEntity entity) where ComponentType : IComponent
        {
            Dictionary<Guid, IComponent> componentsOfType;
            components.TryGetValue(typeof(ComponentType), out componentsOfType);
            if (componentsOfType != null) {
                componentsOfType.Remove(entity.Id);
            }

            foreach (var system in systems)
            {
                if (!system.IsEntityMatchesSignature(entity))
                {
                    system.RemoveEntity(entity);
                }
            }
        }

        public static void RemoveComponent(IComponent component,Guid entityID)
        {
            Dictionary<Guid, IComponent> componentsOfType;
            components.TryGetValue(component.GetType(), out componentsOfType);
            if (componentsOfType != null)
            {
                componentsOfType.Remove(entityID);
            }
        }

        public static ComponentType GetComponentByEntity<ComponentType>(Guid entityID) where ComponentType : IComponent
        {
            Dictionary<Guid, IComponent> componentsOfType;
            components.TryGetValue(typeof(ComponentType), out componentsOfType);
            if (componentsOfType != null) {
                return (ComponentType) componentsOfType[entityID];
            }
        return default(ComponentType);
    }

        internal static List<IComponent> GetAllComponents(Guid entityID)
        {
            List<IComponent> componentsOfEntity = new List<IComponent>();
            foreach (KeyValuePair<Type, Dictionary<Guid, IComponent>> keyValuePair in components)
            {
                keyValuePair.Value.TryGetValue(entityID, out IComponent component);
                if (component != null)
                {
                    componentsOfEntity.Add(component);
                }
            }

            return componentsOfEntity;
        }

        public static ComponentType GetComponent<ComponentType>(Guid componentId) where ComponentType : IComponent
        {
            if (components.TryGetValue(typeof(ComponentType), out var dict))
            {
                dict.TryGetValue(componentId, out var component);
                if (component != null)
                {
                    return (ComponentType) component;
                }
            }

            return default(ComponentType);
        }



        public static void RemoveEntity(IEntity entity) {
            foreach (Dictionary<Guid, IComponent> dict in components.Values) {
                dict.Remove(entity.Id);
            }

            foreach (var system in systems)
            {
                system.RemoveEntity(entity);
            }

        }
    }
}

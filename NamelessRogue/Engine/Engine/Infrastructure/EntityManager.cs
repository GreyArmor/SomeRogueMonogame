using System;
using System.Collections.Generic;
using System.Linq;
using NamelessRogue.Engine.Engine.Components;

namespace NamelessRogue.Engine.Engine.Infrastructure
{
    public class EntityManager {
        static EntityManager() {
            Components = new Dictionary<Type, Dictionary<Guid, IComponent>>();
        }

        public static  void AddComponent<ComponentType>(Guid entityID, ComponentType component) where ComponentType : IComponent {
            Dictionary<Guid, IComponent> componentsOfType;
            Components.TryGetValue(component.GetType(), out componentsOfType);
            if (componentsOfType == null) {
                componentsOfType = new Dictionary<Guid, IComponent>();
                Components.Add(typeof(ComponentType), componentsOfType);
            }
            componentsOfType.Add(entityID, component);
        }

        public static void RemoveComponent<ComponentType>(Guid entityID) where ComponentType : IComponent
        {
            Dictionary<Guid, IComponent> componentsOfType;
            Components.TryGetValue(typeof(ComponentType), out componentsOfType);
            if (componentsOfType != null) {
                componentsOfType.Remove(entityID);
            }
        }

        public static ComponentType GetComponentByEntity<ComponentType>(Guid entityID) where ComponentType : IComponent
        {
            Dictionary<Guid, IComponent> componentsOfType;
            Components.TryGetValue(typeof(ComponentType), out componentsOfType);
            if (componentsOfType != null) {
                return (ComponentType) componentsOfType[entityID];
            }
        return default(ComponentType);
    }

        public static ComponentType GetComponent<ComponentType>(Guid componentId) where ComponentType : IComponent
        {
            if (Components.TryGetValue(typeof(ComponentType), out var dict))
            {
                dict.TryGetValue(componentId, out var component);
                if (component != null)
                {
                    return (ComponentType) component;
                }
            }

            return default(ComponentType);
        }

        public static Dictionary<Type, Dictionary<Guid, IComponent>> Components;

        public static void RemoveEntity(Guid Guid) {
            foreach (Dictionary<Guid, IComponent> dict in Components.Values) {
                dict.Remove(Guid);
            }
        }
    }
}

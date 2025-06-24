using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using MonoGame.Extended.ECS;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components;
using static Assimp.Metadata;

namespace NamelessRogue.Engine.Infrastructure
{
    public class EntityInfrastructureManager {
        const int defaultCapacity = 64000;
        static List<IEntity> entities;
        static Queue<int> freeIndexes;
        static Dictionary<Type, List<IComponent>> components;
        static LinkedList<ISystem> systems;


        public static int NextavailableIndex()
        {
            return freeIndexes.Peek();
        }

		public static Dictionary<Type, List<IComponent>> Components { get { return components; } }

		public static List<IEntity> Entities { get { return entities; } }

		static EntityInfrastructureManager() {
            entities = new List<IEntity>(new IEntity[defaultCapacity]);
            freeIndexes = new Queue<int>();
            for(int i = 0; i < defaultCapacity; i++)
            {
                freeIndexes.Enqueue(i);
            }
            components = new Dictionary<Type, List<IComponent>>();
            systems = new LinkedList<ISystem>();
        }

        public static IEntity GetEntity(Guid id)
        {
            return entities.FirstOrDefault(x=>x.Id == id);
        }
        public static void AddEntity(IEntity entity)
        {
            if (entity.Index == -1)
            {
                var index = freeIndexes.Dequeue();
                entities[index] = entity;
                entity.Index = index;
            }
        }
        public static void AddSystem(ISystem system)
        {
            systems.AddLast(system);
        }

        public static void RemoveSystem(ISystem system)
        {
            systems.Remove(system);
        }


        public static void AddComponent<ComponentType>(IEntity entity, ComponentType component) where ComponentType : IComponent
        {
            components.TryGetValue(component.GetType(), out var componentsOfType);
            if (componentsOfType == null)
            {
                componentsOfType = new List<IComponent>(new IComponent[defaultCapacity]);
                components.Add(component.GetType(), componentsOfType);
            }

            component.ParentEntityId = entity.Id;
            componentsOfType[entity.Index] = component;

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
            components.TryGetValue(typeof(ComponentType), out var componentsOfType);
            if (componentsOfType != null) {
                componentsOfType[entity.Index] = null;
            }

            freeIndexes.Enqueue(entity.Index);

            foreach (var system in systems)
            {
                if (!system.IsEntityMatchesSignature(entity))
                {
                    system.RemoveEntity(entity);
                }
            }
        }

        public static void RemoveComponent(IComponent component, IEntity entity)
        {
            components.TryGetValue(component.GetType(), out var componentsOfType);
            if (componentsOfType != null)
            {
                componentsOfType[entity.Index] = null;
            }

            foreach (var system in systems)
            {
                if (!system.IsEntityMatchesSignature(entity))
                {
                    system.RemoveEntity(entity);
                }
            }

        }

        public static ComponentType GetComponentByEntity<ComponentType>(IEntity entity) where ComponentType : IComponent
        {
            components.TryGetValue(typeof(ComponentType), out var componentsOfType);
            if (componentsOfType != null) {
                return (ComponentType) componentsOfType[entity.Index];
            }
        return default(ComponentType);
    }

        internal static List<IComponent> GetAllComponents(IEntity entity)
        {
            List<IComponent> componentsOfEntity = new List<IComponent>();
            foreach (var keyValuePair in components)
            {
                IComponent component = keyValuePair.Value[entity.Index];
                if (component != null)
                {
                    componentsOfEntity.Add(component);
                }
            }

            return componentsOfEntity;
        }

        public static ComponentType GetComponent<ComponentType>(int index) where ComponentType : IComponent
        {
            if (components.TryGetValue(typeof(ComponentType), out var componentList))
            {
                var component = componentList[index];
                if (component != null)
                {
                    return (ComponentType) component;
                }
            }

            return default(ComponentType);
        }

        public static void RemoveEntity(IEntity entity) {
            foreach (var componentList in components.Values) {
                componentList[entity.Index] = null;
            }

            freeIndexes.Enqueue(entity.Index);

            foreach (var system in systems)
            {
                system.RemoveEntity(entity);
            }
        }

		internal static void ClearGame()
		{
            entities.Clear();
            
            systems.Clear();
            components.Clear();

        }
	}
}

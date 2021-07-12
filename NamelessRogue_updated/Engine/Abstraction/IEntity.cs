

using System;
using System.Collections.Generic;
using NamelessRogue.Engine.Components;
using NamelessRogue.Engine.Serialization;

namespace NamelessRogue.Engine.Abstraction
{
    [SkipClassGeneration]
    public interface IEntity
    {
        Guid Id { get; set; }
        T GetComponentOfType<T>() where T : IComponent;
        void AddComponent<T>(T component) where T : IComponent;

        void RemoveComponent<T>(T component) where T : IComponent;

        void AddComponentDelayed<T>(T component) where T : IComponent;
        void RemoveComponentDelayed<T>(T component) where T : IComponent;
        void RemoveComponentOfType<T>() where T : IComponent;
        List<IComponent> GetAllComponents();

        void AppendDelayedComponents();
        
        IEntity CloneEntity();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NamelessRogue.Engine.Components;
using NamelessRogue.Engine.Serialization;

namespace NamelessRogue.Serialization;

/// <summary>
/// Auto-generated enum mapping component types to stable hash values for FlatBuffers serialization.
/// Populated once on first access by scanning the NamelessRogue.Engine assembly.
/// </summary>
public enum ComponentType
{
    None = 0
}

/// <summary>
/// Static registry that maps ComponentType enum values to concrete IComponent subclasses.
/// Auto-registers all serializable component types on first access.
/// </summary>
public static class ComponentTypeRegistry
{
    private static readonly Dictionary<ulong, ComponentType> _hashToEnum = new();
    private static readonly Dictionary<ComponentType, Type> _enumToType = new();
    private static readonly Dictionary<Type, ComponentType> _typeToEnum = new();
    private static bool _initialized;
    private static readonly object _lock = new();

    public static ComponentType GetForType(Type componentType)
    {
        EnsureInitialized();
        if (_typeToEnum.TryGetValue(componentType, out var typeEnum))
            return typeEnum;
        return ComponentType.None;
    }

    public static Type? GetTypeForEnum(ComponentType typeEnum)
    {
        EnsureInitialized();
        if (_enumToType.TryGetValue(typeEnum, out var type))
            return type;
        return null;
    }

    public static ulong GetHash(ComponentType typeEnum)
    {
        EnsureInitialized();
        if (_hashToEnum.TryGetValue(Fnv1aHash((int)typeEnum), out var resolved) && resolved == typeEnum)
            return Fnv1aHash((int)typeEnum);
        return 0;
    }

    public static ComponentType? FromHash(ulong hash)
    {
        EnsureInitialized();
        if (_hashToEnum.TryGetValue(hash, out var typeEnum))
            return typeEnum;
        return null;
    }

    public static IReadOnlyCollection<ComponentType> AllTypes
    {
        get
        {
            EnsureInitialized();
            return _enumToType.Keys.ToList().AsReadOnly();
        }
    }

    private static void EnsureInitialized()
    {
        if (_initialized) return;

        lock (_lock)
        {
            if (_initialized) return;

            var assembly = typeof(Component).Assembly;
            var componentBase = typeof(Component);
            var skipAttribute = typeof(SkipClassGeneration);

            var serializableTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && componentBase.IsAssignableFrom(t))
                .Where(t => !t.GetCustomAttributes(skipAttribute, false).Any())
                .OrderBy(t => t.Name)
                .ToList();

            int index = 1;
            foreach (var type in serializableTypes)
            {
                var enumValue = (ComponentType)index;
                _enumToType[enumValue] = type;
                _typeToEnum[type] = enumValue;
                _hashToEnum[Fnv1aHash(index)] = enumValue;
                index++;
            }

            _initialized = true;
        }
    }

    private static ulong Fnv1aHash(int value)
    {
        const ulong fnvOffsetBasis = 14695981039571523429UL;
        const ulong fnvPrime = 1099511628211UL;
        ulong hash = fnvOffsetBasis;
        byte[] bytes = System.BitConverter.GetBytes(value);
        foreach (byte b in bytes)
        {
            hash ^= b;
            hash *= fnvPrime;
        }
        return hash;
    }
}

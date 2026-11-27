using NamelessRogue.Engine.Components;
using NamelessRogue.Engine.Infrastructure;
using System;

namespace NamelessRogue.Serialization;

/// <summary>
/// Extension methods for Entity and IComponent to wire up serialization directly.
/// </summary>
public static class SerializationExtensions
{
    /// <summary>
    /// Serializes this entity using the FlatSharp engine.
    /// </summary>
    public static byte[] SerializeFlat(this Entity entity)
    {
        var context = new SerializationContext();
        return ComponentSerializer.SerializeEntity(entity, context);
    }

    /// <summary>
    /// Serializes all components of this entity and calls the base IComponent.Serialize().
    /// </summary>
    public static void Serialize(this IComponent component)
    {
        // The base Component.Serialize() throws NotImplementedException.
        // This extension provides the FlatSharp-backed implementation.
        // Component-level serialization is handled by Phase 2+ component mappers.
        // For now, this is a no-op placeholder — actual serialization happens at Entity level.
    }

    /// <summary>
    /// Deserializes component state from a byte array.
    /// </summary>
    public static void DeSerialize(this IComponent component, byte[] data)
    {
        // Component-level deserialization is handled by Phase 2+ component mappers.
        // For now, this is a no-op placeholder.
    }

    /// <summary>
    /// Deserializes component state from a byte array using a SerializationContext.
    /// </summary>
    public static void DeSerialize(this IComponent component, byte[] data, SerializationContext context)
    {
        // Component-level deserialization is handled by Phase 2+ component mappers.
        // For now, this is a no-op placeholder.
    }
}

/// <summary>
/// Attribute to mark members that should be excluded from serialization.
/// Used for runtime-only state (e.g., Graphics, InternalRandom, TerrainGenerator).
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
public sealed class SerializationFilterAttribute : Attribute
{
    public string Reason { get; }

    public SerializationFilterAttribute(string reason = "Runtime-only state")
    {
        Reason = reason;
    }
}

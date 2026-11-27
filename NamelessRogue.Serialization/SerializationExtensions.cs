using NamelessRogue.Engine.Components;
using NamelessRogue.Engine.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
    /// Serializes this component using the FlatSharp engine.
    /// Returns the serialized byte array, or null if the component type is not mappable.
    /// </summary>
    public static byte[] SerializeFlat(this IComponent component)
    {
        if (component is Component comp)
        {
            return ComponentMapper.SerializeComponent(comp);
        }
        return null;
    }

    /// <summary>
    /// Deserializes component state from a byte array.
    /// </summary>
    public static void DeSerialize(this IComponent component, byte[] data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        var deserialized = ComponentMapper.DeserializeComponent(data, component.GetType());
        if (deserialized != null)
        {
            // Copy deserialized property values back to the original component
            CopyComponentState(deserialized, component);
        }
    }

    /// <summary>
    /// Deserializes component state from a byte array using a SerializationContext.
    /// </summary>
    public static void DeSerialize(this IComponent component, byte[] data, SerializationContext context)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));
        if (context == null)
            throw new ArgumentNullException(nameof(data));

        var deserialized = ComponentMapper.DeserializeComponent(data, component.GetType());
        if (deserialized != null)
        {
            CopyComponentState(deserialized, component);
        }
    }

    /// <summary>
    /// Copies property values from a source component to a target component.
    /// </summary>
    private static void CopyComponentState(Component source, IComponent target)
    {
        var sourceType = source.GetType();
        var targetType = target.GetType();

        var sourceProps = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var targetProps = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var sourceProp in sourceProps)
        {
            var targetProp = targetProps.FirstOrDefault(p => p.Name == sourceProp.Name);
            if (targetProp != null && targetProp.CanWrite)
            {
                var value = sourceProp.GetValue(source);
                if (value != null || !targetProp.PropertyType.IsValueType)
                {
                    targetProp.SetValue(target, value);
                }
            }
        }
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

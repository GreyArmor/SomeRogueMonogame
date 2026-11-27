using FlatSharp;
using NamelessRogue.Engine.Components;
using NamelessRogue.Engine.Serialization;
using NamelessRogue.Serialization.Schemas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NamelessRogue.Serialization;

/// <summary>
/// Maps IComponent instances to/from FlatSharp serialized tables for Phase 2 simple components.
/// Handles scalar types, strings, enums, Vector3Int, Color, and simple structs.
/// </summary>
public static class ComponentMapper
{
    private static readonly Dictionary<Type, Type> _componentToTable = new();
    private static readonly Dictionary<string, Type> _tableNameToType = new();
    private static bool _initialized;
    private static readonly object _lock = new();

    /// <summary>
    /// Serializes a single component to a FlatBuffers byte array.
    /// Returns null if the component type has no corresponding FlatSharp table.
    /// </summary>
    public static byte[] SerializeComponent(Component component)
    {
        EnsureInitialized();
        var componentType = component.GetType();

        if (!_componentToTable.TryGetValue(componentType, out var tableType))
            return null;

        var instance = Activator.CreateInstance(tableType);
        var props = componentType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in props)
        {
            var value = prop.GetValue(component);
            SetTableField(tableType, instance, prop.Name, value);
        }

        var maxSize = GetMaxSize(tableType, instance);
        if (maxSize <= 0)
            return Array.Empty<byte>();

        var buffer = new byte[maxSize];
        var bytesWritten = WriteToBuffer(tableType, buffer, instance);
        return bytesWritten > 0 ? buffer.Take(bytesWritten).ToArray() : Array.Empty<byte>();
    }

    /// <summary>
    /// Deserializes a component from a FlatBuffers byte array.
    /// Returns null if the data is invalid or the component type is not found.
    /// </summary>
    public static Component DeserializeComponent(byte[] data, Type componentType)
    {
        EnsureInitialized();

        if (data == null || data.Length == 0)
            return null;

        if (!_componentToTable.TryGetValue(componentType, out var tableType))
            return null;

        var table = ParseTable(tableType, data);
        if (table == null)
            return null;

        var instance = (Component)Activator.CreateInstance(componentType);
        MapTableToComponent(table, instance);
        return instance;
    }

    /// <summary>
    /// Maps a FlatSharp table instance back to a Component''s properties.
    /// </summary>
    private static void MapTableToComponent(object table, Component component)
    {
        var componentType = component.GetType();
        var tableType = table.GetType();
        var props = componentType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in props)
        {
            // Handle Vector3Int.Point — reconstruct from X, Y, Z fields
            if (prop.Name == "Point" && prop.PropertyType.Name == "Vector3Int")
            {
                var xProp = tableType.GetProperty("X");
                var yProp = tableType.GetProperty("Y");
                var zProp = tableType.GetProperty("Z");
                if (xProp != null && yProp != null && zProp != null)
                {
                    var x = xProp.GetValue(table) as int?;
                    var y = yProp.GetValue(table) as int?;
                    var z = zProp.GetValue(table) as int?;
                    if (x.HasValue && y.HasValue && z.HasValue)
                    {
                        var vectorType = typeof(NamelessRogue.Engine.Utility.Vector3Int);
                        prop.SetValue(component, Activator.CreateInstance(vectorType, x.Value, y.Value, z.Value));
                    }
                }
                continue;
            }

            // Handle Color — reconstruct from R, G, B, A fields
            if (prop.PropertyType.Name == "Color" && prop.PropertyType.Namespace?.StartsWith("Microsoft.Xna.Framework") == true)
            {
                var rProp = tableType.GetProperty(prop.Name + "R");
                var gProp = tableType.GetProperty(prop.Name + "G");
                var bProp = tableType.GetProperty(prop.Name + "B");
                var aProp = tableType.GetProperty(prop.Name + "A");
                if (rProp != null && gProp != null && bProp != null && aProp != null)
                {
                    var r = rProp.GetValue(table) as byte?;
                    var g = gProp.GetValue(table) as byte?;
                    var b = bProp.GetValue(table) as byte?;
                    var a = aProp.GetValue(table) as byte?;
                    if (r.HasValue && g.HasValue && b.HasValue && a.HasValue)
                    {
                        var colorType = prop.PropertyType;
                        prop.SetValue(component, Activator.CreateInstance(colorType, r.Value, g.Value, b.Value, a.Value));
                    }
                }
                continue;
            }

            var tableProp = tableType.GetProperty(prop.Name);
            if (tableProp != null)
            {
                var value = tableProp.GetValue(table);
                if (value != null)
                {
                    prop.SetValue(component, ConvertValue(value, prop.PropertyType));
                }
            }
        }
    }

    /// <summary>
    /// Converts FlatSharp table field values back to .NET types.
    /// </summary>
    private static object ConvertValue(object value, Type targetType)
    {
        if (value == null)
            return null;

        var valType = value.GetType();

        // Handle enum conversion
        if (valType.IsEnum || targetType.IsEnum)
        {
            if (valType.IsEnum && targetType == typeof(int))
                return Convert.ToInt32(value);
            if (targetType.IsEnum)
                return Enum.ToObject(targetType, Convert.ToInt32(value));
        }

        return Convert.ChangeType(value, targetType);
    }

    /// <summary>
    /// Sets a field on a FlatSharp table instance by name.
    /// </summary>
    private static void SetTableField(Type tableType, object instance, string fieldName, object value)
    {
        if (value == null)
            return;

        var prop = tableType.GetProperty(fieldName);
        if (prop == null || !prop.CanWrite)
            return;

        var fieldType = prop.PropertyType;

        // Handle Vector3Int.Point — decompose into X, Y, Z
        if (value.GetType().Name == "Vector3Int")
        {
            var vector = value;
            var xProp = vector.GetType().GetProperty("X")?.GetValue(vector) as int?;
            var yProp = vector.GetType().GetProperty("Y")?.GetValue(vector) as int?;
            var zProp = vector.GetType().GetProperty("Z")?.GetValue(vector) as int?;

            var xField = tableType.GetProperty("X");
            if (xField != null && xField.CanWrite && xProp.HasValue)
                xField.SetValue(instance, xProp.Value);

            var yField = tableType.GetProperty("Y");
            if (yField != null && yField.CanWrite && yProp.HasValue)
                yField.SetValue(instance, yProp.Value);

            var zField = tableType.GetProperty("Z");
            if (zField != null && zField.CanWrite && zProp.HasValue)
                zField.SetValue(instance, zProp.Value);
            return;
        }

        // Handle Color — decompose into R, G, B, A
        if (value.GetType().Name == "Color" && value.GetType().Namespace?.StartsWith("Microsoft.Xna.Framework") == true)
        {
            var color = value;
            var rProp = color.GetType().GetProperty("R")?.GetValue(color) as byte?;
            var gProp = color.GetType().GetProperty("G")?.GetValue(color) as byte?;
            var bProp = color.GetType().GetProperty("B")?.GetValue(color) as byte?;
            var aProp = color.GetType().GetProperty("A")?.GetValue(color) as byte?;

            var rField = tableType.GetProperty(fieldName + "R");
            if (rField != null && rField.CanWrite && rProp.HasValue)
                rField.SetValue(instance, rProp.Value);

            var gField = tableType.GetProperty(fieldName + "G");
            if (gField != null && gField.CanWrite && gProp.HasValue)
                gField.SetValue(instance, gProp.Value);

            var bField = tableType.GetProperty(fieldName + "B");
            if (bField != null && bField.CanWrite && bProp.HasValue)
                bField.SetValue(instance, bProp.Value);

            var aField = tableType.GetProperty(fieldName + "A");
            if (aField != null && aField.CanWrite && aProp.HasValue)
                aField.SetValue(instance, aProp.Value);
            return;
        }

        // Handle Guid — split into Low, High ulongs
        if (value.GetType() == typeof(Guid))
        {
            var guid = (Guid)value;
            var bytes = guid.ToByteArray();
            var low = BitConverter.ToUInt64(bytes, 0);
            var high = BitConverter.ToUInt64(bytes, 8);

            var lowField = tableType.GetProperty("EntityIdLow") ?? tableType.GetProperty("Low");
            if (lowField != null && lowField.CanWrite)
                lowField.SetValue(instance, low);

            var highField = tableType.GetProperty("EntityIdHigh") ?? tableType.GetProperty("High");
            if (highField != null && highField.CanWrite)
                highField.SetValue(instance, high);
            return;
        }

        // Handle enum conversion
        if (value.GetType().IsEnum)
        {
            prop.SetValue(instance, Convert.ChangeType(value, fieldType));
            return;
        }

        // Handle nullable types
        if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            prop.SetValue(instance, value);
            return;
        }

        // Direct assignment
        try
        {
            prop.SetValue(instance, Convert.ChangeType(value, fieldType));
        }
        catch (InvalidCastException)
        {
            // Skip fields that cannot be converted
        }
    }

    /// <summary>
    /// Gets the FlatSharp table type by name.
    /// </summary>
    private static Type GetTableType(string tableName)
    {
        var assembly = typeof(SerializedEntity).Assembly;
        return assembly.GetTypes().FirstOrDefault(t => t.Name == tableName);
    }

    /// <summary>
    /// Gets the maximum buffer size for a FlatSharp table.
    /// </summary>
    private static int GetMaxSize(Type tableType, object instance)
    {
        var serializerProp = tableType.GetProperty("Serializer", BindingFlags.Public | BindingFlags.Static);
        if (serializerProp?.GetValue(null) is FlatSharp.ISerializer serializer)
            return serializer.GetMaxSize(instance);
        return 0;
    }

    /// <summary>
    /// Writes a FlatSharp table to a buffer.
    /// </summary>
    private static int WriteToBuffer(Type tableType, byte[] buffer, object instance)
    {
        var serializerProp = tableType.GetProperty("Serializer", BindingFlags.Public | BindingFlags.Static);
        if (serializerProp?.GetValue(null) is FlatSharp.ISerializer serializer)
            return serializer.Write(buffer, instance);
        return 0;
    }

    /// <summary>
    /// Parses a FlatSharp table from a byte buffer.
    /// </summary>
    private static object ParseTable(Type tableType, byte[] data)
    {
        var serializerProp = tableType.GetProperty("Serializer", BindingFlags.Public | BindingFlags.Static);
        if (serializerProp?.GetValue(null) is FlatSharp.ISerializer serializer)
            return serializer.Parse(data);
        return null;
    }

    /// <summary>
    /// Ensures the type registry is initialized with all component-to-table mappings.
    /// </summary>
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

            foreach (var type in serializableTypes)
            {
                var tableType = GetTableType(type.Name);
                if (tableType != null)
                {
                    _componentToTable[type] = tableType;
                    _tableNameToType[type.Name] = type;
                }
            }

            _initialized = true;
        }
    }
}
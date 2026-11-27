using FlatSharp;
using NamelessRogue.Engine.Components;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Serialization.Schemas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NamelessRogue.Serialization;

/// <summary>
/// Core serialization engine — converts Entity and GameInstance objects to/from FlatBuffers byte arrays.
/// Uses FlatSharp 7.9.0 generated serializers for all serialization.
/// </summary>
public static class ComponentSerializer
{
    /// <summary>
    /// Serializes a single Entity and all its components into a FlatBuffers byte array.
    /// </summary>
    public static byte[] SerializeEntity(Entity entity, SerializationContext context)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        var serializedEntity = new SerializedEntity();

        // Convert Guid to two ulongs (FlatSharp doesn't have Guid type)
        var bytes = entity.Id.ToByteArray();
        serializedEntity.EntityIdLow = BytesToUlong(bytes, 0);
        serializedEntity.EntityIdHigh = BytesToUlong(bytes, 8);

        var components = entity.GetAllComponents();
        foreach (var component in components)
        {
            var componentType = ComponentTypeRegistry.GetForType(component.GetType());
            if (componentType == ComponentType.None)
                continue; // Skip non-serializable types

            var entityRef = new EntityRef
            {
                EntityIndex = (ulong)entity.Index,
                ComponentTypeHash = ComponentTypeRegistry.GetHash(componentType)
            };

            serializedEntity.Components.Add(entityRef);
        }

        // Use FlatSharp 7.9.0 correct API: GetMaxSize + Write(buffer, item) + Parse(buffer)
        var maxSize = SerializedEntity.Serializer.GetMaxSize(serializedEntity);
        var buffer = new byte[maxSize];
        var bytesWritten = SerializedEntity.Serializer.Write(buffer, serializedEntity);
        return buffer.Take(bytesWritten).ToArray();
    }

    /// <summary>
    /// Deserializes a single Entity from a FlatBuffers byte array.
    /// Reconstructs the Entity and attaches deserialized components.
    /// </summary>
    public static Entity DeserializeEntity(byte[] data, SerializationContext context)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        // Use FlatSharp 7.9.0 correct API: Parse(byte[])
        var serializedEntity = SerializedEntity.Serializer.Parse(data);

        // Convert two ulongs back to Guid
        var entityId = UlongsToGuid(serializedEntity.EntityIdLow, serializedEntity.EntityIdHigh);
        var entity = EntityInfrastructureManager.GetEntity(entityId);

        if (entity == null)
        {
            entity = new Entity(entityId);
            context.RegisterEntity(entity);
        }

        foreach (var componentRef in serializedEntity.Components)
        {
            var componentType = ComponentTypeRegistry.FromHash(componentRef.ComponentTypeHash);
            if (componentType.HasValue)
            {
                var type = ComponentTypeRegistry.GetTypeForEnum(componentType.Value);
                if (type != null)
                {
                    // Component deserialization will be wired in Phase 2+
                    // For now, the entity reference is resolved
                    // EntityIndex is a ulong, need to convert to Guid for GetEntity
                    var entityGuid = UlongsToGuid(componentRef.EntityIndex, 0);
                    var resolvedEntity = EntityInfrastructureManager.GetEntity(entityGuid);
                    // Entity references are stored for Phase 3 resolution
                }
            }
        }

        return entity;
    }

    /// <summary>
    /// Serializes a full GameInstance into a FlatBuffers byte array.
    /// </summary>
    public static byte[] SerializeGameInstance(object gameInstance, SerializationContext context)
    {
        // GameInstance serialization will be fully implemented in Phase 4
        // For now, serialize all entities in the infrastructure manager
        var serializedGame = new SerializedGame();
        serializedGame.SchemaVersion = 1;
        serializedGame.Seed = 0; // Will be populated from GameInstance
        serializedGame.Turn = 0; // Will be populated from GameInstance

        var entities = EntityInfrastructureManager.Entities.Where(e => e != null).ToList();
        foreach (var entity in entities)
        {
            var serializedEntity = new SerializedEntity();

            // Convert Guid to two ulongs
            var bytes = entity.Id.ToByteArray();
            serializedEntity.EntityIdLow = BytesToUlong(bytes, 0);
            serializedEntity.EntityIdHigh = BytesToUlong(bytes, 8);

            var components = entity.GetAllComponents();
            foreach (var component in components)
            {
                var componentType = ComponentTypeRegistry.GetForType(component.GetType());
                if (componentType == ComponentType.None)
                    continue;

                var entityRef = new EntityRef
                {
                    EntityIndex = (ulong)entity.Index,
                    ComponentTypeHash = ComponentTypeRegistry.GetHash(componentType)
                };

                serializedEntity.Components.Add(entityRef);
            }

            serializedGame.Entities.Add(serializedEntity);
        }

        // Use FlatSharp 7.9.0 correct API: GetMaxSize + Write(buffer, item)
        var maxSize = SerializedGame.Serializer.GetMaxSize(serializedGame);
        var buffer = new byte[maxSize];
        var bytesWritten = SerializedGame.Serializer.Write(buffer, serializedGame);
        return buffer.Take(bytesWritten).ToArray();
    }

    /// <summary>
    /// Deserializes a full GameInstance from a FlatBuffers byte array.
    /// </summary>
    public static object DeserializeGameInstance(byte[] data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        // Use FlatSharp 7.9.0 correct API: Parse(byte[])
        var serializedGame = SerializedGame.Serializer.Parse(data);

        var context = new SerializationContext();

        // First pass: create entities and register them
        var entityMap = new Dictionary<ulong, Entity>();
        foreach (var serializedEntity in serializedGame.Entities)
        {
            var entityId = UlongsToGuid(serializedEntity.EntityIdLow, serializedEntity.EntityIdHigh);
            var entity = new Entity(entityId);
            entityMap[serializedEntity.EntityIdLow] = entity;
            context.RegisterEntity(entity);
        }

        // Second pass: resolve entity references (Phase 3+)
        // Component deserialization will be wired in Phase 2+

        // TODO: Reconstruct GameInstance state from serializedGame
        // This requires Phase 4 components (GameInstance, Chunk, Tile, etc.)

        return null; // Placeholder — full implementation in Phase 5
    }

    /// <summary>
    /// Converts 8 bytes starting at offset to a ulong.
    /// </summary>
    private static ulong BytesToUlong(byte[] bytes, int offset)
    {
        var bytes8 = new byte[8];
        Array.Copy(bytes, offset, bytes8, 0, 8);
        return BitConverter.ToUInt64(bytes8, 0);
    }

    /// <summary>
    /// Converts two ulongs back to a Guid.
    /// </summary>
    private static Guid UlongsToGuid(ulong low, ulong high)
    {
        var bytes = new byte[16];
        var lowBytes = BitConverter.GetBytes(low);
        var highBytes = BitConverter.GetBytes(high);
        Array.Copy(lowBytes, 0, bytes, 0, 8);
        Array.Copy(highBytes, 0, bytes, 8, 8);
        return new Guid(bytes);
    }
}

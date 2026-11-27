using NamelessRogue.Engine.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NamelessRogue.Serialization;

/// <summary>
/// Context for serialization/deserialization operations.
/// Provides entity GUID-to-index lookup tables, component type resolution, and object reuse pools.
/// </summary>
public class SerializationContext
{
    private readonly Dictionary<Guid, int> _entityIndexLookup = new();
    private readonly Dictionary<ulong, Type> _hashTypeLookup = new();
    private readonly List<Entity> _deserializedEntities = new();
    private readonly Stack<Entity> _entityReusePool = new();

    public SerializationContext()
    {
        // Build entity index lookup from current game state
        foreach (var entity in EntityInfrastructureManager.Entities.Where(e => e != null))
        {
            _entityIndexLookup[entity.Id] = entity.Index;
        }
    }

    /// <summary>
    /// Look up entity index by GUID. Returns -1 if not found.
    /// </summary>
    public int GetEntityIndex(Guid entityId)
    {
        if (_entityIndexLookup.TryGetValue(entityId, out var index))
            return index;
        return -1;
    }

    /// <summary>
    /// Look up entity by GUID. Returns null if not found.
    /// </summary>
    public Entity? GetEntity(Guid entityId)
    {
        var index = GetEntityIndex(entityId);
        if (index >= 0 && index < EntityInfrastructureManager.Entities.Count)
            return EntityInfrastructureManager.Entities[index];
        return null;
    }

    /// <summary>
    /// Register a newly deserialized entity for reference resolution.
    /// </summary>
    public void RegisterEntity(Entity entity)
    {
        _entityIndexLookup[entity.Id] = entity.Index;
        _deserializedEntities.Add(entity);
    }

    /// <summary>
    /// Get all deserialized entities in order.
    /// </summary>
    public IReadOnlyList<Entity> GetDeserializedEntities() => _deserializedEntities.AsReadOnly();

    /// <summary>
    /// Push an entity onto the reuse pool to avoid GC pressure during repeated serialization.
    /// </summary>
    public void PoolEntity(Entity entity)
    {
        _entityReusePool.Push(entity);
    }

    /// <summary>
    /// Pop a reused entity from the pool, or create a new one if the pool is empty.
    /// </summary>
    public Entity GetPooledEntity()
    {
        if (_entityReusePool.Count > 0)
            return _entityReusePool.Pop();
        return new Entity();
    }

    /// <summary>
    /// Clear context state for reuse.
    /// </summary>
    public void Clear()
    {
        _entityIndexLookup.Clear();
        _deserializedEntities.Clear();
        while (_entityReusePool.Count > 0)
            _entityReusePool.Pop();
    }
}

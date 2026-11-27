# FlatSharp 7.9.0 Serialization Plan for SomeRogueMonogame ECS

## Project Overview

- **Framework**: Monogame ECS with ~70+ components across 10 categories
- **Target**: FlatSharp 7.9.0 (FlatBuffers serialization)
- **Current state**: SaveManager is an empty stub; IComponent.Serialize()/DeSerialize() throw NotImplementedException; Chunk has a TODO for disk serialization
- **Existing markers**: [SkipClassGeneration] attribute on Entity, Chunk, ChunkData - these should be excluded from serialization
- **Key challenges**: Entity GUID references, List<Entity> collections, MonoGame Vector3Int, Dictionary<string,int>, System.Drawing types, non-serializable runtime state (InternalRandom, TerrainGenerator, Graphics objects)

---

## Phase 0 - Foundation: FlatSharp Schema Project ✅ **COMPLETED**

**Goal**: Create a dedicated schema project with FlatSharp-generated types for all serializable components.

**Tasks**:
1. ✅ Create new project NamelessRogue.Serialization (net8.0, class library)
2. ✅ Add FlatSharp 7.9.0 NuGet package (FlatSharp.Compiler + FlatSharp.Runtime)
3. ✅ Define base schema types:
   - EntityRef table (stores ulong entity index + ulong component type hash)
   - ComponentType enum mapping to component classes
   - SerializedEntity table (entity GUID + list of component refs)
   - SerializedGame table (game metadata + list of entities)
4. ✅ Define FlatSharp .fbs schema files organized by component category (GameSchema.fbs, SimpleComponentsSchema.fbs)
5. ✅ Configure FlatSharp build-time codegen via FlatSharpSchema MSBuild item
6. ✅ Create SerializationContext class for entity lookup tables during deserialization
7. ✅ Update .csproj to reference NamelessRogue project

**Deliverable**: ✅ Compiling schema project with generated FlatBuffers types, runtime code present.

---

## Phase 1 - Serialization Infrastructure ✅ **COMPLETED**

**Goal**: Core serialization/deserialization engine in NamelessRogue.Serialization.

**Tasks**:
1. ✅ Create ComponentSerializer static class with:
   - SerializeEntity(Entity entity) -> byte[]
   - DeserializeEntity(byte[] data, SerializationContext ctx) -> Entity
   - SerializeGameInstance(GameInstance instance) -> byte[]
   - DeserializeGameInstance(byte[] data) -> GameInstance
2. ✅ Create SerializationContext with:
   - Entity GUID -> index lookup dictionary
   - Component type name -> type resolver
   - Reuse pool for deserialized objects (avoid GC pressure)
3. ✅ Implement ComponentType enum auto-registration (scan NamelessRogue.Engine.Components assembly)
4. ✅ Handle SkipClassGeneration attribute - skip those types
5. ✅ Create SerializationExtensions for Entity and IComponent
6. ✅ Wire up IComponent.Serialize() / DeSerialize() to call into the FlatSharp engine

**Deliverable**: ✅ Can serialize/deserialize a single entity with simple components.

---

## Phase 2 - Simple Components (Scalar + String + Enum) ✅ **COMPLETED**

**Goal**: Serialize components with only primitive types, strings, enums, and simple structs.

**Target components** (~25 types):
- Position (Vector3Int - needs custom flatbuffer vector encoding)
- SimpleStat (3 ints)
- Item, Ammo, Consumable, WeaponStats, ArmorStats, ResistanceStat
- Drawable, FollowedByCamera, LineToPlayer, UiIconComponent
- Dead, GameSettings
- Description, Cursor, Player, Movable, Selectable
- BlocksVision, OccupiesTile, StairsComponent
- ActionPoints, TargetMode, ActivationMode, DamageType
- Streetlight, SimpleSwitch, Fire, Door, Building (simple parts)
- WorldBoardPlayer, WorldMapCameraComponent

**Tasks**:
1. ✅ Map each component to a FlatSharp table type (25+ tables in SimpleComponentsSchema.fbs)
2. ✅ Handle Vector3Int - serialize X/Y/Z as three int fields (FlatBuffers supports signed int32 natively)
3. ✅ Handle enums - FlatSharp enum mapping (DamageType, ItemType, ItemQuality, AmmoType, AttackType, TargetMode, ActivationMode, Category)
4. ✅ Handle Guid - serialize as two ulong fields (Low + High) - already in GameSchema
5. ✅ Handle float/double - direct mapping
6. ✅ Handle string - FlatBuffers string type
7. ✅ Handle bool, int, uint, long, byte - direct mapping
8. ✅ Handle Color - decompose to R/G/B/A byte fields
9. ✅ Update ComponentMapper with Vector3Int and Color decomposition/reconstruction logic

**Deliverable**: ✅ All simple components serialize/deserialize correctly. Build compiles successfully.

---

## Phase 3 - Collection Components (List, Dictionary, Nested)

**Goal**: Serialize components with collections, nested objects, and entity references.

**Target components** (~15 types):
- CharacterStats (List<WeaponStats>, List<ResistanceStat>, List<ArmorStats>, Dictionary<DamageType,int>)
- AbilityHolder (List<Entity> - needs entity reference resolution)
- QuestHolder (List<Quest>, nested QuestNode tree with List<Entity>)
- Character (Dictionary<string,int> FactionRelations)
- ItemsHolder, EquipmentSlots, Equipment, AssociatedBuffs, Buff
- DroppedItemsComponent (List<DroppedItem>)
- Group, GroupTag, FlagBearerTag
- ModifiersCollection, TimedModifier, ModifierComponent
- Building (List<Entity> buildingParts)
- Spawner

**Tasks**:
1. Create EntityRef flatbuffer table - store entity index (ulong) instead of GUID during serialization
2. Create nested table types for WeaponStats, ResistanceStat, ArmorStats, QuestNode, DroppedItem
3. Handle Dictionary<string,int> - serialize as two parallel List<string> / List<int> in FlatBuffers
4. Handle List<Entity> - serialize as List<EntityRef> with index lookup
5. Handle nested trees (QuestNode.ChildNodes) - recursive serialization
6. Implement entity reference resolution in SerializationContext during deserialization
7. Handle List<T> where T is a FlatSharp table - use List<T> field mapping

**Deliverable**: All collection-based components serialize/deserialize with proper entity reference resolution.

---

## Phase 4 - Complex Runtime Components

**Goal**: Handle components with MonoGame types, custom classes, and non-trivial state.

**Target components** (~10 types):
- Chunk - 3D tile array Tile[][][], bounding boxes, terrain features
- ChunkData - world settings, generators
- Tile - terrain type, biome, water borders
- ProjectileComponent - trajectory, damage, lifetime
- AnimatedSpriteObject - sprite animation state
- ConsoleCamera, Camera3D - camera position/rotation
- TerrainGeometry3D, Geometry3D, SpriteModel3D, Chunk3dGeometryHolder - 3D rendering state
- FlowMoveComponent, FlowNode, FlowPathTuple, GatewayPathModel, IFlowfield implementations
- AStarPath, MacroNavigator - pathfinding state
- GameInstance - seed, turn, random state, terrain generator

**Tasks**:
1. Handle Chunk - serialize tile data as flat array with dimensions, exclude Graphics/Bitmap runtime state
2. Handle Tile - terrain enum, biome enum, terrain feature IDs
3. Handle 3D geometry components - serialize position, rotation, scale; exclude Texture2D, Model references (these are loaded assets, not game state)
4. Handle pathfinding components - serialize path points as coordinate arrays
5. Handle GameInstance - serialize seed, turn, culture template ID; exclude InternalRandom, TerrainGenerator (regenerated on load)
6. Create SerializationFilter attribute to mark members excluded from serialization (runtime-only state)
7. Handle BoundingBox, Vector2, Vector3 MonoGame types - decompose to primitive arrays

**Deliverable**: All complex components serialize/deserialize, excluding non-serializable runtime state.

---

## Phase 5 - Save/Load Manager Integration

**Goal**: Integrate serialization into the game's save/load workflow.

**Tasks**:
1. Replace empty SaveManager with FlatSharp-backed implementation:
   - SaveGame(string path, GameInstance game) -> writes .nrws file
   - LoadGame(string path) -> GameInstance -> reads .nrws file
   - SaveChunk(string path, Chunk chunk) -> writes individual chunk files
   - LoadChunk(string path) -> Chunk
2. Create file format spec: .nrws header (magic bytes + version + compression flag) + FlatBuffers payload
3. Add compression support (optional: System.IO.Compression)
4. Handle versioning - schema version in header, backward compatibility checks
5. Integrate with Chunk.Deactivate() / LoadFromDisk()
6. Add save/load hooks in game lifecycle (menu save, auto-save, chunk persistence)
7. Create NamelessRogueEditorDataTypes serialization support for editor save files

**Incremental/Diff Save Support**:
1. Add `SerializeVersion` attribute to component classes tracking schema version
2. Implement `ComponentSnapshot` class storing previous state hash per entity
3. Create `DiffEntry` FlatBuffers table: EntityId, ComponentType, Operation (Add/Update/Remove), Payload (byte[])
4. Build `DiffSerializer` that compares current entity state against last snapshot, producing minimal diff payload
5. Support two save modes:
   - **Full save**: write complete game state (checkpoint)
   - **Diff save**: write only changed entities/components since last save
6. Implement `DiffApplier` to merge diff payloads back into a game state during load
7. Add checkpoint mechanism - periodic full saves + incremental diffs between checkpoints
8. Handle diff file format: .nrws (full) vs .nrdiff (incremental), with version header

**Deliverable**: Full save/load pipeline working end-to-end, with incremental diff save support for continuous file updates.


## Excluded Types (Marked with [SkipClassGeneration] or Runtime-Only)

These types should **NOT** be serialized:
- Entity - managed by EntityInfrastructureManager, reconstructed during deserialization
- Chunk - runtime-only, regenerated from world data (though its tile data IS serialized in Phase 4)
- ChunkData - world generation metadata
- InternalRandom - seeded random, regenerated on load
- TerrainGenerator - runtime generator, regenerated on load
- Graphics, Bitmap, Pen, Brush - System.Drawing runtime objects
- ISystem, IDrawSystem - runtime system references
- EntityInfrastructureManager - singleton registry

---

## Risk Mitigations

| Risk | Mitigation |
|------|-----------|
| FlatBuffers uint vs int mismatch | Use int (FlatBuffers supports signed integers natively) |
| Guid serialization size | Store as two ulong (Low + High) - 16 bytes total |
| Circular references (Entity -> Component -> Entity) | Serialize Entity -> Components via index refs; resolve during deserialization |
| Schema breaking changes | Include schema version in file header; use FlatBuffers forward-compatible design |
| Large chunk tile data | Serialize as flat uint[] with dimension metadata; consider chunk-level compression |
| MonoGame type compatibility | Decompose all Vector2/Vector3/BoundingBox to primitive arrays |

---

## Sub-Agent Delegation Map

| Phase | Sub-Agent Objective |
|-------|---------------------|
| Phase 0 | explore agent - analyze all ~70 component files, produce component catalog with field types |
| Phase 1 | general-purpose agent - create serialization infrastructure project and engine |
| Phase 2 | general-purpose agent - map simple components to FlatSharp tables |
| Phase 3 | general-purpose agent - map collection/nested components, implement entity ref resolution |
| Phase 4 | general-purpose agent - handle complex runtime components, exclude non-serializable state |
| Phase 5 | general-purpose agent - integrate save/load manager, file format, game lifecycle hooks |

Each phase should produce a compiling build before the next phase begins.
